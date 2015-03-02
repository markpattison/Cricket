namespace Cricket.CricketEngine

type BallOutcome =
    | DotBall
    | ScoreRuns of Runs: int
    | Four
    | Six
    | Bowled
    | HitWicket
    | LBW
    | Caught of CaughtBy: Player * Crossed: bool
    | Stumped of StumpedBy: Player
    | RunOut of Runs: int * AlsoCrossed: bool

[<AutoOpen>]
module BallOutcomeFunctions =
    
    let GetRuns ball =
        match ball with
        | DotBall -> 0
        | ScoreRuns runs -> runs
        | Four -> 4
        | Six -> 6
        | RunOut (runs, _) -> runs
        | Bowled | HitWicket | Caught _ | LBW | Stumped _ -> 0

    let HasChangedEnds ball =
        match ball with
        | DotBall | Four | Six | Bowled | HitWicket | LBW | Stumped _ -> false
        | ScoreRuns runs when (runs % 2 = 1) -> true
        | ScoreRuns _ -> false
        | Caught (_, crossed) -> crossed
        | RunOut (runs, alsoCrossed) when (runs % 2 = 1) -> not alsoCrossed
        | RunOut (_, alsoCrossed) -> alsoCrossed

    let GetHowOut bowler ball =
        match ball with
        | Bowled -> Some (HowOut.Bowled bowler)
        | LBW -> Some (HowOut.LBW bowler)
        | HitWicket -> Some (HowOut.HitWicket bowler)
        | Caught (caughtBy, _) -> Some (HowOut.Caught (bowler, caughtBy))
        | Stumped (stumpedBy) -> Some (HowOut.Stumped (bowler, stumpedBy))
        | RunOut (_, _) -> Some (HowOut.RunOut)
        | _ -> None

    let CountsAsBallFaced ball =
        match ball with
        | _ -> true

    let IsFour ball =
        match ball with
        | Four | ScoreRuns 4 | RunOut (4, _) -> true
        | _ -> false

    let IsSix ball =
        match ball with
        | Six | ScoreRuns 6 | RunOut (6, _) -> true
        | _ -> false

