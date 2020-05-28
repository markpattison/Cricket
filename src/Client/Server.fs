module Cricket.Client.Server

open System

open Cricket.Shared
open Fable.Core
open Fable.Remoting.Client

// for details of how to deal with virtual path, see:
// https://github.com/Zaid-Ajaj/SAFE.Simplified/blob/master/client/src/Server.fs

let virtualPath : string =
    #if MOCHA_TESTS
    "/"
    #else
    JS.eval("window.location.pathname")
    #endif

let combine (paths: string list) =
    paths
    |> List.map (fun path -> List.ofArray (path.Split('/')))
    |> List.concat
    |> List.filter (fun segment -> not (segment.Contains(".")))
    |> List.filter (String.IsNullOrWhiteSpace >> not)
    |> String.concat "/"
    |> sprintf "/%s"

let normalize (path: string) = combine [ virtualPath; path ]

let normalizeRoutes typeName methodName =
    Route.builder typeName methodName
    |> normalize

let api : ICricketApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder normalizeRoutes
    |> Remoting.buildProxy<ICricketApi>
