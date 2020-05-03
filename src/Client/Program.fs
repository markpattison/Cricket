module Cricket.Client.Program

open Elmish
open Elmish.Debug
open Elmish.HMR
open Elmish.Navigation
open Elmish.React
open Elmish.UrlParser

open Cricket.Client.State

Program.mkProgram init update View.root
|> Program.toNavigable (parseHash pageParser) urlUpdate
|> Program.withReactBatched "elmish-app"
|> Program.run
