using TaskManager.Services.Configurations.Cache.CacheServices;
using TaskManager.Services.Configurations.Cache.Otp;


namespace TaskManager.Services.Configurations.Cache.Security
{
    public class LockoutAttempt : ILockoutAttempt
    {
        private readonly ICacheService _cacheService;

        public LockoutAttempt(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }


        public async Task<string> LoginAttemptAsync(string userId)
        {
            string cacheKey = CacheKeySelector.AccountLockoutCacheKey(userId);
            var numberofattempts = await _cacheService.ReadFromCache<OtpCodeDto>(cacheKey);

            if (numberofattempts != null)
                numberofattempts.Attempts = numberofattempts.Attempts;
            else
                numberofattempts = new OtpCodeDto("");

            await _cacheService.WriteToCache(cacheKey, numberofattempts, null, TimeSpan.FromDays(365));
            return cacheKey;
        }

        public async Task<OtpCodeDto> CheckLoginAttemptAsync(string userId)
        {
            string cacheKey = CacheKeySelector.AccountLockoutCacheKey(userId);
            var numberofattempts = await _cacheService.ReadFromCache<OtpCodeDto>(cacheKey);

            return numberofattempts;
        }


        public async Task ResetLoginAttemptAsync(string userId)
        {
            var attempt = 0;
            string cacheKey = CacheKeySelector.AccountLockoutCacheKey(userId);
            await _cacheService.ClearFromCache(cacheKey);
        }
    }
}
