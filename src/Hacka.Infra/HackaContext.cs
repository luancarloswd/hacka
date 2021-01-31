using System;
using System.Collections.Generic;
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
        public DbSet<Squad> Squad { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<EventZabbixParams>(ez => ez.HasKey(e => e.EventId));
            builder.Entity<Squad>(buildAction: s => s.HasKey(e => e.Id));
        }
    }
}
