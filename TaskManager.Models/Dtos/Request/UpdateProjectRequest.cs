namespace TaskManager.Models.Dtos.Request
{
    public class UpdateProjectRequest
    {
        public string? ProjectId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
