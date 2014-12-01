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
    
    let formatRuns runs =
        match runs with
        | 1 -> "1 run"
        | n when n > 1 -> sprintf "%i runs" n
        | _ -> failwith "Invalid number of runs"

    let formatWickets wickets =
        match wickets with
        | 1 -> "1 wicket"
        | n when n > 1 -> sprintf "%i wickets" n
        | _ -> failwith "Invalid number of wickets"

    let formatWicketsLeft wickets =
        formatWickets (10 - wickets)

    let SummaryStatus rules _match =
        match _match.State with
        | NotStarted -> "Match not started"
        | Abandoned -> "Match abandoned without a ball being bowled"
        | A_MatchDrawn _ | AB_MatchDrawn _ | ABA_MatchDrawn _ | ABB_MatchDrawn _ | ABAB_MatchDrawn _ | ABBA_MatchDrawn _ -> "Match drawn"
        | A_Ongoing a1 -> sprintf "%s are %i for %i in their first innings" _match.TeamA a1.GetRuns a1.GetWickets
        | A_Completed a1 when a1.IsDeclared -> sprintf "%s scored %i for %i declared in their first innings" _match.TeamA a1.GetRuns a1.GetWickets
        | A_Completed a1 -> sprintf "%s scored %i all out in their first innings" _match.TeamA a1.GetRuns
        | AB_Ongoing (a1, b1) when b1.GetRuns < a1.GetRuns -> sprintf "%s trail by %s with %s remaining in their first innings" _match.TeamB (formatRuns (a1.GetRuns - b1.GetRuns)) (formatWicketsLeft b1.GetWickets)
        | AB_Ongoing (a1, b1) when b1.GetRuns = a1.GetRuns -> sprintf "%s are level with %s remaining in their first innings" _match.TeamB (formatWicketsLeft b1.GetWickets)
        | AB_Ongoing (a1, b1) when b1.GetRuns > a1.GetRuns -> sprintf "%s lead by %s with %s remaining in their first innings" _match.TeamB (formatRuns (b1.GetRuns - a1.GetRuns)) (formatWicketsLeft b1.GetWickets)
        | AB_CompletedNoFollowOn (a1, b1) | AB_CompletedPossibleFollowOn (a1, b1) when b1.GetRuns < a1.GetRuns -> sprintf "%s lead by %s after the first innings" _match.TeamA (formatRuns (a1.GetRuns - b1.GetRuns))
        | AB_CompletedNoFollowOn (a1, b1) | AB_CompletedPossibleFollowOn (a1, b1) when b1.GetRuns = a1.GetRuns -> sprintf "%s are level after the first innings" _match.TeamB
        | AB_CompletedNoFollowOn (a1, b1) | AB_CompletedPossibleFollowOn (a1, b1) when b1.GetRuns > a1.GetRuns -> sprintf "%s lead by %s after the first innings" _match.TeamB (formatRuns (b1.GetRuns - a1.GetRuns))
        | ABA_Ongoing (a1, b1, a2) when a1.GetRuns + a2.GetRuns < b1.GetRuns -> sprintf "%s trail by %s with %s remaining in their second innings" _match.TeamA (formatRuns (b1.GetRuns - a1.GetRuns - a2.GetRuns)) (formatWicketsLeft a2.GetWickets)
        | ABA_Ongoing (a1, b1, a2) when a1.GetRuns + a2.GetRuns = b1.GetRuns -> sprintf "%s are level with %s remaining in their second innings" _match.TeamA (formatWicketsLeft a2.GetWickets)
        | ABA_Ongoing (a1, b1, a2) when a1.GetRuns + a2.GetRuns > b1.GetRuns -> sprintf "%s lead by %s with %s remaining in their second innings" _match.TeamA (formatRuns (a1.GetRuns + a2.GetRuns - b1.GetRuns)) (formatWicketsLeft a2.GetWickets)
        | ABA_VictoryB (a1, b1, a2) -> sprintf "%s won by %s" _match.TeamB (formatRuns (b1.GetRuns - a1.GetRuns - a2.GetRuns))
        | ABA_Completed (a1, b1, a2) -> sprintf "%s need %s to win in their second innings" _match.TeamB (formatRuns (a1.GetRuns + a2.GetRuns - b1.GetRuns))
        | _ -> failwith "not implemented"