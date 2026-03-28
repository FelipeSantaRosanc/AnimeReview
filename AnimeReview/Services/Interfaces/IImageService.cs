namespace AnimeReview.Services.Interfaces
{
    public interface IImageService
    {
        Task<ImageUploadResult> UploadAsync(IFormFile file, string folder = "animes");
        Task<bool> DeleteAsync(string publicId);

    }
    public class ImageUploadResult
    {
        public bool Success { get; set; }
        public string? Url { get; set; }
        public string? PublicId { get; set; }
        public string? Error { get; set; }
    }
}
