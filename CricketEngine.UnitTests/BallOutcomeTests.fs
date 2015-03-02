namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

module ``BallOutcome tests`` =

    let fielder = Name "testFielder"

    [<Test>]
    let ``batsmen don't change ends after a dot ball`` ()=
        (HasChangedEnds DotBall) |> should be False

    [<Test>]
    let ``batsmen change ends after scoring one`` ()=
        (HasChangedEnds (ScoreRuns 1)) |> should be True

    [<Test>]
    let ``batsmen don't change ends after scoring two`` ()=
        (HasChangedEnds (ScoreRuns 2)) |> should be False

    [<Test>]
    let ``batsmen don't change ends after a four`` ()=
        (HasChangedEnds Four) |> should be False

    [<Test>]
    let ``batsmen don't change ends after a six`` ()=
        (HasChangedEnds Six) |> should be False

    [<Test>]
    let ``batsmen don't change ends after being bowled`` ()=
        (HasChangedEnds Bowled) |> should be False

    [<Test>]
    let ``batsmen don't change endsafter beign caught without crossing`` ()=
        (HasChangedEnds (Caught (fielder, false))) |> should be False

    [<Test>]
    let ``batsmen change ends after being caught after crossing`` ()=
        (HasChangedEnds (Caught (fielder, true))) |> should be True
