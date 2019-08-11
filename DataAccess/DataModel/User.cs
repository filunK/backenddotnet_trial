using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilunK.backenddotnet_trial.DataAccess.DataModel
{
    [Table("T_USER", Schema = "BK_DOTNET")]
    public class User : EntityBase
    {
        [Key]
        [Range(8, 100)]
        [Column("USER_ID")]
        public string UserId { get; set; }

        [Required]
        [Column("SALT")]
        public string Salt { get; set; }

        [Required]
        [Column("HASH")]
        public string Hash { get; set; }

        [Required]
        [Column("ITERATION")]
        public int Iteration { get; set; }

        [Required]
        [Column("ACCOUNT_CONFIRMED")]
        public bool IsConfirmed { get; set; }
    }
}