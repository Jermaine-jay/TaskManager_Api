namespace TaskManager.Services.Interfaces
{
    public interface IGenerateEmailPage
    {
        public string EmailVerificationPage(string name, string token);

        string PasswordResetPage(string callbackurl);
    }
}
