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

    let teamName match' team =
        match team with
        | TeamA -> match'.TeamA.Name
        | TeamB -> match'.TeamB.Name

    let summaryStatus match' =
        MatchState.summaryStatus (teamName match' TeamA) (teamName match' TeamB) match'.State

    let inningsList match' =
        match'.State
        |> MatchState.inningsList
        |> List.map (fun (team, inningsNumber, innings) -> (teamName match' team, inningsNumber, innings))
