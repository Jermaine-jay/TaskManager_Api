﻿using TaskManager.Models.Dtos.Request;
using TaskManager.Models.Dtos.Response;
using TaskManager.Services.Infrastructure;

namespace TaskManager.Services.Interfaces
{
    public interface ITaskService
    {
        Task<CreateTaskResponse> CreateTask(string userId, CreateTaskRequest request);
        Task<SuccessResponse> UpdateTask(string userId, UpdateTaskRequest request);
        Task<UpdateTaskResponse> UpdatePriority(UpdatePriorityRequest request);
        Task<UpdateTaskResponse> UpdateStatus(UpdateStatusRequest request);
        Task<SuccessResponse> DeleteTask(string taskId, string userId);
        System.Threading.Tasks.Task AllTask();
    }
}
