module FableCricket.LiveMatch.State

open Cricket.CricketEngine
open Cricket.MatchRunner

open Elmish

open Types

let players =
    [
        "GA Gooch",      1.00, 0.50
        "MA Atherton",   1.05, 0.60
        "DI Gower",      1.00, 0.50
        "AJ Lamb",       0.90, 0.50
        "RA Smith",      0.95, 0.50
        "JE Morris",     0.90, 0.50
        "RC Russell",    0.85, 0.50
        "CC Lewis",      0.80, 0.70
        "EE Hemmings",   0.70, 0.65
        "ARC Fraser",    0.65, 0.75
        "DE Malcolm",    0.60, 0.70
        "RJ Shastri",    0.95, 0.70
        "NS Sidhu",      0.90, 0.70
        "SV Manjrekar",  1.00, 0.70
        "DB Vengsarkar", 0.90, 0.70
        "M Azharuddin",  1.00, 0.70
        "SR Tendulkar",  1.05, 0.70
        "M Prabhakar",   0.90, 0.70
        "N Kapil Dev",   0.85, 0.75
        "KS More",       0.80, 0.70
        "SK Sharma",     0.70, 0.65
        "ND Hirwani",    0.65, 0.70
    ]

let newMatch =
    {
        TeamA =
            {
                Name = "England"
                Players =
                    players
                    |> List.take 11
                    |> List.mapi (fun n (name, _, _) -> { Name = name; ID = n })
                    |> List.toArray
            }
        TeamB =
            {
                Name = "India"
                Players =
                    players
                    |> List.skip 11
                    |> List.take 11
                    |> List.mapi (fun n (name, _, _) -> { Name = name; ID = 11 + n })
                    |> List.toArray
            }
        State = NotStarted
        Rules = { FollowOnMargin = 200 }
    }


let init () : Model * Cmd<Msg> =
    {
        Match = newMatch
        PlayerRecords = Map.empty
        LivePlayerRecords = Map.empty
        InningsExpanded = []
        Series = Series.create "England" "India"
        PlayerAttributes =
            {
                Attributes =
                    players
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
    let updateMatch f =
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
    | ContinueInningsBallMessage -> updateMatch (MatchRunner.continueInningsBall model.PlayerAttributes)
    | ContinueInningsOverMessage -> updateMatch (MatchRunner.continueInningsOver model.PlayerAttributes)
    | ContinueInningsInningsMessage -> updateMatch (MatchRunner.continueInningsInnings model.PlayerAttributes)
    | StartMatchMessage -> updateMatch (MatchRunner.updateMatchState MatchUpdate.StartMatch)
    | StartNextInningsMessage -> updateMatch (MatchRunner.updateMatchState MatchUpdate.StartNextInnings)
    | ResetMatchMessage -> { model with Match = newMatch; InningsExpanded = [] }, []
    | ToggleInningsExpandedMessage index ->
        { model with
            InningsExpanded = model.InningsExpanded |> List.mapi
                (fun i expanded -> if i = index then not expanded else expanded)
        }, []