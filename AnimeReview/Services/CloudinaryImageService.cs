using AnimeReview.Services.Interfaces;
using CloudinaryDotNet;

namespace AnimeReview.Services
{
    public class CloudinaryImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"] ?? "dummy",
                configuration["Cloudinary:ApiKey"] ?? "dummy",
                configuration["Cloudinary:ApiSecret"] ?? "dummy");

            _cloudinary = new Cloudinary(account);
        }

        public async Task<ImageUploadResult> UploadAsync(IFormFile file, string folder = "animes")
        {
            if (file.Length == 0)
                return new ImageUploadResult { Success = false, Error = "Empty file" };

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType))
                return new ImageUploadResult { Success = false, Error = "Invalid file type" };

            if (file.Length > 5 * 1024 * 1024)
                return new ImageUploadResult { Success = false, Error = "File too large. Max 5MB" };

            using var stream = file.OpenReadStream();
            var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                Transformation = new Transformation()
                    .Width(800)
                    .Height(600)
                    .Crop("fill")
                    .Gravity("auto")
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                return new ImageUploadResult { Success = false, Error = result.Error.Message };

            return new ImageUploadResult
            {
                Success = true,
                Url = result.SecureUrl?.ToString(),
                PublicId = result.PublicId
            };
        }

        public async Task<bool> DeleteAsync(string publicId)
        {
            var deleteParams = new CloudinaryDotNet.Actions.DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }


    }
}
