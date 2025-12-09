using Core.SharedKernel.Contracts;
using System.Net.Http.Json;

namespace BookingManagement.Application.Services
{
	public class UserModuleHttpService(IHttpClientFactory httpClientFactory) : IUserModuleService
	{
		private readonly HttpClient _httpClient = httpClientFactory.CreateClient("UserModule");
		public async Task<string?> GetCraftsmanNameAsync(Guid craftsmanId, CancellationToken ct = default)
		{
			try
			{
				var response = await _httpClient.GetFromJsonAsync<string>($"/api/users/get-name/{craftsmanId}", ct);
				return response;
			}
			catch(HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				return null;
			}			
		}

		public async Task<Dictionary<Guid, string>> GetCraftsmanNamesAsync(IEnumerable<Guid> craftsmanIds, CancellationToken ct = default)
		{
			var uniqueIds = craftsmanIds.Distinct().ToList();
			if(!(uniqueIds.Count > 0)) return [];
			try
			{
				var response = await _httpClient.PostAsJsonAsync("/api/users/craftsman/batch-names", uniqueIds, ct);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadFromJsonAsync<Dictionary<Guid, string>>(cancellationToken: ct);
					return result ?? [];
				}

			}
			catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				return [];
			}
			return [];
		}

		public async Task<string?> GetCustomerNameAsync(Guid customerId, CancellationToken ct = default)
		{
			return await GetCraftsmanNameAsync(customerId, ct);
		}

		public async Task<bool> IsCraftsmanValidAsync(Guid craftsmanId, CancellationToken ct = default)
		{
			try
			{
				var request = new HttpRequestMessage(HttpMethod.Head, $"/api/users/craftsman/{craftsmanId}");
				var response = await _httpClient.SendAsync(request, ct);
				return response.IsSuccessStatusCode;
			}
			catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				return false;
			}
		}
	}
}
