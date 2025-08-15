﻿using System.Text.Json.Serialization;

namespace UserManagement.Application.DTOs.UserDTOs
{
	public class UserUpdateDTO
	{
		public Guid UserId { get; set; }
		public required string Email { get; set; }
		public required string PhoneCountryCode { get; set; }
		public required string PhoneNumber { get; set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public required string Role { get; set; }
	}


}
