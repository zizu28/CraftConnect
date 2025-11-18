using Core.SharedKernel.ValueObjects;

namespace LocationServiceManagement.Domain.Contracts
{
	public interface ILocationService
	{
		Task<GeoLocation> GeocodeAddressAsync(Address address);
		Task<Address> ReverseGeocodeAsync(GeoLocation geolocation);
		decimal CalculateDistance(GeoLocation pointA, GeoLocation pointB);
	}
}
