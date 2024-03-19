using ContosoPizza.Models;

namespace ContosoPizza;

public class DbInitializer
{
    // PizzaService 에 대한 CRUD 작업을 코딩 했지만
    // 데이터베이스에 데이터가 있어야 테스트가 가능하므로
    // 시작시 데이터베이스를 시드 하도록함.
    public static void Initialize(PizzaContext context)
    {
        if (context.Pizzas.Any() && context.Toppings.Any() && context.Sauces.Any())
            return; // DB has been seeded

        var pepperoniTopping = new Topping { Name = "Pepperoni", Calories = 130 };
        var sausageTopping = new Topping { Name = "Sausage", Calories = 100 };
        var hamTopping = new Topping { Name = "Ham", Calories = 70 };
        var chickenTopping = new Topping { Name = "Chicken", Calories = 50 };
        var pineappleTopping = new Topping { Name = "Pineapple", Calories = 75 };
        var tomatoSauce = new Sauce { Name = "Tomato", IsVegan = true };
        var alfredoSauce = new Sauce { Name = "Alfredo", IsVegan = false };

        var pizzas = new Pizza[]
        {
                new() {
                        Name = "Meat Lovers",
                        Sauce = tomatoSauce,
                        Toppings =
                            [
                                pepperoniTopping,
                                sausageTopping,
                                hamTopping,
                                chickenTopping
                            ]
                    },
                new() {
                        Name = "Hawaiian",
                        Sauce = tomatoSauce,
                        Toppings =
                            [
                                pineappleTopping,
                                hamTopping
                            ]
                    },
                new() {
                        Name="Alfredo Chicken",
                        Sauce = alfredoSauce,
                        Toppings =
                            [
                                chickenTopping
                            ]
                        }
        };

        context.Pizzas.AddRange(pizzas);
        context.SaveChanges();
    }
}
