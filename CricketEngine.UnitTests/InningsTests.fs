namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine

[<TestFixture>]
type InningsChangeEndsTests ()=

    let innings, _, _ = SampleData.sampleInningsData

    static member TestDataNoWicket =
        [|
            DotBall, false;
            ScoreRuns 1, true;
            ScoreRuns 2, false;
            Four, false;
            Six, false;
        |]

    static member TestDataStrikerOut =
        [|
            Bowled, false;
            LBW, false;
            HitWicket, false;
            Caught (SampleData.sampleFielder, false), false;
            Caught (SampleData.sampleFielder, true), true;
            Stumped SampleData.sampleFielder, false;
            RunOutStriker (2, false), false;
            RunOutStriker (2, true), true;
            RunOutStriker (1, false), true;
            RunOutStriker (1, true), false;
        |]

    static member TestDataNonStrikerOut =
        [|
            RunOutNonStriker (2, false), false;
            RunOutNonStriker (2, true), true;
            RunOutNonStriker (1, false), true;
            RunOutNonStriker (1, true), false;
        |]

    [<TestCaseSource("TestDataNoWicket")>]
    member _x.``batsmen change ends correctly when no wicket falls`` testData =
        let ball, shouldChangeEnds = testData
        let updated = UpdateInningsWithBall innings ball
        if shouldChangeEnds then
            updated.IndexOfBatsmanAtEnd1 |> should equal innings.IndexOfBatsmanAtEnd2
            updated.IndexOfBatsmanAtEnd2 |> should equal innings.IndexOfBatsmanAtEnd1
        else
            updated.IndexOfBatsmanAtEnd1 |> should equal innings.IndexOfBatsmanAtEnd1
            updated.IndexOfBatsmanAtEnd2 |> should equal innings.IndexOfBatsmanAtEnd2

    [<TestCaseSource("TestDataStrikerOut")>]
    member _x.``non-striker changes ends correctly when striker is out`` testData =
        let ball, shouldChangeEnds = testData
        let updated = UpdateInningsWithBall innings ball
        if shouldChangeEnds then
            updated.IndexOfBatsmanAtEnd1 |> should equal innings.IndexOfBatsmanAtEnd2
        else
            updated.IndexOfBatsmanAtEnd2 |> should equal innings.IndexOfBatsmanAtEnd2

    [<TestCaseSource("TestDataNonStrikerOut")>]
    member _x.``striker change ends correctly when non-striker is out`` testData =
        let ball, shouldChangeEnds = testData
        let updated = UpdateInningsWithBall innings ball
        if shouldChangeEnds then
            updated.IndexOfBatsmanAtEnd2 |> should equal innings.IndexOfBatsmanAtEnd1
        else
            updated.IndexOfBatsmanAtEnd1 |> should equal innings.IndexOfBatsmanAtEnd1

[<TestFixture>]
type BatsmanOutTests ()=

    let innings, _, _ = SampleData.sampleInningsData

    static member TestDataStrikerOut =
        [|
            Bowled, false;
            LBW, false;
            HitWicket, false;
            Caught (SampleData.sampleFielder, false), false;
            Caught (SampleData.sampleFielder, true), true;
            Stumped SampleData.sampleFielder, false;
            RunOutStriker (2, false), false;
            RunOutStriker (2, true), true;
            RunOutStriker (1, false), true;
            RunOutStriker (1, true), false;
        |]

    static member TestDataNonStrikerOut =
        [|
            RunOutNonStriker (2, false), false;
            RunOutNonStriker (2, true), true;
            RunOutNonStriker (1, false), true;
            RunOutNonStriker (1, true), false;
        |]

    [<TestCaseSource("TestDataStrikerOut")>]
    member _x.``striker is out correctly`` testData =
        let ball, shouldChangeEnds = testData
        let updated = UpdateInningsWithBall innings ball
        if shouldChangeEnds then
            updated.IndexOfBatsmanAtEnd2 |> should equal None
        else
            updated.IndexOfBatsmanAtEnd1 |> should equal None

    [<TestCaseSource("TestDataNonStrikerOut")>]
    member _x.``non-striker is out correctly`` testData =
        let ball, shouldChangeEnds = testData
        let updated = UpdateInningsWithBall innings ball
        if shouldChangeEnds then
            updated.IndexOfBatsmanAtEnd1 |> should equal None
        else
            updated.IndexOfBatsmanAtEnd2 |> should equal None

[<TestFixture>]
type InningsBallsIncrementedTests ()=

    let innings, _, _ = SampleData.sampleInningsData

    static member TestData =
        [|
            DotBall, true;
            ScoreRuns 1, true;
            ScoreRuns 2, true;
            Four, true;
            Six, true;
            Bowled, true;
            LBW, true;
            HitWicket, true;
            Caught (SampleData.sampleFielder, false), true;
            Caught (SampleData.sampleFielder, true), true;
            Stumped SampleData.sampleFielder, true;
            RunOutStriker (2, false), true;
            RunOutStriker (2, true), true;
            RunOutStriker (1, false), true;
            RunOutStriker (1, true), true;
            RunOutNonStriker (2, false), true;
            RunOutNonStriker (2, true), true;
            RunOutNonStriker (1, false), true;
            RunOutNonStriker (1, true), true;
        |]

    static member BallsFaced = [| 0; 1; 2; 3; 4 |]
    static member Ends = [| End1; End2 |]

    [<Test>]
    member _x.``balls faced this over should increment correctly before end of over`` ([<ValueSource("TestData")>] testData) ([<ValueSource("BallsFaced")>] ballsFaced) =
        let ball, shouldIncrementBalls = testData
        let testInnings = { innings with BallsSoFarThisOver = ballsFaced }
        let updated = UpdateInningsWithBall testInnings ball
        if shouldIncrementBalls then
            updated.BallsSoFarThisOver |> should equal (ballsFaced + 1)
        else
            updated.BallsSoFarThisOver |> should equal ballsFaced

    [<TestCaseSource("TestData")>]
    member _x.``balls faced should be reset to zero at the end of an over`` testData =
        let ball, shouldIncrementBalls = testData
        let testInnings = { innings with BallsSoFarThisOver = 5 }
        let updated = UpdateInningsWithBall testInnings ball
        if shouldIncrementBalls then
            updated.BallsSoFarThisOver |> should equal 0
        else
            updated.BallsSoFarThisOver |> should equal 5 

    [<TestCaseSource("TestData")>]
    member _x.``overs completed should be incremented at the end of an over`` testData =
        let ball, shouldIncrementBalls = testData
        let testInnings = { innings with OversCompleted = 10; BallsSoFarThisOver = 5 }
        let updated = UpdateInningsWithBall testInnings ball
        if shouldIncrementBalls then
            updated.OversCompleted |> should equal 11
        else
            updated.OversCompleted |> should equal 10 

    [<Test>]
    member _x.``end facing next should not change before end of over`` ([<ValueSource("TestData")>] testData) ([<ValueSource("BallsFaced")>] ballsFaced) ([<ValueSource("Ends")>] currentEnd) =
        let ball, (_: bool) = testData
        let testInnings = { innings with BallsSoFarThisOver = ballsFaced; EndFacingNext = currentEnd }
        let updated = UpdateInningsWithBall testInnings ball
        updated.EndFacingNext |> should equal currentEnd

    [<Test>]
    member _x.``end facing next should change at the end of an over`` ([<ValueSource("TestData")>] testData) ([<ValueSource("Ends")>] currentEnd) =
        let ball, (_: bool) = testData
        let testInnings = { innings with BallsSoFarThisOver = 5; EndFacingNext = currentEnd }
        let updated = UpdateInningsWithBall testInnings ball
        updated.EndFacingNext |> should not' (equal currentEnd)

[<TestFixture>]
type SendInNewBatsmanTests ()=

    let innings, _, _ = SampleData.sampleInningsData

    let inningsWithNoBatsmanAtEnd1 = { innings with IndexOfBatsmanAtEnd1 = None }
    let inningsWithNoBatsmanAtEnd2 = { innings with IndexOfBatsmanAtEnd2 = None }
    let inningsWithNoBatsmen = { innings with IndexOfBatsmanAtEnd1 = None; IndexOfBatsmanAtEnd2 = None }

    let testBatsman = Name "sentInBatsman"

    [<Test>]
    member _x.``cannot send in a new batsman to an innings with no batsmen`` ()=
        (fun () -> (SendInNewBatsman inningsWithNoBatsmen testBatsman) |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    member _x.``cannot send in a new batsman to an innings with two batsmen`` ()=
        (fun () -> (SendInNewBatsman innings testBatsman) |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    member _x.``new batsman added correctly to innings at end 1`` ()=
        let updated = SendInNewBatsman inningsWithNoBatsmanAtEnd1 testBatsman
        (updated.IndexOfBatsmanAtEnd1) |> should equal (Some 2)
        updated.Individuals.Item(2) |> should equal (testBatsman, NewIndividualInnings)

    [<Test>]
    member _x.``new batsman added correctly to innings at end 2`` ()=
        let updated = SendInNewBatsman inningsWithNoBatsmanAtEnd2 testBatsman
        (updated.IndexOfBatsmanAtEnd2) |> should equal (Some 2)
        updated.Individuals.Item(2) |> should equal (testBatsman, NewIndividualInnings)