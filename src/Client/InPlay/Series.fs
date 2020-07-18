module Cricket.Client.InPlay.Series

open Fable.React
open Fulma

open Cricket.CricketEngine
open Cricket.Client.Extensions
open Cricket.Client.InPlay.Types

let simpleButton dispatch (txt, action)  =
  Control.div []
    [ Button.button
        [ Button.OnClick (fun e ->
                            e.preventDefault()
                            action |> dispatch) ]
        [ str txt ] ]

let showCompletedMatchSummary dispatch mtch =
  let text = sprintf "Test %i: %s" mtch.Index mtch.Summary
  div []
   [ span [] [ str text ]
     simpleButton dispatch ("Show", SwitchPage (SeriesPage (ShowMatch mtch.Index)))]
  //div [ Props.OnClick (fun _ -> dispatch (SwitchPage (SeriesPage (ShowMatch mtch.Index)))) ] [ str text ]

let showCompletedMatchSummaries dispatch matches =
  div [] (matches |> List.map (showCompletedMatchSummary dispatch))

let listCompletedMatches deferredSeries dispatch =
  match deferredSeries with
  | Resolved series ->
      div []
        [ Level.level [] [ Series.summary series |> str ]
          Content.content []
            [ showCompletedMatchSummaries dispatch series.CompletedMatches ] ]
  | _ ->
    Level.level [] [ str "Series loading..." ]

let showMatch completedMatches matchId dispatch =
  match Map.tryFind matchId completedMatches with
  | Some (Resolved mtch) -> Level.level [] [ str "Show a single match" ]
  | _ -> Level.level [] [ str "Match loading..." ]

let view seriesView deferredSeries completedMatches dispatch =
  match seriesView with
  | ListMatches -> listCompletedMatches deferredSeries dispatch
  | ShowMatch matchId -> showMatch completedMatches matchId dispatch
