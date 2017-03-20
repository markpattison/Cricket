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

let showInnings (team, innings) =
    li [] [ text (team + " " + Innings.summary innings) ]

let showAllInnings match' =
    let allInnings = match' |> Match.inningsList
    ul [] (allInnings |> List.map showInnings)

let showOption option =
    match option with
    | StartMatchUI -> button [ onMouseClick (fun x -> StartMatch) ] [ text "Start match" ]
    | StartNextInningsUI -> button [ onMouseClick (fun x -> StartNextInnings) ] [ text "Start next innings" ]
    | ContinueInningsUI -> button [ onMouseClick (fun x -> ContinueInnings) ] [ text "Continue innings" ]
    | MatchOverUI -> label [] [ text "Match over" ]

let showOptions match' =
    let option = MatchRunner.getOptionsUI match'
    showOption option

let view model =
    div
        []
        [ showSummary model; showAllInnings model; showOptions model ]

let initModel = { TeamA = "England"; TeamB = "Australia"; State = NotStarted; Rules = { FollowOnMargin = 200 } }

createApp initModel view update Virtualdom.createRender
|> withStartNodeSelector "#app"
|> start