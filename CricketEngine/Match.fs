namespace Cricket.CricketEngine

type Match =
    {
        TeamA: string;
        TeamB: string;
        State: MatchState;
        Rules: MatchRules;
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix )>]
module Match =
    
    let updateMatchState update match' = { match' with State = MatchState.update match'.Rules update match'.State }
    let updateCurrentInnings update = updateMatchState (UpdateInnings update)

    let newMatch rules teamA teamB =
        { TeamA = teamA; TeamB = teamB; State = NotStarted; Rules = rules }

    let private formatRuns runs =
        match runs with
        | 1 -> "1 run"
        | n when n > 1 -> sprintf "%i runs" n
        | _ -> failwith "Invalid number of runs"

    let private formatWickets wickets =
        match wickets with
        | 1 -> "1 wicket"
        | n when n > 1 -> sprintf "%i wickets" n
        | _ -> failwith "Invalid number of wickets"

    let private formatWicketsLeft wickets =
        formatWickets (10 - wickets)

    let private (|ALeads|ScoresLevel|BLeads|) state =
        match (MatchState.leadA state) with
        | x when x > 0 -> ALeads
        | x when x < 0 -> BLeads
        | _ -> ScoresLevel

    let summaryStatus match' =
        let teamA = match'.TeamA
        let teamB = match'.TeamB
        let leadA = MatchState.leadA match'.State
        let leadB = MatchState.leadB match'.State
        match match'.State with
        | NotStarted -> "Match not started"
        | Abandoned -> "Match abandoned without a ball being bowled"
        | A'MatchDrawn _ | AB'MatchDrawn _ | ABA'MatchDrawn _ | ABB'MatchDrawn _ | ABAB'MatchDrawn _ | ABBA'MatchDrawn _ -> "Match drawn"
        | A'Ongoing a1 -> sprintf "%s are %i for %i in their first innings" teamA a1.GetRuns a1.GetWickets
        | A'Completed a1 when a1.IsDeclared -> sprintf "%s scored %i for %i declared in their first innings" teamA a1.GetRuns a1.GetWickets
        | A'Completed a1 -> sprintf "%s scored %i all out in their first innings" teamA a1.GetRuns
        | AB'Ongoing (_, b1) & ALeads -> sprintf "%s trail by %s with %s remaining in their first innings" teamB (formatRuns leadA) (formatWicketsLeft b1.GetWickets)
        | AB'Ongoing (_, b1) & ScoresLevel -> sprintf "%s are level with %s remaining in their first innings" teamB (formatWicketsLeft b1.GetWickets)
        | AB'Ongoing (_, b1) & BLeads -> sprintf "%s lead by %s with %s remaining in their first innings" teamB (formatRuns leadB) (formatWicketsLeft b1.GetWickets)
        | (AB'CompletedNoFollowOn (a1, b1) | AB'CompletedPossibleFollowOn (a1, b1)) & ALeads -> sprintf "%s lead by %s after the first innings" teamA (formatRuns leadA)
        | (AB'CompletedNoFollowOn (a1, b1) | AB'CompletedPossibleFollowOn (a1, b1)) & ScoresLevel -> sprintf "%s are level after the first innings" teamB
        | (AB'CompletedNoFollowOn (a1, b1) | AB'CompletedPossibleFollowOn (a1, b1)) & BLeads -> sprintf "%s lead by %s after the first innings" teamB (formatRuns leadB)
        | ABA'Ongoing (_, _, a2) & BLeads -> sprintf "%s trail by %s with %s remaining in their second innings" teamA (formatRuns leadB) (formatWicketsLeft a2.GetWickets)
        | ABA'Ongoing (_, _, a2) & ScoresLevel -> sprintf "%s are level with %s remaining in their second innings" teamA (formatWicketsLeft a2.GetWickets)
        | ABA'Ongoing (_, _, a2) & ALeads -> sprintf "%s lead by %s with %s remaining in their second innings" teamA (formatRuns leadA) (formatWicketsLeft a2.GetWickets)
        | ABA'VictoryB _ -> sprintf "%s won by %s" teamB (formatRuns leadB)
        | ABA'Completed _ -> sprintf "%s need %s to win in their second innings" teamB (formatRuns leadA)
        | _ -> failwith "not implemented"