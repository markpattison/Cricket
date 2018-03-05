module FableCricket.LiveMatch.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma.Elements
open Fulma.Elements.Form
open Fulma.Extra.FontAwesome
open Fulma.Layouts

open Cricket.CricketEngine
open Cricket.CricketEngine.Formatting
open Cricket.MatchRunner

open Types

let simpleButton dispatch (txt, action)  =
  Control.div []
    [ Button.button
        [ Button.OnClick (fun _ -> action |> dispatch) ]
        [ str txt ]
    ]

let showSummaryStatus match' =
  let summary = match' |> Match.summaryStatus
  Heading.h5
    [ Heading.IsSubtitle ]
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

  Table.table [ Table.IsFullwidth ] rows

let showInnings ((team, inningsNumber, innings), expanded) index dispatch =
  let teamInnings = sprintf "%s %s" team.TeamName (formatInningsNumber inningsNumber)
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

let showOption (option: UpdateOptionsForUI) =
  match option with
  | StartMatchUI -> "Start match", StartMatchMessage
  | StartNextInningsUI -> "Start next innings", StartNextInningsMessage
  | ContinueInningsBallUI -> "Continue (ball)", ContinueInningsBallMessage
  | ContinueInningsOverUI -> "Continue (over)", ContinueInningsOverMessage
  | ContinueInningsInningsUI -> "Continue (innings)", ContinueInningsInningsMessage
  | MatchOverUI -> "Reset match", ResetMatchMessage

let showOptions dispatch match' =
  let options = MatchRunner.getOptionsUI match'
  Level.level []
    [ Level.left []
        [ Field.div [ Field.IsGrouped ]
            (List.map (showOption >> simpleButton dispatch) options )
        ]
    ]

// main render method
let root model dispatch =
  let match' = model.Match
  div
    []
    [
      showOptions dispatch match'
      showSummaryStatus match'
      showAllInnings match' model.InningsExpanded dispatch
    ]