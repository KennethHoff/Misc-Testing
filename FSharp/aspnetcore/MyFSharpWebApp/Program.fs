open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection


[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateSlimBuilder(args)
    builder.Services.AddSingleton(TimeProvider.System) |> ignore

    let app = builder.Build()

    app.MapGet("/{name}", Func<_, _>(fun (name: string) -> name)) |> ignore

    app.MapGet("/ping", Func<TimeProvider, _>(fun (timeProvider) -> timeProvider.GetUtcNow() |> sprintf "%A"))
    |> ignore

    app.MapGet("/pong", Func<_, _>(fun (timeProvider: TimeProvider) -> timeProvider |> _.GetUtcNow() |> sprintf "%A"))
    |> ignore

    app.MapGet("/peng", Func<TimeProvider, _>(_.GetUtcNow())) |> ignore

    app.MapGet("/{name}/{age}", Func<_, _, _>(fun (name: string) (age: int) -> $"{name} is {age} years old"))
    |> ignore

    app.Run()

    0 // Exit code
