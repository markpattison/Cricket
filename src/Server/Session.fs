module Cricket.Server.Session

open Cricket.CricketEngine
open Cricket.Shared
open Cricket.MatchRunner

let saveDelay = 5000 // ms

type SessionMsg =
    | GetState of AsyncReplyChannel<DataFromServer>
    | GetAverages of AsyncReplyChannel<Averages>
    | GetSeries of AsyncReplyChannel<Series>
    | GetCompletedMatch of int * AsyncReplyChannel<Result<CompletedMatch, string>>
    | Update of ServerMsg * AsyncReplyChannel<ServerModel>
    | SaveIfNotUpdated of ServerModel

let dataFromServerState state : DataFromServer =
    state.Match

let averagesFromServerState state : Averages =
    state.LivePlayerRecords

let seriesFromServerState state : Series =
    state.Series

let completedMatchFromServerState state matchId : Result<CompletedMatch, string> =
    match Map.tryFind matchId state.CompletedMatches with
    | Some mtch -> Ok (matchId, mtch)
    | None -> Error "match not found"

type Session(initialState: ServerModel, saveState: ServerModel -> unit) =

    let agent = MailboxProcessor.Start(fun inbox ->

        let rec messageLoop state = async {
            let! msg = inbox.Receive()

            let updatedState =
                match msg with
                | GetState rc ->
                    rc.Reply(dataFromServerState state)
                    state
                | GetAverages rc ->
                    rc.Reply(averagesFromServerState state)
                    state
                | GetSeries rc ->
                    rc.Reply(seriesFromServerState state)
                    state
                | GetCompletedMatch (matchId, rc) ->
                    rc.Reply(completedMatchFromServerState state matchId)
                    state
                | Update (sessionMsg, rc) ->
                    let updated = MatchRunner.update sessionMsg state
                    rc.Reply(updated)
                    updated
                | SaveIfNotUpdated oldState ->
                    if oldState = state then
                        saveState state
                        printfn "Saving..."
                    state
            
            return! messageLoop updatedState
            }

        // start the loop
        messageLoop initialState
        )

    let delayedSave state =
        async {
            do! Async.Sleep(saveDelay)
            agent.Post(SaveIfNotUpdated state)
        }

    // public interface
    member this.GetData() = agent.PostAndReply(fun rc -> GetState rc)
    member this.GetAverages() = agent.PostAndReply(fun rc -> GetAverages rc)
    member this.GetSeries() = agent.PostAndReply(fun rc -> GetSeries rc)
    member this.GetCompletedMatch(matchId) = agent.PostAndReply(fun rc -> GetCompletedMatch (matchId, rc))
    member this.Update(serverMsg) =
        let state = agent.PostAndReply(fun rc -> Update (serverMsg, rc))
        Async.Start(delayedSave state)
        dataFromServerState state
