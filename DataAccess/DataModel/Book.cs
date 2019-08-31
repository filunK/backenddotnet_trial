using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilunK.backenddotnet_trial.DataAccess.DataModel
{
    [Table("t_book", Schema = "bk_dotnet")]
    public class Book : EntityBase
    {
        // ユーザID (複合キー1)
        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        [Column("user_id", Order = 1)]
        public string UserId { get; set; }

        // ISBN13(複合キー2)
        [Required]
        [MaxLength(13)]
        [Column("isbn_13", Order = 2)]
        public string Isbn { get; set; }

        // 書籍タイトル
        [Required]
        [Column("title")]
        public string Title { get; set; }

        // 著者
        [Required]
        [Column("author")]
        public string Author { get; set; }

        // ページ数
        [Column("page_count")]
        public int PageCount { get; set; }

        // 購入日
        [Column("purchase_date")]
        public DateTime PurchaseDate { get; set; }

        // コメント・感想
        [Column("comment")]
        public string Comment { get; set; }

        public Book() : base()
        {

        }
    }
}