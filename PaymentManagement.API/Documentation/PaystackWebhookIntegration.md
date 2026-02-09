# Paystack Webhook Integration Guide

## Overview

This document describes the complete Paystack webhook integration for processing refund events asynchronously.

## Architecture

```
┌─────────────┐
│  Paystack   │
│   Server    │
└──────┬──────┘
       │ HTTP POST (webhook)
       │ x-paystack-signature: <hmac_signature>
       ▼
┌──────────────────────┐
│ PaystackWebhook      │
│ Controller           │
│ - Verify signature   │
│ - Route event        │
└──────┬───────────────┘
       │
       │ Send(ProcessPaystackWebhookCommand)
       ▼
┌──────────────────────────┐
│ PaystackRefundWebhook    │
│ Handler                  │
│ - Get refund by ext ID   │
│ - Complete/Fail refund   │
│ - Send notification      │
└──────────────────────────┘
```

## Setup Instructions

### 1. Register Services in DI Container

In your `Program.cs` or `Startup.cs`:

```csharp
// Add webhook verifier
builder.Services.AddScoped<IPaystackWebhookVerifier>(sp =>
{
    var secretKey = builder.Configuration["Paystack:SecretKey"];
    return new PaystackWebhookVerifier(secretKey!);
});

// Add refund repository
builder.Services.AddScoped<IRefundRepository, RefundRepository>();
```

### 2. Enable Request Buffering

Webhooks need to read the request body multiple times (for model binding and signature verification):

```csharp
// In Program.cs, before app.UseRouting()
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});
```

### 3. Configure Paystack Dashboard

1. Log in to [Paystack Dashboard](https://dashboard.paystack.com)
2. Go to **Settings** > **Webhooks**
3. Add webhook URL: `https://yourdomain.com/api/webhooks/paystack`
4. Select events to receive:
   - ✅ `refund.processed`
   - ✅ `refund.failed`
   - ✅ `charge.success` (optional)

### 4. Webhook Security

The controller automatically verifies webhook signatures using HMAC-SHA512:

```csharp
var signature = Request.Headers["x-paystack-signature"];
var isValid = _webhookVerifier.VerifySignature(signature, rawBody);
```

**Important**: Never disable signature verification in production!

## Webhook Events

### refund.processed

Sent when a refund is successfully processed by Paystack.

**Action**: Updates refund status to `Processed` and sends completion email.

### refund.failed

Sent when a refund fails at the payment gateway.

**Action**: Updates refund status to `Failed` and sends failure notification.

## Complete Refund Flow

### Step 1: Refund Initiation (User Request)

```
User → API → RefundPaymentCommandHandler
             ↓
             1. Create Refund entity (Status: Pending)
             2. Call Paystack API POST /refund
             3. Paystack returns: { id: 123, status: "pending" }
             4. refund.SetExternalRefundId("123")
             5. Save to database
             6. Send email: "REFUND INITIATED"
```

**Database State**:
```
Refunds Table:
- Status: Pending
- ExternalRefundId: "123"
- ProcessedAt: NULL
```

### Step 2: Paystack Processing (Asynchronous)

```
Paystack processes the refund internally...
(This can take seconds to hours depending on the payment method)
```

### Step 3: Webhook Notification

```
Paystack → Webhook Controller → PaystackRefundWebhookHandler
                                 ↓
                                 1. Get refund by ExternalRefundId
                                 2. Verify status is Pending
                                 3. Call refund.Complete() or refund.Fail()
                                 4. Save to database
                                 5. Send email: "REFUND COMPLETED" or "REFUND FAILED"
```

**Database State (Success)**:
```
Refunds Table:
- Status: Processed
- ExternalRefundId: "123"
- ProcessedAt: 2026-02-09 04:30:00
```

## Testing Webhooks

### Local Testing with Ngrok

1. Install [ngrok](https://ngrok.com):
   ```bash
   ngrok http 5000
   ```

2. Copy the HTTPS URL (e.g., `https://abc123.ngrok.io`)

3. Add to Paystack dashboard:
   ```
   https://abc123.ngrok.io/api/webhooks/paystack
   ```

### Manual Testing

Send a test webhook using curl:

```bash
curl -X POST https://localhost:5000/api/webhooks/paystack \
  -H "Content-Type: application/json" \
  -H "x-paystack-signature: YOUR_COMPUTED_SIGNATURE" \
  -d '{
    "event": "refund.processed",
    "data": {
      "id": 3018284,
      "status": "success",
      "amount": 10000,
      "currency": "NGN",
      "domain": "test",
      "createdAt": "2026-02-09T04:30:17.122Z",
      "updatedAt": "2026-02-09T04:30:17.122Z"
    }
  }'
```

## Error Handling

### Webhook Failures

If webhook processing fails:
- Controller returns **500 Internal Server Error**
- Paystack **retries** the webhook (with exponential backoff)
- Check logs for error details

### Duplicate Webhooks

The handler includes **idempotency** check:

```csharp
if (refund.Status != RefundStatus.Pending)
{
    logger.LogWarning("Refund already processed");
    return Unit.Value; // Ignore duplicate
}
```

### Invalid Signatures

- Controller returns **401 Unauthorized**
- Webhook is **not processed**
- Logged as security warning

## Monitoring

### Key Metrics to Track

1. **Webhook receive rate**: Events/minute
2. **Signature validation failures**: Should be near zero
3. **Processing errors**: Failed webhook handlers
4. **Refund completion time**: Time from initiation to completion

### Logging

All webhook events are logged with structured logging:

```csharp
_logger.LogInformation("Received Paystack webhook event: {Event}, Data ID: {DataId}, Status: {Status}",
    payload.Event, payload.Data.Id, payload.Data.Status);
```

## Troubleshooting

### Webhook Not Received

1. Verify webhook URL in Paystack dashboard
2. Check firewall/security group allows Paystack IPs
3. Ensure HTTPS is enabled (required for production)

### Signature Verification Fails

1. Verify correct secret key is configured
2. Check request body is not modified before verification
3. Ensure `Request.EnableBuffering()` is called

### Refund Not Found

1. Verify refund was created with correct ExternalRefundId
2. Check repository query: `GetByExternalIdAsync()`
3. Verify database contains the refund record

## Security Best Practices

1. ✅ **Always verify signatures** - Never trust webhooks without verification
2. ✅ **Use HTTPS** - Webhooks must be sent over secure connections
3. ✅ **Implement idempotency** - Handle duplicate webhooks gracefully
4. ✅ **Log everything** - Audit trail for debugging and compliance
5. ✅ **Rate limiting** - Protect against webhook flooding
6. ✅ **IP whitelist** (optional) - Restrict to Paystack IPs

## Related Files

- `PaystackWebhookPayload.cs` - DTO for webhook payload
- `PaystackWebhookVerifier.cs` - Signature verification service
- `PaystackWebhookController.cs` - Webhook endpoint
- `PaystackRefundWebhookHandler.cs` - Refund event handler
- `RefundPaymentCommandHandler.cs` - Refund initiation handler

## References

- [Paystack Webhooks Documentation](https://paystack.com/docs/payments/webhooks)
- [Paystack Refunds API](https://paystack.com/docs/api/refund)
- [Webhook Security Best Practices](https://paystack.com/docs/payments/webhooks#verify-webhook-signature)
