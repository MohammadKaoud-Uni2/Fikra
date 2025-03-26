namespace Fikra.Service.Interface
{
    public interface IRSAService
    {
        string SignData(string data);
        bool VerifySignature(string data, string signature);
    }
}
