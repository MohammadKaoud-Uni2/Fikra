using SparkLink.Data;
using SparkLink.Service.Interface;
using System.Linq;

namespace SparkLink.Service.Implementation
{
    public class PhotoService : IPhotoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        public PhotoService(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment,IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _environment = webHostEnvironment;
            _configuration = configuration;

        }

        public async Task<string> UploadComplaint(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "No file uploaded.";
            }

            var allowedExtensions = new string[] { ".jpeg", ".png", ".jpg" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return "Invalid file format. Only JPEG, PNG, and JPG are allowed.";
            }

            var uniqueFileName = $"{file.FileName}";


            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "images", "complaints");


            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var customUrl = _configuration["AppSettings:BaseUrl"];
            var request = _httpContextAccessor.HttpContext.Request;
            var imageUrl = $"{customUrl}complaints/{uniqueFileName}";

            return imageUrl;
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

            var uniqueFileName = $"{profilePicture.FileName}";

         
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

            var customUrl = _configuration["AppSettings:BaseUrl"];
            var request = _httpContextAccessor.HttpContext.Request;
            var imageUrl = $"{customUrl}images/profilePictures/{uniqueFileName}";

            return imageUrl;
        }


    }
}
