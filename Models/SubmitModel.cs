using System.ComponentModel.DataAnnotations;

namespace TasksManager.Models
{
    public class SubmitModel
    {
        public List<TaskCommentModel> Comments { get; set; } = new List<TaskCommentModel>();
        [Required]
        [Display(Name = "Comment")]
        public required string message { get; set; }
        [Display(Name = "Attachment")]
        public IFormFile? file { get; set; }
    }
}
