using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public partial class DataGoogleAPIContext : DbContext
    {
        public virtual DbSet<GoogleContactsEntity> GoogleContacts { get; set; }
        public virtual DbSet<BankEntity> Bank { get; set; }
        public DataGoogleAPIContext(DbContextOptions options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

