namespace Cricket.CricketEngine

type BallOutcome =
    | DotBall
    | ScoreRuns of Runs: int
    | Four
    | Six
    | Bowled
    | HitWicket
    | Caught of CaughtBy: Player * Crossed: bool

[<AutoOpen>]
module BallOutcomeFunctions =
    
    let GetRuns ball =
        match ball with
        | DotBall -> 0
        | ScoreRuns runs -> runs
        | Four -> 4
        | Six -> 6
        | Bowled | HitWicket | Caught _ -> 0

    let HasChangedEnds ball =
        match ball with
        | DotBall | Four | Six | Bowled | HitWicket -> false
        | ScoreRuns runs when (runs % 2 = 1) -> true
        | ScoreRuns _ -> false
        | Caught (_, crossed) -> crossed

    let GetHowOut bowler ball =
        match ball with
        | Bowled -> Some (HowOut.Bowled bowler)
        | HitWicket -> Some (HowOut.HitWicket bowler)
        | Caught (caughtBy, _) -> Some (HowOut.Caught (bowler, caughtBy))
        | _ -> None

    let CountsAsBallFaced ball =
        match ball with
        | _ -> true

    let IsFour ball =
        match ball with
        | Four | ScoreRuns 4 -> true
        | _ -> false

    let IsSix ball =
        match ball with
        | Six | ScoreRuns 6 -> true
        | _ -> false

