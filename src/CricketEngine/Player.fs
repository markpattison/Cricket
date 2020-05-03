namespace Cricket.CricketEngine

[<CustomEquality; CustomComparison>]
type Player =
    {
        ID: int
        Name: string
    }

    override _this.Equals(obj2) =
        match obj2 with
        | :? Player as player2 -> (_this.ID = player2.ID)
        | _ -> false
    
    override _this.GetHashCode() = _this.ID

    interface System.IComparable with
        member _this.CompareTo otherObj =
            match otherObj with
            | :? Player as otherPlayer -> _this.ID - otherPlayer.ID
            | _ -> 0
