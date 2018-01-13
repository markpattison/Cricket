module FableCricket.LiveMatch.State

open Cricket.CricketEngine
open Cricket.MatchRunner

open Elmish

open Types

let newMatch =
    {
        TeamA =
            {
                Name = "England"
                Players =
                    [|
                        { Name = "GA Gooch"; ID = 1 }
                        { Name = "MA Atherton"; ID = 2 }
                        { Name = "DI Gower"; ID = 3 }
                        { Name = "AJ Lamb"; ID = 4 }
                        { Name = "RA Smith"; ID = 5 }
                        { Name = "JE Morris"; ID = 6 }
                        { Name = "RC Russell"; ID = 7 }
                        { Name = "CC Lewis"; ID = 8 }
                        { Name = "EE Hemmings"; ID = 9 }
                        { Name = "ARC Fraser"; ID = 10 }
                        { Name = "DE Malcolm"; ID = 11 }
                    |]
            }
        TeamB =
            {
                Name = "India"
                Players =
                    [|
                        { Name = "RJ Shastri"; ID = 12 }
                        { Name = "NS Sidhu"; ID = 13 }
                        { Name = "SV Manjrekar"; ID = 14 }
                        { Name = "DB Vengsarkar"; ID = 15 }
                        { Name = "M Azharuddin"; ID = 16 }
                        { Name = "SR Tendulkar"; ID = 17 }
                        { Name = "M Prabhakar"; ID = 18 }
                        { Name = "N Kapil Dev"; ID = 19 }
                        { Name = "KS More"; ID = 20 }
                        { Name = "SK Sharma"; ID = 21 }
                        { Name = "ND Hirwani"; ID = 22 }
                    |]
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
    | ContinueInningsBallMessage -> updateMatch (MatchRunner.continueInningsBall)
    | ContinueInningsOverMessage -> updateMatch (MatchRunner.continueInningsOver)
    | StartMatchMessage -> updateMatch (MatchRunner.updateMatchState MatchUpdate.StartMatch)
    | StartNextInningsMessage -> updateMatch (MatchRunner.updateMatchState MatchUpdate.StartNextInnings)
    | ResetMatchMessage -> { model with Match = newMatch; InningsExpanded = [] }, []
    | ToggleInningsExpandedMessage index ->
        { model with
            InningsExpanded = model.InningsExpanded |> List.mapi
                (fun i expanded -> if i = index then not expanded else expanded)
        }, []