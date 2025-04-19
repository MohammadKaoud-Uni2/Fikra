namespace Fikra.Models.Dto
{
    public class UpdateUserDto
    {
        public string ?UserName { get; set; }    
        public string ?Email { get; set; }
        public string ?CompanyName { get; set; }
        public string ?ImageProfileUrl { get; set; }
        public string ?LinkedInUrl { get; set; }
        public string ?PhoneNumber { get; set; }

    }
}
