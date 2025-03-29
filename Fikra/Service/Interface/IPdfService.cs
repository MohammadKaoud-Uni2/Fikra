namespace Fikra.Service.Interface
{
    public interface IPdfService
    {
        public Task<string> GenerateContract(string ideaOwnerFullName, string investorFullName, double budget, DateTime date,
                                  string ownerSignature, string investorSignature, byte[] logoBytes);
        public Task<byte[]> ReciveImage();
    }
}
