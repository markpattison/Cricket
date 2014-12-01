namespace Match

open MatchState

type Match =
    {
        TeamA: string;
        TeamB: string;
        State: MatchState
    }
    member x.UpdateState (update: MatchState -> MatchState) = { x with State = (update x.State) }

[<AutoOpen>]
module MatchFunctions =
    
    let SummaryStatus rules _match =
        match _match.State with
        | NotStarted -> "Match not started"
        | Abandoned -> "Match abandoned without a ball being bowled"
        | A_MatchDrawn _ | AB_MatchDrawn _ | ABA_MatchDrawn _ | ABB_MatchDrawn _ | ABAB_MatchDrawn _ | ABBA_MatchDrawn _ -> "Match drawn"
        | A_Ongoing a1 -> sprintf "%s are %i for %i in their first innings" _match.TeamA a1.GetRuns a1.GetWickets
        | A_Completed a1 when a1.IsDeclared -> sprintf "%s scored %i for %i declared in their first innings" _match.TeamA a1.GetRuns a1.GetWickets
        | A_Completed a1 -> sprintf "%s scored %i all out in their first innings" _match.TeamA a1.GetRuns
        | _ -> failwith "not implemented"