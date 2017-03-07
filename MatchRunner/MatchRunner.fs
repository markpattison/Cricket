namespace Cricket.MatchRunner

open Cricket.CricketEngine

module MatchRunner =

    let updateOptions match' =
        let state = match'.State
        let summaryState = state |> MatchState.summaryState
        let battingTeam = state |> MatchState.currentBattingTeam
        let bowlingTeam = state |> MatchState.currentBowlingTeam

        match summaryState with
        | NotYetStarted -> UpdateOptions.StartMatch
        | BetweenInnings -> UpdateOptions.StartNextInnings
        | AwaitingFollowOnDecision -> ModalMessageToCaptain (TeamA, match', FollowOnDecision)
        | MatchCompleted -> UpdateOptions.MatchOver
        | InningsInProgress summaryInningsState ->
            match summaryInningsState with
            | BatsmanRequired n -> ModalMessageToCaptain (battingTeam, match', NewBatsmanRequired n)
            | BowlerRequiredTo end' -> ModalMessageToCaptain (bowlingTeam, match', NewBowlerRequiredTo end')
            | EndOver -> ContinueInnings [ (battingTeam, match', CanDeclare); (bowlingTeam, match', EndOfOver) ]
            | MidOver -> ContinueInnings [ (battingTeam, match', CanDeclare) ]
            | Completed -> failwith "invalid innings state"

    let rec updateForUI match' =
        let options = updateOptions match'
        match options with
        | ModalMessageToCaptain msg ->
            let action = SimpleCaptain.replyModal msg
            Match.updateMatchState action match' |> updateForUI
        | ContinueInnings msgList ->
            let actions = msgList |> List.choose SimpleCaptain.replyOptional
            match actions with
            | [] -> ContinueInningsUI
            | [ action ] -> Match.updateMatchState action match' |> updateForUI
            | action :: _ -> Match.updateMatchState action match' |> updateForUI
        | UpdateOptions.StartMatch -> StartMatchUI
        | UpdateOptions.StartNextInnings -> StartNextInningsUI
        | UpdateOptions.MatchOver -> MatchOverUI
