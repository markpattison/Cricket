
#r "node_modules/fable-core/Fable.Core.dll"
#load "node_modules/fable-arch/Fable.Arch.Html.fs"
#load "node_modules/fable-arch/Fable.Arch.App.fs"
#load "node_modules/fable-arch/Fable.Arch.Virtualdom.fs"

#r "../build/CricketEngine.dll"
#r "../build/MatchRunner.dll"

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open Cricket.CricketEngine

// Cricket model

type Model =
    {
        Match: Match
    }

// Todo update
let update model msg =
    model, []

// Todo view

let view model =
    section
        [attribute "class" "todoapp"]
        [ h1 [] [text "todos"] ]


let initModel = { TeamA = "England"; TeamB = "Australia"; State = NotStarted; Rules = { FollowOnMargin = 200 } }

createApp initModel view update Virtualdom.createRender
|> withStartNodeSelector "#app"
|> start