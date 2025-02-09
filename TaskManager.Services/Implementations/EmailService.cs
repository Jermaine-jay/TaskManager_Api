using MimeKit;
using Newtonsoft.Json;
using MailKit.Net.Smtp;
using TaskManager.Models.Enums;
using Microsoft.AspNetCore.Http;
using TaskManager.Models.Entities;
using TaskManager.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using TaskManager.Services.Infrastructure;
using Task = TaskManager.Models.Entities.Task;
using TaskManager.Services.Configurations.Email;
using TaskManager.Services.Configurations.Cache.Otp;
using System.Text.RegularExpressions;


namespace TaskManager.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly Settings _settings;
        private readonly AppConstants _appConstants;
        private readonly ZeroBounceConfig _zeroBounce;
        private readonly IConfiguration _configuration;
        private readonly IServiceFactory _serviceFactory;
        private readonly EmailSenderOptions _senderOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(IConfiguration configuration, EmailSenderOptions senderOptions, 
            AppConstants appConstants, IServiceFactory serviceFactory, IHttpContextAccessor httpContextAccessor, 
            Settings settings, ZeroBounceConfig zeroBounce)
        {
            _settings = settings;
            _zeroBounce = zeroBounce;
            _appConstants = appConstants;
            _configuration = configuration;
            _senderOptions = senderOptions;
            _serviceFactory = serviceFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<object> SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(_senderOptions.Password))
                throw new InvalidOperationException($"Invalid Email configuration Details");

            await Execute(email, subject, message);
            return true;
        }

        public async Task<bool> Execute(string email, string subject, string htmlMessage)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.AppName, _senderOptions.Username));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlMessage;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {

                client.Connect(_senderOptions.SmtpServer, _senderOptions.Port, true);
                client.Authenticate(_senderOptions.Email, _senderOptions.Password);
                client.Send(message);
                client.Disconnect(true);
            }

            return true;
        }

        public async Task<bool> VerifyEmailAddress(string emailAddress)
        {
            using (var httpClient = new HttpClient())
            {
                string parameters = $"api_key={_zeroBounce.ApiKey}&email={emailAddress}";
                HttpResponseMessage response = await httpClient.GetAsync($"{_zeroBounce.Url}?{parameters}");
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();
                dynamic getResponse = JsonConvert.DeserializeObject<dynamic>(responseContent).status;
                if (getResponse == "valid")
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> RegistrationMail(ApplicationUser user)
        {
            var page = _serviceFactory.GetService<IGenerateEmailPage>().EmailVerificationPage;
            string validToken = await _serviceFactory.GetService<IOtpService>().GenerateUniqueOtpAsync(user.Id.ToString(), OtpOperation.EmailConfirmation);

            string appUrl = $"{_appConstants.AppUrl}/api/Auth/confirm-email?token={validToken}";
            await SendEmailAsync(user.Email, "Confirm your email", page(user.FirstName, appUrl));

            return true;
        }

        public async Task<string> ResetPasswordMail(ApplicationUser user)
        {
            string validToken = await _serviceFactory.GetService<IOtpService>().GenerateUniqueOtpAsync(user.Id.ToString(), OtpOperation.PasswordReset);
            string appUrl = $"{_appConstants.AppUrl}api/Auth/reset-password?Token={validToken}";

            string page = _serviceFactory.GetService<IGenerateEmailPage>().PasswordResetPage(appUrl);
            await SendEmailAsync(user.Email, "Reset Password", page);

            return validToken;
        }

        public async Task<bool> TaskMail(string taskId, string email, string message, NotificationType notification)
        {
            string appUrl = $"{_appConstants.AppUrl}api/Task/get-task/{taskId}";
            string page = _serviceFactory.GetService<IGenerateEmailPage>().TaskNotificationPage(message, appUrl, notification);

            string subject = Regex.Replace(notification.GetStringValue(), "(?<!^)([A-Z])", " $1");

            await SendEmailAsync(email, subject, page);

            return true;
        }

    }
}
