using Xunit;
using BookingManagement.Domain.Entities;
using Core.SharedKernel.Enums;
using System;

namespace BookingManagement.Tests
{
    public class BookingTests
    {
        [Fact]
        public void CreateBooking_ShouldSetPropertiesCorrectly()
        {
            var customerId = Guid.NewGuid();
            var craftmanId = Guid.NewGuid();
            var address = new Address("123 Main St", "City", "12345");
            var description = "Fix sink";
            var duration = new DateTimeRange(DateTime.UtcNow, DateTime.UtcNow.AddHours(2));

            var booking = Booking.Create(customerId, craftmanId, address, description, duration);

            Assert.Equal(customerId, booking.CustomerId);
            Assert.Equal(craftmanId, booking.CraftmanId);
            Assert.Equal(address, booking.ServiceAddress);
            Assert.Equal(description, booking.Details.Description);
            Assert.Equal(BookingStatus.Pending, booking.Status);
            Assert.Equal(duration, booking.Duration);
        }

        [Fact]
        public void AddLineItem_ShouldAddItemToBooking()
        {
            var booking = Booking.Create(Guid.NewGuid(), Guid.NewGuid(), new Address("Street", "City", "Code"), "Job", new DateTimeRange(DateTime.UtcNow, DateTime.UtcNow.AddHours(1)));
            booking.AddLineItem("Install faucet", 100, 2);
            Assert.Single(booking.LineItems);
            Assert.Equal("Install faucet", booking.LineItems[0].Description);
            Assert.Equal(100, booking.LineItems[0].Price);
            Assert.Equal(2, booking.LineItems[0].Quantity);
        }

        [Fact]
        public void CancelBooking_ShouldSetStatusToCancelled()
        {
            var booking = Booking.Create(Guid.NewGuid(), Guid.NewGuid(), new Address("Street", "City", "Code"), "Job", new DateTimeRange(DateTime.UtcNow, DateTime.UtcNow.AddHours(1)));
            booking.CancelBooking(CancellationReason.CustomerRequest);
            Assert.Equal(BookingStatus.Cancelled, booking.Status);
        }
    }
}
