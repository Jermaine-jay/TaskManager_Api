using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models.Enums
{
    public enum Priority
    {
        Low,
        Medium,
        High
    }

    public static class PriorityTypeExtension
    {
        public static string? GetStringValue(this Priority priority)
        {
            return priority switch
            {
                Priority.Low => "Low",
                Priority.Medium => "Medium",
                Priority.High => "High",
                _ => null
            };
        }
    }
}
