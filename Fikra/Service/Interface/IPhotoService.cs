namespace SparkLink.Service.Interface
{
    public interface IPhotoService
    {
        public Task<string>UploadPhoto(IFormFile file);
        public Task<string>UploadComplaint(IFormFile file); 

    }
}
