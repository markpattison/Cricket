namespace Cricket.CricketEngine

type MatchState = 
    | NotStarted
    | Abandoned
    | A_Ongoing of Innings
    | A_Completed of Innings
    | A_MatchDrawn of Innings
    | AB_Ongoing of Innings * Innings
    | AB_CompletedNoFollowOn of Innings * Innings
    | AB_CompletedPossibleFollowOn of Innings * Innings
    | AB_MatchDrawn of Innings * Innings
    | ABA_Ongoing of Innings * Innings * Innings
    | ABA_VictoryB of Innings * Innings * Innings
    | ABA_Completed of Innings * Innings * Innings
    | ABA_MatchDrawn of Innings * Innings * Innings
    | ABB_Ongoing of Innings * Innings * Innings
    | ABB_VictoryA of Innings * Innings * Innings
    | ABB_Completed of Innings * Innings * Innings
    | ABB_MatchDrawn of Innings * Innings * Innings
    | ABAB_Ongoing of Innings * Innings * Innings * Innings
    | ABAB_VictoryA of Innings * Innings * Innings * Innings
    | ABAB_VictoryB of Innings * Innings * Innings * Innings
    | ABAB_MatchDrawn of Innings * Innings * Innings * Innings
    | ABAB_MatchTied of Innings * Innings * Innings * Innings
    | ABBA_Ongoing of Innings * Innings * Innings * Innings
    | ABBA_VictoryA of Innings * Innings * Innings * Innings
    | ABBA_VictoryB of Innings * Innings * Innings * Innings
    | ABBA_MatchDrawn of Innings * Innings * Innings * Innings
    | ABBA_MatchTied of Innings * Innings * Innings * Innings

type SummaryState =
    | NotYetStarted
    | InningsInProgress of Innings
    | BetweenInnings
    | AwaitingFollowOnDecision
    | Completed

type MatchUpdate =
    | StartMatch
    | AbandonMatch
    | DrawMatch
    | StartNextInnings
    | EnforceFollowOn
    | DeclineFollowOn
    | UpdateInnings of (Innings -> Innings)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix )>]
module MatchState = 

    let private startMatch state = 
        match state with
        | NotStarted -> A_Ongoing(Innings.create)
        | _ -> failwith "Call to StartMatch in invalid state"
    
    let private abandonMatch state = 
        match state with
        | NotStarted -> Abandoned
        | _ -> failwith "Call to AbandonMatch in invalid state"
    
    let private drawMatch state = 
        match state with
        | A_Ongoing(a1) | A_Completed(a1) -> A_MatchDrawn(a1)
        | AB_Ongoing(a1, b1) | AB_CompletedNoFollowOn(a1, b1) | AB_CompletedPossibleFollowOn(a1, b1) -> 
            AB_MatchDrawn(a1, b1)
        | ABA_Ongoing(a1, b1, a2) | ABA_Completed(a1, b1, a2) -> ABA_MatchDrawn(a1, b1, a2)
        | ABB_Ongoing(a1, b1, b2) | ABB_Completed(a1, b1, b2) -> ABB_MatchDrawn(a1, b1, b2)
        | ABAB_Ongoing(a1, b1, a2, b2) -> ABAB_MatchDrawn(a1, b1, a2, b2)
        | ABBA_Ongoing(a1, b1, b2, a2) -> ABBA_MatchDrawn(a1, b1, b2, a2)
        | _ -> failwith "Call to DrawMatch in invalid state"
    
    let private startNextInnings state =
        match state with
        | A_Completed a1 -> AB_Ongoing(a1, Innings.create)
        | AB_CompletedNoFollowOn (a1, b1) -> ABA_Ongoing(a1, b1, Innings.create)
        | ABA_Completed (a1, b1, a2) -> ABAB_Ongoing(a1, b1, a2, Innings.create)
        | ABB_Completed (a1, b1, b2) -> ABBA_Ongoing(a1, b1, b2, Innings.create)
        | _ -> failwith "Call to StartNextInnings in invalid state"

    let private enforceFollowOn state = 
        match state with
        | AB_CompletedPossibleFollowOn(a1, b1) -> ABB_Ongoing(a1, b1, Innings.create)
        | _ -> failwith "Call to EnforceFollowOn in invalid state"
    
    let private declineFollowOn state = 
        match state with
        | AB_CompletedPossibleFollowOn(a1, b1) -> ABA_Ongoing(a1, b1, Innings.create)
        | _ -> failwith "Call to DeclineFollowOn in invalid state"
    
    let private updateInnings inningsUpdater rules state = 
        match state with
        | A_Ongoing(a1) -> 
            match inningsUpdater a1 with
            | Innings.InningsOngoing i -> A_Ongoing(i)
            | Innings.InningsCompleted i -> A_Completed(i)
        | AB_Ongoing(a1, b1) -> 
            match inningsUpdater b1 with
            | Innings.InningsOngoing i -> AB_Ongoing(a1, i)
            | Innings.InningsCompleted i when (a1.GetRuns >= i.GetRuns + rules.FollowOnMargin) -> AB_CompletedPossibleFollowOn(a1, i)
            | Innings.InningsCompleted i -> AB_CompletedNoFollowOn(a1, i)
        | ABA_Ongoing(a1, b1, a2) -> 
            match inningsUpdater a2 with
            | Innings.InningsOngoing i -> ABA_Ongoing(a1, b1, i)
            | Innings.InningsCompleted i when (b1.GetRuns > a1.GetRuns + i.GetRuns) -> ABA_VictoryB(a1, b1, i)
            | Innings.InningsCompleted i -> ABA_Completed(a1, b1, i)
        | ABB_Ongoing(a1, b1, b2) -> 
            match inningsUpdater b2 with
            | Innings.InningsOngoing i -> ABB_Ongoing(a1, b1, i)
            | Innings.InningsCompleted i when (a1.GetRuns > b1.GetRuns + i.GetRuns) -> ABB_VictoryA(a1, b1, i)
            | Innings.InningsCompleted i -> ABB_Completed(a1, b1, i)
        | ABAB_Ongoing(a1, b1, a2, b2) -> 
            match inningsUpdater b2 with
            | Innings.InningsOngoing i when (b1.GetRuns + i.GetRuns > a1.GetRuns + a2.GetRuns) -> ABAB_VictoryB(a1, b1, a2, i)
            | Innings.InningsOngoing i -> ABAB_Ongoing(a1, b1, a2, i)
            | Innings.InningsCompleted i when (a1.GetRuns + a2.GetRuns > b1.GetRuns + i.GetRuns) -> ABAB_VictoryA(a1, b1, a2, i)
            | Innings.InningsCompleted i when (a1.GetRuns + a2.GetRuns = b1.GetRuns + i.GetRuns) -> ABAB_MatchTied(a1, b1, a2, i)
            | Innings.InningsCompleted _ -> failwith "Call to UpdateInnings in inconsistent state"
        | ABBA_Ongoing(a1, b1, b2, a2) -> 
            match inningsUpdater a2 with
            | Innings.InningsOngoing i when (a1.GetRuns + i.GetRuns > b1.GetRuns + b2.GetRuns) -> ABBA_VictoryA(a1, b1, b2, i)
            | Innings.InningsOngoing i -> ABBA_Ongoing(a1, b1, b2, i)
            | Innings.InningsCompleted i when (b1.GetRuns + b2.GetRuns > a1.GetRuns + i.GetRuns) -> ABBA_VictoryB(a1, b1, b2, i)
            | Innings.InningsCompleted i when (b1.GetRuns + b2.GetRuns = a1.GetRuns + i.GetRuns) -> 
                ABBA_MatchTied(a1, b1, b2, i)
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
            | A_MatchDrawn _
            | AB_MatchDrawn _
            | ABBA_VictoryA _
            | ABB_VictoryA _
            | ABA_MatchDrawn _
            | ABA_VictoryB _ 
            | ABBA_VictoryB _
            | ABBA_MatchDrawn _
            | ABBA_MatchTied _
            | ABAB_MatchDrawn _
            | ABAB_MatchTied _
            | ABAB_VictoryB _
            | ABAB_VictoryA _
            | ABB_MatchDrawn _ -> Completed
        | AB_CompletedPossibleFollowOn _ -> AwaitingFollowOnDecision
        | A_Ongoing a1 -> InningsInProgress a1
        | AB_Ongoing (_, b1) -> InningsInProgress b1
        | ABA_Ongoing (_, _, a2) -> InningsInProgress a2
        | ABB_Ongoing (_, _, b2) -> InningsInProgress b2
        | ABAB_Ongoing (_, _, _, b2) -> InningsInProgress b2
        | ABBA_Ongoing (_, _, _, a2) -> InningsInProgress a2
        | A_Completed _
            | AB_CompletedNoFollowOn _
            | ABA_Completed _
            | ABB_Completed _ -> BetweenInnings

    let currentInnings state =
        match state with
        | NotStarted | Abandoned | A_Completed _ | A_MatchDrawn _ | AB_CompletedNoFollowOn _ -> failwith "no current innings"
        | AB_CompletedPossibleFollowOn _ | AB_MatchDrawn _ | ABA_VictoryB _ -> failwith "no current innings"
        | ABA_Completed _ | ABA_MatchDrawn _ | ABB_VictoryA _ -> failwith "no current innings"
        | ABB_Completed _ | ABB_MatchDrawn _ | ABAB_VictoryA _ -> failwith "no current innings"
        | ABAB_VictoryB _ | ABAB_MatchDrawn _ | ABAB_MatchTied _ -> failwith "no current innings"
        | ABBA_VictoryA _ | ABBA_VictoryB _ | ABBA_MatchDrawn _ -> failwith "no current innings"
        | ABBA_MatchTied _ -> failwith "no current innings"
        | A_Ongoing a1 -> a1
        | AB_Ongoing (_, b1) -> b1
        | ABA_Ongoing (_, _, a2) | ABBA_Ongoing (_, _, _, a2) -> a2
        | ABB_Ongoing (_, _, b2) | ABAB_Ongoing (_, _, _, b2) -> b2

    let totalRunsA state = 
        match state with
        | NotStarted | Abandoned -> 0
        | A_Ongoing a1
            | A_Completed a1
            | A_MatchDrawn a1 -> a1.GetRuns
        | AB_Ongoing(a1, b1)
            | AB_CompletedNoFollowOn(a1, b1)
            | AB_CompletedPossibleFollowOn(a1, b1)
            | AB_MatchDrawn(a1, b1) -> a1.GetRuns
        | ABA_Ongoing(a1, b1, a2)
            | ABA_VictoryB(a1, b1, a2)
            | ABA_Completed(a1, b1, a2)
            | ABA_MatchDrawn(a1, b1, a2) -> a1.GetRuns + a2.GetRuns
        | ABB_Ongoing(a1, b1, b2)
            | ABB_VictoryA(a1, b1, b2)
            | ABB_Completed(a1, b1, b2)
            | ABB_MatchDrawn(a1, b1, b2) -> a1.GetRuns
        | ABAB_Ongoing(a1, b1, a2, b2)
            | ABAB_VictoryA(a1, b1, a2, b2)
            | ABAB_VictoryB(a1, b1, a2, b2)
            | ABAB_MatchDrawn(a1, b1, a2, b2)
            | ABAB_MatchTied(a1, b1, a2, b2)
            | ABBA_Ongoing(a1, b1, b2, a2)
            | ABBA_VictoryA(a1, b1, b2, a2)
            | ABBA_VictoryB(a1, b1, b2, a2)
            | ABBA_MatchDrawn(a1, b1, b2, a2)
            | ABBA_MatchTied(a1, b1, b2, a2) -> a1.GetRuns + a2.GetRuns

    let totalRunsB state = 
        match state with
        | NotStarted | Abandoned -> 0
        | A_Ongoing a1
            | A_Completed a1
            | A_MatchDrawn a1 -> 0
        | AB_Ongoing(a1, b1)
            | AB_CompletedNoFollowOn(a1, b1)
            | AB_CompletedPossibleFollowOn(a1, b1)
            | AB_MatchDrawn(a1, b1) -> b1.GetRuns
        | ABA_Ongoing(a1, b1, a2)
            | ABA_VictoryB(a1, b1, a2)
            | ABA_Completed(a1, b1, a2)
            | ABA_MatchDrawn(a1, b1, a2) -> b1.GetRuns
        | ABB_Ongoing(a1, b1, b2)
            | ABB_VictoryA(a1, b1, b2)
            | ABB_Completed(a1, b1, b2)
            | ABB_MatchDrawn(a1, b1, b2) -> b1.GetRuns + b2.GetRuns
        | ABAB_Ongoing(a1, b1, a2, b2)
            | ABAB_VictoryA(a1, b1, a2, b2)
            | ABAB_VictoryB(a1, b1, a2, b2)
            | ABAB_MatchDrawn(a1, b1, a2, b2)
            | ABAB_MatchTied(a1, b1, a2, b2)
            | ABBA_Ongoing(a1, b1, b2, a2)
            | ABBA_VictoryA(a1, b1, b2, a2)
            | ABBA_VictoryB(a1, b1, b2, a2)
            | ABBA_MatchDrawn(a1, b1, b2, a2)
            | ABBA_MatchTied(a1, b1, b2, a2) -> b1.GetRuns + b2.GetRuns

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

