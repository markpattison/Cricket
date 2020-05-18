module Cricket.Client.State

open Elmish

open Cricket.Client.Types

let init () =
    { OuterState = Lobby Lobby.StartPage }, Cmd.none

let update msg model =
    match model.OuterState, msg with
    | Lobby, LobbyMsg Lobby.StartOnClient ->
        let cricketModel, cmd = CricketState.initClient()
        { model with OuterState = Playing cricketModel }, Cmd.map CricketMsg cmd
    
    | Lobby, LobbyMsg Lobby.StartOnServer ->
        let cricketModel, cmd = CricketState.initServer()
        { model with OuterState = Playing cricketModel }, Cmd.map CricketMsg cmd
    
    | Lobby, LobbyMsg (Lobby.SwitchPage page) ->
        { model with OuterState = Lobby page }, Cmd.none
    
    | Lobby, _ ->
        model, Cmd.none
    
    | Playing cricketModel, CricketMsg cricketMsg ->
        let updatedCricketModel, cmd = CricketState.update cricketMsg cricketModel
        { model with OuterState = Playing updatedCricketModel }, Cmd.map CricketMsg cmd
    
    | Playing _, _ ->
        model, Cmd.none
