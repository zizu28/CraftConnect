using UserManagement.Application.Contracts;

namespace UserManagement.Infrastructure.RepositoryImplementations
{
	public class S3FileStorageService : IFileStorageService
	{
		public Task DeleteFileAsync(string fileUrl)
		{
			throw new NotImplementedException();
		}

		public Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
		{
			throw new NotImplementedException();
		}
	}
}
