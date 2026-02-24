using AutoMapper;
using Core.Logging;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using Infrastructure.Cache;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Commands.CategoryCommands;
using ProductInventoryManagement.Application.CQRS.Commands.ProductCommands;
using ProductInventoryManagement.Application.CQRS.Handlers.CommandHandlers.CategoryCommandHandlers;
using ProductInventoryManagement.Application.CQRS.Handlers.CommandHandlers.ProductCommandHandlers;
using ProductInventoryManagement.Domain.Entities;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands.CraftmanCommands;
using UserManagement.Application.CQRS.Commands.UserCommands;
using UserManagement.Application.CQRS.Handlers.CommandHandlers.CraftmanCommandHandlers;
using UserManagement.Application.CQRS.Handlers.CommandHandlers.UserCommandHandlers;
using UserManagement.Domain.Entities;

namespace CraftConnect.Tests.Handlers.CommandHandlers
{
    // ─────────────────────────────────────────────────────────────────────────────
    // DeleteUserCommandHandler — uses ApplicationDbContext, evicts AllUsers + UserById
    // ─────────────────────────────────────────────────────────────────────────────
    public class DeleteUserCommandHandlerCacheTests
    {
        private readonly Mock<ILoggingService<DeleteUserCommandHandler>> _loggerMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly ApplicationDbContext _dbContext;
        private readonly DeleteUserCommandHandler _handler;

        public DeleteUserCommandHandlerCacheTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);

            _handler = new DeleteUserCommandHandler(
                _dbContext,
                _loggerMock.Object,
                _unitOfWorkMock.Object,
                _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_EvictsAllUsersAndUserByIdCache_AfterSuccessfulDelete()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User(new Email("del@del.com"), UserRole.Customer) { RowVersion = [] };
            // Set the user's Id to the specific userId we'll look up
            typeof(User).GetProperty("Id")!.SetValue(user, userId);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            _cacheMock
                .Setup(c => c.RemoveSync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(new DeleteUserCommand { UserId = userId }, CancellationToken.None);

            // Assert — both cache keys were evicted
            _cacheMock.Verify(c => c.RemoveSync(CacheKeys.UserById(userId), It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(c => c.RemoveSync(CacheKeys.AllUsers, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsKeyNotFoundException_AndNoCacheEviction_WhenUserNotFound()
        {
            // Arrange — no user in DB
            var missingId = Guid.NewGuid();

            // Act + Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _handler.Handle(new DeleteUserCommand { UserId = missingId }, CancellationToken.None));

            // Cache eviction should NOT have been called
            _cacheMock.Verify(c => c.RemoveSync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // DeleteCraftmanCommandHandler — evicts AllCraftsmen + CraftsmanById
    // ─────────────────────────────────────────────────────────────────────────────
    public class DeleteCraftmanCommandHandlerCacheTests
    {
        private readonly Mock<ICraftsmanRepository> _craftsmanRepoMock = new();
        private readonly Mock<ILogger<DeleteCraftmanCommandHandler>> _loggerMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly DeleteCraftmanCommandHandler _handler;

        public DeleteCraftmanCommandHandlerCacheTests()
        {
            _handler = new DeleteCraftmanCommandHandler(
                _craftsmanRepoMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object,
                _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_EvictsAllCraftsmenAndCraftsmanByIdCache_AfterSuccessfulDelete()
        {
            // Arrange
            var id = Guid.NewGuid();
            var craftman = new Craftman(new Email("craftman@craft.com"), Profession.Plumber);
            typeof(Craftman).GetProperty("Id")!.SetValue(craftman, id);

            _craftsmanRepoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(craftman);

            _unitOfWorkMock
                .Setup(u => u.ExecuteInTransactionAsync(
                    It.IsAny<Func<Task<Unit>>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<Task<Unit>>, CancellationToken>((fn, ct) => fn());

            _cacheMock
                .Setup(c => c.RemoveSync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(new DeleteCraftmanCommand { CraftmanId = id }, CancellationToken.None);

            // Assert
            _cacheMock.Verify(c => c.RemoveSync(CacheKeys.CraftsmanById(id), It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(c => c.RemoveSync(CacheKeys.AllCraftsmen, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsKeyNotFoundException_WhenCraftmanNotFound()
        {
            var id = Guid.NewGuid();

            _craftsmanRepoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Craftman?)null);

            _unitOfWorkMock
                .Setup(u => u.ExecuteInTransactionAsync(
                    It.IsAny<Func<Task<Unit>>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<Task<Unit>>, CancellationToken>((fn, ct) => fn());

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _handler.Handle(new DeleteCraftmanCommand { CraftmanId = id }, CancellationToken.None));

            _cacheMock.Verify(c => c.RemoveSync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // CategoryCreateCommandHandler — evicts AllCategories on successful create
    // ─────────────────────────────────────────────────────────────────────────────
    public class CategoryCreateCommandHandlerCacheTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILoggingService<CategoryCreateCommandHandler>> _loggerMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly CategoryCreateCommandHandler _handler;

        public CategoryCreateCommandHandlerCacheTests()
        {
            _handler = new CategoryCreateCommandHandler(
                _categoryRepoMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_EvictsAllCategoriesCache_AfterSuccessfulCreate()
        {
            // Arrange
            var command = new CategoryCreateCommand
            {
                CategoryCreateDTO = new CategoryCreateDTO { Name = "Power Tools" }
            };

            var categoryEntity = new Category("Power Tools", "Power tools category");
            var responseDTO = new CategoryResponseDTO { IsSuccess = true, CategoryId = Guid.NewGuid() };

            _mapperMock.Setup(m => m.Map<Category>(command.CategoryCreateDTO)).Returns(categoryEntity);
            _mapperMock.Setup(m => m.Map<CategoryResponseDTO>(categoryEntity)).Returns(responseDTO);

            _unitOfWorkMock
                .Setup(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _cacheMock
                .Setup(c => c.RemoveSync(CacheKeys.AllCategories, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert — cache evicted once
            _cacheMock.Verify(c => c.RemoveSync(CacheKeys.AllCategories, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DoesNotEvictCache_WhenValidationFails()
        {
            // Arrange — CategoryCreateDTO with null Name triggers validation failure
            var command = new CategoryCreateCommand
            {
                CategoryCreateDTO = new CategoryCreateDTO { Name = "" }   // empty name: invalid
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert — validation failure path: cache must NOT be touched
            _cacheMock.Verify(c => c.RemoveSync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // DeleteCategoryCommandHandler — evicts AllCategories + CategoryById on delete
    // ─────────────────────────────────────────────────────────────────────────────
    public class DeleteCategoryCommandHandlerCacheTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
        private readonly Mock<ILoggingService<DeleteCategoryCommandHandler>> _loggerMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly DeleteCategoryCommandHandler _handler;

        public DeleteCategoryCommandHandlerCacheTests()
        {
            _handler = new DeleteCategoryCommandHandler(
                _categoryRepoMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object,
                _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_EvictsAllCategoriesAndCategoryByIdCache_AfterSuccessfulDelete()
        {
            var id = Guid.NewGuid();
            var category = new Category("Old Category", "description");

            _categoryRepoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _cacheMock
                .Setup(c => c.RemoveSync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _handler.Handle(new DeleteCategoryCommand { CategoryId = id }, CancellationToken.None);

            _cacheMock.Verify(c => c.RemoveSync(CacheKeys.CategoryById(id), It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(c => c.RemoveSync(CacheKeys.AllCategories, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFalseAndDoesNotEvictCache_WhenCategoryNotFound()
        {
            var id = Guid.NewGuid();
            _categoryRepoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var result = await _handler.Handle(new DeleteCategoryCommand { CategoryId = id }, CancellationToken.None);

            Assert.False(result);
            _cacheMock.Verify(c => c.RemoveSync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // UpdateCategoryCommandHandler — evicts AllCategories + CategoryById on update
    // ─────────────────────────────────────────────────────────────────────────────
    public class UpdateCategoryCommandHandlerCacheTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
        private readonly Mock<ILoggingService<UpdateCategoryCommandHandler>> _loggerMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly UpdateCategoryCommandHandler _handler;

        public UpdateCategoryCommandHandlerCacheTests()
        {
            // Constructor: (ICategoryRepository, ILoggingService, IMapper, IUnitOfWork, ICacheService)
            _handler = new UpdateCategoryCommandHandler(
                _categoryRepoMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_EvictsAllCategoriesAndCategoryByIdCache_AfterSuccessfulUpdate()
        {
            var id = Guid.NewGuid();
            var category = new Category("Old Name", "description");
            var command = new UpdateCategoryCommand
            {
                CategoryUpdateDTO = new CategoryUpdateDTO { CategoryId = id, Name = "New Name" }
            };
            var responseDTO = new CategoryResponseDTO { CategoryId = id, IsSuccess = true };

            _categoryRepoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            _mapperMock
                .Setup(m => m.Map(It.IsAny<CategoryUpdateDTO>(), It.IsAny<Category>()));
            _mapperMock
                .Setup(m => m.Map<CategoryResponseDTO>(category))
                .Returns(responseDTO);
            _unitOfWorkMock
                .Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Mock.Of<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>());
            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            _cacheMock
                .Setup(c => c.RemoveSync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _handler.Handle(command, CancellationToken.None);

            _cacheMock.Verify(c => c.RemoveSync(CacheKeys.CategoryById(id), It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(c => c.RemoveSync(CacheKeys.AllCategories, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // DeleteProductCommandHandler — evicts AllProducts + ProductById on delete
    // ─────────────────────────────────────────────────────────────────────────────
    public class DeleteProductCommandHandlerCacheTests
    {
        private readonly Mock<IProductRepository> _productRepoMock = new();
        private readonly Mock<ILoggingService<DeleteProductCommandHandler>> _loggerMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductCommandHandlerCacheTests()
        {
            _handler = new DeleteProductCommandHandler(
                _productRepoMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object,
                _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_EvictsAllProductsAndProductByIdCache_AfterSuccessfulDelete()
        {
            var id = Guid.NewGuid();
            var product = Product.Create("Drill", "Heavy duty drill", 99m, 10, Guid.NewGuid(), Guid.NewGuid());
            // Capture the entity's actual Id (set internally by the factory method)
            var productEntityId = product.Id;

            _productRepoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _unitOfWorkMock
                .Setup(u => u.ExecuteInTransactionAsync(
                    It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<Task>, CancellationToken>((fn, ct) => fn());
            _cacheMock
                .Setup(c => c.RemoveSync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _handler.Handle(new DeleteProductCommand { ProductId = id }, CancellationToken.None);

            _cacheMock.Verify(c => c.RemoveSync(CacheKeys.AllProducts, It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(c => c.RemoveSync(CacheKeys.ProductById(productEntityId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFalseAndDoesNotEvictCache_WhenProductNotFound()
        {
            var id = Guid.NewGuid();
            _productRepoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var result = await _handler.Handle(new DeleteProductCommand { ProductId = id }, CancellationToken.None);

            Assert.False(result);
            _cacheMock.Verify(c => c.RemoveSync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // UpdateProductCommandHandler — evicts AllProducts + ProductById on update
    // ─────────────────────────────────────────────────────────────────────────────
    public class UpdateProductCommandHandlerCacheTests
    {
        private readonly Mock<IProductRepository> _productRepoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILoggingService<UpdateProductCommandHandler>> _loggerMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductCommandHandlerCacheTests()
        {
            // Constructor: (IProductRepository, IMapper, ILoggingService, IUnitOfWork, ICacheService)
            _handler = new UpdateProductCommandHandler(
                _productRepoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object,
                _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_EvictsAllProductsAndProductByIdCache_AfterSuccessfulUpdate()
        {
            var id = Guid.NewGuid();
            var product = Product.Create("Saw", "Old desc", 30m, 5, Guid.NewGuid(), Guid.NewGuid());
            // Capture entity's actual Id (assigned internally by factory method)
            var productEntityId = product.Id;
            var responseDTO = new ProductResponseDTO { IsSuccess = true };

            _productRepoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _mapperMock
                .Setup(m => m.Map(It.IsAny<ProductUpdateDTO>(), It.IsAny<Product>()));
            _mapperMock
                .Setup(m => m.Map<ProductResponseDTO>(product))
                .Returns(responseDTO);
            _unitOfWorkMock
                .Setup(u => u.ExecuteInTransactionAsync(
                    It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<Task>, CancellationToken>((fn, ct) => fn());
            _cacheMock
                .Setup(c => c.RemoveSync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var command = new UpdateProductCommand
            {
                ProductDTO = new ProductUpdateDTO
                {
                    ProductId = id,
                    Name = "Power Saw",
                    Description = "New desc",
                    Price = 55m,
                    StockQuantity = 8,
                    CategoryId = Guid.NewGuid(),
                    CraftmanId = Guid.NewGuid()
                }
            };

            await _handler.Handle(command, CancellationToken.None);

            _cacheMock.Verify(c => c.RemoveSync(CacheKeys.AllProducts, It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(c => c.RemoveSync(CacheKeys.ProductById(productEntityId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFalseAndDoesNotEvictCache_WhenProductNotFound()
        {
            var id = Guid.NewGuid();
            _productRepoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var command = new UpdateProductCommand
            {
                ProductDTO = new ProductUpdateDTO { ProductId = id, Name = "X" }
            };

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            _cacheMock.Verify(c => c.RemoveSync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
