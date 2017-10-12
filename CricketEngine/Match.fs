namespace Cricket.CricketEngine

open Formatting

type Match =
    {
        TeamA: Team;
        TeamB: Team;
        State: MatchState;
        Rules: MatchRules;
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix )>]
module Match =
    
    let updateMatchState update match' = { match' with State = MatchState.update match'.Rules update match'.State }
    let updateCurrentInnings update = updateMatchState (UpdateInnings update)

    let newMatch rules teamA teamB =
        { TeamA = teamA; TeamB = teamB; State = NotStarted; Rules = rules }

    let private (|ALeads|ScoresLevel|BLeads|) state =
        match (MatchState.leadA state) with
        | x when x > 0 -> ALeads
        | x when x < 0 -> BLeads
        | _ -> ScoresLevel

    let summaryStatus match' =
        let teamA = match'.TeamA.Name
        let teamB = match'.TeamB.Name
        let leadA = MatchState.leadA match'.State
        let leadB = MatchState.leadB match'.State
        match match'.State with
        | NotStarted -> "Match not started"
        | Abandoned -> "Match abandoned without a ball being bowled"
        | A'MatchDrawn _ | AB'MatchDrawn _ | ABA'MatchDrawn _ | ABB'MatchDrawn _ | ABAB'MatchDrawn _ | ABBA'MatchDrawn _ -> "Match drawn"
        | ABBA'MatchTied _ | ABAB'MatchTied _ -> "Match tied"
        | A'Ongoing a1 -> sprintf "%s are %i for %i in their first innings" teamA a1.GetRuns a1.GetWickets
        | A'Completed a1 when a1.IsDeclared -> sprintf "%s scored %i for %i declared in their first innings" teamA a1.GetRuns a1.GetWickets
        | A'Completed a1 -> sprintf "%s scored %i all out in their first innings" teamA a1.GetRuns
        | AB'Ongoing (_, b1) & ALeads -> sprintf "%s trail by %s with %s remaining in their first innings" teamB (formatRuns leadA) (formatWicketsLeft b1.GetWickets)
        | AB'Ongoing (_, b1) & ScoresLevel -> sprintf "%s are level with %s remaining in their first innings" teamB (formatWicketsLeft b1.GetWickets)
        | AB'Ongoing (_, b1) & BLeads -> sprintf "%s lead by %s with %s remaining in their first innings" teamB (formatRuns leadB) (formatWicketsLeft b1.GetWickets)
        | AB'CompletedNoFollowOn _ & ALeads -> sprintf "%s lead by %s after the first innings" teamA (formatRuns leadA)
        | AB'CompletedNoFollowOn _ & ScoresLevel -> sprintf "%s are level after the first innings" teamB
        | AB'CompletedNoFollowOn _ & BLeads -> sprintf "%s lead by %s after the first innings" teamB (formatRuns leadB)
        | AB'CompletedPossibleFollowOn _ & ALeads -> sprintf "%s lead by %s after the first innings" teamA (formatRuns leadA)
        | AB'CompletedPossibleFollowOn _ & ScoresLevel -> sprintf "%s are level after the first innings" teamB
        | AB'CompletedPossibleFollowOn _ & BLeads -> sprintf "%s lead by %s after the first innings" teamB (formatRuns leadB)
        | ABA'Ongoing (_, _, a2) & BLeads -> sprintf "%s trail by %s with %s remaining in their second innings" teamA (formatRuns leadB) (formatWicketsLeft a2.GetWickets)
        | ABA'Ongoing (_, _, a2) & ScoresLevel -> sprintf "%s are level with %s remaining in their second innings" teamA (formatWicketsLeft a2.GetWickets)
        | ABA'Ongoing (_, _, a2) & ALeads -> sprintf "%s lead by %s with %s remaining in their second innings" teamA (formatRuns leadA) (formatWicketsLeft a2.GetWickets)
        | ABA'VictoryB _ -> sprintf "%s won by %s" teamB (formatRuns leadB)
        | ABA'Completed _ -> sprintf "%s need %s to win in their second innings" teamB (leadA + 1 |> formatRuns)
        | ABB'Ongoing (_, _, b2) & ALeads -> sprintf "%s trail by %s with %s remaining in their second innings" teamB (formatRuns leadA) (formatWicketsLeft b2.GetWickets)
        | ABB'Ongoing (_, _, b2) & ScoresLevel -> sprintf "%s are level with %s remaining in their second innings" teamB (formatWicketsLeft b2.GetWickets)
        | ABB'Ongoing (_, _, b2) & BLeads -> sprintf "%s lead by %s with %s remaining in their second innings" teamB (formatRuns leadB) (formatWicketsLeft b2.GetWickets)
        | ABB'VictoryA _ -> sprintf "%s won by an innings and %s" teamA (formatRuns leadA)
        | ABB'Completed _ -> sprintf "%s need %s to win in their second innings" teamA (leadB + 1 |> formatRuns)
        | ABAB'Ongoing (_, _, _, b2) -> sprintf "%s need %s to win with %s remaining" teamB (leadA + 1 |> formatRuns) (formatWicketsLeft b2.GetWickets)
        | ABAB'VictoryA _ -> sprintf "%s won by %s" teamA (leadA |> formatRuns)
        | ABAB'VictoryB (_, _, _, b2) -> sprintf "%s won by %s" teamB (formatWicketsLeft b2.GetWickets)
        | ABBA'Ongoing (_, _, _, a2) -> sprintf "%s need %s to win with %s remaining" teamA (leadB + 1 |> formatRuns) (formatWicketsLeft a2.GetWickets)
        | ABBA'VictoryB _ -> sprintf "%s won by %s" teamB (leadB |> formatRuns)
        | ABBA'VictoryA (_, _, _, a2) -> sprintf "%s won by %s" teamA (formatWicketsLeft a2.GetWickets)
        | _ -> failwith "Unexpected match state"

    let inningsList match' =
        let teamA = match'.TeamA.Name
        let teamB = match'.TeamB.Name
        match match'.State with
        | NotStarted -> []
        | Abandoned -> []
        | A'Ongoing a1
            | A'Completed a1
            | A'MatchDrawn a1 -> [ (teamA, a1) ]
        | AB'Ongoing (a1, b1)
            | AB'CompletedNoFollowOn (a1, b1)
            | AB'CompletedPossibleFollowOn (a1, b1)
            | AB'MatchDrawn (a1, b1) -> [ (teamA, a1); (teamB, b1) ]
        | ABA'Ongoing (a1, b1, a2)
            | ABA'VictoryB (a1, b1, a2)
            | ABA'Completed (a1, b1, a2)
            | ABA'MatchDrawn (a1, b1, a2) -> [ (teamA, a1); (teamB, b1); (teamA, a2) ]
        | ABB'Ongoing (a1, b1, b2)
            | ABB'VictoryA (a1, b1, b2)
            | ABB'Completed (a1, b1, b2)
            | ABB'MatchDrawn (a1, b1, b2) -> [ (teamA, a1); (teamB, b1); (teamB, b2) ]
        | ABAB'Ongoing (a1, b1, a2, b2)
            | ABAB'VictoryA (a1, b1, a2, b2)
            | ABAB'VictoryB (a1, b1, a2, b2)
            | ABAB'MatchDrawn (a1, b1, a2, b2)
            | ABAB'MatchTied (a1, b1, a2, b2) -> [ (teamA, a1); (teamB, b1); (teamA, a2); (teamB, b2) ]
        | ABBA'Ongoing (a1, b1, b2, a2)
            | ABBA'VictoryA (a1, b1, b2, a2)
            | ABBA'VictoryB (a1, b1, b2, a2)
            | ABBA'MatchDrawn (a1, b1, b2, a2)
            | ABBA'MatchTied (a1, b1, b2, a2) -> [ (teamA, a1); (teamB, b1); (teamB, b2); (teamA, a2) ]

