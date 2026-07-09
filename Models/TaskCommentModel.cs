using System.ComponentModel.DataAnnotations;

namespace TasksManager.Models
{
    public class TaskCommentModel
    {
        public int Id { get; set; }
        public int TaskModelId { get; set; }
        public DateTime? DateTime { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public string? Message { get; set; }

        public int? FileId { get; set; }
        [Display(Name = "Attachment")]
        public FileModel? File { get; set; }

        public string? UserName { get; set; }
    }
}
