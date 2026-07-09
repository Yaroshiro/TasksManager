using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TasksManager.Models
{    
    public class FileModel
    {
        public const string UploadsPath = "wwwroot/Uploads/";

        public int Id { get; set; }

        public required int TaskId { get; set; }

        public int? TaskCommentId { get; set; }

        public required string FilePath { get; set; }

        [Display(Name = "Name")]
        public required string OriginalFileName { get; set; }
    }
}
