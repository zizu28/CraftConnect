namespace UserManagement.Application.Contracts
{
	public interface IFileStorageService
	{
		Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
		Task DeleteFileAsync(string fileUrl);
	}
}
