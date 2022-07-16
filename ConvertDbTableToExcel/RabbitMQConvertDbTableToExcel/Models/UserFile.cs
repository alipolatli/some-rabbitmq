using System.ComponentModel.DataAnnotations.Schema;

namespace RabbitMQConvertDbTableToExcel.Models
{
    public class UserFile
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string? FileName { get; set; }

        public string? FilePath { get; set; }

        public DateTime? CreatedDate { get; set; }

        public FileStatus? FileStatus { get; set; }

        [NotMapped]
        public string GetCreatedDate
        {
            get
            {
                return CreatedDate.HasValue ? CreatedDate.Value.ToShortDateString() : "-";
            }
        }

    }

}
