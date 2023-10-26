using ConsoleApp1.Entitys;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1.DB;

public class Context : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseNpgsql("Server=localhost; Port=5432; Database=products; User Id=postgres; Password=postgressupermercado;");
}
