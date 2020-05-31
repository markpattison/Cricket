module Cricket.Client.Types

type Msg =
    | LobbyMsg of Lobby.Msg
    | CricketMsg of InPlay.Types.Msg

type OuterState =
    | Lobby of Lobby.Model
    | Playing of InPlay.Types.Model

type Model =
    {
        OuterState: OuterState
    }
