module Cricket.State

open Cricket.CricketEngine
open Cricket.MatchRunner

open Elmish

open Types

let init () : Model * Cmd<Msg> =
    { Match =
        {
            TeamA =
                {
                    Name = "England"
                    Players =
                        [|
                            { Name = "GA Gooch" }
                            { Name = "MA Atherton" }
                            { Name = "DI Gower" }
                            { Name = "AJ Lamb" }
                            { Name = "RA Smith" }
                            { Name = "JE Morris" }
                            { Name = "RC Russell" }
                            { Name = "CC Lewis" }
                            { Name = "EE Hemmings" }
                            { Name = "ARC Fraser" }
                            { Name = "DE Malcolm" }
                        |]
                }
            TeamB =
                {
                    Name = "India"
                    Players =
                        [|
                            { Name = "RJ Shastri" }
                            { Name = "NS Sidhu" }
                            { Name = "SV Manjrekar" }
                            { Name = "DB Vengsarkar" }
                            { Name = "M Azharuddin" }
                            { Name = "SR Tendulkar" }
                            { Name = "M Prabhakar" }
                            { Name = "N Kapil Dev" }
                            { Name = "KS More" }
                            { Name = "SK Sharma" }
                            { Name = "ND Hirwani" }
                        |]
                }
            State = NotStarted
            Rules = { FollowOnMargin = 200 } }
        },
    []

let update msg model =
    let updateMatch f =
        let updated = model.Match |> f |> MatchRunner.runCaptains
        { model with Match = updated }, []
    match msg with
    | StartMatch -> updateMatch (Match.updateMatchState MatchUpdate.StartMatch)
    | StartNextInnings -> updateMatch (Match.updateMatchState MatchUpdate.StartNextInnings)
    | ContinueInnings -> updateMatch (MatchRunner.continueInnings)
    | ResetMatch -> init ()