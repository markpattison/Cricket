module Cricket.Client.Lobby.State

open Elmish

open Cricket.Shared

let init() =
    {
        State = Ready
        SessionIdText = ""
        LoadSessionError = false
    }, Cmd.none

let initiateSession = Cmd.OfAsync.perform Cricket.Client.Server.api.newSession () NewServerGameResponse
let loadSession sessionId = Cmd.OfAsync.perform Cricket.Client.Server.api.loadSession sessionId (fun response -> LoadServerGameResponse (sessionId, response))

let update msg model =
    match model.State, msg with
    | Ready, RequestNewServerGame ->
        { model with State = WaitingForNewServerGame; LoadSessionError = false }, initiateSession
    
    | WaitingForNewServerGame, NewServerGameResponse (sessionId, mtch) ->
        model, Cmd.ofMsg (ServerSessionInitiated (sessionId, mtch))
    
    | Ready, RequestLoadServerGame ->
        match System.Guid.TryParse(model.SessionIdText) with
        | true, guid ->
            let sessionId = SessionId guid
            { model with State = WaitingToLoadServerGame }, loadSession sessionId
        | false, _ ->
            { model with LoadSessionError = true }, Cmd.none
    
    | WaitingToLoadServerGame, LoadServerGameResponse (sessionId, Ok mtch) ->
        model, Cmd.ofMsg (ServerSessionInitiated (sessionId, mtch))
    
    | WaitingToLoadServerGame, LoadServerGameResponse (_, Error err) ->
        { model with State = Ready; LoadSessionError = true }, Cmd.none

    | _, SessionIdTextChanged s ->
        { model with SessionIdText = s }, Cmd.none

    | _, _ ->
        model, Cmd.none