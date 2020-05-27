module Cricket.Client.Server

open Cricket.Shared
open Fable.Remoting.Client

let api : ICricketApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ICricketApi>
