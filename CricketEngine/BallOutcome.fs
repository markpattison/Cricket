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

    member _this.GetRuns =
        match _this with
        | DotBall -> 0
        | ScoreRuns runs -> runs
        | Four -> 4
        | Six -> 6
        | RunOut (runs, _) -> runs
        | Bowled | HitWicket | Caught _ | LBW | Stumped _ -> 0

    member _this.HasChangedEnds =
        match _this with
        | DotBall | Four | Six | Bowled | HitWicket | LBW | Stumped _ -> false
        | ScoreRuns runs when (runs % 2 = 1) -> true
        | ScoreRuns _ -> false
        | Caught (_, crossed) -> crossed
        | RunOut (runs, alsoCrossed) when (runs % 2 = 1) -> not alsoCrossed
        | RunOut (_, alsoCrossed) -> alsoCrossed

    member _this.GetHowOut bowler =
        match _this with
        | Bowled -> Some (HowOut.Bowled bowler)
        | LBW -> Some (HowOut.LBW bowler)
        | HitWicket -> Some (HowOut.HitWicket bowler)
        | Caught (caughtBy, _) -> Some (HowOut.Caught (bowler, caughtBy))
        | Stumped (stumpedBy) -> Some (HowOut.Stumped (bowler, stumpedBy))
        | RunOut (_, _) -> Some (HowOut.RunOut)
        | _ -> None

    member _this.CountsAsBallFaced =
        match _this with
        | _ -> true

    member _this.IsAFour =
        match _this with
        | Four | ScoreRuns 4 | RunOut (4, _) -> true
        | _ -> false

    member _this.IsASix =
        match _this with
        | Six | ScoreRuns 6 | RunOut (6, _) -> true
        | _ -> false
