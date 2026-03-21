using AutoMapper;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using Infrastructure.Cache;
using MediatR;
using Moq;
using System.Linq.Expressions;
using UserManagement.Application.CQRS.Handlers.QueryHandlers.CraftmanQueryHandlers;
using UserManagement.Application.CQRS.Handlers.QueryHandlers.UserQueryHandlers;
using UserManagement.Application.CQRS.Queries.CraftmanQueries;
using UserManagement.Application.CQRS.Queries.UserQueries;
using UserManagement.Application.Exceptions;
using UserManagement.Application.Contracts;
using UserManagement.Domain.Entities;

namespace CraftConnect.Tests.Handlers.UserQueryHandlers
{
    // ──────────────────────────────────────────────────────────────────────────────
    // GetAllUsersQueryHandler
    // ──────────────────────────────────────────────────────────────────────────────
    public class GetAllUsersQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IUserRepository> _repoMock = new();
        private readonly GetAllUsersQueryHandler _handler;

        public GetAllUsersQueryHandlerTests()
        {
            _handler = new GetAllUsersQueryHandler(
                _mapperMock.Object,
                _repoMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsMappedUsers()
        {
            // Arrange
            var users = new List<User> { CreateUser("a@a.com"), CreateUser("b@b.com") };
            var dtos = users.Select(_ => new UserResponseDTO()).ToList();

            _repoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<UserResponseDTO>>(users))
                .Returns(dtos);

            // Act
            var result = await _handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count());
            _repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenDbReturnsNull()
        {
            _repoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<User>)null!);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _handler.Handle(new GetAllUsersQuery(), CancellationToken.None));
        }

        private static User CreateUser(string email) =>
            new(new Email(email), UserRole.Customer) { RowVersion = [] };
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // GetUserByIdQueryHandler
    // ──────────────────────────────────────────────────────────────────────────────
    public class GetUserByIdQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IUserRepository> _repoMock = new();
        private readonly GetUserByIdQueryHandler _handler;

        public GetUserByIdQueryHandlerTests()
        {
            _handler = new GetUserByIdQueryHandler(
                _mapperMock.Object,
                _repoMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsMappedUser()
        {
            var userId = Guid.NewGuid();
            var user = new User(new Email("c@c.com"), UserRole.Customer) { RowVersion = [] };
            var dto = new UserResponseDTO();

            _repoMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<UserResponseDTO>(user)).Returns(dto);

            var result = await _handler.Handle(new GetUserByIdQuery { UserId = userId }, CancellationToken.None);

            Assert.Same(dto, result);
            _repoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null!);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _handler.Handle(new GetUserByIdQuery { UserId = userId }, CancellationToken.None));
        }
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // GetUserByEmailQueryHandler
    // ──────────────────────────────────────────────────────────────────────────────
    public class GetUserByEmailQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IUserRepository> _repoMock = new();
        private readonly GetUserByEmailQueryHandler _handler;

        public GetUserByEmailQueryHandlerTests()
        {
            _handler = new GetUserByEmailQueryHandler(
                _mapperMock.Object,
                _repoMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsMappedUser()
        {
            var email = "test@example.com";
            var user = new User(new Email(email), UserRole.Customer) { RowVersion = [] };
            var dto = new UserResponseDTO();

            _repoMock
                .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<UserResponseDTO>(user)).Returns(dto);

            var result = await _handler.Handle(
                new GetUserByEmailQuery { Email = email }, CancellationToken.None);

            Assert.Same(dto, result);
            _repoMock.Verify(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenUserNotFound()
        {
            var email = "unknown@example.com";
            _repoMock
                .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null!);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _handler.Handle(new GetUserByEmailQuery { Email = email }, CancellationToken.None));
        }
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // GetAllCraftmenQueryHandler
    // ──────────────────────────────────────────────────────────────────────────────
    public class GetAllCraftmenQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ICraftsmanRepository> _repoMock = new();
        private readonly GetAllCraftmenQueryHandler _handler;

        public GetAllCraftmenQueryHandlerTests()
        {
            _handler = new GetAllCraftmenQueryHandler(
                _mapperMock.Object,
                _repoMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsMappedCraftmen()
        {
            var craftmen = new List<Craftman> { new(new Email("d@d.com"), Profession.Plumber) };
            var dtos = craftmen.Select(_ => new CraftmanResponseDTO()).ToList();

            _repoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(craftmen);

            _mapperMock.Setup(m => m.Map<IEnumerable<CraftmanResponseDTO>>(craftmen)).Returns(dtos);

            var result = await _handler.Handle(new GetAllCraftmenQuery(), CancellationToken.None);

            Assert.Single(result);
            _repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenNoneFound()
        {
            _repoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<Craftman>)null!);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _handler.Handle(new GetAllCraftmenQuery(), CancellationToken.None));
        }
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // GetCraftmanByIdQueryHandler
    // ──────────────────────────────────────────────────────────────────────────────
    public class GetCraftmanByIdQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ICraftsmanRepository> _repoMock = new();
        private readonly GetCraftmanByIdQueryHandler _handler;

        public GetCraftmanByIdQueryHandlerTests()
        {
            _handler = new GetCraftmanByIdQueryHandler(
                _mapperMock.Object,
                _repoMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsMappedDto()
        {
            var id = Guid.NewGuid();
            var craftman = new Craftman(new Email("e@e.com"), Profession.Electrician);
            var dto = new CraftmanResponseDTO();

            _repoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(craftman);

            _mapperMock.Setup(m => m.Map<CraftmanResponseDTO>(craftman)).Returns(dto);

            var result = await _handler.Handle(
                new GetCraftmanByIdQuery { CraftmanId = id }, CancellationToken.None);

            Assert.Same(dto, result);
        }

        [Fact]
        public async Task Handle_ThrowsNotFoundException_WhenCraftmanNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Craftman)null!);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _handler.Handle(new GetCraftmanByIdQuery { CraftmanId = id }, CancellationToken.None));
        }
    }
}
