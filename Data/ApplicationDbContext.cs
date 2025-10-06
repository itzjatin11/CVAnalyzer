using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CVAnalyzer.Models;
using Microsoft.EntityFrameworkCore;

namespace CVAnalyzer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
    }
}
