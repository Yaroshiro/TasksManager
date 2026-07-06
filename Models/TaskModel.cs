using System.ComponentModel.DataAnnotations;

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

        public int Id { get; set; }
        [StringLength(100)]
        public required string Name { get; set; }
        public TaskStatus Status { get; set; }
        [DataType(DataType.Date)]
        [Display(Name ="Due to date")]
        public DateOnly? DueToDate { get; set; }
        [Required]
        public required string Description { get; set; }
        // Foreign keys
        public string? OwnerId { get; set; }
        // Navigation properties
        [Display(Name = "Assigned members")]
        public ICollection<ApplicationUser> AssignedUsers { get; set; } = new List<ApplicationUser>();
        [Display(Name = "Assigned by")]
        public ApplicationUser? Owner { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Deleted date")]
        public DateOnly? DeletedDate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Completed date")]
        public DateOnly? CompletedDate { get; set; }
    }
}
