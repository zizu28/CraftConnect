using AutoMapper;
using Core.Logging;
using Core.SharedKernel.DTOs;
using Infrastructure.Cache;
using Moq;
using ProductInventoryManagement.Application.Contracts;
using ProductInventoryManagement.Application.CQRS.Handlers.QueryHandlers.CategoriesQueryHandlers;
using ProductInventoryManagement.Application.CQRS.Handlers.QueryHandlers.ProductsQueryHandlers;
using ProductInventoryManagement.Application.CQRS.Queries.CategoryQueries;
using ProductInventoryManagement.Application.CQRS.Queries.ProductQueries;
using ProductInventoryManagement.Domain.Entities;
using System.Linq.Expressions;

namespace CraftConnect.Tests.Handlers.ProductQueryHandlers
{
    // ──────────────────────────────────────────────────────────────────────────────
    // GetAllProductsQueryHandler
    // ──────────────────────────────────────────────────────────────────────────────
    public class GetAllProductsQueryHandlerTests
    {
        private readonly Mock<IProductRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILoggingService<GetAllProductsQueryHandler>> _loggerMock = new();
        private readonly GetAllProductsQueryHandler _handler;
        public GetAllProductsQueryHandlerTests()
        {
            _handler = new GetAllProductsQueryHandler(
                _repoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsMappedProducts()
        {
            var products = new List<Product> { Product.Create("Drill", "Desc", 99m, 10, Guid.NewGuid(), Guid.NewGuid()) };
            var dtos = products.Select(p => new ProductResponseDTO { IsSuccess = true }).ToList();

            _repoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<ProductResponseDTO>>(products))
                .Returns(dtos);

            var result = await _handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

            Assert.Single(result);
            Assert.True(result.First().IsSuccess);
            _repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFailureDto_WhenNoneFound()
        {
            _repoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());

            var result = await _handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

            Assert.Single(result);
            Assert.False(result.First().IsSuccess);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // GetProductByIdQueryHandler
    // ──────────────────────────────────────────────────────────────────────────────
    public class GetProductByIdQueryHandlerTests
    {
        private readonly Mock<IProductRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILoggingService<GetProductByIdQueryHandler>> _loggerMock = new();
        private readonly GetProductByIdQueryHandler _handler;
        public GetProductByIdQueryHandlerTests()
        {
            _handler = new GetProductByIdQueryHandler(
                _repoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsMappedProduct()
        {
            var id = Guid.NewGuid();
            var product = Product.Create("Saw", "Desc", 49m, 5, Guid.NewGuid(), Guid.NewGuid());
            var dto = new ProductResponseDTO { ProductId = id, IsSuccess = true };

            _repoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _mapperMock.Setup(m => m.Map<ProductResponseDTO>(product)).Returns(dto);

            var result = await _handler.Handle(
                new GetProductByIdQuery { ProductId = id }, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _repoMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFailureDto_WhenProductNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null!);

            var result = await _handler.Handle(
                new GetProductByIdQuery { ProductId = id }, CancellationToken.None);

            Assert.False(result.IsSuccess);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // GetAllCategoriesQueryHandler
    // ──────────────────────────────────────────────────────────────────────────────
    public class GetAllCategoriesQueryHandlerTests
    {
        private readonly Mock<ICategoryRepository> _repoMock = new();
        private readonly Mock<ILoggingService<GetAllCategoriesQueryHandler>> _loggerMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly GetAllCategoriesQueryHandler _handler;
        public GetAllCategoriesQueryHandlerTests()
        {
            _handler = new GetAllCategoriesQueryHandler(
                _repoMock.Object,
                _loggerMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsMappedCategories()
        {
            var categories = new List<Category> { new("Tools", "Category of tools") };
            var dtos = categories.Select(_ => new CategoryResponseDTO { IsSuccess = true }).ToList();

            _repoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<CategoryResponseDTO>>(categories))
                .Returns(dtos);

            var result = await _handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

            Assert.Single(result);
            Assert.True(result.First().IsSuccess);
            _repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFailureDto_WhenNoneFound()
        {
            _repoMock
                .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Category>());

            var result = await _handler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

            Assert.Single(result);
            Assert.False(result.First().IsSuccess);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // GetCategoryByIdQueryHandler
    // ──────────────────────────────────────────────────────────────────────────────
    public class GetCategoryByIdQueryHandlerTests
    {
        private readonly Mock<ICategoryRepository> _repoMock = new();
        private readonly Mock<ILoggingService<GetCategoryByIdQueryHandler>> _loggerMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly GetCategoryByIdQueryHandler _handler;
        public GetCategoryByIdQueryHandlerTests()
        {
            _handler = new GetCategoryByIdQueryHandler(
                _repoMock.Object,
                _loggerMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsMappedCategory()
        {
            var id = Guid.NewGuid();
            var category = new Category("Electrical", "Electrical category");
            var dto = new CategoryResponseDTO { CategoryId = id, IsSuccess = true };

            _repoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            _mapperMock.Setup(m => m.Map<CategoryResponseDTO>(category)).Returns(dto);

            var result = await _handler.Handle(
                new GetCategoryByIdQuery { Id = id }, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _repoMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFailureDto_WhenNotFound()
        {
            var id = Guid.NewGuid();
            _repoMock
                .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category)null!);

            var result = await _handler.Handle(
                new GetCategoryByIdQuery { Id = id }, CancellationToken.None);

            Assert.False(result.IsSuccess);
        }
    }
}
