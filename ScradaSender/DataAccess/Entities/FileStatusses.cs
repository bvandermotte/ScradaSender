using System.ComponentModel.DataAnnotations;
using ScradaSender.DataAccess.Entities.Base;

namespace ScradaSender.DataAccess.Entities
{
    public class FileStatusses : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string FileName { get; set; }
        [Required]
        [MaxLength(50)]
        public string Status { get; set; }
        public DateTime LastProcessed { get; set; }
        public string? Error { get; set; }
        [MaxLength(50)]
        public string? PeppolId { get; set; }
    }
}
