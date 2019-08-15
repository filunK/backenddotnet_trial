using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using Npgsql.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using FilunK.backenddotnet_trial.Models.Configure;
using FilunK.backenddotnet_trial.DataAccess.DataModel;

namespace FilunK.backenddotnet_trial.DataAccess
{
    public class PgContext : DbContext
    {

        public DbSet<User> Users { get; set; }

        public DbSet<AccountConfirm> AccountConfirms { get; set; }

        private readonly AppSettings _config;

        public PgContext(DbContextOptions<PgContext> option, IOptions<AppSettings> configuration) : base(option)
        {
            this._config = configuration.Value;
            this.Database.SetCommandTimeout(60);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(this._config.ConnectionStrings.Postgres);
            optionsBuilder.UseLazyLoadingProxies();
        }

    }
}