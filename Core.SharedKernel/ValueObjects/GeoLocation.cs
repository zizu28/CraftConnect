namespace Core.SharedKernel.ValueObjects
{
	public record GeoLocation
	{
		public double Latitude { get; init; } = 0.0;
		public double Longitude { get; init; } = 0.0;

		private GeoLocation() 
		{
			Latitude = 0.0;
			Longitude = 0.0;
		}
		public GeoLocation(double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}

		public bool IsValid()
		{
			return Latitude >= -90 && Latitude <= 90 && Longitude >= -180 && Longitude <= 180;
		}
		public double CalculateDistance(GeoLocation other)
		{
			var R = 6371e3; // meters
			var φ1 = Latitude * Math.PI / 180; // φ in radians
			var φ2 = other.Latitude * Math.PI / 180;
			var Δφ = (other.Latitude - Latitude) * Math.PI / 180;
			var Δλ = (other.Longitude - Longitude) * Math.PI / 180;
			var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
					Math.Cos(φ1) * Math.Cos(φ2) *
					Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			return R * c; // in meters
		}
	}
}
