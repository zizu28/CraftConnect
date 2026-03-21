using UserManagement.Application.Contracts;

namespace UserManagement.Infrastructure.RepositoryImplementations
{
	public class S3FileStorageService : IFileStorageService
	{
		public Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
	}
}
