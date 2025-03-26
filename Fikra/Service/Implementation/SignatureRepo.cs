using Fikra.Models;
using Fikra.Service.Interface;
using SparkLink.Data;
using SparkLink.Service.Implementation;


namespace Fikra.Service.Implementation
{
    public class SignatureRepo:GenericRepo<Signature>,ISignatureRepo
    {
        public SignatureRepo(ApplicationDbContext context) : base(context) { 

        }
    }
}
