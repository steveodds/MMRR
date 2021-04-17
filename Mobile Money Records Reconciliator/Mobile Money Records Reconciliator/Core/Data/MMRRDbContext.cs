using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile_Money_Records_Reconciliator.Core.Data
{
    class MMRRDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=mmrr.db");

        public DbSet<Core.Models.PDFStatement> PDFStatements { get; set; }
        public DbSet<Core.Models.MpesaRecord> MpesaRecords { get; set; }
        public DbSet<Core.Models.FinalFile> FinalFiles { get; set; }
    }
}
