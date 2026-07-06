using Microsoft.AspNetCore.Identity;

namespace TasksManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public enum RoleType
        {
            Member,
            Leader
        }

        public required string Name { get; set; }
        public RoleType Role { get; set; }
        public ICollection<TaskModel>? AssignedTasks { get; set; } = new List<TaskModel>(); // Member's tasks
        public ICollection<TaskModel>? OwnedTasks { get; set; } = new List<TaskModel>(); // Leader's created tasks
    }
}
