using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using UserManagement.Application.Contracts;

namespace UserManagement.Infrastructure.RepositoryImplementations
{
	public class LocalFileStorageService(
		IWebHostEnvironment env,
		IHttpContextAccessor httpContextAccessor) : IFileStorageService
	{
		private readonly IWebHostEnvironment _env = env;
		private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

		public Task DeleteFileAsync(string fileUrl)
		{
			var fileName = Path.GetFileName(fileUrl);
			var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
			if (File.Exists(filePath)) File.Delete(filePath);
			return Task.CompletedTask;
		}

		public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
		{
			var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
			var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
			if(!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
			var filePath = Path.Combine(uploadsFolder, uniqueFileName);
			using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
			await fileStream.CopyToAsync(fileStreamOutput);
			var request = _httpContextAccessor.HttpContext!.Request;
			var baseUrl = $"{request.Scheme}://{request.Host}";
			return $"{baseUrl}/uploads/{uniqueFileName}";
		}
	}
}
