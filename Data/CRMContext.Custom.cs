using Microsoft.EntityFrameworkCore;
using CleverCRM.Models;

namespace CleverCRM.Data
{
    public partial class CRMContext
    {
        partial void OnModelBuilding(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().ToTable("AspNetUsers");
        }
    }
}