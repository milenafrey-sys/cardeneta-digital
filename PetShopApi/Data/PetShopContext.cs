using Microsoft.EntityFrameworkCore;
using PetShopApi.Models;

namespace PetShopApi.Data;

public class PetShopContext : DbContext
{
    public PetShopContext(DbContextOptions<PetShopContext> options) : base(options)
    {
    }

    public DbSet<Tutor> Tutores { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Funcionario> Funcionarios { get; set; }
    public DbSet<Atendimento> Atendimentos { get; set; }
}
