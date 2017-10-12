module Cricket.View

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props

open Elmish

open Cricket.CricketEngine
open Cricket.MatchRunner

open Types

let simpleButton txt action dispatch =
  div
    [ ClassName "column is-narrow" ]
    [ a
        [ ClassName "button"
          OnClick (fun _ -> action |> dispatch) ]
        [ str txt ] ]

let summaryStatus match' =
  let summary = match' |> Match.summaryStatus
  div
    [ ClassName "content" ]
    [ h1 [] [ str summary ]]

let howOutString (howOut: HowOut option) =
    match howOut with
    | None -> "not out"
    | Some out -> out.ToString()

let oversString (innings: Innings) =
    match innings.BallsSoFarThisOver with
    | 0 -> sprintf @"(%i overs)" innings.OversCompleted
    | n -> sprintf @"(%i.%i overs)" innings.OversCompleted n

let showInnings (team, innings) =
  let showIndividualInnings (p: Player, ii) = tr [] [ td[][str p.Name]; td[][str (howOutString ii.HowOut)]; td[][str (ii.Score.ToString())] ]
  let totalRow = tr [] [ td [][str "Total"]; td[][str (oversString innings)]; td[][str (innings.GetRuns.ToString())] ]
  let rows = List.append (innings.Batsmen |> List.map showIndividualInnings) [ totalRow ]

  div [] [ table [ ClassName "table" ] rows; hr [] ]

let showAllInnings match' =
  let allInnings = match' |> Match.inningsList
  div [] (allInnings |> List.map showInnings)

let showOption (option: UpdateOptionsForUI) dispatch =
  match option with
  | StartMatchUI -> simpleButton "Start match" StartMatch dispatch
  | StartNextInningsUI -> simpleButton "Start next innings" StartNextInnings dispatch
  | ContinueInningsUI -> simpleButton "Continue innings" ContinueInnings dispatch
  | MatchOverUI -> div [] [ str "Match over" ]

let showOptions match' dispatch =
  let option = MatchRunner.getOptionsUI match'
  showOption option dispatch

let showReset dispatch = simpleButton "Reset match" ResetMatch dispatch


let root model dispatch =
  let match' = model.Match
  div
    [ ClassName "content" ]
    [ showOptions match' dispatch; summaryStatus match'; showAllInnings match'; showReset dispatch ]