using Microsoft.EntityFrameworkCore;

namespace MSOrleansDemo.Context
{
    public class OrleansDemoDbContext : DbContext
    {
        public OrleansDemoDbContext(DbContextOptions<OrleansDemoDbContext> options) : base(options)
        {
            //try
            //{
            //    var script = Database.GenerateCreateScript();
            //    Database.ExecuteSqlRaw(script);
            //}
            //catch (Exception ex)
            //{
            //    // nothing
            //}
        }

        public DbSet<AgreementState> AgreementStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgreementState>().HasKey(x => x.AgreementId);
            modelBuilder.Entity<AgreementState>().Property(x => x.PdfFileLocation).IsRequired(false);
            modelBuilder.Entity<AgreementState>().Property(x => x.SignerId).IsRequired(false);

            base.OnModelCreating(modelBuilder);
        }

        public AgreementState? GetByAgreementId(string agreementId)
        {
            return GetStateByAgreementId(this, agreementId);
        }

        public Task<AgreementState?> GetByAgreementIdAsync(string agreementId)
        {
            return GetStateByAgreementIdAsync(this, agreementId);
        }

        private static Func<OrleansDemoDbContext, string, AgreementState?> GetStateByAgreementId =
       EF.CompileQuery<OrleansDemoDbContext, string, AgreementState?>(
            (dbContext, id) => dbContext.AgreementStates.AsNoTracking().FirstOrDefault(n => n.AgreementId == id));

        private static Func<OrleansDemoDbContext, string, Task<AgreementState?>> GetStateByAgreementIdAsync =
      EF.CompileAsyncQuery<OrleansDemoDbContext, string, AgreementState?>(
           (dbContext, id) => dbContext.AgreementStates.AsNoTracking().FirstOrDefault(n => n.AgreementId == id));
    }

    public class AgreementState
    {
        public string AgreementId { get; set; }
        public string PdfFileLocation { get; set; }
        public string SignerId { get; set; }
    }

}


