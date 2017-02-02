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
    | RunOutStriker of Runs: int * AlsoCrossed: bool
    | RunOutNonStriker of Runs: int * AlsoCrossed: bool

    member _this.GetRuns =
        match _this with
        | DotBall -> 0
        | ScoreRuns runs -> runs
        | Four -> 4
        | Six -> 6
        | RunOutStriker (runs, _) -> runs
        | RunOutNonStriker (runs, _) -> runs
        | Bowled | HitWicket | Caught _ | LBW | Stumped _ -> 0

    member _this.HasChangedEnds =
        match _this with
        | DotBall | Four | Six | Bowled | HitWicket | LBW | Stumped _ -> false
        | ScoreRuns runs when (runs % 2 = 1) -> true
        | ScoreRuns _ -> false
        | Caught (_, crossed) -> crossed
        | RunOutStriker (runs, alsoCrossed) when (runs % 2 = 1) -> not alsoCrossed
        | RunOutNonStriker (runs, alsoCrossed) when (runs % 2 = 1) -> not alsoCrossed
        | RunOutStriker (_, alsoCrossed) -> alsoCrossed
        | RunOutNonStriker (_, alsoCrossed) -> alsoCrossed

    member _this.GetHowStrikerOut bowler =
        match _this with
        | Bowled -> Some (HowOut.Bowled bowler)
        | LBW -> Some (HowOut.LBW bowler)
        | HitWicket -> Some (HowOut.HitWicket bowler)
        | Caught (caughtBy, _) -> Some (HowOut.Caught (bowler, caughtBy))
        | Stumped (stumpedBy) -> Some (HowOut.Stumped (bowler, stumpedBy))
        | RunOutStriker (_, _) -> Some (HowOut.RunOut)
        | RunOutNonStriker (_, _) -> None
        | DotBall | ScoreRuns _ | Four | Six -> None

    member _this.GetHowNonStrikerOut =
        match _this with
        | RunOutNonStriker (_, _) -> Some (HowOut.RunOut)
        | _ -> None

    member _this.CountsAsBallFaced =
        match _this with
        | _ -> true

    member _this.IsAFour =
        match _this with
        | Four | ScoreRuns 4 | RunOutStriker (4, _) | RunOutNonStriker (4, _) -> true
        | _ -> false

    member _this.IsASix =
        match _this with
        | Six | ScoreRuns 6 | RunOutStriker (6, _) | RunOutNonStriker (6, _) -> true
        | _ -> false

    override _this.ToString() =
        match _this with
        | DotBall -> "dot ball"
        | ScoreRuns(runs) -> sprintf "run %i" runs
        | Four -> "four"
        | Six -> "six"
        | Bowled -> "out bowled"
        | HitWicket -> "out hit wicket"
        | LBW -> "out lbw"
        | Caught(Name name, crossed) -> (sprintf "out caught (%s)" name) + if crossed then ", batsmen crossed" else ""
        | Stumped(Name name) -> sprintf "out stumped (%s)" name
        | RunOutStriker(runs, alsoCrossed) -> (sprintf "striker run out (%i runs)" runs) + if alsoCrossed then ", batsmen crossed" else ""
        | RunOutNonStriker(runs, alsoCrossed) -> (sprintf "non-striker run out (%i runs)" runs) + if alsoCrossed then ", batsmen crossed" else ""
