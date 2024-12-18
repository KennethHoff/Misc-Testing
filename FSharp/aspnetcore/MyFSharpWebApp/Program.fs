open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting


[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateSlimBuilder(args)
    let app = builder.Build()

    app.MapGet("/{name}", Func<_, _>(fun (name: string) -> name)) |> ignore

    app.MapGet("/{name}/{age}", Func<_, _, _>(fun (name: string) (age: int) -> $"{name} is {age} years old"))
    |> ignore

    app.Run()

    0 // Exit code
