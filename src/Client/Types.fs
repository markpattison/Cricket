module Cricket.Client.Types

open Cricket.CricketEngine
open Cricket.MatchRunner
open Cricket.Shared
open Cricket.Client.Extensions

type LobbyPage =
    | StartPage
    | LobbyAboutPage

type LobbyMsg =
    | SwitchPage of LobbyPage
    | StartOnClient
    | StartOnServer

type Msg =
    | LobbyMsg of LobbyMsg
    | CricketMsg of CricketTypes.Msg

type OuterState =
    | Lobby of LobbyPage
    | Playing of CricketTypes.Model

type Model =
    {
        OuterState: OuterState
    }
