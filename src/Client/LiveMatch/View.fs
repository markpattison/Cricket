module Cricket.Client.LiveMatch.View

open Fable.React
open Fable.React.Props

open Fulma
open Fable.FontAwesome

open Cricket.CricketEngine
open Cricket.CricketEngine.Formatting
open Cricket.MatchRunner
open Cricket.Client.Extensions

open Types

let simpleButton dispatch isDisabled (txt, action)  =
  Control.div []
    [ Button.button
        [ Button.OnClick (fun e ->
                            e.preventDefault()
                            action |> dispatch)
          Button.Disabled isDisabled ]
        [ str txt ] ]

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
    td [] [ str p.Name ]
    td [] [ str (howOutString ii.HowOut) ]
    td [] [ strong [] [ str (ii.Score.ToString()) ] ]
    td [] [ str (ii.BallsFaced.ToString()) ]
    td [] [ str (ii.Fours.ToString()) ]
    td [] [ str (ii.Sixes.ToString()) ] ]

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
  let header =
    thead []
      [ tr []
          [ td [] [ strong [] [ str "Batsmen" ] ]
            td [] []
            td [] [ strong [] [ str "R" ] ]
            td [] [ strong [] [ str "B" ] ]
            td [] [ strong [] [ str "4" ] ]
            td [] [ strong [] [ str "6" ] ] ] ]

  let allIndividualInnings = tbody [] (innings.Batsmen |> List.map showIndividualInnings)
  
  let totalRow =
    tr []
      [ td [] [ strong [] [ str "Total"] ]
        td [] [ strong [] [ str (oversString innings)] ]
        td [] [ strong [] [ str (Innings.summary innings)] ]
        td [] []
        td [] []
        td [] [] ]

  let fallOfWicketsRow =
    showFallOfWickets innings
    |> Option.map (fun s ->
        tr []
          [ td
              [ ColSpan 6 ]
              [ Content.content
                  [ Content.Size IsSmall ]
                  [ str s ] ] ])
    |> Option.toList
  
  let footer =
    tfoot []
      (totalRow :: fallOfWicketsRow)

  Table.table
    [ Table.IsFullWidth ]
    [ header
      footer
      allIndividualInnings ]

let showIndividualBowling (p: Player, bowling) =
  tr [] [
    td [] [ str p.Name ]
    td [] [ str (formatOversFromBalls bowling.Balls) ]
    td [] [ str (bowling.Maidens.ToString()) ]
    td [] [ str (bowling.RunsConceded.ToString()) ]
    td [] [ str (bowling.Wickets.ToString()) ] ]

let showBowling innings =
  let headerRow =
    thead []
      [ tr []
          [ td [] [ strong [] [ str "Bowling" ] ]
            td [] [ strong [] [ str "O" ] ]
            td [] [ strong [] [ str "M" ] ]
            td [] [ strong [] [ str "R" ] ]
            td [] [ strong [] [ str "W" ] ] ] ]
  
  let allBowling =
    tbody []
      (innings.Bowlers |> List.map showIndividualBowling)

  Table.table
    [ Table.IsFullWidth ]
    [ headerRow
      allBowling ]

let showInnings ((team, inningsNumber, innings), expanded) index dispatch =
  let teamInnings = sprintf "%s %s" team.TeamName (formatInningsNumber inningsNumber)
  let score = Innings.summary innings
  Box.box' []
    [ yield Level.level
        [ Level.Level.Props [ OnClick (fun _ -> (ToggleInningsExpandedMessage index) |> dispatch) ] ]
        [ Level.left []
            [ Level.item []
                [ Icon.icon []
                    [ Fa.i [ (if expanded then Fa.Solid.CaretDown else Fa.Solid.CaretRight) ] [] ]
                  Heading.h5
                    [ Heading.IsSubtitle ]
                    [ str teamInnings ] ] ]
          Level.right []
            [ Level.item []
                [ Heading.h5
                    [ Heading.IsSubtitle ]
                    [ str score ] ] ] ]
      if expanded then yield! [ showBatting innings; showBowling innings ] ]

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

let showOptions dispatch match' isUpdateInProgress =
  let options = MatchRunner.getOptionsUI match'
  Level.level []
    [ Level.left []
        [ Field.div
            [ Field.IsGrouped ]
            (List.map (showOption >> simpleButton dispatch isUpdateInProgress) options ) ] ]

let showMatch mtch isUpdateInProgress inningsExpanded dispatch =
    div []
      [ showOptions (ServerMsg >> dispatch) mtch isUpdateInProgress
        showSummaryStatus mtch
        showAllInnings mtch inningsExpanded dispatch ]

// main render method
let root model dispatch =
  match model.Match with
  | Resolved mtch ->
    showMatch mtch false model.InningsExpanded dispatch
  | InProgress (Some mtch) ->
    showMatch mtch true model.InningsExpanded dispatch
  | _ -> Level.level [] [ str "Match loading..." ]