namespace Cricket.MatchRunner

open Cricket.CricketEngine

module MatchRunner =

    let random = new System.Random()

    let getOptions match' =
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

    let getOptionsUI match' =
        let options = getOptions match'
        match options with
        | ModalMessageToCaptain _ -> failwith "shouldn't happen"
        | ContinueInnings _ -> [ ContinueInningsBallUI; ContinueInningsOverUI ]
        | UpdateOptions.StartMatch -> [ StartMatchUI ]
        | UpdateOptions.StartNextInnings -> [ StartNextInningsUI ]
        | UpdateOptions.MatchOver -> [ MatchOverUI ]

    let rec runCaptainsWithExclusions exclude match' =
        let options = getOptions match'
        match options with
        | ModalMessageToCaptain msg ->
            let action = SimpleCaptain.replyModal (msg, match')
            Match.updateMatchState action match' |> runCaptainsWithExclusions []
        | ContinueInnings msgList ->
            let afterExclusions = List.except exclude msgList
            match afterExclusions with
            | [] -> match'
            | msg :: _ ->
                let response = SimpleCaptain.replyOptional (msg, match')
                match response with
                | None -> match' |> runCaptainsWithExclusions (msg::exclude)
                | Some action -> Match.updateMatchState action match' |> runCaptainsWithExclusions (msg::exclude)
        | UpdateOptions.StartMatch -> match'
        | UpdateOptions.StartNextInnings -> match'
        | UpdateOptions.MatchOver -> match'

    let runCaptains match' =
        runCaptainsWithExclusions [] match'

    let continueInningsBall match' =
        let ball =
            match random.Next(0, 100) with
            | x when x <= 5 -> Bowled
            | x when x <= 7 -> LBW
            | x when x <= 8 -> RunOutStriker (0, false)
            | x when x >= 99 -> Six
            | x when x >= 97 -> Four
            | x when x >= 95 -> ScoreRuns 2
            | x when x >= 3 -> ScoreRuns 1
            | _ -> DotBall
        match'
        |> Match.updateCurrentInnings (UpdateForBall ball)
        |> runCaptains
    
    let rec continueInningsOver match' =
        let updated = continueInningsBall match'
        match updated.State |> MatchState.summaryState with
        | InningsInProgress MidOver -> continueInningsOver updated
        | _ -> updated

