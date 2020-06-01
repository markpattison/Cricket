module Cricket.Client.InPlay.Series

open Fable.React
open Fulma

open Cricket.CricketEngine
open Cricket.CricketEngine.Formatting
open Cricket.CricketEngine.Averages
open Cricket.Client.Extensions

let showSeriesSummary deferredSeries =
  let summary =
    match deferredSeries with
    | Resolved series -> Series.summary series
    | _ -> "Series loading..."
  Level.level [] [ str summary ]

// main render method
let view deferredSeries =
  div []
    [ showSeriesSummary deferredSeries ]