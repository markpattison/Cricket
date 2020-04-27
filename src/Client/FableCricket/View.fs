module FableCricket.App.View

open Fable.Core.JsInterop
open FableCricket
open FableCricket.App.Types

importAll "./sass/main.sass"

open Fable.React
open Fable.React.Props

open Fulma

open Cricket.CricketEngine
open FableCricket.Router

let menuItem label page currentPage =
    Menu.Item.li
      [ Menu.Item.IsActive (page = currentPage)
        Menu.Item.Props [ Router.href page ] ]
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
    | AveragesPage -> Averages.view model.cricket
    | CricketPage -> LiveMatch.View.root model.cricket (CricketMsg >> dispatch)

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
                    [ menu model.currentPage ]
                  Column.column []
                    [ pageHtml model.currentPage ] ] ] ] ]
