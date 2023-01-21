namespace APIAuthenticationAuthorization.Context
{
    public class DBContext:DbContext
    {
        public DBContext(DbContextOptions<DBContext>options):base(options) 
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=.;Database=APIAuthenticationAuthorization;Trusted_Connection=True");
        }

        public DbSet<User> Users => Set<User>();
    }
}
