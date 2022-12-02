using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("bank")]
    public partial class BankEntity : BaseEntity
    {
        [Column("name")]
        public string NAME { get; set; }
        [Column("queue")]
        public int QUEUE { get; set; }
    }
}