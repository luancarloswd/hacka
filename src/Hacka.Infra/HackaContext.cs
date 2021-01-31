using System;
using Hacka.Domain;
using Microsoft.EntityFrameworkCore;

namespace Hacka.Infra
{
    public class HackaContext : DbContext
    {
        public HackaContext(DbContextOptions<HackaContext> options)
            : base(options)
        {}

        public DbSet<EventZabbixParams> EventZabbix { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<EventZabbixParams>(ez => ez.HasKey(e => e.EventId));
            base.OnModelCreating(builder);
        }
    }
}
