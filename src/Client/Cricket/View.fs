module Cricket.Client.CricketView

open Fable.React
open Fulma

open Cricket.CricketEngine
open Cricket.Client.CricketTypes

let menuItem label page currentPage dispatch =
    Menu.Item.li
      [ Menu.Item.IsActive (page = currentPage)
        Menu.Item.OnClick (fun _ -> CricketTypes.SwitchPage page |> dispatch)]
      [ str label ]

let menu currentPage dispatch =
  Menu.menu []
    [ Menu.label []
        [ str "General" ]
      Menu.list []
        [ menuItem "Scorecard" CricketPage currentPage dispatch
          menuItem "Averages" AveragesPage currentPage dispatch
          menuItem "About" AboutPage currentPage dispatch ] ]

let view cricketModel dispatch =
  let page =
    match cricketModel.CurrentPage with
    | AboutPage -> About.view
    | AveragesPage -> Averages.view cricketModel
    | CricketPage -> LiveMatch.root cricketModel dispatch
  
  Columns.columns []
    [ Column.column
        [ Column.Width (Screen.All, Column.Is3) ]
        [ menu cricketModel.CurrentPage dispatch ]
      Column.column []
        [ page ] ]