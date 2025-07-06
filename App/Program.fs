open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Logging.Console

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    builder.Logging.AddSimpleConsole(fun options ->
        options.SingleLine <- true
        options.TimestampFormat <- "HH:mm:ss "
        options.ColorBehavior <- LoggerColorBehavior.Enabled
    ) |> ignore

    let app = builder.Build()

    app.MapGet("/", Func<string>(fun () -> "Hello World!")) |> ignore

    app.Run()

    0 // Exit code

