using Org.BouncyCastle.Asn1.Crmf;
using SparkLink.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fikra.Models
{
    public class Signature

    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id{ get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; }

        public string Sign {  get; set; }
    }
}
