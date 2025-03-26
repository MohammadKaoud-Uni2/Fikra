namespace Fikra.Service.Interface
{
    public interface IPdfService
    {
        public Task<string> GenerateContract(string ideaOwner, string investor, decimal budget, DateTime date,
                                  string ownerSignature, string investorSignature, byte[] logoBytes);
        public Task<byte[]> ReciveImage();
    }
}
