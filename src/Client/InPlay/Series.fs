module Cricket.Client.InPlay.Series

open Fable.React
open Fulma

open Cricket.CricketEngine
open Cricket.Client.Extensions

let showCompletedMatch mtch =
  p [] [ sprintf "Test %i: %s" mtch.Index mtch.Summary |> str ]

let showCompletedMatches matches =
  div [] (matches |> List.map showCompletedMatch)

let showSeries series =
  div []
    [ Level.level [] [ Series.summary series |> str ]
      Content.content []
        [ showCompletedMatches series.CompletedMatches ] ]

let view deferredSeries =
  match deferredSeries with
  | Resolved series -> showSeries series
  | _ ->
    Level.level [] [ str "Series loading..." ]
