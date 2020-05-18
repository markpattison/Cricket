module Cricket.Client.View

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Fulma

open Cricket.CricketEngine
open Cricket.Client.CricketTypes
open Cricket.Client.Lobby
open Cricket.Client.Types

importAll "./sass/main.sass"

let lobbyMenuItem label page currentPage dispatch =
    Menu.Item.li
      [ Menu.Item.IsActive (page = currentPage)
        Menu.Item.OnClick (fun _ -> Lobby.SwitchPage page |> dispatch)]
      [ str label ]

let menuItem label page currentPage dispatch =
    Menu.Item.li
      [ Menu.Item.IsActive (page = currentPage)
        Menu.Item.OnClick (fun _ -> CricketTypes.SwitchPage page |> dispatch)]
      [ str label ]

let lobbyMenu currentPage dispatch =
  Menu.menu []
    [ Menu.label []
        [ str "Lobby menu" ]
      Menu.list []
        [ lobbyMenuItem "Start" StartPage currentPage dispatch
          lobbyMenuItem "About" LobbyAboutPage currentPage dispatch ] ]

let cricketMenu currentPage dispatch =
  Menu.menu []
    [ Menu.label []
        [ str "General" ]
      Menu.list []
        [ menuItem "Scorecard" CricketPage currentPage dispatch
          menuItem "Averages" AveragesPage currentPage dispatch
          menuItem "About" AboutPage currentPage dispatch ] ]

let simpleButton dispatch txt action =
  Control.div []
    [ Button.button
        [ Button.OnClick (fun e ->
                            e.preventDefault()
                            action |> dispatch) ]
        [ str txt ] ]

let lobbyView lobbyPage dispatch =
  match lobbyPage with
  | LobbyAboutPage -> About.view
  | StartPage ->
    div []
      [
        simpleButton dispatch "Play in browser" StartOnClient
        simpleButton dispatch "Play on server" StartOnServer ]

let cricketView cricketModel dispatch =
  match cricketModel.CurrentPage with
  | AboutPage -> About.view
  | AveragesPage -> Averages.view cricketModel
  | CricketPage -> LiveMatch.root cricketModel dispatch

let root (model: Model) dispatch =

  let menu =
    match model.OuterState with
    | Lobby lobbyPage -> lobbyMenu lobbyPage (LobbyMsg >> dispatch)
    | Playing cricketModel -> cricketMenu cricketModel.CurrentPage (CricketMsg >> dispatch)

  let page =
    match model.OuterState with
    | Lobby lobbyPage -> lobbyView lobbyPage (LobbyMsg >> dispatch)
    | Playing cricketModel -> cricketView cricketModel (CricketMsg >> dispatch)

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
                    [ menu ]
                  Column.column []
                    [ page ] ] ] ] ]