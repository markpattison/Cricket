namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

module ``BallOutcome tests`` =

    let fielder = Name "testFielder"

    [<Test>]
    let ``batsmen don't change ends after a dot ball`` ()=
        (DotBall.HasChangedEnds) |> should be False

    [<Test>]
    let ``batsmen change ends after scoring one`` ()=
        (ScoreRuns 1).HasChangedEnds |> should be True

    [<Test>]
    let ``batsmen don't change ends after scoring two`` ()=
        (ScoreRuns 2).HasChangedEnds |> should be False

    [<Test>]
    let ``batsmen don't change ends after a four`` ()=
        Four.HasChangedEnds |> should be False

    [<Test>]
    let ``batsmen don't change ends after a six`` ()=
        Six.HasChangedEnds |> should be False

    [<Test>]
    let ``batsmen don't change ends after being bowled`` ()=
        Bowled.HasChangedEnds |> should be False

    [<Test>]
    let ``batsmen don't change endsafter beign caught without crossing`` ()=
        (Caught (fielder, false)).HasChangedEnds |> should be False

    [<Test>]
    let ``batsmen change ends after being caught after crossing`` ()=
        (Caught (fielder, true)).HasChangedEnds |> should be True

    [<Test>]
    let ``batsmen don't change ends after being run out after scoring two without then crossing`` ()=
        (RunOut (2, false)).HasChangedEnds |> should be False

    [<Test>]
    let ``batsmen change ends after being run out after scoring two then crossing`` ()=
        (RunOut (2, true)).HasChangedEnds |> should be True

    [<Test>]
    let ``batsmen change ends after being run out after scoring one without then crossing`` ()=
        (RunOut (1, false)).HasChangedEnds |> should be True

    [<Test>]
    let ``batsmen don't change ends after being run out after scoring one then crossing`` ()=
        (RunOut (1, true)).HasChangedEnds |> should be False
