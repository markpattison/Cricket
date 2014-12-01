namespace Innings

type Innings =
    | Innings of int * int
    member x.GetRuns =
        match x with
        | Innings (runs, _) -> runs
    member x.GetWickets =
        match x with
        | Innings (_, wickets) -> wickets

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
        | Innings (score, wickets) -> InningsOngoing (Innings (score + runs, wickets))

    let LoseWicket innings =
        match innings with
        | Innings (score, 9) -> InningsCompleted (Innings (score, 10))
        | Innings (score, wickets) -> InningsOngoing (Innings (score, wickets + 1))

    let Declare innings = InningsCompleted innings
