open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open CustomHttpLoggingMiddleware

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    let app = builder.Build()
    app.UseMiddleware<CustomHttpLoggingMiddleware>() |> ignore

    app.MapGet("/", Func<string>(fun () -> "Hello World!")) |> ignore

    app.Run()

    0 // Exit code

