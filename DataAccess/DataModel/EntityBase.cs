using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilunK.backenddotnet_trial.DataAccess.DataModel
{
    public class EntityBase
    {
        [Column("CREATION_ID")]
        [MaxLength(30)]
        [Required]
        public string CreationId{get;set;}

        [Column("CREATION_PG")]
        [MaxLength(30)]
        [Required]
        public string CreationProgram{get;set;}

        [Column("CREATION_DATE")]
        [Required]
        public DateTime CreationDate{get;set;}

        [Column("UPDATE_ID", Order=910003)]
        [MaxLength(30)]
        [Required]
        public string UpdateId{get;set;}

        [Column("UPDATE_ID")]
        [MaxLength(30)]
        [Required]
        public string UpdateProgram{get;set;}

        [Column("UPDATE_DATE")]
        [Required]
        public DateTime UpdateDate{get;set;}

        [Column("DEL_FLAG")]
        [Required]
        public bool IsDeleted{get;set;}
    }
}