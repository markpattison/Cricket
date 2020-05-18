module Cricket.Client.Lobby

open Fable.React
open Fulma

type Page =
    | StartPage
    | LobbyAboutPage

type Msg =
    | SwitchPage of Page
    | StartOnClient
    | StartOnServer

let menuItem label page currentPage dispatch =
    Menu.Item.li
      [ Menu.Item.IsActive (page = currentPage)
        Menu.Item.OnClick (fun _ -> SwitchPage page |> dispatch)]
      [ str label ]

let menu currentPage dispatch =
  Menu.menu []
    [ Menu.list []
        [ menuItem "Start" StartPage currentPage dispatch
          menuItem "About" LobbyAboutPage currentPage dispatch ] ]

let simpleButton dispatch txt action =
  Control.div []
    [ Button.button
        [ Button.OnClick (fun e ->
                            e.preventDefault()
                            action |> dispatch) ]
        [ str txt ] ]

let view lobbyPage dispatch =
  let page =
    match lobbyPage with
    | LobbyAboutPage -> About.view ""
    | StartPage ->
      div []
        [
          simpleButton dispatch "Play in browser" StartOnClient
          simpleButton dispatch "Play on server" StartOnServer ]

  Columns.columns []
    [ Column.column
        [ Column.Width (Screen.All, Column.Is3) ]
        [ menu lobbyPage dispatch ]
      Column.column []
        [ page ] ]
