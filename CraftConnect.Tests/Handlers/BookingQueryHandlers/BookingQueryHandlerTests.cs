using AutoMapper;
using BookingManagement.Application.Contracts;
using BookingManagement.Application.CQRS.Handlers.QueryHandlers.BookingQueriesHandlers;
using BookingManagement.Application.CQRS.Queries.BookingQueries;
using BookingManagement.Domain.Entities;
using Core.SharedKernel.DTOs;
using Core.SharedKernel.ValueObjects;
using Infrastructure.Cache;
using Moq;
using System.Linq.Expressions;

namespace CraftConnect.Tests.Handlers.BookingQueryHandlers
{
    // ──────────────────────────────────────────────────────────────────────────────
    // GetAllBookingQueryHandler
    // ──────────────────────────────────────────────────────────────────────────────
    public class GetAllBookingQueryHandlerTests
    {
        private readonly Mock<IBookingRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly GetAllBookingQueryHandler _handler;

        public GetAllBookingQueryHandlerTests()
        {
            _handler = new GetAllBookingQueryHandler(
                _repoMock.Object,
                _mapperMock.Object,
                _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsMappedBookings_OnCacheHit()
        {
            // Arrange
            var address = new Address("Street", "Lagos", "12345");
            var bookings = new List<Booking>
            {
                Booking.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), address, "Job1", DateTime.UtcNow, DateTime.UtcNow.AddDays(1)),
                Booking.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), address, "Job2", DateTime.UtcNow, DateTime.UtcNow.AddDays(1))
            };
            var dtos = new List<BookingResponseDTO>
            {
                new() { IsSuccess = true },
                new() { IsSuccess = true }
            };

            _cacheMock
                .Setup(c => c.GetOrCreateManyAsync<Booking>(
                    CacheKeys.AllBookings, It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<BookingResponseDTO>>(bookings))
                .Returns(dtos);

            // Act
            var result = await _handler.Handle(new GetAllBookingQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.True(r.IsSuccess));

            _cacheMock.Verify(c => c.GetOrCreateManyAsync<Booking>(
                CacheKeys.AllBookings, It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsArgumentNullException_WhenCacheAndDbReturnNull()
        {
            _cacheMock
                .Setup(c => c.GetOrCreateManyAsync<Booking>(
                    CacheKeys.AllBookings, It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<Booking>?)null);

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _handler.Handle(new GetAllBookingQuery(), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UsesAllBookingsCacheKey()
        {
            _cacheMock
                .Setup(c => c.GetOrCreateManyAsync<Booking>(
                    It.IsAny<string>(), It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Booking>());

            _mapperMock
                .Setup(m => m.Map<IEnumerable<BookingResponseDTO>>(It.IsAny<IEnumerable<Booking>>()))
                .Returns(new List<BookingResponseDTO>());

            await _handler.Handle(new GetAllBookingQuery(), CancellationToken.None);

            // Verify the correct cache key is used
            _cacheMock.Verify(c => c.GetOrCreateManyAsync<Booking>(
                CacheKeys.AllBookings, It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // GetBookingByIdQueryHandler
    // ──────────────────────────────────────────────────────────────────────────────
    public class GetBookingByIdQueryHandlerTests
    {
        private readonly Mock<IBookingRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly GetBookingByIdQueryHandler _handler;

        public GetBookingByIdQueryHandlerTests()
        {
            _handler = new GetBookingByIdQueryHandler(
                _repoMock.Object,
                _mapperMock.Object,
                _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsMappedBooking_OnCacheHit()
        {
            var id = Guid.NewGuid();
            var address = new Address("Street", "Abuja", "00000");
            var booking = Booking.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), address, "Fix plumbing", DateTime.UtcNow, DateTime.UtcNow.AddDays(2));
            var dto = new BookingResponseDTO { BookingId = id, IsSuccess = true };

            _cacheMock
                .Setup(c => c.GetOrCreateAsync<Booking>(
                    CacheKeys.BookingById(id), It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            _mapperMock.Setup(m => m.Map<BookingResponseDTO>(booking)).Returns(dto);

            var result = await _handler.Handle(
                new GetBookingByIdQuery { Id = id }, CancellationToken.None);

            Assert.Same(dto, result);
            _cacheMock.Verify(c => c.GetOrCreateAsync<Booking>(
                CacheKeys.BookingById(id), It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_UsesPerBookingCacheKey()
        {
            var id = Guid.NewGuid();
            var otherId = Guid.NewGuid();
            var address = new Address("Avenue", "Port Harcourt", "00001");
            var booking = Booking.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), address, "Fix electrical", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

            _cacheMock
                .Setup(c => c.GetOrCreateAsync<Booking>(
                    CacheKeys.BookingById(id), It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);

            _mapperMock
                .Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(new BookingResponseDTO());

            await _handler.Handle(new GetBookingByIdQuery { Id = id }, CancellationToken.None);

            // The per-booking key was used, not a different booking's key
            _cacheMock.Verify(c => c.GetOrCreateAsync<Booking>(
                CacheKeys.BookingById(otherId), It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
