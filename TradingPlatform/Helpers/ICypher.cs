namespace TradingPlatform.Helpers
{
    interface ICypher
    {
        string CreateSignature(string data);
        bool CheckSignature(string data, string signature);
        
    }
}