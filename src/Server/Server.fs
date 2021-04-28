module Cricket.Server.Server

open System.IO
open Microsoft.AspNetCore.Http
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Giraffe
open Saturn

open Cricket.Shared
open Cricket.Server.SessionManager

let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let publicPath = Path.GetFullPath "./public"

let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us

let getStorageConnectionString (config: Microsoft.Extensions.Configuration.IConfiguration) : string =
    let azureConnectionString = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(config, "cricketStorageConnectionString")

    if isNull azureConnectionString then
        let environmentConnectionString = tryGetEnv "cricketStorageConnectionString"

        match environmentConnectionString with
        | Some s -> s
        | None -> failwith "Storage connection string not found"
    else
        azureConnectionString

let getConfig (config: Microsoft.Extensions.Configuration.IConfiguration) : Cricket.Server.Config =
    {
        StorageConnectionString = getStorageConnectionString config
    }

let sessionManager = SessionManager()

let cricketApi (ctx: HttpContext) = {
    newSession = fun () -> async { return sessionManager.NewSession(ctx) }
    loadSession = fun sessionId -> async { return sessionManager.LoadSession(ctx, sessionId)}
    update = fun (sessionId, serverMsg) -> async { return sessionManager.Update(sessionId, serverMsg) }
    getAverages = fun sessionId -> async { return sessionManager.GetAverages(sessionId) }
    getSeries = fun sessionId -> async { return sessionManager.GetSeries(sessionId) }
    getCompletedMatch = fun (sessionId, matchId) -> async { return sessionManager.GetCompletedMatch(sessionId, matchId) }
}

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromContext cricketApi
    |> Remoting.buildHttpHandler

let app = application {
    url ("http://localhost:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    use_gzip
    error_handler (fun exn _ -> printfn "%s" exn.Message; clearResponse >=> ServerErrors.INTERNAL_ERROR exn.Message)
    use_config getConfig
}

run app
