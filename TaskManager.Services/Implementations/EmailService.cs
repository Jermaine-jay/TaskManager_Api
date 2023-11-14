﻿using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json;
using TaskManager.Models.Entities;
using TaskManager.Services.Configurations.Cache.Otp;
using TaskManager.Services.Configurations.Email;
using TaskManager.Services.Interfaces;


namespace TaskManager.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly ZeroBounceConfig _zeroBounceConfig;
        private readonly EmailSenderOptions _emailSenderOptions;
        private readonly IServiceFactory _serviceFactory;
        private readonly IConfiguration _configuration;
        private readonly LinkGenerator _link;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(IConfiguration configuration, ZeroBounceConfig zeroBounceconfig,LinkGenerator linkGenerator, 
            EmailSenderOptions emailSenderOptions, IServiceFactory serviceFactory,IHttpContextAccessor httpContextAccessor)
        {
            _zeroBounceConfig = zeroBounceconfig;
            _emailSenderOptions = emailSenderOptions;
            _serviceFactory = serviceFactory;
            _configuration = configuration;
            _link = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<object> SendEmailAsync(string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(_emailSenderOptions.Password))
                throw new InvalidOperationException($"Invalid Email configuration Details");

            await Execute(email, subject, message);
            return true;
        }


        public async Task<bool> Execute(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("TaskManager", _emailSenderOptions.Username));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlMessage;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {

                client.Connect(_emailSenderOptions.SmtpServer, _emailSenderOptions.Port, true);
                client.Authenticate(_emailSenderOptions.Email, _emailSenderOptions.Password);
                client.Send(message);
                client.Disconnect(true);
            }

            return true;
        }

        public async Task<bool> VerifyEmailAddress(string emailAddress)
        {
            using (var httpClient = new HttpClient())
            {
                var parameters = $"api_key={_zeroBounceConfig.ApiKey}&email={emailAddress}";
                var response = await httpClient.GetAsync($"{_zeroBounceConfig.Url}?{parameters}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var getResponse = JsonConvert.DeserializeObject<dynamic>(responseContent).status;
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
            var validToken = await _serviceFactory.GetService<IOtpService>().GenerateUniqueOtpAsync(user.Id.ToString(), OtpOperation.EmailConfirmation);

            string appUrl = $"{_configuration["AppUrl:Url"]}api/Auth/confirm-email?Token={validToken}";
            await SendEmailAsync(user.Email, "Confirm your email", page(user.FirstName, appUrl));
            return true;
        }


        public async Task<string> ResetPasswordMail(ApplicationUser user)
        {
            var validToken = await _serviceFactory.GetService<IOtpService>().GenerateUniqueOtpAsync(user.Id.ToString(), OtpOperation.PasswordReset);
            string appUrl = $"{_configuration["AppUrl:Url"]}api/Auth/reset-password?Token={validToken}";


            var page = _serviceFactory.GetService<IGenerateEmailPage>().PasswordResetPage(appUrl);
            await SendEmailAsync(user.Email, "Reset Password", page);
            return validToken;
        }

    }
}
