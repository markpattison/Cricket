namespace Cricket.CricketEngine

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

type SummaryMatchState =
    | NotYetStarted
    | InningsInProgress of SummaryInningsState
    | BetweenInnings
    | AwaitingFollowOnDecision
    | MatchCompleted

type MatchUpdate =
    | StartMatch
    | AbandonMatch
    | DrawMatch
    | StartNextInnings
    | EnforceFollowOn
    | DeclineFollowOn
    | UpdateInnings of InningsUpdate

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix )>]
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
        | Abandoned
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
            | ABB'MatchDrawn _ -> MatchCompleted
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
        | ABB'Ongoing (_, _, b2) | ABAB'Ongoing (_, _, _, b2) -> b2

    let totalRunsA state = 
        match state with
        | NotStarted | Abandoned -> 0
        | A'Ongoing a1
            | A'Completed a1
            | A'MatchDrawn a1 -> a1.GetRuns
        | AB'Ongoing(a1, b1)
            | AB'CompletedNoFollowOn(a1, b1)
            | AB'CompletedPossibleFollowOn(a1, b1)
            | AB'MatchDrawn(a1, b1) -> a1.GetRuns
        | ABA'Ongoing(a1, b1, a2)
            | ABA'VictoryB(a1, b1, a2)
            | ABA'Completed(a1, b1, a2)
            | ABA'MatchDrawn(a1, b1, a2) -> a1.GetRuns + a2.GetRuns
        | ABB'Ongoing(a1, b1, b2)
            | ABB'VictoryA(a1, b1, b2)
            | ABB'Completed(a1, b1, b2)
            | ABB'MatchDrawn(a1, b1, b2) -> a1.GetRuns
        | ABAB'Ongoing(a1, b1, a2, b2)
            | ABAB'VictoryA(a1, b1, a2, b2)
            | ABAB'VictoryB(a1, b1, a2, b2)
            | ABAB'MatchDrawn(a1, b1, a2, b2)
            | ABAB'MatchTied(a1, b1, a2, b2)
            | ABBA'Ongoing(a1, b1, b2, a2)
            | ABBA'VictoryA(a1, b1, b2, a2)
            | ABBA'VictoryB(a1, b1, b2, a2)
            | ABBA'MatchDrawn(a1, b1, b2, a2)
            | ABBA'MatchTied(a1, b1, b2, a2) -> a1.GetRuns + a2.GetRuns

    let totalRunsB state = 
        match state with
        | NotStarted | Abandoned -> 0
        | A'Ongoing a1
            | A'Completed a1
            | A'MatchDrawn a1 -> 0
        | AB'Ongoing(a1, b1)
            | AB'CompletedNoFollowOn(a1, b1)
            | AB'CompletedPossibleFollowOn(a1, b1)
            | AB'MatchDrawn(a1, b1) -> b1.GetRuns
        | ABA'Ongoing(a1, b1, a2)
            | ABA'VictoryB(a1, b1, a2)
            | ABA'Completed(a1, b1, a2)
            | ABA'MatchDrawn(a1, b1, a2) -> b1.GetRuns
        | ABB'Ongoing(a1, b1, b2)
            | ABB'VictoryA(a1, b1, b2)
            | ABB'Completed(a1, b1, b2)
            | ABB'MatchDrawn(a1, b1, b2) -> b1.GetRuns + b2.GetRuns
        | ABAB'Ongoing(a1, b1, a2, b2)
            | ABAB'VictoryA(a1, b1, a2, b2)
            | ABAB'VictoryB(a1, b1, a2, b2)
            | ABAB'MatchDrawn(a1, b1, a2, b2)
            | ABAB'MatchTied(a1, b1, a2, b2)
            | ABBA'Ongoing(a1, b1, b2, a2)
            | ABBA'VictoryA(a1, b1, b2, a2)
            | ABBA'VictoryB(a1, b1, b2, a2)
            | ABBA'MatchDrawn(a1, b1, b2, a2)
            | ABBA'MatchTied(a1, b1, b2, a2) -> b1.GetRuns + b2.GetRuns

    let leadA state =
        (totalRunsA state) - (totalRunsB state)

    let leadB state =
        (totalRunsB state) - (totalRunsA state)




//    for convenience when writing new functions...
//        | NotStarted
//        | Abandoned
//        | A_Ongoing a1
//        | A_Completed a1
//        | A_MatchDrawn a1
//        | AB_Ongoing (a1, b1)
//        | AB_CompletedNoFollowOn (a1, b1)
//        | AB_CompletedPossibleFollowOn (a1, b1)
//        | AB_MatchDrawn (a1, b1)
//        | ABA_Ongoing (a1, b1, a2)
//        | ABA_VictoryB (a1, b1, a2)
//        | ABA_Completed (a1, b1, a2)
//        | ABA_MatchDrawn (a1, b1, a2)
//        | ABB_Ongoing (a1, b1, b2)
//        | ABB_VictoryA (a1, b1, b2)
//        | ABB_Completed (a1, b1, b2)
//        | ABB_MatchDrawn (a1, b1, b2)
//        | ABAB_Ongoing (a1, b1, a2, b2)
//        | ABAB_VictoryA (a1, b1, a2, b2)
//        | ABAB_VictoryB (a1, b1, a2, b2)
//        | ABAB_MatchDrawn (a1, b1, a2, b2)
//        | ABAB_MatchTied (a1, b1, a2, b2)
//        | ABBA_Ongoing (a1, b1, b2, a2)
//        | ABBA_VictoryA (a1, b1, b2, a2)
//        | ABBA_VictoryB (a1, b1, b2, a2)
//        | ABBA_MatchDrawn (a1, b1, b2, a2)
//        | ABBA_MatchTied (a1, b1, b2, a2)

