using Microsoft.EntityFrameworkCore;

namespace RMD.Data
{
    public class PuntoVentaDbContext(DbContextOptions<PuntoVentaDbContext> options) : DbContext(options)
    {

    }
}
