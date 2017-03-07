module Client

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
    match model.State with
    | NotStarted ->
        section
            [attribute "class" "todoapp"]
            [ h1 [] [text "Match not started"] ]
    | Abandoned ->
        section
            [attribute "class" "todoapp"]
            [ h1 [] [text "Match abandoned"] ]
    | _ ->
        let summary = model |> Match.summaryStatus
        let innings = model |> Match.inningsList
        section
            [attribute "class" "todoapp"]
            [ h1 [] [text summary] ]

let initModel = { TeamA = "England"; TeamB = "Australia"; State = NotStarted; Rules = { FollowOnMargin = 200 } }

createApp initModel view update Virtualdom.createRender
|> withStartNodeSelector "#app"
|> start