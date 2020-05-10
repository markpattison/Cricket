module Cricket.Server.Session

open System
open Cricket.CricketEngine
open Cricket.Shared
open Cricket.MatchRunner

let saveDelay = 5000 // ms

type SessionMsg =
    | GetState of AsyncReplyChannel<DataFromServer>
    | GetStatistics of AsyncReplyChannel<Statistics>
    | Update of ServerMsg * AsyncReplyChannel<ServerModel>
    | SaveIfNotUpdated of ServerModel

let dataFromServerState state : DataFromServer =
    state.Match

let statisticsFromServerState state : Statistics =
    { LivePlayerRecords = state.LivePlayerRecords; Series = state.Series }

type Session () =

    let initialState =
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

    let agent = MailboxProcessor.Start(fun inbox ->

        let rec messageLoop state = async {
            let! msg = inbox.Receive()

            let updatedState =
                match msg with
                | GetState rc ->
                    rc.Reply(dataFromServerState state)
                    state
                | GetStatistics rc ->
                    rc.Reply(statisticsFromServerState state)
                    state
                | Update (sessionMsg, rc) ->
                    let updated = MatchRunner.update sessionMsg state
                    rc.Reply(updated)
                    updated
                | SaveIfNotUpdated oldState ->
                    if oldState = state then
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
    member this.GetStatistics() = agent.PostAndReply(fun rc -> GetStatistics rc)
    member this.Update(serverMsg) =
        let state = agent.PostAndReply(fun rc -> Update (serverMsg, rc))
        Async.Start(delayedSave state)
        dataFromServerState state
