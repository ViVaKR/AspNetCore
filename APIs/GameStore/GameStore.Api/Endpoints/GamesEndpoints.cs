using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    public const string GetGameEndpointName = "GetGame";

    private static readonly List<GameDto> games =
    [
        new (1, "Street Fighter II==", "Fighting", 19.99M, new DateOnly(1992, 7, 15)),
        new (2, "Final Fantasy XIV", "Roleplaying", 59.99M, new DateOnly(2010, 9, 30)),
        new (3, "FIFA 23", "Sports", 69.99M, new DateOnly(2003, 11, 30))
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        // Group
        // Validation -> <PackageReference Include="MinimalApis.Extensions" />
        var group = app.MapGroup(nameof(games)).WithParameterValidation();

        // GET /games
        group.MapGet("/", () => games);

        // GET /games/1
        group.MapGet("/{id}", (int id) =>
        {
            GameDto? game = games.Find(x => x.Id == id);
            return game is null ? Results.NotFound() : Results.Ok(game);
        }).WithName(GetGameEndpointName);

        // POST /games
        group.MapPost("/", (CreateGameDto newGame, GameStoreContext db) =>
        {
            Game game = new()
            {
                Name = newGame.Name,
                Genre = db.Genres.Find(newGame.GenreId),
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate
            };

            db.Games.Add(game);
            db.SaveChanges();

            GameDto dto = new(
                game.Id,
                game.Name,
                game.Genre!.Name,
                game.Price,
                game.ReleaseDate
            );
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, dto);
        });

        // PUT /games
        group.MapPut("/{id}", (int id, UpdateGameDto updateGameDto) =>
        {
            var index = games.FindIndex(x => x.Id == id);
            if (index == -1)
                return Results.NotFound($"Game Id => ({id}) Not Found");

            games[index] = new GameDto(id, updateGameDto.Name, updateGameDto.Genre, updateGameDto.Price, updateGameDto.ReleaseDate);

            return Results.NoContent();
        });

        // DELETE /games/1
        group.MapDelete("/{id}", (int id) =>
        {

            games.RemoveAll(x => x.Id == id);
            return Results.NoContent();
        });

        return group;
    }
}
