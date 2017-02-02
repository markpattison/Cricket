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

[<AutoOpen>]
module MatchStateTransitions = 
    //    let (|MatchNotStarted|MatchAbandoned|MatchDrawn|MatchVictoryA|MatchVictoryB|MatchTied|MatchOngoing|) state =
    //        match state with
    //        | NotStarted -> MatchNotStarted
    //        | Abandoned -> MatchAbandoned
    //        | A_MatchDrawn _ | AB_MatchDrawn _ | ABA_MatchDrawn _ | ABB_MatchDrawn _ | ABAB_MatchDrawn _ | ABBA_MatchDrawn _ -> MatchDrawn
    //        | ABB_VictoryA _ | ABAB_VictoryA _ | ABBA_VictoryA _ -> MatchVictoryA
    //        | ABA_VictoryB _ | ABAB_VictoryB _ | ABBA_VictoryB _ -> MatchVictoryB
    //        | ABAB_MatchTied _ | ABBA_MatchTied _ -> MatchTied
    //        | _ -> MatchOngoing
    //
    //    let (|NotStarted|Completed|Ongoing|) state =
    //        match state with
    //        | MatchNotStarted -> NotStarted
    //        | MatchOngoing -> Ongoing
    //        | _ -> Completed
    let private startMatch state = 
        match state with
        | NotStarted -> A_Ongoing(NewInnings)
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
    
    let private enforceFollowOn state = 
        match state with
        | AB_CompletedPossibleFollowOn(a1, b1) -> ABB_Ongoing(a1, b1, NewInnings)
        | _ -> failwith "Call to EnforceFollowOn in invalid state"
    
    let private declineFollowOn state = 
        match state with
        | AB_CompletedPossibleFollowOn(a1, b1) -> ABA_Ongoing(a1, b1, NewInnings)
        | _ -> failwith "Call to DeclineFollowOn in invalid state"
    
    let private updateInnings inningsUpdater rules state = 
        match state with
        | A_Ongoing(a1) -> 
            match inningsUpdater a1 with
            | InningsOngoing i -> A_Ongoing(i)
            | InningsCompleted i -> A_Completed(i)
        | AB_Ongoing(a1, b1) -> 
            match inningsUpdater b1 with
            | InningsOngoing i -> AB_Ongoing(a1, i)
            | InningsCompleted i when (a1.GetRuns >= i.GetRuns + rules.FollowOnMargin) -> 
                AB_CompletedPossibleFollowOn(a1, i)
            | InningsCompleted i -> AB_CompletedNoFollowOn(a1, i)
        | ABA_Ongoing(a1, b1, a2) -> 
            match inningsUpdater a2 with
            | InningsOngoing i -> ABA_Ongoing(a1, b1, i)
            | InningsCompleted i when (b1.GetRuns > a1.GetRuns + i.GetRuns) -> ABA_VictoryB(a1, b1, i)
            | InningsCompleted i -> ABA_Completed(a1, b1, i)
        | ABB_Ongoing(a1, b1, b2) -> 
            match inningsUpdater b2 with
            | InningsOngoing i -> ABB_Ongoing(a1, b1, i)
            | InningsCompleted i when (a1.GetRuns > b1.GetRuns + i.GetRuns) -> ABB_VictoryA(a1, b1, i)
            | InningsCompleted i -> ABB_Completed(a1, b1, i)
        | ABAB_Ongoing(a1, b1, a2, b2) -> 
            match inningsUpdater b2 with
            | InningsOngoing i when (b1.GetRuns + i.GetRuns > a1.GetRuns + a2.GetRuns) -> ABAB_VictoryB(a1, b1, a2, i)
            | InningsOngoing i -> ABAB_Ongoing(a1, b1, a2, i)
            | InningsCompleted i when (a1.GetRuns + a2.GetRuns > b1.GetRuns + i.GetRuns) -> ABAB_VictoryA(a1, b1, a2, i)
            | InningsCompleted i when (a1.GetRuns + a2.GetRuns = b1.GetRuns + i.GetRuns) -> 
                ABAB_MatchTied(a1, b1, a2, i)
            | InningsCompleted _ -> failwith "Call to UpdateInnings in inconsistent state"
        | ABBA_Ongoing(a1, b1, b2, a2) -> 
            match inningsUpdater a2 with
            | InningsOngoing i when (a1.GetRuns + i.GetRuns > b1.GetRuns + b2.GetRuns) -> ABBA_VictoryA(a1, b1, b2, i)
            | InningsOngoing i -> ABBA_Ongoing(a1, b1, b2, i)
            | InningsCompleted i when (b1.GetRuns + b2.GetRuns > a1.GetRuns + i.GetRuns) -> ABBA_VictoryB(a1, b1, b2, i)
            | InningsCompleted i when (b1.GetRuns + b2.GetRuns = a1.GetRuns + i.GetRuns) -> 
                ABBA_MatchTied(a1, b1, b2, i)
            | InningsCompleted _ -> failwith "Call to UpdateInnings in inconsistent state"
        | _ -> failwith "Call to UpdateInnings in invalid state"

    let update rules update state =
        match update with
        | StartMatch -> startMatch state
        | AbandonMatch -> abandonMatch state
        | DrawMatch -> drawMatch state
        | StartNextInnings -> failwith "Not implemented"
        | EnforceFollowOn -> enforceFollowOn state
        | DeclineFollowOn -> declineFollowOn state
        | UpdateInnings inningsUpdate -> updateInnings inningsUpdate rules state


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

[<AutoOpen>]
module MatchStateFunctions = 

    let SummaryState state =
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

    let CurrentInnings state =
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

    let TotalRunsA state = 
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

    let TotalRunsB state = 
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

    let LeadA state =
        (TotalRunsA state) - (TotalRunsB state)

    let LeadB state =
        (TotalRunsB state) - (TotalRunsA state)

    let (|ALeads|ScoresLevel|BLeads|) state =
        match (LeadA state) with
        | x when x > 0 -> ALeads
        | x when x < 0 -> BLeads
        | _ -> ScoresLevel
