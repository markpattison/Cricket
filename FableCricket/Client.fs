﻿module Client

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

let update model msg =
    let updated =
        match msg with
        | StartMatch -> Match.updateMatchState MatchUpdate.StartMatch model
        | StartNextInnings -> Match.updateMatchState MatchUpdate.StartNextInnings model
        | ContinueInnings -> MatchRunner.continueInnings model
    updated |> MatchRunner.runCaptains, []

let row xs = tr [] [ for x in xs -> td [] [x]]

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

let showInningsShort (team, innings) =
    label [] [ text (team + " " + Innings.summary innings) ]

let howOutString (howOut: HowOut option) =
    match howOut with
    | None -> "not out"
    | Some out -> out.ToString()

let oversString (innings: Innings) =
    match innings.BallsSoFarThisOver with
    | 0 -> sprintf @"(%i overs)" innings.OversCompleted
    | n -> sprintf @"(%i.%i overs)" innings.OversCompleted n

let showInnings (team, innings) =
    let showIndividualInnings (p, ii) = row [ text p.Name; text (howOutString ii.HowOut); text (ii.Score.ToString()) ]
    let totalRow = row [ text "Total"; text (oversString innings); text (innings.GetRuns.ToString()) ]
    let rows = List.append (innings.Batsmen |> List.map showIndividualInnings) [ totalRow ]

    div [] [ table [] rows; hr [] ]

let showAllInnings match' =
    let allInnings = match' |> Match.inningsList
    div [] (allInnings |> List.map showInnings)

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