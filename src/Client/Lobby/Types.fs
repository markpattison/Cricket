namespace Cricket.Client.Lobby

open Cricket.Shared

type LobbyState =
    | Ready
    | WaitingForNewServerGame
    | NewServerGameReady of SessionId * DataFromServer
    | WaitingToLoadServerGame

type Msg =
    | RequestNewServerGame
    | RequestLoadServerGame
    | NewServerGameResponse of SessionId * DataFromServer
    | LoadServerGameResponse of SessionId * Result<DataFromServer, string>
    | StartNewServerGame
    | ServerSessionInitiated of SessionId * DataFromServer
    | ClientSessionInitiated
    | SessionIdTextChanged of string

type Model =
    {
        State: LobbyState
        SessionIdText: string
        LoadSessionError: bool
    }
