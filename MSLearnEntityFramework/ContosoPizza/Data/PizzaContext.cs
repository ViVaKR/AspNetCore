using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza;

public class PizzaContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Pizza> Pizzas => Set<Pizza>();

    public DbSet<Topping> Toppings => Set<Topping>();

    public DbSet<Sauce> Sauces => Set<Sauce>();

}
