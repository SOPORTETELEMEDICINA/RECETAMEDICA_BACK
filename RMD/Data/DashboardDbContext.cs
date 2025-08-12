using Microsoft.EntityFrameworkCore;

namespace RMD.Data
{
    public class DashboardDbContext(DbContextOptions<DashboardDbContext> options) : DbContext(options)
    {
        
    }
}
