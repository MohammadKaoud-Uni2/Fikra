namespace Fikra.Models.Dto
{
    public class GetProfileDto
    {
      public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string FatherName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string? CompanyName { get; set; }
        public string? LinkedinUrl { get; set; }
        public string? ImageProfileUrl { get; set; }
    }
}
