using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace htgt.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //static ApplicationDbContext()
        //{
        //    Database.SetInitializer(new MySqlInitializer());
        //}

        public ApplicationDbContext()
            : base("htgtconnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>()
                .Property(c => c.Name).HasMaxLength(128).IsRequired();

            modelBuilder.Entity<Microsoft.AspNet.Identity.EntityFramework.IdentityUser>()
                .Property(iu => iu.Email).HasColumnName("EmailAddress").HasMaxLength(254).IsRequired();

            modelBuilder.Entity<Microsoft.AspNet.Identity.EntityFramework.IdentityUser>()
                .Property(iu => iu.PasswordHash).HasColumnName("Password");

            modelBuilder.Entity<Microsoft.AspNet.Identity.EntityFramework.IdentityUser>().ToTable("AspNetUsers")//I have to declare the table name, otherwise IdentityUser will be created
                .Property(c => c.UserName).HasMaxLength(128).IsRequired();
        }
    }
}