namespace Cricket.CricketEngine

type Player =
    { Name: string }

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
        | Bowled (bowler) -> sprintf "b %s" bowler.Name
        | TimedOut -> "timed out"
        | Caught(bowler, catcher) -> sprintf "c %s b %s" catcher.Name bowler.Name
        | HandledTheBall -> "out handled the ball"
        | HitTheBallTwice -> "out hit the ball twice"
        | HitWicket(bowler) -> sprintf "hit wicket b %s" bowler.Name
        | LBW(bowler) -> sprintf "lbw b %s" bowler.Name
        | ObstructingTheField -> "out obstructing the field"
        | RunOut -> "run out"
        | Stumped(bowler, stumper) -> sprintf "st %s b %s" stumper.Name bowler.Name

type Team = TeamA | TeamB
