module Cricket.Client.Program

open Elmish
open Elmish.Debug
open Elmish.HMR
open Elmish.Navigation
open Elmish.React

open State

Program.mkProgram init update View.root
|> Program.withReactBatched "elmish-app"
|> Program.run
