using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilunK.backenddotnet_trial.DataAccess.DataModel
{
    public class EntityBase
    {
        [Column("creation_id")]
        [MaxLength(30)]
        [Required]
        public string CreationId{get;set;}

        [Column("creation_pg")]
        [MaxLength(30)]
        [Required]
        public string CreationProgram{get;set;}

        [Column("creation_date")]
        [Required]
        public DateTime CreationDate{get;set;}

        [Column("update_id")]
        [MaxLength(30)]
        [Required]
        public string UpdateId{get;set;}

        [Column("update_pg")]
        [MaxLength(30)]
        [Required]
        public string UpdateProgram{get;set;}

        [Column("update_date")]
        [Required]
        public DateTime UpdateDate{get;set;}

        [Column("del_flg")]
        [Required]
        public bool IsDeleted{get;set;}


        public EntityBase()
        {
            this.CreationDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.IsDeleted = false;
        }
    }
}