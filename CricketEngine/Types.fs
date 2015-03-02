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

