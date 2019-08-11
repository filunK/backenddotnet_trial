using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilunK.backenddotnet_trial.DataAccess.DataModel
{
    [Table("WK_ACCOUNT_CONFIRM", Schema = "BK_DOTNET")]
    public class AccountConfirm : EntityBase
    {
        [Key]
        [MaxLength(40)]
        [Column("CONFIRM_URI")]
        public string ConfirmUri { get; set; }

        [Range(8, 100)]
        [Column("USER_ID")]
        public string UserId { get; set; }

        [Required]
        [Column("EXPIRE_LIMIT")]
        public DateTime ExpireLimit { get; set; }
    }

}
