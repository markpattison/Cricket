namespace Cricket.CricketEngine

type Player =
    | Name of string

type HowOut =
    | Bowled of BowledBy: Player
    | TimedOut
    | Caught of BowledBy: Player * CaughtBy: Player
    | HandledTheBall
    | HitTheBallTwice
    | HitWicket of BowledBy: Player
    | LBW of BowledBy: Player
    | ObstructingTheField
    | RunOut
    | Stumped of BowledBy: Player * StumpedBy: Player

    override _this.ToString() =
        match _this with
        | Bowled (Name bowler) -> sprintf "b %s" bowler
        | TimedOut -> "timed out"
        | Caught(Name bowler, Name catcher) -> sprintf "c %s b %s" catcher bowler
        | HandledTheBall -> "out handled the ball"
        | HitTheBallTwice -> "out hit the ball twice"
        | HitWicket(Name bowler) -> sprintf "hit wicket b %s" bowler
        | LBW(Name bowler) -> sprintf "lbw b %s" bowler
        | ObstructingTheField -> "out obstructing the field"
        | RunOut -> "run out"
        | Stumped(Name bowler, Name stumper) -> sprintf "st %s b %s" stumper bowler


