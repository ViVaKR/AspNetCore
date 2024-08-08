using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

/// <summary>
/// Game Endpoints
/// </summary>
public static class GamesEndpoints
{
    public const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        //* Group
        //* `Validation Extension` --> <PackageReference Include="MinimalApis.Extensions" />
        var group = app.MapGroup("games").WithParameterValidation();

        //! GET /games
        //? DI, Async
        group.MapGet("/", async (GameStoreContext db) => await db.Games.Include(x => x.Genre).Select(x => x.ToGameSummaryDto()).AsNoTracking().ToListAsync()); // 읽기전용에 -> AsNotTracking(), 자원관리

        //! GET /games/1
        //? DI, Async
        group.MapGet("/{id}", async (int id, GameStoreContext db) =>
        {
            Game? game = await db.Games.FindAsync(id);

            return game is null
                ? Results.NotFound($"Game Id: ({id}) Not Found")
                : Results.Ok(game.ToGameDetailsDto());

        }).WithName(GetGameEndpointName);

        //! POST /games
        //? DI, Async
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext db) =>
        {
            Game game = newGame.ToEntity();
            db.Games.Add(game);
            await db.SaveChangesAsync();
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToGameDetailsDto());
        });

        //! PUT /games
        //? DI, Async
        group.MapPut("/{id}", async (int id, UpdateGameDto updateGame, GameStoreContext db) =>
        {
            var existingGame = await db.Games.FindAsync(id);

            if (existingGame is null)
                return Results.NotFound($"Game Id => ({id}) Not Found");

            db.Entry(existingGame).CurrentValues.SetValues(updateGame.ToEntity(id));

            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        //! DELETE /games/{id}
        //? DI, Async
        group.MapDelete("/{id}", async (int id, GameStoreContext db) =>
        {
            //! 기존 방식
            // db.Games.Remove();
            // var game = db.Games.Find(id);
            // if (game is null)
            //     return Results.NotFound($"Game Id: {id} Not Found!");
            // db.Games.Remove(game);
            // var result = db.SaveChanges();

            //! 권장하는 방식, 일괄 삭제 포함
            int result = await db.Games.Where(x => x.Id == id).ExecuteDeleteAsync();
            return Results.Ok($"({result}) -> Game Id: {id}, Remove Completed.");
        });

        return group;
    }
}
