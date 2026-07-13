using Microsoft.EntityFrameworkCore;

namespace LaundryManagement.Model.data
{
    public class LaundryDb : DbContext
    {
        public LaundryDb(DbContextOptions<LaundryDb>opt):base(opt)
        {
            
        }
        public DbSet<Userla> Users { get; set; }
        public DbSet<Itemla> LaundryItem {  get; set; }
        public DbSet<orderla> Orders { get; set; }

    }
}
