namespace Cricket.CricketEngine

type Innings =
    | Innings of Runs: int * Wickets: int * Declared: bool
    member x.GetRuns =
        match x with
        | Innings (runs, _, _) -> runs
    member x.GetWickets =
        match x with
        | Innings (_, wickets, _) -> wickets
    member x.IsDeclared =
        match x with
        | Innings (_, _, declared) -> declared

type InningsStatus =
    | InningsCompleted of Innings
    | InningsOngoing of Innings
    member x.GetInnings =
        match x with
        | InningsCompleted innings -> innings
        | InningsOngoing innings -> innings

[<AutoOpen>]
module InningsFunctions =

    let ScoreRuns runs innings =
        match innings with
        | Innings (score, wickets, false) -> InningsOngoing (Innings (score + runs, wickets, false))
        | _ -> failwith "Call to ScoreRuns after innings declared"

    let LoseWicket innings =
        match innings with
        | Innings (score, 9, false) -> InningsCompleted (Innings (score, 10, false))
        | Innings (score, wickets, false) -> InningsOngoing (Innings (score, wickets + 1, false))
        | _ -> failwith "Call to LoseWicket after innings declared"

    let Declare innings =
        match innings with
        | Innings (score, wickets, false) -> InningsCompleted (Innings (score, wickets, true))
        | _ -> failwith "Call to Declare after innings declared"
