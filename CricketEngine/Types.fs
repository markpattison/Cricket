namespace Cricket.CricketEngine

[<CustomEquality; NoComparison>]
type Player =
    { ID: int; Name: string }

    override _this.Equals(obj2) =
        match obj2 with
        | :? Player as player2 -> (_this.ID = player2.ID)
        | _ -> false
    
    override _this.GetHashCode() = _this.ID

type Team =
    { Name: string; Players: Player [] }

type HowOut =
    | OutBowled of BowledBy: Player
    | TimedOut
    | OutCaught of BowledBy: Player * CaughtBy: Player
    | OutHandledTheBall
    | OutHitTheBallTwice
    | OutHitWicket of BowledBy: Player
    | OutLBW of BowledBy: Player
    | ObstructingTheField
    | RunOut
    | OutStumped of BowledBy: Player * StumpedBy: Player

    override _this.ToString() =
        match _this with
        | OutBowled (bowler) -> sprintf "b %s" bowler.Name
        | TimedOut -> "timed out"
        | OutCaught(bowler, catcher) -> sprintf "c %s b %s" catcher.Name bowler.Name
        | OutHandledTheBall -> "out handled the ball"
        | OutHitTheBallTwice -> "out hit the ball twice"
        | OutHitWicket(bowler) -> sprintf "hit wicket b %s" bowler.Name
        | OutLBW(bowler) -> sprintf "lbw b %s" bowler.Name
        | ObstructingTheField -> "out obstructing the field"
        | RunOut -> "run out"
        | OutStumped(bowler, stumper) -> sprintf "st %s b %s" stumper.Name bowler.Name

type TeamChoice = TeamA | TeamB

type InningsNumber = FirstInnings | SecondInnings