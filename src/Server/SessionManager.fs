module Cricket.Server.SessionManager

open System
open Microsoft.Azure
open Microsoft.Azure.Cosmos.Table
open Microsoft.AspNetCore.Http
open Saturn

open Cricket.Shared
open Cricket.CricketEngine
open Cricket.MatchRunner
open Session

type SessionManagerState =
    {
        Sessions: Map<SessionId, Session>
    }

type SessionManagerMsg =
    | NewSession of Config * AsyncReplyChannel<SessionId * DataFromServer>
    | LoadSession of Config * SessionId * AsyncReplyChannel<Result<DataFromServer, string>>
    | Update of SessionId * ServerMsg * AsyncReplyChannel<Result<DataFromServer, string>>
    | GetStatistics of SessionId * AsyncReplyChannel<Result<Statistics, string>>

let getStorageTable config =
    let storageAccount = CloudStorageAccount.Parse(config.StorageConnectionString)
    let tableClient = storageAccount.CreateCloudTableClient()
    let table = tableClient.GetTableReference("cricket")
    table.CreateIfNotExists() |> ignore
    
    table

let initialSessionState =
    {   
        Match = MatchData.newMatch
        PlayerRecords = Map.empty
        LivePlayerRecords = Map.empty
        PlayerAttributes =
            {
                Attributes =
                    MatchData.players
                    |> List.mapi (fun n (_, bat, bowl) -> n, { BattingSkill = bat; BowlingSkill = bowl })
                    |> Map.ofList
            }
        Series = Series.create "England" "India"
    }

type SessionManager () =

    let initialState = { Sessions = Map.empty }

    let agent = MailboxProcessor.Start(fun inbox ->

        let rec messageLoop state = async {
            let! msg = inbox.Receive()

            let updatedState =
                match msg with
                | NewSession (config, rc) ->
                    let table = getStorageTable config
                    let newSessionId = SessionId (Guid.NewGuid())
                    let saveState = Storage.save table newSessionId

                    let newSession = Session(initialSessionState, saveState)
                    let newSessionState = newSession.GetData()

                    rc.Reply(newSessionId, newSessionState)
                    { state with
                        Sessions = Map.add newSessionId newSession state.Sessions
                    }
                
                | LoadSession (config, sessionId, rc) ->
                    match Map.tryFind sessionId state.Sessions with
                    | Some session ->
                        let data = session.GetData()
                        rc.Reply(Ok data)
                        state
                    | None ->
                        let table = getStorageTable config
                        let loadResult = Storage.load table sessionId

                        match loadResult with
                            | Ok sessionState ->
                                let table = getStorageTable config
                                let newSessionId = SessionId (Guid.NewGuid())
                                let saveState = Storage.save table newSessionId

                                let newSession = Session(sessionState, saveState)
                                let newSessionState = newSession.GetData()

                                rc.Reply(Ok newSessionState)
                                { state with
                                    Sessions = Map.add sessionId newSession state.Sessions
                                }
                            
                            | Error msg ->
                                rc.Reply(Error msg)
                                state

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
        let config : Config = Controller.getConfig ctx
        agent.PostAndReply(fun rc -> NewSession (config, rc))
    member this.LoadSession(ctx, sessionId) =
        let config : Config = Controller.getConfig ctx
        agent.PostAndReply(fun rc -> LoadSession (config, sessionId, rc))
    member this.Update(sessionId, serverMsg) = agent.PostAndReply(fun rc -> Update (sessionId, serverMsg, rc))
    member this.GetStatistics(sessionId) = agent.PostAndReply(fun rc -> GetStatistics (sessionId, rc))
