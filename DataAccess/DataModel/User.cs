using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilunK.backenddotnet_trial.DataAccess.DataModel
{
    [Table("t_user", Schema = "bk_dotnet")]
    public class User : EntityBase
    {
        [Key]
        [MinLength(8)]
        [MaxLength(100)]
        [Column("user_id")]
        public string UserId { get; set; }

        [Required]
        [Column("mail_address")]
        public string MailAddress { get; set; }

        [Required]
        [Column("salt")]
        public string Salt { get; set; }

        [Required]
        [Column("hash")]
        public string Hash { get; set; }

        [Required]
        [Column("iteration")]
        public int Iteration { get; set; }

        [Required]
        [Column("account_confirmed")]
        public bool IsConfirmed { get; set; }

        public User(): base()
        {
            
        }
    }
}