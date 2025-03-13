using SparkLink.Data;
using SparkLink.Service.Interface;

namespace SparkLink.Service.Implementation
{
    public class PhotoService : IPhotoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _environment;
        public PhotoService(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _environment = webHostEnvironment;

        }
        public async Task<string> UploadPhoto(IFormFile profilePicture)
        {
            if (profilePicture == null || profilePicture.Length == 0)
            {
                return "No file uploaded.";
            }

            var allowedExtensions = new string[] { ".jpeg", ".png", ".jpg" };
            var fileExtension = Path.GetExtension(profilePicture.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return "Invalid file format. Only JPEG, PNG, and JPG are allowed.";
            }

            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

         
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "images", "profilePictures");

     
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

       
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePicture.CopyToAsync(stream);
            }

            
            var request = _httpContextAccessor.HttpContext.Request;
            var imageUrl = $"{request.Scheme}://{request.Host}/images/profilePictures/{uniqueFileName}";

            return imageUrl;
        }


    }
}
