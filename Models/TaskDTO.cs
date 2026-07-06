using System.ComponentModel.DataAnnotations;

namespace TasksManager.Models
{
    public class TaskDTO
    {
        public ICollection<ApplicationUser>? Users { get; set; } = new List<ApplicationUser>();
        public required string Name { get; set; }
        public required string Description { get; set; }
        public TaskModel.TaskStatus Status { get; set; }
        [DataType(DataType.Date)]
        [Display(Name="Due to date")]
        public DateOnly? DueToDate { get; set; }
        [Display(Name="Assigned members")]
        public List<string> SelectedUserIds { get; set; } = [];
    }
}
