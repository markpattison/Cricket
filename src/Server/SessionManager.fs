module Cricket.Server.SessionManager

open System
open Microsoft.Azure
open Microsoft.Azure.Cosmos.Table
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.ContextInsensitive

open Saturn
open Cricket.Shared
open Cricket.MatchRunner
open Session

type SessionManagerState =
    {
        Sessions: Map<SessionId, Session>
    }

type SessionManagerMsg =
    | NewSession of AsyncReplyChannel<SessionId * DataFromServer>
    | Update of SessionId * ServerMsg * AsyncReplyChannel<Result<DataFromServer, string>>
    | GetStatistics of SessionId * AsyncReplyChannel<Result<Statistics, string>>

type SessionManager () =

    let initialState = { Sessions = Map.empty }

    let agent = MailboxProcessor.Start(fun inbox ->

        let rec messageLoop state = async {
            let! msg = inbox.Receive()

            let updatedState =
                match msg with
                | NewSession rc ->
                    let newSessionId = SessionId (Guid.NewGuid())
                    let newSession = Session()
                    let newSessionState = newSession.GetData()

                    rc.Reply(newSessionId, newSessionState)
                    { state with
                        Sessions = Map.add newSessionId newSession state.Sessions
                    }
                
                | Update (sessionId, sessionMsg, rc) ->
                    match Map.tryFind sessionId state.Sessions with
                    | Some session ->
                        let updatedData = session.Update(sessionMsg)
                        rc.Reply(Ok updatedData)
                        state
                    
                    | None ->
                        printfn "Session not found: %A" sessionId
                        rc.Reply(Error (sprintf "Session not found: %A" sessionId))
                        state
                
                | GetStatistics (sessionId, rc) ->
                    match Map.tryFind sessionId state.Sessions with
                    | Some session ->
                        let statistics = session.GetStatistics()
                        rc.Reply(Ok statistics)
                        state
                    
                    | None ->
                        printfn "Session not found: %A" sessionId
                        rc.Reply(Error (sprintf "Session not found: %A" sessionId))
                        state
            
            return! messageLoop updatedState
            }

        // start the loop
        messageLoop initialState
        )

    // public interface
    member this.NewSession(ctx) =
        // let config : Config = Controller.getConfig ctx
        // printfn "Conn Str: %s" config.StorageConnectionString
        agent.PostAndReply(fun rc -> NewSession rc)
    member this.Update(ctx, sessionId, serverMsg) = agent.PostAndReply(fun rc -> Update (sessionId, serverMsg, rc))
    member this.GetStatistics(ctx, sessionId) = agent.PostAndReply(fun rc -> GetStatistics (sessionId, rc))
