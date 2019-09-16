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
        public DbSet<Modules.Users> Users { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySql("Server=127.0.0.1;Port=6603;Database=test; User=root;Password=LinzAsm-l%16*.cn;");
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
        /// <param name="options"></param>
        /// <returns></returns>
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }
    }//End Class
}