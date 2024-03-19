using ContosoPizza.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza.Services;

/// <summary>
/// CRUD
/// </summary>
/// <param name="context"></param>
public class PizzaService(PizzaContext context)
{
    private readonly PizzaContext _context = context;

    public IEnumerable<Pizza> GetAll()
    {
        return [.. _context.Pizzas.AsNoTracking()];
    }

    public Pizza? GetById(int id)
    {
        return _context.Pizzas.Include(x => x.Toppings).Include(x => x.Sauce)
        .AsNoTracking() // 읽기 전용 쿼리를 작성시, 개체 변경 내용 추적하지 않음.
        .SingleOrDefault(x => x.Id == id);
    }

    public Pizza? Create(Pizza newPizza)
    {
        _context.Pizzas.Add(newPizza);
        _context.SaveChanges();
        return newPizza;
    }

    public void AddTopping(int pizzaId, int toppingId)
    {
        var pizzaToUpdate = _context.Pizzas.Find(pizzaId);
        var toppingToAdd = _context.Toppings.Find(toppingId);

        if (pizzaToUpdate is null || toppingToAdd is null)
            throw new InvalidOperationException("Pizza or topping dows not exist");

        if (pizzaToUpdate.Toppings is null)
            pizzaToUpdate.Toppings = new List<Topping>();

        pizzaToUpdate.Toppings.Add(toppingToAdd);

        _context.SaveChanges();
    }

    public void UpdateSauce(int pizzaId, int sauceId)
    {
        var pizzaToUpdate = _context.Pizzas.Find(pizzaId);
        var sauceToUpdate = _context.Sauces.Find(sauceId);

        if (pizzaToUpdate is null || sauceToUpdate is null)
            throw new InvalidOperationException("Pizza or sauce does not exist");

        pizzaToUpdate.Sauce = sauceToUpdate;

        _context.SaveChanges();

    }

    public void DeleteById(int id)
    {
        // Find 는 기본키( 이 경우 Id)로 피자를 검색함.
        var pizzaToDelete = _context.Pizzas.Find(id);

        if (pizzaToDelete is not null)
        {
            _context.Pizzas.Remove(pizzaToDelete);
            _context.SaveChanges();
        }
    }
}
