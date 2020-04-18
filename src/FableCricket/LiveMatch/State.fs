module FableCricket.LiveMatch.State

open Cricket.CricketEngine
open Cricket.MatchRunner

open Elmish

open Types

let init () : Model * Cmd<Msg> =
    {
        Match = MatchData.newMatch
        PlayerRecords = Map.empty
        LivePlayerRecords = Map.empty
        InningsExpanded = []
        Series = Series.create "England" "India"
        PlayerAttributes =
            {
                Attributes =
                    MatchData.players
                    |> List.mapi (fun n (_, bat, bowl) -> n, { BattingSkill = bat; BowlingSkill = bowl })
                    |> Map.ofList
            }
    }, []

let checkForNewInnings model =
    let numInnings = model.Match |> Match.inningsList |> List.length
    let numExpanders = model.InningsExpanded |> List.length
    if numInnings > numExpanders then
        let newExpanders = List.init numInnings (fun i -> i = numInnings - 1)
        { model with InningsExpanded = newExpanders }
    else
        model

let update msg model =
    let updateModel f =
        let updated =
            let alreadyCompleted = Match.isCompleted model.Match
            let updatedMatch = f model.Match
            let nowCompleted = Match.isCompleted updatedMatch
            let justCompleted = nowCompleted && not alreadyCompleted
            let updatedRecords = Averages.updatePlayersForMatch model.PlayerRecords updatedMatch
            let updatedSeries =
                if justCompleted then
                    let result =
                        match MatchState.summaryState updatedMatch.State with
                        | MatchCompleted result -> result
                        | _ -> NoResult
                    Series.update model.Series result BatFirst.Team1 // TODO change this if not always Team1 batting first
                else
                    model.Series
            {
                model with
                    Match = updatedMatch
                    PlayerRecords = if justCompleted then updatedRecords else model.PlayerRecords
                    LivePlayerRecords = updatedRecords
                    Series = updatedSeries
            }
            |> checkForNewInnings
        updated, []
    match msg with
    | ContinueInningsBallMessage -> updateModel (MatchRunner.continueInningsBall model.PlayerAttributes)
    | ContinueInningsOverMessage -> updateModel (MatchRunner.continueInningsOver model.PlayerAttributes)
    | ContinueInningsInningsMessage -> updateModel (MatchRunner.continueInningsInnings model.PlayerAttributes)
    | StartMatchMessage -> updateModel (MatchRunner.updateMatchState MatchUpdate.StartMatch)
    | StartNextInningsMessage -> updateModel (MatchRunner.updateMatchState MatchUpdate.StartNextInnings)
    | ResetMatchMessage -> { model with Match = MatchData.newMatch; InningsExpanded = [] }, []
    | ToggleInningsExpandedMessage index ->
        { model with
            InningsExpanded = model.InningsExpanded |> List.mapi
                (fun i expanded -> if i = index then not expanded else expanded)
        }, []