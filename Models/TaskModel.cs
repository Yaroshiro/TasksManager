using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasksManager.Models
{
    public class TaskModel
    {
        public enum TaskStatus
        {
            [Display(Name = "In Progress")]
            InProgress,
            [Display(Name = "Confirmation Pending")]
            ConfirmationPending,
            Completed,
            Deleted
        }
        
        public enum TaskPriority
        {
            High,
            Medium,
            Low
        }

        public int Id { get; set; }

        [StringLength(100)]
        public required string Name { get; set; }

        public TaskStatus Status { get; set; }

        public TaskPriority Priority { get; set; }

        [DataType(DataType.Date)]
        [Display(Name ="Due to date")]
        public DateOnly? DueToDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Creation date")]
        public DateTime? CreationDate { get; set; }

        [Required]
        public required string Description { get; set; }

        public string? OwnerId { get; set; }
        [Display(Name = "Assigned by")]
        public ApplicationUser? Owner { get; set; }

        [MinLength(1, ErrorMessage = "Select at least one member to assign")]
        [Display(Name = "Assigned members")]
        public List<ApplicationUser> AssignedUsers { get; set; } = new List<ApplicationUser>();

        [DataType(DataType.Date)]
        [Display(Name = "Deleted date")]
        public DateOnly? DeletedDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Completion date")]
        public DateTime? CompletedDate { get; set; }        

        public List<TaskCommentModel> Comments { get; set; } = new List<TaskCommentModel>();

        public int? FileId { get; set; }

        [Display(Name="File Attachment")]
        public FileModel? File { get; set; }
    }
}
