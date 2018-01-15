namespace Cricket.CricketEngine

open Formatting

type MatchState = 
    | NotStarted
    | Abandoned
    | A'Ongoing of Innings
    | A'Completed of Innings
    | A'MatchDrawn of Innings
    | AB'Ongoing of Innings * Innings
    | AB'CompletedNoFollowOn of Innings * Innings
    | AB'CompletedPossibleFollowOn of Innings * Innings
    | AB'MatchDrawn of Innings * Innings
    | ABA'Ongoing of Innings * Innings * Innings
    | ABA'VictoryB of Innings * Innings * Innings
    | ABA'Completed of Innings * Innings * Innings
    | ABA'MatchDrawn of Innings * Innings * Innings
    | ABB'Ongoing of Innings * Innings * Innings
    | ABB'VictoryA of Innings * Innings * Innings
    | ABB'Completed of Innings * Innings * Innings
    | ABB'MatchDrawn of Innings * Innings * Innings
    | ABAB'Ongoing of Innings * Innings * Innings * Innings
    | ABAB'VictoryA of Innings * Innings * Innings * Innings
    | ABAB'VictoryB of Innings * Innings * Innings * Innings
    | ABAB'MatchDrawn of Innings * Innings * Innings * Innings
    | ABAB'MatchTied of Innings * Innings * Innings * Innings
    | ABBA'Ongoing of Innings * Innings * Innings * Innings
    | ABBA'VictoryA of Innings * Innings * Innings * Innings
    | ABBA'VictoryB of Innings * Innings * Innings * Innings
    | ABBA'MatchDrawn of Innings * Innings * Innings * Innings
    | ABBA'MatchTied of Innings * Innings * Innings * Innings

type MatchResult = NoResult | WinTeamA | WinTeamB | Draw | Tie

type SummaryMatchState =
    | NotYetStarted
    | InningsInProgress of SummaryInningsState
    | BetweenInnings
    | AwaitingFollowOnDecision
    | MatchCompleted of MatchResult

type StateForPlayerRecords =
    | NoMatch
    | InProgress
    | Completed

type MatchUpdate =
    | StartMatch
    | AbandonMatch
    | DrawMatch
    | StartNextInnings
    | EnforceFollowOn
    | DeclineFollowOn
    | UpdateInnings of InningsUpdate

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module MatchState = 

    let private startMatch state = 
        match state with
        | NotStarted -> A'Ongoing(Innings.create)
        | _ -> failwith "Call to StartMatch in invalid state"
    
    let private abandonMatch state = 
        match state with
        | NotStarted -> Abandoned
        | _ -> failwith "Call to AbandonMatch in invalid state"
    
    let private drawMatch state = 
        match state with
        | A'Ongoing(a1) | A'Completed(a1) -> A'MatchDrawn(a1)
        | AB'Ongoing(a1, b1) | AB'CompletedNoFollowOn(a1, b1) | AB'CompletedPossibleFollowOn(a1, b1) -> 
            AB'MatchDrawn(a1, b1)
        | ABA'Ongoing(a1, b1, a2) | ABA'Completed(a1, b1, a2) -> ABA'MatchDrawn(a1, b1, a2)
        | ABB'Ongoing(a1, b1, b2) | ABB'Completed(a1, b1, b2) -> ABB'MatchDrawn(a1, b1, b2)
        | ABAB'Ongoing(a1, b1, a2, b2) -> ABAB'MatchDrawn(a1, b1, a2, b2)
        | ABBA'Ongoing(a1, b1, b2, a2) -> ABBA'MatchDrawn(a1, b1, b2, a2)
        | _ -> failwith "Call to DrawMatch in invalid state"
    
    let private startNextInnings state =
        match state with
        | A'Completed a1 -> AB'Ongoing(a1, Innings.create)
        | AB'CompletedNoFollowOn (a1, b1) -> ABA'Ongoing(a1, b1, Innings.create)
        | ABA'Completed (a1, b1, a2) -> ABAB'Ongoing(a1, b1, a2, Innings.create)
        | ABB'Completed (a1, b1, b2) -> ABBA'Ongoing(a1, b1, b2, Innings.create)
        | _ -> failwith "Call to StartNextInnings in invalid state"

    let private enforceFollowOn state = 
        match state with
        | AB'CompletedPossibleFollowOn(a1, b1) -> ABB'Ongoing(a1, b1, Innings.create)
        | _ -> failwith "Call to EnforceFollowOn in invalid state"
    
    let private declineFollowOn state = 
        match state with
        | AB'CompletedPossibleFollowOn(a1, b1) -> ABA'Ongoing(a1, b1, Innings.create)
        | _ -> failwith "Call to DeclineFollowOn in invalid state"
    
    let private updateInnings inningsUpdater rules state = 
        match state with
        | A'Ongoing(a1) -> 
            match Innings.update inningsUpdater None a1 with
            | Innings.InningsOngoing i -> A'Ongoing(i)
            | Innings.InningsCompleted i -> A'Completed(i)
        | AB'Ongoing(a1, b1) -> 
            match Innings.update inningsUpdater None b1 with
            | Innings.InningsOngoing i -> AB'Ongoing(a1, i)
            | Innings.InningsCompleted i when (a1.GetRuns >= i.GetRuns + rules.FollowOnMargin) -> AB'CompletedPossibleFollowOn(a1, i)
            | Innings.InningsCompleted i -> AB'CompletedNoFollowOn(a1, i)
        | ABA'Ongoing(a1, b1, a2) -> 
            match Innings.update inningsUpdater None a2 with
            | Innings.InningsOngoing i -> ABA'Ongoing(a1, b1, i)
            | Innings.InningsCompleted i when (b1.GetRuns > a1.GetRuns + i.GetRuns) -> ABA'VictoryB(a1, b1, i)
            | Innings.InningsCompleted i -> ABA'Completed(a1, b1, i)
        | ABB'Ongoing(a1, b1, b2) -> 
            match Innings.update inningsUpdater None b2 with
            | Innings.InningsOngoing i -> ABB'Ongoing(a1, b1, i)
            | Innings.InningsCompleted i when (a1.GetRuns > b1.GetRuns + i.GetRuns) -> ABB'VictoryA(a1, b1, i)
            | Innings.InningsCompleted i -> ABB'Completed(a1, b1, i)
        | ABAB'Ongoing(a1, b1, a2, b2) ->
            let toWin = Some (1 + a1.GetRuns + a2.GetRuns - b1.GetRuns - b2.GetRuns)
            match Innings.update inningsUpdater toWin b2 with
            | Innings.InningsOngoing i when (b1.GetRuns + i.GetRuns > a1.GetRuns + a2.GetRuns) -> ABAB'VictoryB(a1, b1, a2, i)
            | Innings.InningsOngoing i -> ABAB'Ongoing(a1, b1, a2, i)
            | Innings.InningsCompleted i when (a1.GetRuns + a2.GetRuns > b1.GetRuns + i.GetRuns) -> ABAB'VictoryA(a1, b1, a2, i)
            | Innings.InningsCompleted i when (a1.GetRuns + a2.GetRuns = b1.GetRuns + i.GetRuns) -> ABAB'MatchTied(a1, b1, a2, i)
            | Innings.InningsCompleted _ -> failwith "Call to UpdateInnings in inconsistent state"
        | ABBA'Ongoing(a1, b1, b2, a2) -> 
            let toWin = Some (1 + b1.GetRuns + b2.GetRuns - a1.GetRuns - a2.GetRuns)
            match Innings.update inningsUpdater toWin a2 with
            | Innings.InningsOngoing i when (a1.GetRuns + i.GetRuns > b1.GetRuns + b2.GetRuns) -> ABBA'VictoryA(a1, b1, b2, i)
            | Innings.InningsOngoing i -> ABBA'Ongoing(a1, b1, b2, i)
            | Innings.InningsCompleted i when (b1.GetRuns + b2.GetRuns > a1.GetRuns + i.GetRuns) -> ABBA'VictoryB(a1, b1, b2, i)
            | Innings.InningsCompleted i when (b1.GetRuns + b2.GetRuns = a1.GetRuns + i.GetRuns) -> ABBA'MatchTied(a1, b1, b2, i)
            | Innings.InningsCompleted _ -> failwith "Call to UpdateInnings in inconsistent state"
        | _ -> failwith "Call to UpdateInnings in invalid state"

    let update rules transition state =
        match transition with
        | StartMatch -> startMatch state
        | AbandonMatch -> abandonMatch state
        | DrawMatch -> drawMatch state
        | StartNextInnings -> startNextInnings state
        | EnforceFollowOn -> enforceFollowOn state
        | DeclineFollowOn -> declineFollowOn state
        | UpdateInnings inningsUpdate -> updateInnings inningsUpdate rules state

    let summaryState state =
        match state with
        | NotStarted -> NotYetStarted
        | Abandoned -> MatchCompleted NoResult
        | A'MatchDrawn _
            | AB'MatchDrawn _
            | ABBA'MatchDrawn _
            | ABA'MatchDrawn _
            | ABB'MatchDrawn _
            | ABAB'MatchDrawn _ -> MatchCompleted Draw
        | ABBA'MatchTied _
            | ABAB'MatchTied _ -> MatchCompleted Tie
        | ABBA'VictoryA _
            | ABB'VictoryA _
            | ABAB'VictoryA _ -> MatchCompleted WinTeamA
        | ABA'VictoryB _ 
            | ABBA'VictoryB _
            | ABAB'VictoryB _ -> MatchCompleted WinTeamB
        | AB'CompletedPossibleFollowOn _ -> AwaitingFollowOnDecision
        | A'Ongoing a1 -> Innings.summaryState a1 |> InningsInProgress
        | AB'Ongoing (_, b1) -> Innings.summaryState b1 |> InningsInProgress
        | ABA'Ongoing (_, _, a2) -> Innings.summaryState a2 |> InningsInProgress
        | ABB'Ongoing (_, _, b2) -> Innings.summaryState b2 |> InningsInProgress
        | ABAB'Ongoing (_, _, _, b2) -> Innings.summaryState b2 |> InningsInProgress
        | ABBA'Ongoing (_, _, _, a2) -> Innings.summaryState a2 |> InningsInProgress
        | A'Completed _
            | AB'CompletedNoFollowOn _
            | ABA'Completed _
            | ABB'Completed _ -> BetweenInnings

    let summaryStateForPlayerRecords state =
        match state with
        | NotStarted
            | Abandoned -> NoMatch
        | A'MatchDrawn _
            | AB'MatchDrawn _
            | ABBA'VictoryA _
            | ABB'VictoryA _
            | ABA'MatchDrawn _
            | ABA'VictoryB _ 
            | ABBA'VictoryB _
            | ABBA'MatchDrawn _
            | ABBA'MatchTied _
            | ABAB'MatchDrawn _
            | ABAB'MatchTied _
            | ABAB'VictoryB _
            | ABAB'VictoryA _
            | ABB'MatchDrawn _ -> Completed
        | AB'CompletedPossibleFollowOn _
            | A'Ongoing _
            | AB'Ongoing _
            | ABA'Ongoing _
            | ABB'Ongoing _
            | ABAB'Ongoing _
            | ABBA'Ongoing _
            | A'Completed _
            | AB'CompletedNoFollowOn _
            | ABA'Completed _
            | ABB'Completed _ -> InProgress

    let currentInnings state =
        match state with
        | NotStarted | Abandoned | A'Completed _ | A'MatchDrawn _ | AB'CompletedNoFollowOn _ -> failwith "no current innings"
        | AB'CompletedPossibleFollowOn _ | AB'MatchDrawn _ | ABA'VictoryB _ -> failwith "no current innings"
        | ABA'Completed _ | ABA'MatchDrawn _ | ABB'VictoryA _ -> failwith "no current innings"
        | ABB'Completed _ | ABB'MatchDrawn _ | ABAB'VictoryA _ -> failwith "no current innings"
        | ABAB'VictoryB _ | ABAB'MatchDrawn _ | ABAB'MatchTied _ -> failwith "no current innings"
        | ABBA'VictoryA _ | ABBA'VictoryB _ | ABBA'MatchDrawn _ -> failwith "no current innings"
        | ABBA'MatchTied _ -> failwith "no current innings"
        | A'Ongoing a1 -> a1
        | AB'Ongoing (_, b1) -> b1
        | ABA'Ongoing (_, _, a2) | ABBA'Ongoing (_, _, _, a2) -> a2
        | ABB'Ongoing (_, _, b2) | ABAB'Ongoing(_, _, _, b2) -> b2

    let inningsList state =
        match state with
        | NotStarted -> []
        | Abandoned -> []
        | A'Ongoing a1
            | A'Completed a1
            | A'MatchDrawn a1 -> [ (TeamA, FirstInnings, a1) ]
        | AB'Ongoing (a1, b1)
            | AB'CompletedNoFollowOn (a1, b1)
            | AB'CompletedPossibleFollowOn (a1, b1)
            | AB'MatchDrawn (a1, b1) -> [ (TeamA, FirstInnings, a1); (TeamB, FirstInnings, b1) ]
        | ABA'Ongoing (a1, b1, a2)
            | ABA'VictoryB (a1, b1, a2)
            | ABA'Completed (a1, b1, a2)
            | ABA'MatchDrawn (a1, b1, a2) -> [ (TeamA, FirstInnings, a1); (TeamB, FirstInnings, b1); (TeamA, SecondInnings, a2) ]
        | ABB'Ongoing (a1, b1, b2)
            | ABB'VictoryA (a1, b1, b2)
            | ABB'Completed (a1, b1, b2)
            | ABB'MatchDrawn (a1, b1, b2) -> [ (TeamA, FirstInnings, a1); (TeamB, FirstInnings, b1); (TeamB, SecondInnings, b2) ]
        | ABAB'Ongoing (a1, b1, a2, b2)
            | ABAB'VictoryA (a1, b1, a2, b2)
            | ABAB'VictoryB (a1, b1, a2, b2)
            | ABAB'MatchDrawn (a1, b1, a2, b2)
            | ABAB'MatchTied (a1, b1, a2, b2) -> [ (TeamA, FirstInnings, a1); (TeamB, FirstInnings, b1); (TeamA, SecondInnings, a2); (TeamB, SecondInnings, b2) ]
        | ABBA'Ongoing (a1, b1, b2, a2)
            | ABBA'VictoryA (a1, b1, b2, a2)
            | ABBA'VictoryB (a1, b1, b2, a2)
            | ABBA'MatchDrawn (a1, b1, b2, a2)
            | ABBA'MatchTied (a1, b1, b2, a2) -> [ (TeamA, FirstInnings, a1); (TeamB, FirstInnings, b1); (TeamB, SecondInnings, b2); (TeamA, SecondInnings, a2) ]

    let currentBattingTeam state =
        match state with
        | NotStarted | Abandoned | A'Completed _ | A'MatchDrawn _ | AB'CompletedNoFollowOn _ -> failwith "no current innings"
        | AB'CompletedPossibleFollowOn _ | AB'MatchDrawn _ | ABA'VictoryB _ -> failwith "no current innings"
        | ABA'Completed _ | ABA'MatchDrawn _ | ABB'VictoryA _ -> failwith "no current innings"
        | ABB'Completed _ | ABB'MatchDrawn _ | ABAB'VictoryA _ -> failwith "no current innings"
        | ABAB'VictoryB _ | ABAB'MatchDrawn _ | ABAB'MatchTied _ -> failwith "no current innings"
        | ABBA'VictoryA _ | ABBA'VictoryB _ | ABBA'MatchDrawn _ -> failwith "no current innings"
        | ABBA'MatchTied _ -> failwith "no current innings"
        | A'Ongoing _ | ABA'Ongoing _ | ABBA'Ongoing _ -> TeamA
        | AB'Ongoing _ | ABB'Ongoing _ | ABAB'Ongoing _ -> TeamB

    let currentBowlingTeam state =
        match state with
        | NotStarted | Abandoned | A'Completed _ | A'MatchDrawn _ | AB'CompletedNoFollowOn _ -> failwith "no current innings"
        | AB'CompletedPossibleFollowOn _ | AB'MatchDrawn _ | ABA'VictoryB _ -> failwith "no current innings"
        | ABA'Completed _ | ABA'MatchDrawn _ | ABB'VictoryA _ -> failwith "no current innings"
        | ABB'Completed _ | ABB'MatchDrawn _ | ABAB'VictoryA _ -> failwith "no current innings"
        | ABAB'VictoryB _ | ABAB'MatchDrawn _ | ABAB'MatchTied _ -> failwith "no current innings"
        | ABBA'VictoryA _ | ABBA'VictoryB _ | ABBA'MatchDrawn _ -> failwith "no current innings"
        | ABBA'MatchTied _ -> failwith "no current innings"
        | A'Ongoing _ | ABA'Ongoing _ | ABBA'Ongoing _ -> TeamB
        | AB'Ongoing _ | ABB'Ongoing _ | ABAB'Ongoing _ -> TeamA

    let totalRuns state = 
        match state with
        | NotStarted | Abandoned -> 0, 0
        | A'Ongoing a1
            | A'Completed a1
            | A'MatchDrawn a1 -> a1.GetRuns, 0
        | AB'Ongoing(a1, b1)
            | AB'CompletedNoFollowOn(a1, b1)
            | AB'CompletedPossibleFollowOn(a1, b1)
            | AB'MatchDrawn(a1, b1) -> a1.GetRuns, b1.GetRuns
        | ABA'Ongoing(a1, b1, a2)
            | ABA'VictoryB(a1, b1, a2)
            | ABA'Completed(a1, b1, a2)
            | ABA'MatchDrawn(a1, b1, a2) -> a1.GetRuns + a2.GetRuns, b1.GetRuns
        | ABB'Ongoing(a1, b1, b2)
            | ABB'VictoryA(a1, b1, b2)
            | ABB'Completed(a1, b1, b2)
            | ABB'MatchDrawn(a1, b1, b2) -> a1.GetRuns, b1.GetRuns + b2.GetRuns
        | ABAB'Ongoing(a1, b1, a2, b2)
            | ABAB'VictoryA(a1, b1, a2, b2)
            | ABAB'VictoryB(a1, b1, a2, b2)
            | ABAB'MatchDrawn(a1, b1, a2, b2)
            | ABAB'MatchTied(a1, b1, a2, b2)
            | ABBA'Ongoing(a1, b1, b2, a2)
            | ABBA'VictoryA(a1, b1, b2, a2)
            | ABBA'VictoryB(a1, b1, b2, a2)
            | ABBA'MatchDrawn(a1, b1, b2, a2)
            | ABBA'MatchTied(a1, b1, b2, a2) -> a1.GetRuns + a2.GetRuns, b1.GetRuns + b2.GetRuns

    let leadA state =
        let runsA, runsB = totalRuns state
        runsA - runsB

    let private (|ALeads|ScoresLevel|BLeads|) state =
        match (leadA state) with
        | x when x > 0 -> ALeads
        | x when x < 0 -> BLeads
        | _ -> ScoresLevel

    let summaryStatus teamA teamB state =
        let leadA = leadA state
        let leadB = -leadA

        match state with
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
        | ABA'VictoryB _ -> sprintf "%s won by an innings and %s" teamB (formatRuns leadB)
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


//    for convenience when writing new functions...
//        | NotStarted
//        | Abandoned
//        | A'Ongoing a1
//        | A'Completed a1
//        | A'MatchDrawn a1
//        | AB'Ongoing (a1, b1)
//        | AB'CompletedNoFollowOn (a1, b1)
//        | AB'CompletedPossibleFollowOn (a1, b1)
//        | AB'MatchDrawn (a1, b1)
//        | ABA'Ongoing (a1, b1, a2)
//        | ABA'VictoryB (a1, b1, a2)
//        | ABA'Completed (a1, b1, a2)
//        | ABA'MatchDrawn (a1, b1, a2)
//        | ABB'Ongoing (a1, b1, b2)
//        | ABB'VictoryA (a1, b1, b2)
//        | ABB'Completed (a1, b1, b2)
//        | ABB'MatchDrawn (a1, b1, b2)
//        | ABAB'Ongoing (a1, b1, a2, b2)
//        | ABAB'VictoryA (a1, b1, a2, b2)
//        | ABAB'VictoryB (a1, b1, a2, b2)
//        | ABAB'MatchDrawn (a1, b1, a2, b2)
//        | ABAB'MatchTied (a1, b1, a2, b2)
//        | ABBA'Ongoing (a1, b1, b2, a2)
//        | ABBA'VictoryA (a1, b1, b2, a2)
//        | ABBA'VictoryB (a1, b1, b2, a2)
//        | ABBA'MatchDrawn (a1, b1, b2, a2)
//        | ABBA'MatchTied (a1, b1, b2, a2)

