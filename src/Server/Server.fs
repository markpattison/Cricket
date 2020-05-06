open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Giraffe
open Saturn

open Cricket.Shared
open Cricket.Server.SessionManager

let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let publicPath = Path.GetFullPath "../Client/public"

let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us

let sessionManager = SessionManager()

let cricketApi = {
    newSession = fun () -> async { return sessionManager.NewSession() }
    update = fun (sessionId, serverMsg) -> async { return sessionManager.Update(sessionId, serverMsg) }
    getStatistics = fun sessionId -> async { return sessionManager.GetStatistics(sessionId) }
}

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue cricketApi
    |> Remoting.buildHttpHandler

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    use_gzip
}

run app
