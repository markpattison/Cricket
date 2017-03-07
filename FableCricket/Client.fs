module Client

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

open Cricket.CricketEngine
open Cricket.MatchRunner

type Model =
    {
        Match: Match
    }

type CricketAction =
    | StartMatch
    | StartNextInnings
    | ContinueInnings
    | DoNothing // TODO - remove this

let parseOption option =
    match option with
    | StartMatchUI -> "Start match", fun x -> StartMatch
    | StartNextInningsUI -> "Start next innings", fun x -> StartNextInnings
    | ContinueInningsUI -> "Continue innings", fun x -> ContinueInnings
    | MatchOverUI -> "Match over", fun x -> DoNothing

let update model msg =
    let updated =
        match msg with
        | StartMatch -> Match.updateMatchState MatchUpdate.StartMatch model
        | StartNextInnings -> Match.updateMatchState MatchUpdate.StartNextInnings model
        | ContinueInnings -> MatchRunner.continueInnings model
        | DoNothing -> model
    updated |> MatchRunner.runCaptains, []

let showSummary match' =
    match match'.State with
    | NotStarted ->
        div
            []
            [ h1 [] [text "Match not started"] ]
    | Abandoned ->
        div
            []
            [ h1 [] [text "Match abandoned"] ]
    | _ ->
        let summary = match' |> Match.summaryStatus
        let innings = match' |> Match.inningsList
        div
            []
            [ h1 [] [text summary] ]

let showOption option =
    let optionText, action = parseOption option
    li [ onMouseClick action ]
        [ label [] [ text optionText ] ]

let showOptions match' =
    let options = MatchRunner.getOptionsUI match'
    ul [ ]
       [ (showOption options) ]

let view model =
    div
        []
        [ showSummary model; showOptions model ]

let initModel = { TeamA = "England"; TeamB = "Australia"; State = NotStarted; Rules = { FollowOnMargin = 200 } }

createApp initModel view update Virtualdom.createRender
|> withStartNodeSelector "#app"
|> start