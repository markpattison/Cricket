module Cricket.Client.State

open Elmish

open Cricket.Client.Types

let init () =
    let lobbyState, lobbyCmd = Lobby.State.init()
    { OuterState = Lobby lobbyState }, Cmd.map LobbyMsg lobbyCmd

let update msg model =
    match model.OuterState, msg with
    | Lobby _, LobbyMsg (Lobby.ServerSessionInitiated (sessionId, mtch)) ->
        let cricketModel, cricketCmd = CricketState.initServer(sessionId, mtch)
        { model with OuterState = Playing cricketModel }, Cmd.map CricketMsg cricketCmd

    | Lobby _, LobbyMsg (Lobby.ClientSessionInitiated) ->
        let cricketModel, cricketCmd = CricketState.initClient()
        { model with OuterState = Playing cricketModel }, Cmd.map CricketMsg cricketCmd        
    
    | Lobby lobbyModel, LobbyMsg lobbyMsg ->
        let updatedLobbyModel, lobbyCmd = Lobby.State.update lobbyMsg lobbyModel
        { model with OuterState = Lobby updatedLobbyModel }, Cmd.map LobbyMsg lobbyCmd

    | Playing cricketModel, CricketMsg cricketMsg ->
        let updatedCricketModel, cricketCmd = CricketState.update cricketMsg cricketModel
        { model with OuterState = Playing updatedCricketModel }, Cmd.map CricketMsg cricketCmd
    
    | _, _ ->
        model, Cmd.none
