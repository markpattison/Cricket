module Cricket.View

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props

open Elmish

open Cricket.CricketEngine
open Cricket.CricketEngine.Formatting
open Cricket.MatchRunner

open Types

let simpleButton txt action dispatch =
  div
    [ ClassName "column is-narrow" ]
    [ a
        [ ClassName "button"
          OnClick (fun _ -> action |> dispatch) ]
        [ str txt ] ]

let showSummaryStatus match' =
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
 
let showIndividualInnings (p: Player, ii) =
  tr [] [
    td [] [str p.Name]
    td [] [str (howOutString ii.HowOut)]
    td [ ClassName "has-text-weight-bold" ] [str (ii.Score.ToString())]
    td [] [str (ii.BallsFaced.ToString())]
    td [] [str (ii.Fours.ToString())]
    td [] [str (ii.Sixes.ToString())]
  ]

let showBatting (team, innings) =
  let headerRow = thead [] [ tr [ ClassName "has-text-weight-bold" ] [ td [] [str "Batsmen"]; td [] []; td [] [str "R"]; td [] [str "B"]; td [] [str "4"]; td [] [str "6"] ] ]
  let allIndividualInnings = innings.Batsmen |> List.map showIndividualInnings
  let totalRow = tr [ ClassName "has-text-weight-bold" ] [ td [] [str "Total"]; td[] [str (oversString innings)]; td[] [str (innings.GetRuns.ToString())] ]
  let rows = List.concat [ [ headerRow ]; allIndividualInnings; [ totalRow ] ]

  div [] [ table [ ClassName "table is-narrow" ] rows ]

let showIndividualBowling (p: Player, bowling) =
  tr [] [
    td [] [str p.Name]
    td [] [str (formatOversFromBalls bowling.Balls)]
    td [] [str (bowling.Maidens.ToString())]
    td [] [str (bowling.RunsConceded.ToString())]
    td [] [str (bowling.Wickets.ToString())]
  ]

let showBowling (team, innings) =
  let headerRow = thead [] [ tr [ ClassName "has-text-weight-bold" ] [ td [] [str "Bowling"]; td [] [str "O"]; td [] [str "M"]; td [] [str "R"]; td [] [str "W"] ] ]
  let allBowling = innings.Bowlers |> List.map showIndividualBowling
  let rows = List.concat [ [ headerRow ]; allBowling ]

  div [] [ table [ ClassName "table is-narrow" ] rows ]

let showInnings (team, inningsNumber, innings) =
  let teamInnings = sprintf "%s %s" team (formatInningsNumber inningsNumber)
  let score = Innings.summary innings
  div [ ClassName "box" ] [ div [ ClassName "level" ] [ h2 [ ClassName "level-left" ] [str teamInnings]; h2 [ ClassName "level-right" ] [str score] ]; showBatting (team, innings); showBowling (team, innings) ]

let showAllInnings match' =
  let allInnings = match' |> Match.inningsList
  div [] (allInnings |> List.map showInnings)

let showOption (option: UpdateOptionsForUI) dispatch =
  match option with
  | StartMatchUI -> simpleButton "Start match" StartMatch dispatch
  | StartNextInningsUI -> simpleButton "Start next innings" StartNextInnings dispatch
  | ContinueInningsBallUI -> simpleButton "Continue (ball)" ContinueInningsBall dispatch
  | ContinueInningsOverUI -> simpleButton "Continue (over)" ContinueInningsOver dispatch
  | MatchOverUI -> simpleButton "Reset match" ResetMatch dispatch

let showOptions match' dispatch =
  let options = MatchRunner.getOptionsUI match'
  div [ ClassName "level" ] [ div [ ClassName "level-left" ] (options |> List.map (fun option -> div [ ClassName "level-left" ] [ showOption option dispatch ])) ]

// main render method

let root model dispatch =
  let match' = model.Match
  div
    [ ClassName "content" ]
    [
      showOptions match' dispatch
      showSummaryStatus match'
      showAllInnings match'
    ]