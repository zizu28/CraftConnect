using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using UserManagement.Application.Contracts;

namespace UserManagement.Infrastructure.RepositoryImplementations
{
	public class CloudinaryStorageService : IFileStorageService
	{
		private readonly Cloudinary cloudinary;
		public CloudinaryStorageService(IConfiguration config)
		{
			var account = new Account
			{
				ApiKey = config["Cloudinary:ApiKey"],
				ApiSecret = config["Cloudinary:ApiSecret"],
				Cloud = config["Cloudinary:CloudName"]
			};
			cloudinary = new Cloudinary(account);
		}
		public async Task DeleteFileAsync(string fileUrl)
		{
			string publicId = ExtractPublicIdFromUrl(fileUrl);
			var deleteParams = new DeletionParams(publicId);
			await cloudinary.DestroyAsync(deleteParams);
		}

		private static string ExtractPublicIdFromUrl(string fileUrl)
		{
			try
			{
				var uri = new Uri(fileUrl);
				var path = uri.AbsolutePath; // "/demo/image/upload/v123456789/craftconnect/avatars/user123.jpg"
				var uploadToken = "upload/";
				var uploadIndex = path.IndexOf(uploadToken, StringComparison.OrdinalIgnoreCase);
				if (uploadIndex == -1) throw new ArgumentException("Not a valid Cloudinary URL");
				var pathAfterUpload = path.Substring(uploadIndex + uploadToken.Length); // Result: "v123456789/craftconnect/avatars/user123.jpg"
				var segments = pathAfterUpload.Split('/');
				var segmentsToKeep = (segments[0].StartsWith("v") && segments[0].Any(char.IsDigit))
				? segments.Skip(1)
				: segments; // If the first segment looks like a version (v123...), skip it
				var publicIdWithExtension = string.Join("/", segmentsToKeep); // Result: "craftconnect/avatars/user123.jpg"
				var lastDotIndex = publicIdWithExtension.LastIndexOf('.');
				if (lastDotIndex > 0)
				{
					return publicIdWithExtension.Substring(0, lastDotIndex);
				}

				return publicIdWithExtension;
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
		{
			if (fileStream.Length == 0) return string.Empty;
			fileStream.Position = 0;
			var imageUploadParams = new ImageUploadParams
			{
				File = new FileDescription(fileName, fileStream),
				Folder = "craftconnect/avatars",
				Overwrite = false,
				PublicId = Path.GetFileNameWithoutExtension(fileName),
				
			};
			var uploadResult = await cloudinary.UploadAsync(imageUploadParams);
			if (uploadResult.Error != null)
			{
				throw new Exception($"Cloudinary Upload Error: {uploadResult.Error.Message}");
			}
			return uploadResult.SecureUrl.ToString();
		}
	}
}
