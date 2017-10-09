module Cricket.State

open Cricket.CricketEngine
open Cricket.MatchRunner

open Elmish

open Types

let init () : Model * Cmd<Msg> =
    { Match = { TeamA = "England"; TeamB = "Australia"; State = NotStarted; Rules = { FollowOnMargin = 200 } } }, []

let update msg model =
    let updateMatch f =
        let updated = model.Match |> f |> MatchRunner.runCaptains
        { model with Match = updated }, []
    match msg with
    | StartMatch -> updateMatch (Match.updateMatchState MatchUpdate.StartMatch)
    | StartNextInnings -> updateMatch (Match.updateMatchState MatchUpdate.StartNextInnings)
    | ContinueInnings -> updateMatch (MatchRunner.continueInnings)
    | ResetMatch -> init ()