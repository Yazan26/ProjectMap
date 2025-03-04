using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MijnWebApi.WebApi.Classes
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define DbSets for your entities here
        public DbSet<Object2D> Objects2D { get; set; }
        public DbSet<Enviroment2D> Environments2D { get; set; }
    }
    
}
