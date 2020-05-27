module Cricket.Client.CricketView

open Fable.React
open Fulma

open Cricket.CricketEngine
open Cricket.Shared
open Cricket.Client.CricketTypes
open Cricket.Client.Extensions

let menuItem label page currentPage dispatch =
    Menu.Item.li
      [ Menu.Item.IsActive (page = currentPage)
        Menu.Item.OnClick (fun _ -> CricketTypes.SwitchPage page |> dispatch)]
      [ str label ]

let menu currentPage dispatch =
  Menu.menu []
    [ Menu.list []
        [ menuItem "Scorecard" CricketPage currentPage dispatch
          menuItem "Averages" AveragesPage currentPage dispatch
          menuItem "About" AboutPage currentPage dispatch ] ]

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
    | AveragesPage -> Averages.view cricketModel
    | CricketPage -> LiveMatch.root cricketModel dispatch
  
  Columns.columns []
    [ Column.column
        [ Column.Width (Screen.All, Column.Is3) ]
        [ menu cricketModel.CurrentPage dispatch ]
      Column.column []
        [ page ] ]