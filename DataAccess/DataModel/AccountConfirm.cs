using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilunK.backenddotnet_trial.DataAccess.DataModel
{
    [Table("wk_account_confirm", Schema = "bk_dotnet")]
    public class AccountConfirm : EntityBase
    {
        [Key]
        [MaxLength(40)]
        [Column("confirm_uri")]
        public string ConfirmUri { get; set; }

        [MinLength(8)]
        [MaxLength(100)]
        [Column("user_id")]
        public string UserId { get; set; }

        [Required]
        [Column("expire_limit")]
        public DateTime ExpireLimit { get; set; }

        public AccountConfirm(): base()
        {
            
        }
    }

}
