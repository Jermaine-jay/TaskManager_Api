using System.Text;

namespace TaskManager.Services.Utilities
{
    public static class DecodeToken
    {
        public static async Task<(string user, string operation)> DecodeVerificationToken(string validToken)
        {
            int opslen = (validToken.Count() - 84);
            string getstring = validToken.Substring(opslen, 48);
            string getoperation = validToken.Substring(0, opslen);

            byte[] getUserId = Convert.FromBase64String(getstring);
            byte[] operation = Convert.FromBase64String(getoperation);

            string decodedStr = Encoding.ASCII.GetString(getUserId);
            string ops = Encoding.UTF8.GetString(operation);

            return (decodedStr, ops);
        }
    }
}
