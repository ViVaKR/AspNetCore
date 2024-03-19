namespace ContosoPizza;

public static class Extensions
{
    public static void CreateDbIfNotExists(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<PizzaContext>();

        // 데이터베이스가 존재하는지 확인
        // EnsureCreated : 데이터베이스가 없으면 새로 만듦
        context.Database.EnsureCreated();
        DbInitializer.Initialize(context);
    }
}
