namespace Cricket.MatchRunner

open Cricket.CricketEngine

module MatchRunner =

    let updateOptions match' =
        let state = match'.State
        let summaryState = state |> MatchState.summaryState

        match summaryState with
        | NotYetStarted -> UpdateOptions.StartMatch
        | BetweenInnings -> UpdateOptions.StartNextInnings
        | AwaitingFollowOnDecision -> ModalMessageToCaptain (TeamA, FollowOnDecision)
        | MatchCompleted -> UpdateOptions.MatchOver
        | InningsInProgress summaryInningsState ->
            let battingTeam = state |> MatchState.currentBattingTeam
            let bowlingTeam = state |> MatchState.currentBowlingTeam
            match summaryInningsState with
            | BatsmanRequired n -> ModalMessageToCaptain (battingTeam,NewBatsmanRequired n)
            | BowlerRequiredTo end' -> ModalMessageToCaptain (bowlingTeam, NewBowlerRequiredTo end')
            | EndOver -> ContinueInnings [ (battingTeam, CanDeclare); (bowlingTeam, EndOfOver) ]
            | MidOver -> ContinueInnings [ (battingTeam, CanDeclare) ]
            | Completed -> failwith "invalid innings state"

    let rec updateWithExclusions exclude match' =
        let options = updateOptions match'
        match options with
        | ModalMessageToCaptain msg ->
            let action = SimpleCaptain.replyModal (msg, match')
            Match.updateMatchState action match' |> updateWithExclusions []
        | ContinueInnings msgList ->
            let afterExclusions = List.except exclude msgList
            match afterExclusions with
            | [] -> ContinueInningsUI
            | msg :: _ ->
                let response = SimpleCaptain.replyOptional (msg, match')
                match response with
                | None -> match' |> updateWithExclusions (msg::exclude)
                | Some action -> Match.updateMatchState action match' |> updateWithExclusions (msg::exclude)
        | UpdateOptions.StartMatch -> StartMatchUI
        | UpdateOptions.StartNextInnings -> StartNextInningsUI
        | UpdateOptions.MatchOver -> MatchOverUI

    let updateForUI match' =
        updateWithExclusions [] match'