module Cricket.Client.View

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Fulma

open Cricket.CricketEngine
open Cricket.Client.CricketTypes

importAll "./sass/main.sass"

let menuItem label page currentPage =
    Menu.Item.li
      [ Menu.Item.IsActive (page = currentPage) ]
      [ str label ]

let menu currentPage =
  Menu.menu []
    [ Menu.label []
        [ str "General" ]
      Menu.list []
        [ menuItem "Scorecard" CricketPage currentPage
          menuItem "Averages" AveragesPage currentPage
          menuItem "About" AboutPage currentPage ] ]

let root model dispatch =

  let pageHtml =
    function
    | AboutPage -> About.view
    | AveragesPage -> Averages.view model
    | CricketPage -> LiveMatch.root model dispatch

  div []
    [ div
        [ ClassName "navbar-bg" ]
        [ Container.container []
            [ Navbar.view ] ]
      Section.section []
        [ Container.container []
            [ Columns.columns []
                [ Column.column
                    [ Column.Width (Screen.All, Column.Is3) ]
                    [ menu model.CurrentPage ]
                  Column.column []
                    [ pageHtml model.CurrentPage ] ] ] ] ]
