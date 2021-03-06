module Cricket.Client.View

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Fulma

open Cricket.Client.Types

importAll "./sass/main.sass"

let root (model: Model) dispatch =
  let page =
    match model.OuterState with
    | Lobby lobbyModel -> Lobby.View.view lobbyModel (LobbyMsg >> dispatch)
    | Playing cricketModel -> InPlay.View.view cricketModel (CricketMsg >> dispatch)

  div []
    [ div
        [ ClassName "navbar-bg" ]
        [ Container.container []
            [ Navbar.view ] ]
      Section.section []
        [ Container.container []
            [ page ] ] ]
