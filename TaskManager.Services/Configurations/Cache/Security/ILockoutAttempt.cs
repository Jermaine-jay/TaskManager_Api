using TaskManager.Services.Configurations.Cache.Otp;

namespace TaskManager.Services.Configurations.Cache.Security
{
    public interface ILockoutAttempt
    {
        Task<string> LoginAttemptAsync(string userId);
        Task<OtpCodeDto> CheckLoginAttemptAsync(string userId);
        Task ResetLoginAttemptAsync(string userId);
    }
}
