namespace Cricket.CricketEngine

type HowOut =
    | Bowled
    | TimedOut
    | Caught
    | HandledTheBall
    | HitTheBallTwice
    | HitWicket
    | LegBeforeWicket
    | ObstructingTheField
    | RunOut
    | Stumped

type IndividualInnings =
    { Score: int; HowOut: HowOut option; BallsFaced: int }

[<AutoOpen>]
module IndividualInningsTransitions =

    let NewIndividualInnings = { Score = 0; HowOut = None; BallsFaced = 0 }

    let ScoreRuns runs innings =
        { innings with Score = innings.Score + runs; BallsFaced = innings.BallsFaced + 1 }

    let DotBall = ScoreRuns 0

    let GetOut innings howOut =
        { innings with HowOut = Some howOut }