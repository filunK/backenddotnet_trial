using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using FilunK.backenddotnet_trial.Models;
using FilunK.backenddotnet_trial.Models.Configure;
using FilunK.backenddotnet_trial.Models.Book;
using FilunK.backenddotnet_trial.DataAccess;
using FilunK.backenddotnet_trial.DataAccess.DataModel;


namespace FilunK.backenddotnet_trial.Controllers
{

    /// <summary>
    ///     ログインAPI
    /// </sumamry>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppSettings _config;

        private readonly PgContext _context;

        private readonly IHttpClientFactory _clientFactory;

        private readonly string ProgramId = "Book";

        public BookController(IOptions<AppSettings> configuration, PgContext context, IHttpClientFactory clientFactory)
        {
            this._config = configuration.Value;
            this._context = context;
            this._clientFactory = clientFactory;
        }

        /// <summary>
        ///     Bookリストを返します。
        /// </summary>
        [Route("")]
        [Authorize]
        public IActionResult GetBooks()
        {
            IActionResult response = this.NotFound();


            var userName = this.getUserName(this.HttpContext);

            try
            {
                using (this._context)
                {
                    var bookQuery =
                        from books in this._context.Books
                        where books.UserId == userName
                        select books
                    ;

                    if (bookQuery.Count() > 0)
                    {
                        var bookList = new List<BookModel>();
                        foreach (var entity in bookQuery)
                        {
                            var book = new BookModel()
                            {
                                Isbn = entity.Isbn,
                                Title = entity.Title,
                                Author = entity.Author,
                                PageCount = entity.PageCount,
                                PurchaseDate = entity.PurchaseDate,
                                Comment = entity.Comment
                            };

                            bookList.Add(book);

                        }
                        response = this.Ok(bookList.ToArray());

                    }

                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }

        /// <summary>
        ///     Bookを返します。
        /// </summary>
        [Route("{isbn}")]
        [Authorize]
        public IActionResult GetBook(string isbn)
        {
            IActionResult response = this.NotFound();

            var userName = this.getUserName(this.HttpContext);

            try
            {
                using (this._context)
                {
                    var entity =
                    (
                        from books in this._context.Books
                        where books.UserId == userName && books.Isbn == isbn
                        select books
                    ).FirstOrDefault();

                    if (entity != null)
                    {
                        var book = new BookModel()
                        {
                            Isbn = entity.Isbn,
                            Title = entity.Title,
                            Author = entity.Author,
                            PageCount = entity.PageCount,
                            PurchaseDate = entity.PurchaseDate,
                            Comment = entity.Comment
                        };

                        response = this.Ok(book);

                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }


        [Authorize]
        [HttpPost]
        [Route("")]
        public IActionResult RegistBook([FromBody]BookModel model)
        {
            IActionResult response = this.StatusCode(StatusCodes.Status500InternalServerError);

            var userName = this.getUserName(this.HttpContext);

            try
            {
                using (this._context)
                {
                    var entity =
                    (
                        from books in this._context.Books
                        where books.UserId == userName && books.Isbn == model.Isbn
                        select books
                    ).FirstOrDefault();

                    if (entity == null)
                    {
                        var book = new Book()
                        {
                            UserId = userName,
                            Isbn = model.Isbn,
                            Title = model.Title,
                            Author = model.Author,
                            PageCount = model.PageCount,
                            PurchaseDate = model.PurchaseDate,
                            Comment = model.Comment,
                            CreationId = userName,
                            CreationProgram = this.ProgramId + "_REGIST",
                            UpdateId = userName,
                            UpdateProgram = this.ProgramId
                        };

                        this._context.Books.Add(book);
                        this._context.SaveChanges();
                        response = this.Ok(model);
                    }
                    else
                    {
                        // すでに存在する場合は412(前提条件エラー)をリターンさせる、これはあくまで登録。UPSERTではない
                        response = this.StatusCode(StatusCodes.Status412PreconditionFailed);
                    }

                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return response;
        }

        [Authorize]
        [HttpPut]
        [Route("{isbn}")]
        public IActionResult UpdateBook(string isbn, [FromBody]BookModel model)
        {
            IActionResult response = this.StatusCode(StatusCodes.Status500InternalServerError);

            var userName = this.getUserName(this.HttpContext);

            try
            {
                using (this._context)
                {
                    var entity =
                    (
                        from books in this._context.Books
                        where books.UserId == userName && books.Isbn == isbn
                        select books
                    ).FirstOrDefault();

                    if (entity != null)
                    {
                        if (entity.Title != model.Title)
                        {
                            entity.Title = model.Title;
                        }

                        if (entity.Author != model.Author)
                        {
                            entity.Author = model.Author;
                        }

                        if (entity.PageCount != model.PageCount)
                        {
                            entity.PageCount = model.PageCount;
                        }

                        if (entity.PurchaseDate != model.PurchaseDate)
                        {
                            entity.PurchaseDate = model.PurchaseDate;
                        }

                        if (entity.Comment != model.Comment)
                        {
                            entity.Comment = model.Comment;
                        }

                        entity.UpdateId = userName;
                        entity.UpdateProgram = this.ProgramId + "_UPDATE";
                        entity.UpdateDate = DateTime.Now;

                        this._context.Books.Update(entity);
                        this._context.SaveChanges();
                        response = this.Ok(model);
                    }
                    else
                    {
                        // 存在しなければNotFound
                        response = this.NotFound();
                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return response;
        }

        [Authorize]
        // [AllowAnonymous]
        [HttpGet]
        [Route("googleBook/{isbn}")]
        public async Task<IActionResult> SearchGoogleBooks(string isbn)
        {
            IActionResult response = this.NotFound();

            var queryDictionary = new Dictionary<string, string>(){
                {"q", isbn}
            };

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://www.googleapis.com/books/v1/volumes?{await new FormUrlEncodedContent(queryDictionary).ReadAsStringAsync()}"
            );

            var client = this._clientFactory.CreateClient();

            var googleResponse = await client.SendAsync(request);

            var contents = await googleResponse.Content.ReadAsAsync<GoogleBooksVolumesModel>();

            if (contents.Items.Count() > 0)
            {
                // ISBN13が一致するものを抽出
                var googleBook =
                (
                    from items in contents.Items
                    where
                    items.VolumeInfo.IndustryIdentifiers != null
                    && items.VolumeInfo.IndustryIdentifiers.Contains(
                        (
                            from identifier in items.VolumeInfo.IndustryIdentifiers
                            where identifier.Type == "ISBN_13" && identifier.Identifier == isbn
                            select identifier
                        ).FirstOrDefault()
                    )
                    select items
                ).FirstOrDefault();

                if (googleBook != null)
                {
                    var authorString = string.Empty;
                    if (googleBook.VolumeInfo.Authors == null)
                    {
                        authorString = string.Empty;
                    }
                    else
                    {
                        authorString = string.Join(",", googleBook.VolumeInfo.Authors);
                    }

                    var book = new BookModel()
                    {
                        Isbn = isbn,
                        Title = googleBook.VolumeInfo.Title,
                        Author = authorString,
                        PageCount = googleBook.VolumeInfo.PageCount,
                    };

                    response = this.Ok(book);

                }

            }

            return response;
        }

        #region 非HTTPメソッド

        private string getUserName(HttpContext context)
        {
            var userName =
            (
                from claims in this.HttpContext.User.Claims
                where claims.Type == "username"
                select claims.Value
            ).FirstOrDefault();

            return userName;
        }

        #endregion

    }
}
