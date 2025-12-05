using Core.SharedKernel.Contracts;
using System.Net.Http.Json;

namespace BookingManagement.Application.Services
{
	public class UserModuleHttpService(IHttpClientFactory httpClientFactory) : IUserModuleService
	{
		private readonly HttpClient _httpClient = httpClientFactory.CreateClient("UserModule");
		public async Task<string?> GetCraftsmanNameAsync(Guid craftsmanId, CancellationToken ct = default)
		{
			//var response = await _httpClient.GetFromJsonAsync<CraftmanResponseDTO>($"/api/users/{craftsmanId}/name", ct);
			//return response?.Name;
			throw new NotImplementedException();
		}

		public Task<Dictionary<Guid, string>> GetCraftsmanNamesAsync(IEnumerable<Guid> craftsmanIds, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<bool> IsCraftsmanValidAsync(Guid craftsmanId, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}
	}
}
