module Cricket.Server.Session

open System
open Cricket.CricketEngine
open Cricket.Shared
open Cricket.MatchRunner

type SessionMsg =
    | GetState of AsyncReplyChannel<DataFromServer>
    | Update of ServerMsg * AsyncReplyChannel<DataFromServer>

let dataFromServerState state : DataFromServer =
    (state.Match, state.LivePlayerRecords, state.Series)

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

            let updatedState, rc =
                match msg with
                | GetState rc -> state, rc
                | Update (sessionMsg, rc) -> MatchRunner.update sessionMsg state, rc
            
            rc.Reply(dataFromServerState updatedState)
            return! messageLoop updatedState
            }

        // start the loop
        messageLoop initialState
        )

    // public interface
    member this.GetData() = agent.PostAndReply(fun rc -> GetState rc)
    member this.Update(serverMsg) = agent.PostAndReply(fun rc -> Update (serverMsg, rc))
