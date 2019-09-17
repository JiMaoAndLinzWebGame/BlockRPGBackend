using BlockRPGBackend.Modules;
using Microsoft.EntityFrameworkCore;

namespace BlockRPGBackend
{
    /// <summary>
    /// 
    /// </summary>
    public class MyDbContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public DbSet<Users> Users { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public DbSet<Blocks> Blocks { get; set; }

        /// <summary>
        /// 链接字串
        /// </summary>
        /// <value></value>
        private string _ConnectString { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySql(_ConnectString);
            //base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blocks>().HasKey(t => new { t.X, t.Y });
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MyDbContext() : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MyDbContext(string connectstring) : base()
        {
            _ConnectString = connectstring;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }
    }//End Class
}