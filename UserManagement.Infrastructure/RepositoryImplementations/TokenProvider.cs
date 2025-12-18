using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using UserManagement.Application.Contracts;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.RepositoryImplementations
{
	public class TokenProvider( 
		IConfiguration _configuration,
		ApplicationDbContext dbContext) : ITokenProvider
	{
		public string GenerateAccessToken(Guid userId, string emailAddress, string role)
		{
			var claims = new List<Claim>
			{
				new(JwtRegisteredClaimNames.Sub, userId.ToString()),
				new(JwtRegisteredClaimNames.Email, emailAddress),
				new(ClaimTypes.Role, role),
				new(ClaimTypes.Name, emailAddress)
			};
			var issuer = _configuration["Jwt:Issuer"];
			var audience = _configuration["Jwt:Audience"];
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var securityToken = new JwtSecurityToken(issuer, audience, claims,
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddMinutes(15),
				signingCredentials: creds);
			var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
			return token;
		}

		public string GenerateCraftmanEmailConfirmationToken(Craftman craftman)
		{
			byte[] SecretKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
			var payload = new
			{
				CraftmanId = craftman.Id,
				Email = craftman.Email.Address,
				craftman.Role,
				ExpiresOn = DateTime.UtcNow.AddDays(1)
			};
			var payloadJson = JsonSerializer.Serialize(payload);
			var payloadBytes = Encoding.UTF8.GetBytes(payloadJson);
			var payloadBase64 = Convert.ToBase64String(payloadBytes)
										.Replace("+", "-").Replace("/", "_");
			var signatureBytes = HMACSHA256.HashData(SecretKey, Encoding.UTF8.GetBytes(payloadBase64));
			var signatureBase64 = Convert.ToBase64String(signatureBytes)
										.Replace("+", "-").Replace("/", "_");
			return $"{payloadBase64}.{signatureBase64}";
		}

		public string GenerateCustomerEmailConfirmationToken(Customer customer)
		{
			byte[] SecretKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
			var payload = new
			{
				CustomerId = customer.Id,
				Email = customer.Email.Address,
				ExpiresOn = DateTime.UtcNow.AddDays(1)
			};
			var payloadJson = JsonSerializer.Serialize(payload);
			var payloadBytes = Encoding.UTF8.GetBytes(payloadJson);
			var payloadBase64 = Convert.ToBase64String(payloadBytes)
										.Replace("+", "-").Replace("/", "_");
			var signatureBytes = HMACSHA256.HashData(SecretKey, Encoding.UTF8.GetBytes(payloadBase64));
			var signatureBase64 = Convert.ToBase64String(signatureBytes)
										.Replace("+", "-").Replace("/", "_");
			return $"{payloadBase64}.{signatureBase64}";
		}

		public bool ValidateCustomerEmailConfirmationToken(string token, Customer customer)
		{
			byte[] SecretKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
			try
			{
				var parts = token.Split('.');
				if (parts.Length != 2) return false;

				var payloadBase64 = parts[0];
				var providedSignatureBase64 = parts[1];

				var signatureBytes = HMACSHA256.HashData(SecretKey, Encoding.UTF8.GetBytes(payloadBase64));
				var recreatedSignatureBase64 = Convert.ToBase64String(signatureBytes)
												   .Replace('+', '-').Replace('/', '_');

				if (recreatedSignatureBase64 != providedSignatureBase64)
				{
					return false;
				}

				var payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payloadBase64.Replace('-', '+').Replace('_', '/')));
				var payload = JsonSerializer.Deserialize<JsonElement>(payloadJson);

				var userId = payload.GetProperty("CustomerId").GetString();
				var expiration = payload.GetProperty("ExpiresOn").GetInt64();

				if (userId != customer.Id.ToString() || expiration < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
				{
					return false;
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool ValidateCraftmanEmailConfirmationToken(string token, Craftman craftman)
		{
			byte[] SecretKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
			try
			{
				var parts = token.Split('.');
				if (parts.Length != 2) return false;

				var payloadBase64 = parts[0];
				var providedSignatureBase64 = parts[1];

				var signatureBytes = HMACSHA256.HashData(SecretKey, Encoding.UTF8.GetBytes(payloadBase64));
				var recreatedSignatureBase64 = Convert.ToBase64String(signatureBytes)
												   .Replace('+', '-').Replace('/', '_');

				if (recreatedSignatureBase64 != providedSignatureBase64)
				{
					return false;
				}

				var payloadJson = Encoding.UTF8.GetString(Convert.FromBase64String(payloadBase64.Replace('-', '+').Replace('_', '/')));
				var payload = JsonSerializer.Deserialize<JsonElement>(payloadJson);

				var userId = payload.GetProperty("CustomerId").GetString();
				var expiration = payload.GetProperty("ExpiresOn").GetInt64();

				if (userId != craftman.Id.ToString() || expiration < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
				{
					return false;
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		public string GenerateRefreshToken(User user)
		{
			var tokensToRemove = dbContext.RefreshTokens
				.Where(t => t.IsRevoked || t.RevokedOnUtc <= DateTime.UtcNow)
				.ToList();
			foreach (var token in tokensToRemove)
			{
				user.RefreshTokens.Remove(token);
				dbContext.Entry(token).State = EntityState.Deleted;
			}
			var refreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
			var refreshTokenEntity = new RefreshToken
			{
				Id = Guid.NewGuid(),
				Token = refreshTokenString,
				UserId = user.Id,
				ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
				IsRevoked = false
			};

			dbContext.RefreshTokens.Add(refreshTokenEntity);

			return refreshTokenString;
		}
	}
}
