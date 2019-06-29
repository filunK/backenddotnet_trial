using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using FilunK.backenddotnet_trial.Models;

namespace FilunK.backenddotnet_trial.Controllers
{

    /// <summary>
    ///     ログインAPI
    /// </sumamry>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private IConfiguration _config ;

        public BookController(IConfiguration configuration)
        {
            this._config = configuration;
        }

        /// <summary>
        ///     Bookリストを返します。
        /// </summary>
        [Authorize]
        [HttpGet]
        public IEnumerable<BookModel> Get()
        {

            var currentUser = HttpContext.User;

            var response = new BookModel[] {
                new BookModel { Author = "Ray Bradbury",Title = "Fahrenheit 451" },
                new BookModel { Author = "Gabriel García Márquez", Title = "One Hundred years of Solitude" },
                new BookModel { Author = "George Orwell", Title = "1984" },
                new BookModel { Author = "Anais Nin", Title = "Delta of Venus" }
            };

            return response;
        }

    }
}
