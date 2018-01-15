namespace Cricket.CricketEngine

open MatchState

type Match =
    {
        TeamA: Team;
        TeamB: Team;
        State: MatchState;
        Rules: MatchRules;
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Match =
    
    let updateMatchState update match' = { match' with State = MatchState.update match'.Rules update match'.State }
    
    let updateCurrentInnings update = updateMatchState (UpdateInnings update)

    let newMatch rules teamA teamB =
        { TeamA = teamA; TeamB = teamB; State = NotStarted; Rules = rules }

    let isCompleted match' =
        summaryStateForPlayerRecords match'.State = Completed

    let private team match' t =
        match t with
        | TeamA -> match'.TeamA
        | TeamB -> match'.TeamB

    let summaryStatus match' =
        MatchState.summaryStatus (team match' TeamA) (team match' TeamB) match'.State

    let inningsList match' =
        match'.State
        |> MatchState.inningsList
        |> List.map (fun (t, inningsNumber, innings) -> (team match' t, inningsNumber, innings))
