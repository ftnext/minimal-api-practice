module CustomHttpLoggingMiddleware

open System
open System.Diagnostics
open System.Text.Json
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration

type CustomHttpLoggingOptions = {
    Enabled: bool
    JsonFormat: bool
}

type HttpLogEntry = {
    Timestamp: DateTime
    Method: string
    Path: string
    StatusCode: int
    Duration: float
}

type CustomHttpLoggingMiddleware(next: RequestDelegate, logger: ILogger<CustomHttpLoggingMiddleware>, config: IConfiguration) =
    
    let getOptions () =
        let section = config.GetSection("CustomHttpLogging")
        {
            Enabled = section.GetValue("Enabled", true)
            JsonFormat = section.GetValue("JsonFormat", false)
        }
    
    member _.InvokeAsync(context: HttpContext) : Task =
        task {
            let options = getOptions()
            if not options.Enabled then
                return! next.Invoke(context)
            else
                let stopwatch = Stopwatch.StartNew()
                let timestamp = DateTime.UtcNow
                
                do! next.Invoke(context)
                
                stopwatch.Stop()
                let duration = stopwatch.Elapsed.TotalMilliseconds
                
                let logEntry = {
                    Timestamp = timestamp
                    Method = context.Request.Method
                    Path = context.Request.Path.Value
                    StatusCode = context.Response.StatusCode
                    Duration = duration
                }
                
                if options.JsonFormat then
                    let json = JsonSerializer.Serialize(logEntry)
                    logger.LogInformation(json)
                else
                    logger.LogInformation("Timestamp: {Timestamp}, Method: {Method}, Path: {Path}, StatusCode: {StatusCode}, Duration: {Duration}ms", 
                        logEntry.Timestamp, logEntry.Method, logEntry.Path, logEntry.StatusCode, logEntry.Duration)
        }
