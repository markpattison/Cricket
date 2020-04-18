module FableCricket.App.Program

open Elmish
open Elmish.HMR
open Elmish.Navigation
open Elmish.UrlParser

open FableCricket.App.State

Program.mkProgram init update View.root
|> Program.toNavigable (parseHash pageParser) urlUpdate
|> Program.withReactBatched "elmish-app"
|> Program.run
