module Cricket.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

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
    [ ClassName "subtitle is-5" ]
    [ str summary ]

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

let showBatting innings =
  let headerRow = thead [] [ tr [ ClassName "has-text-weight-bold" ] [ td [] [str "Batsmen"]; td [] []; td [] [str "R"]; td [] [str "B"]; td [] [str "4"]; td [] [str "6"] ] ]
  let allIndividualInnings = tbody [] (innings.Batsmen |> List.map showIndividualInnings)
  let totalRow = tfoot [] [ tr [ ClassName "has-text-weight-bold" ] [ td [] [str "Total"]; td[] [str (oversString innings)]; td[] [str (Innings.summary innings)] ] ]
  let rows = [ headerRow ; allIndividualInnings; totalRow ]

  div [] [ table [ ClassName "table is-fullwidth" ] rows ]

let showIndividualBowling (p: Player, bowling) =
  tr [] [
    td [] [str p.Name]
    td [] [str (formatOversFromBalls bowling.Balls)]
    td [] [str (bowling.Maidens.ToString())]
    td [] [str (bowling.RunsConceded.ToString())]
    td [] [str (bowling.Wickets.ToString())]
  ]

let showBowling innings =
  let headerRow = thead [] [ tr [ ClassName "has-text-weight-bold" ] [ td [] [str "Bowling"]; td [] [str "O"]; td [] [str "M"]; td [] [str "R"]; td [] [str "W"] ] ]
  let allBowling = tbody [] (innings.Bowlers |> List.map showIndividualBowling)
  let rows = [ headerRow ; allBowling ]

  div [] [ table [ ClassName "table is-fullwidth" ] rows ]

let showInnings ((team, inningsNumber, innings), expanded) index dispatch =
  let teamInnings = sprintf "%s %s" team (formatInningsNumber inningsNumber)
  let score = Innings.summary innings
  div [ ClassName "box" ]
    [
      yield div
        [
          ClassName "level subtitle is-5";
          OnClick (fun _ -> (ToggleInningsExpandedMessage index) |> dispatch)
        ]
        [
          div [ ClassName "level-left" ]
            [
              div [ ClassName "level-item" ]
                [
                  span
                    [ ClassName "icon" ]
                    [ i [ ClassName (if expanded then "fa fa-caret-down" else "fa fa-caret-right") ] [] ]
                  str teamInnings
                ]
            ]
          div [ ClassName "level-right" ]
            [
              p [ ClassName "level-item" ]
                [ str score ]
            ]            
        ]
      if expanded then yield! [ showBatting innings; showBowling innings ]
    ]

let showAllInnings match' inningsExpanded dispatch =
  let allInnings = match' |> Match.inningsList
  let withExpanded = List.zip allInnings inningsExpanded
  div [] (withExpanded |> List.mapi (fun i innings -> showInnings innings i dispatch))

let showOption (option: UpdateOptionsForUI) dispatch =
  match option with
  | StartMatchUI -> simpleButton "Start match" StartMatchMessage dispatch
  | StartNextInningsUI -> simpleButton "Start next innings" StartNextInningsMessage dispatch
  | ContinueInningsBallUI -> simpleButton "Continue (ball)" ContinueInningsBallMessage dispatch
  | ContinueInningsOverUI -> simpleButton "Continue (over)" ContinueInningsOverMessage dispatch
  | MatchOverUI -> simpleButton "Reset match" ResetMatchMessage dispatch

let showOptions match' dispatch =
  let options = MatchRunner.getOptionsUI match'
  div [ ClassName "level" ] [ div [ ClassName "level-left" ] (options |> List.map (fun option -> div [ ClassName "level-left" ] [ showOption option dispatch ])) ]

// main render method
let root model dispatch =
  let match' = model.Match
  div
    []
    [
      showOptions match' dispatch
      showSummaryStatus match'
      showAllInnings match' model.InningsExpanded dispatch
    ]