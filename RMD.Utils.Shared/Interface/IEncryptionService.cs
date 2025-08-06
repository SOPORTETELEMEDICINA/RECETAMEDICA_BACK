namespace RMD.Shared.Utils.Interface
{
    public interface IEncryptionService
    {
        string EncryptString(string plainText);
        string DecryptString(string ivBase64, string cipherTextBase64);
    }
}
