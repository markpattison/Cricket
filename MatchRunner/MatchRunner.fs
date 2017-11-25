namespace Cricket.MatchRunner

open Cricket.CricketEngine

module MatchRunner =

    let random = new System.Random()

    let private optionalCanDeclare battingTeam match' =
        let response = SimpleCaptain.replyOptional (battingTeam, CanDeclare) match'
        match response with
        | None -> match'
        | Some update -> Match.updateMatchState update match'

    let private optionalCanChangeBowler bowlingTeam match' =
        let response = SimpleCaptain.replyOptional (bowlingTeam, EndOfOver) match'
        match response with
        | None -> match'
        | Some update -> Match.updateMatchState update match'

    let private getOption match' =
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
            | BatsmanRequired n -> ModalMessageToCaptain (battingTeam, NewBatsmanRequired n)
            | BowlerRequiredTo end' -> ModalMessageToCaptain (bowlingTeam, NewBowlerRequiredTo end')
            | EndOver | MidOver -> ContinueInnings
            | SummaryInningsState.Completed -> failwith "invalid innings state"

    let getOptionsUI match' =
        let option = getOption match'
        match option with
        | ModalMessageToCaptain _ -> failwith "shouldn't happen"
        | ContinueInnings _ -> [ ContinueInningsBallUI; ContinueInningsOverUI ]
        | UpdateOptions.StartMatch -> [ StartMatchUI ]
        | UpdateOptions.StartNextInnings -> [ StartNextInningsUI ]
        | UpdateOptions.MatchOver -> [ MatchOverUI ]

    let rec runCaptains match' =
        let state = match'.State
        let summaryState = state |> MatchState.summaryState

        match summaryState with
        | NotYetStarted -> match'
        | BetweenInnings -> match'
        | AwaitingFollowOnDecision ->
            let update = SimpleCaptain.replyModal (TeamA, FollowOnDecision) match'
            Match.updateMatchState update match' |> runCaptains
        | MatchCompleted -> match'
        | InningsInProgress summaryInningsState ->
            let battingTeam = state |> MatchState.currentBattingTeam
            let bowlingTeam = state |> MatchState.currentBowlingTeam
            match summaryInningsState with
            | BatsmanRequired n ->
                let update = SimpleCaptain.replyModal (battingTeam, NewBatsmanRequired n) match'
                Match.updateMatchState update match' |> runCaptains
            | BowlerRequiredTo end' ->
                let update = SimpleCaptain.replyModal (bowlingTeam, NewBowlerRequiredTo end') match'
                Match.updateMatchState update match' |> runCaptains
            | EndOver -> match' |> optionalCanChangeBowler bowlingTeam |> optionalCanDeclare battingTeam
            | MidOver -> match' |> optionalCanDeclare battingTeam
            | SummaryInningsState.Completed -> failwith "invalid innings state"

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

    let updateMatchState update match' =
        match'
        |> Match.updateMatchState update
        |> runCaptains
