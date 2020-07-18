module Cricket.Client.InPlay.View

open Fable.React
open Fulma

open Cricket.CricketEngine
open Cricket.Shared
open Cricket.Client
open Cricket.Client.Extensions
open Types


let menu currentPage dispatch =
  let menuItem label page =
    Menu.Item.li
      [ Menu.Item.IsActive (page = currentPage)
        Menu.Item.OnClick (fun _ -> Types.SwitchPage page |> dispatch)]
      [ str label ]
  
  Menu.menu []
    [ Menu.list []
        [ menuItem "Scorecard" CricketPage
          menuItem "Averages" AveragesPage
          menuItem "Series" (SeriesPage ListMatches)
          menuItem "About" AboutPage ] ]

let view cricketModel dispatch =
  let page =
    match cricketModel.CurrentPage with
    | AboutPage ->
        let extraText =
          match cricketModel.RunOption with
          | OnClient -> "Running in-browser."
          | OnServer (Resolved (SessionId sessionId)) -> sprintf "Running on server, session ID: %O" sessionId
          | OnServer _ -> "Running on server, not connected."
        About.view extraText
    | AveragesPage -> Averages.view cricketModel.Averages
    | SeriesPage seriesView -> Series.view seriesView cricketModel.Series cricketModel.CompletedMatches dispatch
    | CricketPage -> LiveMatch.root cricketModel dispatch
  
  Columns.columns []
    [ Column.column
        [ Column.Width (Screen.All, Column.Is3) ]
        [ menu cricketModel.CurrentPage dispatch ]
      Column.column []
        [ page ] ]
