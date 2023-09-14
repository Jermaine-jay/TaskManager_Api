namespace TaskManager.Models.Dtos.Request
{
    public class UpdateStatusRequest
    {
        public string taskId { get; set; }
        public int? Status { get; set; }
    }

    public class UpdatePriorityRequest
    {
        public string taskId { get; set; }
        public int? Priority { get; set; }
    }
}
