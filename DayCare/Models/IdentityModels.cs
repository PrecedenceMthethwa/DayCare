using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DayCare.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
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
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<IdentityUserRole> UserInRole { get; set; }
        // public DbSet<ApplicationUser> appUsers { get; set; }
        public DbSet<ApplicationRole> appRoles { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<DayCare.Models.Parent> Parents { get; set; }
        public System.Data.Entity.DbSet<DayCare.Models.QRCode> QRCodes  { get; set; }
        public System.Data.Entity.DbSet<DayCare.Models.Child> Children { get; set; }
        public System.Data.Entity.DbSet<DayCare.Models.Deliverystatus> Deliverystatuses { get; set; }

        public System.Data.Entity.DbSet<DayCare.Models.ClassRoom> ClassRooms { get; set; }

        public System.Data.Entity.DbSet<DayCare.Models.Employee> Employees { get; set; }

        public System.Data.Entity.DbSet<DayCare.Models.Driver> Drivers { get; set; }

		public System.Data.Entity.DbSet<DayCare.Models.Beneficiary_Signature> Beneficiary_Signature { get; set; }
	}
}