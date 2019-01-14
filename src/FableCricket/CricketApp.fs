module FableCricket.App.View

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

open Fable.Core.JsInterop
open FableCricket
open FableCricket.App.Types
open FableCricket.App.State

open Elmish.HMR

importAll "./sass/main.sass"

open Fable.Helpers.React
open Fable.Helpers.React.Props

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
    | AboutPage -> Info.View.root
    | AveragesPage -> Averages.View.root model.cricket
    | CricketPage -> LiveMatch.View.root model.cricket (CricketMsg >> dispatch)

  div []
    [ div
        [ ClassName "navbar-bg" ]
        [ Container.container []
            [ Navbar.View.root ] ]
      Section.section []
        [ Container.container []
            [ Columns.columns []
                [ Column.column
                    [ Column.Width (Screen.All, Column.Is3) ]
                    [ menu model.currentPage ]
                  Column.column []
                    [ pageHtml model.currentPage ] ] ] ] ]

open Elmish.React
open Elmish.Debug
open Elmish.HMR

// App
Program.mkProgram init update root
|> Program.toNavigable (parseHash pageParser) urlUpdate
#if DEBUG
//|> Program.withDebugger
#endif
|> Program.withReact "elmish-app"
|> Program.run
