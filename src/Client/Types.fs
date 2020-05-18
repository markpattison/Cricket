module Cricket.Client.Types

open Lobby

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
