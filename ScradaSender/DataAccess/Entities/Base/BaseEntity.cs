using System.ComponentModel.DataAnnotations;

namespace ScradaSender.DataAccess.Entities.Base
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
