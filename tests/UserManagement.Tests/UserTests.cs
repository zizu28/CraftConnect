using Xunit;
using UserManagement.Domain;
using System;

namespace UserManagement.Tests
{
    public class UserTests
    {
        [Fact]
        public void CreateUser_ShouldSetPropertiesCorrectly()
        {
            var email = new Email("test@example.com");
            var user = new User(email, UserRole.Customer);
            Assert.Equal(email, user.Email);
            Assert.Equal(UserRole.Customer, user.Role);
            Assert.False(user.IsEmailConfirmed);
        }

        [Fact]
        public void SetUsername_ShouldUpdateUsername()
        {
            var user = new User(new Email("user@example.com"), UserRole.Craftman);
            user.Username = "crafty";
            Assert.Equal("crafty", user.Username);
        }
    }
}
