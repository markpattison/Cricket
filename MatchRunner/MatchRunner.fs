namespace Cricket.MatchRunner

open Cricket.CricketEngine

type MessageToCaptain =
    | FollowOnDecision
    | NewBatsmanRequired of int
    | NewBowlerRequiredTo of End
    | EndOfOver
    | CanDeclare

type UpdateOptions =
    | MessageCaptain of (Team * Match * MessageToCaptain)
    | StartMatch
    | StartNextInnings
    | ContinueInnings

module MatchRunner =

    let updateOptions match' =
        let state = match'.State
        let summaryState = state |> MatchState.summaryState
        let battingTeam = state |> MatchState.currentBattingTeam
        let bowlingTeam = state |> MatchState.currentBowlingTeam

        match summaryState with
        | NotYetStarted -> [ StartMatch ]
        | BetweenInnings -> [ StartNextInnings ]
        | AwaitingFollowOnDecision -> [ MessageCaptain (TeamA, match', FollowOnDecision) ]
        | MatchCompleted -> []
        | InningsInProgress summaryInningsState ->
            match summaryInningsState with
            | BatsmanRequired n -> [ MessageCaptain (battingTeam, match', NewBatsmanRequired n) ]
            | BowlerRequiredTo end' -> [ MessageCaptain (bowlingTeam, match', NewBowlerRequiredTo end') ]
            | EndOver -> [ MessageCaptain (bowlingTeam, match', EndOfOver); MessageCaptain (battingTeam, match', CanDeclare) ]
            | MidOver -> [ MessageCaptain (battingTeam, match', CanDeclare) ]
            | Completed -> failwith "invalid innings state"

    let simpleCaptain (team, match', msg) =
        let innings = match'.State |> MatchState.currentInnings
        match msg with
        | FollowOnDecision -> EnforceFollowOn |> Some
        | NewBatsmanRequired n -> SendInBatsman { Name = sprintf "%s Batsman %i" team n } |> UpdateInnings |> Some
        | NewBowlerRequiredTo _ | EndOfOver ->
            let overs = innings.OversCompleted
            let bowler =
                match (overs / 8) % 2, overs % 2 with
                | 0, 0 -> { Name = sprintf "%s Bowler %i" team 10 }
                | 0, 1 -> { Name = sprintf "%s Bowler %i" team 11 }
                | 1, 0 -> { Name = sprintf "%s Bowler %i" team 8 }
                | 1, 1 -> { Name = sprintf "%s Bowler %i" team 9 }
                | _ -> { Name = "???" }
            SendInBowler bowler |> UpdateInnings |> Some
        | CanDeclare -> None