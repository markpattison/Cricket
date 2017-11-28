module Averages.View

open Fable.Core
open Fable.Helpers.React
open Fable.Helpers.React.Props

open Elmish

open Cricket.CricketEngine
open Cricket.CricketEngine.Formatting

open Cricket.Types
open Cricket.CricketEngine.Averages
open Cricket.View

let showIndividualBatting (batting: BattingAverage) =
  tr [] [
    td [] [str batting.Player.Name]
    td [] [str (formatMatches batting.Matches batting.InProgress)]
    td [] [str (batting.Innings.ToString())]
    td [] [str (batting.NotOuts.ToString())]
    td [] [str (batting.Runs.ToString())]
    td [] [str (formatHighScore batting.HighScore)]
    td [ ClassName "has-text-weight-bold" ] [str (formatAverage batting.Average)]
    td [] [str (batting.BallsFaced.ToString())]
    td [] [str (formatAverage batting.StrikeRate)]
    td [] [str (batting.Hundreds.ToString())]
    td [] [str (batting.Fifties.ToString())]
    td [] [str (batting.Ducks.ToString())]
    td [] [str (batting.Fours.ToString())]
    td [] [str (batting.Sixes.ToString())]
  ]

let showBatting batting =
  let headerRow = thead [] [ tr [] [ td [] [str "Player"]; td [] [str "Mat"]; td [] [str "Inns"]; td [] [str "NO"]; td [] [str "Runs"]; td [] [str "HS"]; td [ ClassName "has-text-weight-bold" ] [str "Ave"]; td [] [str "BF"]; td [] [str "SR"]; td [] [str "100"]; td [] [str "50"]; td [] [str "0"]; td [] [str "4s"]; td [] [str "6s"] ] ]
  let averages = tbody [] (batting |> List.map showIndividualBatting)
  let rows = [ headerRow ; averages ]
  div [] [ table [ ClassName "table is-fullwidth is-narrow is-size-7" ] rows ]

let showIndividualBowling (bowling: BowlingAverage) =
  tr [] [
    td [] [str bowling.Player.Name]
    td [] [str (formatMatches bowling.Matches bowling.InProgress)]
    td [] [str (bowling.Innings.ToString())]
    td [] [str (formatOversFromBalls bowling.BallsBowled)]
    td [] [str (bowling.Maidens.ToString())]
    td [] [str (bowling.RunsConceded.ToString())]
    td [] [str (bowling.Wickets.ToString())]
    td [] [str (formatBestBowling bowling.BestInnings)]
    td [] [str (formatBestBowling bowling.BestMatch)]
    td [ ClassName "has-text-weight-bold" ] [str (formatAverage bowling.Average)]
    td [] [str (formatAverage bowling.Economy)]
    td [] [str (formatAverage bowling.StrikeRate)]
    td [] [str (bowling.FiveWicketInnings.ToString())]
    td [] [str (bowling.TenWicketMatches.ToString())]
    td [] [str (bowling.Catches.ToString())]
    td [] [str (bowling.Stumpings.ToString())]
  ]

let showBowling bowling =
  let headerRow = thead [] [ tr [] [ td [] [str "Player"]; td [] [str "Mat"]; td [] [str "Inns"]; td [] [str "Overs"]; td [] [str "Mdns"]; td [] [str "Runs"]; td [] [str "Wkts"]; td [] [str "BBI"]; td [] [str "BBM"]; td [ ClassName "has-text-weight-bold" ] [str "Ave"]; td [] [str "Econ"]; td [] [str "SR"]; td [] [str "5"]; td [] [str "10"]; td [] [str "Ct"]; td [] [str "St"] ] ]
  let averages = tbody [] (bowling |> List.map showIndividualBowling)
  let rows = [ headerRow ; averages ]
  div [] [ table [ ClassName "table is-fullwidth is-narrow is-size-7" ] rows ]

let showAverages playerRecords =
    let batting =
        playerRecords
        |> Map.toList
        |> List.map Averages.createBattingAverage
        |> List.sort
    let bowling =
        playerRecords
        |> Map.toList
        |> List.map Averages.createBowlingAverage
        |> List.sort
    div
        []
        [
            showBatting batting
            showBowling bowling
        ]

// main render method
let root model =
    div []
        [
            showAverages model.LivePlayerRecords
        ]