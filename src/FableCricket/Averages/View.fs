module FableCricket.Averages

open Fable.React
open Fulma

open Cricket.CricketEngine
open Cricket.CricketEngine.Formatting
open Cricket.CricketEngine.Averages

let showIndividualBatting (batting: BattingAverage) =
  tr [] [
    td [] [ str batting.Player.Name ]
    td [] [ str (formatMatches batting.Matches batting.InProgress) ]
    td [] [ str (batting.Innings.ToString()) ]
    td [] [ str (batting.NotOuts.ToString()) ]
    td [] [ str (batting.Runs.ToString()) ]
    td [] [ str (formatHighScore batting.HighScore) ]
    td [] [ strong [] [str (formatAverage batting.Average) ] ]
    td [] [ str (batting.BallsFaced.ToString()) ]
    td [] [ str (formatAverage batting.StrikeRate) ]
    td [] [ str (batting.Hundreds.ToString()) ]
    td [] [ str (batting.Fifties.ToString()) ]
    td [] [ str (batting.Ducks.ToString()) ]
    td [] [ str (batting.Fours.ToString()) ]
    td [] [ str (batting.Sixes.ToString()) ] ]

let showBatting batting =
  let headerRow =
    thead []
      [ tr [] [
          td [] [ str "Player" ]
          td [] [ str "Mat" ]
          td [] [ str "Inns" ]
          td [] [ str "NO" ]
          td [] [ str "Runs" ]
          td [] [ str "HS" ]
          td [] [ strong [] [ str "Ave"] ]
          td [] [ str "BF" ]
          td [] [ str "SR" ]
          td [] [ str "100" ]
          td [] [ str "50" ]
          td [] [ str "0" ]
          td [] [ str "4s" ]
          td [] [ str "6s" ] ] ]
  
  let averages = tbody [] (batting |> List.map showIndividualBatting)
  Table.table
    [ Table.IsFullWidth ]
    [ headerRow 
      averages ]

let showIndividualBowling (bowling: BowlingAverage) =
  tr [] [
    td [] [ str bowling.Player.Name ]
    td [] [ str (formatMatches bowling.Matches bowling.InProgress) ]
    td [] [ str (bowling.Innings.ToString()) ]
    td [] [ str (formatOversFromBalls bowling.BallsBowled) ]
    td [] [ str (bowling.Maidens.ToString()) ]
    td [] [ str (bowling.RunsConceded.ToString()) ]
    td [] [ str (bowling.Wickets.ToString()) ]
    td [] [ str (formatBestBowling bowling.BestInnings) ]
    td [] [ str (formatBestBowling bowling.BestMatch) ]
    td [] [ strong [] [str (formatAverage bowling.Average)] ]
    td [] [ str (formatAverage bowling.Economy) ]
    td [] [ str (formatAverage bowling.StrikeRate) ]
    td [] [ str (bowling.FiveWicketInnings.ToString()) ]
    td [] [ str (bowling.TenWicketMatches.ToString()) ]
    td [] [ str (bowling.Catches.ToString()) ]
    td [] [ str (bowling.Stumpings.ToString()) ] ]

let showBowling bowling =
  let headerRow =
    thead []
      [ tr [] [
          td [] [str "Player"]
          td [] [str "Mat"]
          td [] [str "Inns"]
          td [] [str "Overs"]
          td [] [str "Mdns"]
          td [] [str "Runs"]
          td [] [str "Wkts"]
          td [] [str "BBI"]
          td [] [str "BBM"]
          td [] [strong [] [str "Ave"] ]
          td [] [str "Econ"]
          td [] [str "SR"]
          td [] [str "5"]
          td [] [str "10"]
          td [] [str "Ct"]
          td [] [str "St"] ] ]
  
  let averages = tbody [] (bowling |> List.map showIndividualBowling)
  Table.table
    [ Table.IsFullWidth ]
    [ headerRow
      averages ]

let showAverages playerRecords =
  let batting =
    playerRecords
    |> Map.toList
    |> List.map createBattingAverage
    |> List.sort
  let bowling =
    playerRecords
    |> Map.toList
    |> List.map createBowlingAverage
    |> List.sort
  Content.content
    [ Content.Size IsSmall ]
    [ showBatting batting
      showBowling bowling ]

let showSeriesSummary series =
  let summary = Series.summary series
  Level.level [] [ str summary ]

// main render method
let view (model: FableCricket.LiveMatch.Types.Model) =
  div []
    [ showSeriesSummary model.Series
      showAverages model.LivePlayerRecords ]