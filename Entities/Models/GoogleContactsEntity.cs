using System.ComponentModel.DataAnnotations.Schema;


namespace Entities.Models
{
    [Table("main_table_test")]
    public partial class GoogleContactsEntity : BaseEntity
    {
        [Column("fio")]
        public string FIO { get; set; }
        [Column("phone")]
        public string PHONE { get; set; }
        [Column("doljnost")]
        public string DOLJNOST { get; set; }
        [Column("bank")]
        public string BANK { get; set; }
        [Column("emp_oid")]
        public string EMP_OID { get; set; }
        [Column("blocked")]
        public bool BLOCKED { get; set; }
        [Column("place_name")]
        public string PLACE_NAME { get; set; }
        [Column("email_sd")]
        public string EMAIL_SD { get; set; }
        [Column("tabnum")]
        public int TABNUM { get; set; }
        [Column("resource_name")]
        public string RESOURCE_NAME { get; set; }
        [Column("internal_number")]
        public int INTERNAL_NUMBER { get; set; }
        [Column("bank_queue")]
        public int BANK_QUEUE { get; set; }
    }
}
