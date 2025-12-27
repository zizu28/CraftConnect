using Microsoft.EntityFrameworkCore;
using Moq;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.UnitOfWork;
using Core.Logging;

using Core.SharedKernel.ValueObjects;
using Core.SharedKernel.Enums;

namespace CraftConnect.Tests.Handlers.UserCommandHandlers
{
    public class LoginUserCommandHandlerTests
    {
        private readonly Mock<ILoggingService<LoginUserCommandHandler>> _loggerMock;
        private readonly Mock<ITokenProvider> _tokenProviderMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ApplicationDbContext _dbContext;
        private readonly LoginUserCommandHandler _handler;

        public LoginUserCommandHandlerTests()
        {
            _loggerMock = new Mock<ILoggingService<LoginUserCommandHandler>>();
            _tokenProviderMock = new Mock<ITokenProvider>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);

            _handler = new LoginUserCommandHandler(
                _loggerMock.Object,
                _tokenProviderMock.Object,
                _unitOfWorkMock.Object,
                _dbContext
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyResponse_WhenValidationFails()
        {
            // Arrange
            var command = new LoginUserCommand { Email = "invalid-email", Password = "" };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(result.AccessToken);
            Assert.Null(result.RefreshToken);
            _loggerMock.Verify(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowKeyNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var command = new LoginUserCommand { Email = "nonexistent@example.com", Password = "Password123!" };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyResponse_WhenPasswordIsInvalid()
        {
            // Arrange
            var emailStr = "test@example.com";
            var wrongPassword = "WrongPassword1!";
            var correctPassword = "CorrectPassword1!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);
            var email = new Email(emailStr);

            var user = new User(email, UserRole.Customer)
            { 
                PasswordHash = hashedPassword,
                IsEmailConfirmed = true,
                RowVersion = []
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var command = new LoginUserCommand { Email = emailStr, Password = wrongPassword };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(result.AccessToken);
            Assert.Null(result.RefreshToken);
             _loggerMock.Verify(x => x.LogWarning(It.Is<string>(s => s.Contains("Invalid login attempt")), It.Is<object[]>(o => o.Contains(emailStr))), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyResponse_WhenEmailNotConfirmed()
        {
            // Arrange
            var emailStr = "test@example.com";
            var password = "CorrectPassword1!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var email = new Email(emailStr);

            var user = new User(email, UserRole.Customer)
            { 
                PasswordHash = hashedPassword,
                IsEmailConfirmed = false,
                RowVersion = []
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var command = new LoginUserCommand { Email = emailStr, Password = password };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(result.AccessToken);
            Assert.Null(result.RefreshToken);
            _loggerMock.Verify(x => x.LogWarning(It.Is<string>(s => s.Contains("Invalid login attempt")), It.Is<object[]>(o => o.Contains(emailStr))), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_ShouldRevokeOldRefreshTokens_WhenLoginSucceeds()
        {
             // Arrange
            var emailStr = "test@example.com";
            var password = "Password123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            
            var oldToken = new RefreshToken
            {
                Token = "old_token",
                IsRevoked = false,
                ExpiresOnUtc = DateTime.UtcNow.AddDays(7)
            };
            var email = new Email(emailStr);
            var user = new User(email, UserRole.Customer)
            { 
                PasswordHash = hashedPassword,
                IsEmailConfirmed = true,
                RefreshTokens = new List<RefreshToken> { oldToken },
                RowVersion = []
            };
            var userId = user.Id;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var command = new LoginUserCommand { Email = emailStr, Password = password };
            var newAccessToken = "new_access_token";
            var newRefreshToken = "new_refresh_token";

            _tokenProviderMock.Setup(x => x.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(newAccessToken);
            _tokenProviderMock.Setup(x => x.GenerateRefreshToken(It.IsAny<User>()))
                .Returns(newRefreshToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(newAccessToken, result.AccessToken);
            Assert.Equal(newRefreshToken, result.RefreshToken);

            // Verify revocation
            var dbUser = await _dbContext.Users.Include(u => u.RefreshTokens).FirstAsync(u => u.Id == userId);
            var revokedToken = dbUser.RefreshTokens.First(t => t.Token == "old_token");
            Assert.True(revokedToken.IsRevoked);
            Assert.NotEqual(DateTime.MinValue, revokedToken.RevokedOnUtc);
            Assert.Equal("Replaced", revokedToken.RevokedReason);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
