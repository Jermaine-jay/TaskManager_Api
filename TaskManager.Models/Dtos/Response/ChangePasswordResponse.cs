namespace TaskManager.Models.Dtos.Response
{
    public class ChangePasswordResponse
    {
        public string? Message { get; set; }
        public string? Token { get; set; }
        public bool Success { get; set; }
    }
}
