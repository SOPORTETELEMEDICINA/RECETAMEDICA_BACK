using Microsoft.EntityFrameworkCore;

namespace RMD.Data
{
    public class ConsultaDbContext : DbContext
    {
        public ConsultaDbContext(DbContextOptions<ConsultaDbContext> options) : base(options)
        {
        }
    }
}
