using System.ComponentModel.DataAnnotations;

namespace TasksManager.Models
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        public List<ApplicationUser>? Users { get; set; } = new List<ApplicationUser>();

        public required string Name { get; set; }

        public required string Description { get; set; }

        public TaskModel.TaskStatus Status { get; set; }

        public TaskModel.TaskPriority Priority { get; set; }

        [Display(Name = "Assigned by")]
        public string? OwnerName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Creation date")]
        public DateOnly? CreationDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name="Due to date")]
        public DateOnly? DueToDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Completion date")]
        public DateOnly? CompletedDate { get; set; }

        [MinLength(1, ErrorMessage ="Select at least one member to assign")]  
        [Display(Name="Assigned members")]
        public List<string> AssignedUserIds { get; set; } = [];

        [Display(Name = "Assigned members")]
        public List<string> AssignedUserNames { get; set; } = [];

        public int? FileId { get; set; }

        public string? OriginalFileName { get; set; }

        [Display(Name = "File Attachment")]
        public IFormFile? File { get; set; }
    }
}
