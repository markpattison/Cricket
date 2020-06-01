namespace Cricket.MatchRunner

open Cricket.CricketEngine

type ServerModel =
    {
        Match: Match
        PlayerRecords: Map<Player, PlayerRecord>
        LivePlayerRecords: Map<Player, PlayerRecord>
        PlayerAttributes: PlayerAttributes
        Series: Series
    }

type ServerMsg =
    | StartMatchMessage
    | StartNextInningsMessage
    | ContinueInningsBallMessage
    | ContinueInningsOverMessage
    | ContinueInningsInningsMessage
    | ResetMatchMessage

module MatchRunner =

    let random = System.Random()

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
        | MatchCompleted _ -> UpdateOptions.MatchOver
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
        | ContinueInnings _ -> [ ContinueInningsBallUI; ContinueInningsOverUI(*; ContinueInningsInningsUI*) ]
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
        | MatchCompleted _ -> match'
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

    let continueInningsBall attributes match' =
        let innings = match'.State |> MatchState.currentInnings
        let batsman = innings |> Innings.batsmanFacingNext
        let bowler = innings |> Innings.bowlerBowlingNext

        let ball = RandomBall.ball attributes batsman bowler
        match'
        |> Match.updateCurrentInnings (UpdateForBall ball)
        |> runCaptains
    
    let rec continueInningsOver attributes match' =
        let updated = continueInningsBall attributes match'
        match updated.State |> MatchState.summaryState with
        | InningsInProgress MidOver -> continueInningsOver attributes updated
        | _ -> updated

    let rec continueInningsInnings attributes match' =
        let updated = continueInningsBall attributes match'
        match updated.State |> MatchState.summaryState with
        | InningsInProgress _ -> continueInningsInnings attributes updated
        | _ -> updated

    let updateMatchState update match' =
        match'
        |> Match.updateMatchState update
        |> runCaptains

    let updateModel f model =
        let alreadyCompleted = Match.isCompleted model.Match
        let updatedMatch = f model.Match
        let justCompleted = (Match.isCompleted updatedMatch) && not alreadyCompleted
        let updatedRecords = Averages.updatePlayersForMatch model.PlayerRecords updatedMatch

        if justCompleted then
            let updatedSeries = Series.update model.Series updatedMatch BatFirst.Team1 // TODO change this if not always Team1 batting first
            {
                model with
                    Match = updatedMatch
                    Series = updatedSeries
                    PlayerRecords = updatedRecords
                    LivePlayerRecords = updatedRecords
            }
        else
            {
                model with
                    Match = updatedMatch
                    LivePlayerRecords = updatedRecords
            }

    let update msg model =
        match msg with
        | ContinueInningsBallMessage -> updateModel (continueInningsBall model.PlayerAttributes) model
        | ContinueInningsOverMessage -> updateModel (continueInningsOver model.PlayerAttributes) model
        | ContinueInningsInningsMessage -> updateModel (continueInningsInnings model.PlayerAttributes) model
        | StartMatchMessage -> updateModel (updateMatchState MatchUpdate.StartMatch) model
        | StartNextInningsMessage -> updateModel (updateMatchState MatchUpdate.StartNextInnings) model
        | ResetMatchMessage -> { model with Match = MatchData.newMatch }
