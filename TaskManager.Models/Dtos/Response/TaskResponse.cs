namespace TaskManager.Models.Dtos.Response
{
    public class TaskResponse
    {
        public string Title { get; set; }
        public string DueDate { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}
