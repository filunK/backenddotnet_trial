using System;

namespace FilunK.backenddotnet_trial.Models
{
    public class BookModel
    {

        // ISBN13
        public string Isbn { get; set; }

        // 書籍タイトル
        public string Title { get; set; }

        // 著者
        public string Author { get; set; }

        // ページ数
        public int PageCount { get; set; }

        // 購入日
        public DateTime PurchaseDate { get; set; }

        // コメント・感想
        public string Comment { get; set; }
    }
}
