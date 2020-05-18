module Cricket.Client.Lobby

type LobbyPage =
    | StartPage
    | LobbyAboutPage

type LobbyMsg =
    | SwitchPage of LobbyPage
    | StartOnClient
    | StartOnServer

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Fulma

importAll "./sass/main.sass"

let lobbyMenuItem label page currentPage dispatch =
    Menu.Item.li
      [ Menu.Item.IsActive (page = currentPage)
        Menu.Item.OnClick (fun _ -> SwitchPage page |> dispatch)]
      [ str label ]

let lobbyMenu currentPage dispatch =
  Menu.menu []
    [ Menu.label []
        [ str "Lobby menu" ]
      Menu.list []
        [ lobbyMenuItem "Start" StartPage currentPage dispatch
          lobbyMenuItem "About" LobbyAboutPage currentPage dispatch ] ]

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
