module FableCricket.LiveMatch.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma.Elements
open Fulma.Extra.FontAwesome
open Fulma.Layouts

open Cricket.CricketEngine
open Cricket.CricketEngine.Formatting
open Cricket.MatchRunner

open Types

let simpleButton txt action dispatch =
  Button.button_a
    [ Button.onClick (fun _ -> action |> dispatch) ]
    [ str txt ]

let showSummaryStatus match' =
  let summary = match' |> Match.summaryStatus
  Heading.h5
    [ Heading.isSubtitle ]
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
    td [ ClassName "has-text-weight-bold" ]  [ str (ii.Score.ToString()) ]
    td [] [str (ii.BallsFaced.ToString())]
    td [] [str (ii.Fours.ToString())]
    td [] [str (ii.Sixes.ToString())]
  ]

let showFallOfWickets innings =
  if List.isEmpty innings.FallOfWickets then
    None
  else
    let wicketText =
      innings.FallOfWickets
      |> List.map (fun fow -> sprintf "%i-%i (%s, %i.%i overs)" fow.Wicket fow.Runs fow.BatsmanOut.Name fow.Overs fow.BallsWithinOver)
      |> String.concat ", "
    Some ("Fall of wickets: " + wicketText)

let showBatting innings =
  let header = thead [] [ tr [ ClassName "has-text-weight-bold" ] [ td [] [str "Batsmen"]; td [] []; td [] [str "R"]; td [] [str "B"]; td [] [str "4"]; td [] [str "6"] ] ]
  let allIndividualInnings = tbody [] (innings.Batsmen |> List.map showIndividualInnings)
  
  let totalRow = tr [ ClassName "has-text-weight-bold" ] [ td [] [str "Total"]; td[] [str (oversString innings)]; td[] [str (Innings.summary innings)]; td[] []; td[] []; td[] [] ]
  let fallOfWicketsRow =
    showFallOfWickets innings
    |> Option.map (fun s -> tr [ ClassName "is-size-7" ] [ td [ ColSpan 6.0 ] [str s] ])
    |> Option.toList
  
  let footer = tfoot [] ([ totalRow ] @ fallOfWicketsRow)

  let rows = [ header; footer; allIndividualInnings ]

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

  Table.table [ Table.isFullwidth ] rows

let showInnings ((team, inningsNumber, innings), expanded) index dispatch =
  let teamInnings = sprintf "%s %s" team.Name (formatInningsNumber inningsNumber)
  let score = Innings.summary innings
  Box.box' []
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
                  Icon.faIcon
                    []
                    [ Fa.icon (if expanded then Fa.I.CaretDown else Fa.I.CaretRight) ]
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
  | ContinueInningsInningsUI -> simpleButton "Continue (innings)" ContinueInningsInningsMessage dispatch
  | MatchOverUI -> simpleButton "Reset match" ResetMatchMessage dispatch

let showOptions match' dispatch =
  let options = MatchRunner.getOptionsUI match'
  div [ ClassName "level" ] [ div [ ClassName "buttons level-left" ] (options |> List.map (fun option -> showOption option dispatch)) ]

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