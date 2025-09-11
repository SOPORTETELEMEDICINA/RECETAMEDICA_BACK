using Microsoft.EntityFrameworkCore;

namespace RMD.Data
{
    public class MedicosDbContext(DbContextOptions<MedicosDbContext> options) : DbContext(options)
    {

    }
}
