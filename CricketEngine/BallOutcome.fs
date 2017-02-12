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

    override _this.ToString() =
        match _this with
        | DotBall -> "dot ball"
        | ScoreRuns(runs) -> sprintf "run %i" runs
        | Four -> "four"
        | Six -> "six"
        | Bowled -> "out bowled"
        | HitWicket -> "out hit wicket"
        | LBW -> "out lbw"
        | Caught(catcher, crossed) -> (sprintf "out caught (%s)" catcher.Name) + if crossed then ", batsmen crossed" else ""
        | Stumped(stumper) -> sprintf "out stumped (%s)" stumper.Name
        | RunOutStriker(runs, alsoCrossed) -> (sprintf "striker run out (%i runs)" runs) + if alsoCrossed then ", batsmen crossed" else ""
        | RunOutNonStriker(runs, alsoCrossed) -> (sprintf "non-striker run out (%i runs)" runs) + if alsoCrossed then ", batsmen crossed" else ""


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix )>]
module BallOutcome =
    let getRuns ball =
        match ball with
        | DotBall -> 0
        | ScoreRuns runs -> runs
        | Four -> 4
        | Six -> 6
        | RunOutStriker (runs, _) -> runs
        | RunOutNonStriker (runs, _) -> runs
        | Bowled | HitWicket | Caught _ | LBW | Stumped _ -> 0

    let changedEnds ball =
        match ball with
        | DotBall | Four | Six | Bowled | HitWicket | LBW | Stumped _ -> false
        | ScoreRuns runs when (runs % 2 = 1) -> true
        | ScoreRuns _ -> false
        | Caught (_, crossed) -> crossed
        | RunOutStriker (runs, alsoCrossed) when (runs % 2 = 1) -> not alsoCrossed
        | RunOutNonStriker (runs, alsoCrossed) when (runs % 2 = 1) -> not alsoCrossed
        | RunOutStriker (_, alsoCrossed) -> alsoCrossed
        | RunOutNonStriker (_, alsoCrossed) -> alsoCrossed

    let howStrikerOut bowler ball =
        match ball with
        | Bowled -> Some (HowOut.Bowled bowler)
        | LBW -> Some (HowOut.LBW bowler)
        | HitWicket -> Some (HowOut.HitWicket bowler)
        | Caught (caughtBy, _) -> Some (HowOut.Caught (bowler, caughtBy))
        | Stumped (stumpedBy) -> Some (HowOut.Stumped (bowler, stumpedBy))
        | RunOutStriker _ -> Some (HowOut.RunOut)
        | RunOutNonStriker _ -> None
        | DotBall | ScoreRuns _ | Four | Six -> None

    let howNonStrikerOut ball =
        match ball with
        | RunOutNonStriker _ -> Some (HowOut.RunOut)
        | _ -> None

    let countsAsBallFaced ball =
        match ball with
        | _ -> true

    let isAFour ball =
        match ball with
        | Four | ScoreRuns 4 | RunOutStriker (4, _) | RunOutNonStriker (4, _) -> true
        | _ -> false

    let isASix ball =
        match ball with
        | Six | ScoreRuns 6 | RunOutStriker (6, _) | RunOutNonStriker (6, _) -> true
        | _ -> false

    let isWicketForBowler ball =
        match ball with
        | Bowled | HitWicket | LBW | Caught _ | Stumped _ -> true
        | _ -> false
