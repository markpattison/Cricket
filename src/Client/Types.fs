module Cricket.Client.Types

type Msg =
    | LobbyMsg of Lobby.Msg
    | CricketMsg of CricketTypes.Msg

type OuterState =
    | Lobby of Lobby.Model
    | Playing of CricketTypes.Model

type Model =
    {
        OuterState: OuterState
    }
