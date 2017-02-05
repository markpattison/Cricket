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
        let updated = Innings.updateForBall ball innings
        if shouldChangeEnds then
            updated.IndexOfBatsmanAtEnd1 |> should equal innings.IndexOfBatsmanAtEnd2
            updated.IndexOfBatsmanAtEnd2 |> should equal innings.IndexOfBatsmanAtEnd1
        else
            updated.IndexOfBatsmanAtEnd1 |> should equal innings.IndexOfBatsmanAtEnd1
            updated.IndexOfBatsmanAtEnd2 |> should equal innings.IndexOfBatsmanAtEnd2

    [<TestCaseSource("TestDataStrikerOut")>]
    member _x.``non-striker changes ends correctly when striker is out`` testData =
        let ball, shouldChangeEnds = testData
        let updated = Innings.updateForBall ball innings
        if shouldChangeEnds then
            updated.IndexOfBatsmanAtEnd1 |> should equal innings.IndexOfBatsmanAtEnd2
        else
            updated.IndexOfBatsmanAtEnd2 |> should equal innings.IndexOfBatsmanAtEnd2

    [<TestCaseSource("TestDataNonStrikerOut")>]
    member _x.``striker change ends correctly when non-striker is out`` testData =
        let ball, shouldChangeEnds = testData
        let updated = Innings.updateForBall ball innings
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
        let updated = Innings.updateForBall ball innings
        if shouldChangeEnds then
            updated.IndexOfBatsmanAtEnd2 |> should equal None
        else
            updated.IndexOfBatsmanAtEnd1 |> should equal None

    [<TestCaseSource("TestDataNonStrikerOut")>]
    member _x.``non-striker is out correctly`` testData =
        let ball, shouldChangeEnds = testData
        let updated = Innings.updateForBall ball innings
        if shouldChangeEnds then
            updated.IndexOfBatsmanAtEnd1 |> should equal None
        else
            updated.IndexOfBatsmanAtEnd2 |> should equal None

[<TestFixture>]
type InningsIndividualsUpdatedCorrectly ()=

    let innings, _, _ = SampleData.sampleInningsData

    static member TestData =
        [|
            DotBall;
            ScoreRuns 1;
            ScoreRuns 2;
            Four;
            Six;
            Bowled;
            LBW;
            HitWicket;
            Caught (SampleData.sampleFielder, false);
            Caught (SampleData.sampleFielder, true);
            Stumped SampleData.sampleFielder;
            RunOutStriker (2, false);
            RunOutStriker (2, true);
            RunOutStriker (1, false);
            RunOutStriker (1, true);
            RunOutNonStriker (2, false);
            RunOutNonStriker (2, true);
            RunOutNonStriker (1, false);
            RunOutNonStriker (1, true);
        |]

    static member NonStrikerRunOutTestData =
        [|
            RunOutNonStriker (2, false);
            RunOutNonStriker (2, true);
            RunOutNonStriker (1, false);
            RunOutNonStriker (1, true);
        |]

    static member Ends = [| End1; End2 |]

    [<Test>]
    member _x.``striker's individual innings is updated correctly`` ([<ValueSource("TestData")>] testData) ([<ValueSource("Ends")>] currentEnd) =
        let ball = testData
        let testInnings = { innings with EndFacingNext = currentEnd }
        let updated = Innings.updateForBall ball testInnings
        let index = (if currentEnd = End1 then innings.IndexOfBatsmanAtEnd1 else innings.IndexOfBatsmanAtEnd2).Value
        let (player, testIndividualInnings) = List.item index innings.Individuals
        let expectedIndividualInnings = IndividualInnings.update SampleData.sampleBowler ball testIndividualInnings
        (List.item index updated.Individuals) |> should equal (player, expectedIndividualInnings)

    [<Test>]
    member _x.``non-striker's individual innings is updated correctly when he is run out`` ([<ValueSource("NonStrikerRunOutTestData")>] testData) ([<ValueSource("Ends")>] currentEnd) =
        let ball = testData
        let testInnings = { innings with EndFacingNext = currentEnd }
        let updated = Innings.updateForBall ball testInnings
        let nonStrikerIndex = (if currentEnd = End1 then innings.IndexOfBatsmanAtEnd2 else innings.IndexOfBatsmanAtEnd1).Value
        let (player, testIndividualInnings) = List.item nonStrikerIndex innings.Individuals
        let expectedIndividualInnings = IndividualInnings.updateNonStriker ball testIndividualInnings
        (List.item nonStrikerIndex updated.Individuals) |> should equal (player, expectedIndividualInnings)

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
        let updated = Innings.updateForBall ball testInnings
        if shouldIncrementBalls then
            updated.BallsSoFarThisOver |> should equal (ballsFaced + 1)
        else
            updated.BallsSoFarThisOver |> should equal ballsFaced

    [<TestCaseSource("TestData")>]
    member _x.``balls faced should be reset to zero at the end of an over`` testData =
        let ball, shouldIncrementBalls = testData
        let testInnings = { innings with BallsSoFarThisOver = 5 }
        let updated = Innings.updateForBall ball testInnings
        if shouldIncrementBalls then
            updated.BallsSoFarThisOver |> should equal 0
        else
            updated.BallsSoFarThisOver |> should equal 5 

    [<TestCaseSource("TestData")>]
    member _x.``overs completed should be incremented at the end of an over`` testData =
        let ball, shouldIncrementBalls = testData
        let testInnings = { innings with OversCompleted = 10; BallsSoFarThisOver = 5 }
        let updated = Innings.updateForBall ball testInnings
        if shouldIncrementBalls then
            updated.OversCompleted |> should equal 11
        else
            updated.OversCompleted |> should equal 10 

    [<Test>]
    member _x.``end facing next should not change before end of over`` ([<ValueSource("TestData")>] testData) ([<ValueSource("BallsFaced")>] ballsFaced) ([<ValueSource("Ends")>] currentEnd) =
        let ball, (_: bool) = testData
        let testInnings = { innings with BallsSoFarThisOver = ballsFaced; EndFacingNext = currentEnd }
        let updated = Innings.updateForBall ball testInnings
        updated.EndFacingNext |> should equal currentEnd

    [<Test>]
    member _x.``end facing next should change at the end of an over`` ([<ValueSource("TestData")>] testData) ([<ValueSource("Ends")>] currentEnd) =
        let ball, (_: bool) = testData
        let testInnings = { innings with BallsSoFarThisOver = 5; EndFacingNext = currentEnd }
        let updated = Innings.updateForBall ball testInnings
        updated.EndFacingNext |> should not' (equal currentEnd)

[<TestFixture>]
type SendInNewBatsmanTests ()=

    let innings, _, _ = SampleData.sampleInningsData

    let inningsWithNoBatsmanAtEnd1 = { innings with IndexOfBatsmanAtEnd1 = None }
    let inningsWithNoBatsmanAtEnd2 = { innings with IndexOfBatsmanAtEnd2 = None }
    let inningsWithNoBatsmen = { innings with IndexOfBatsmanAtEnd1 = None; IndexOfBatsmanAtEnd2 = None }

    let testBatsman = { Name = "sentInBatsman" }
    let testBatsman2 = { Name = "sentInBatsman2" }

    [<Test>]
    member _x.``cannot send in a new batsman to an already-started innings with no batsmen`` ()=
        (fun () -> (Innings.sendInBatsman testBatsman inningsWithNoBatsmen) |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    member _x.``first opener in is added correctly to innings`` ()=
        let updated = Innings.sendInBatsman testBatsman Innings.create
        updated.IndexOfBatsmanAtEnd1 |> should equal (Some 0)
        updated.Individuals |> should haveLength 1
        (updated.Individuals.Item 0) |> should equal (testBatsman, IndividualInnings.create)

    [<Test>]
    member _x.``second opener in is added correctly to innings`` ()=
        let updated1 = Innings.sendInBatsman testBatsman Innings.create
        let updated2 = Innings.sendInBatsman testBatsman2 updated1
        updated2.IndexOfBatsmanAtEnd2 |> should equal (Some 1)
        updated2.Individuals |> should haveLength 2
        (updated2.Individuals.Item 1) |> should equal (testBatsman2, IndividualInnings.create)

    [<Test>]
    member _x.``cannot send in a new batsman to an innings with two batsmen`` ()=
        (fun () -> (Innings.sendInBatsman testBatsman innings) |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    member _x.``new batsman added correctly to innings at end 1`` ()=
        let updated = Innings.sendInBatsman testBatsman inningsWithNoBatsmanAtEnd1
        updated.IndexOfBatsmanAtEnd1 |> should equal (Some 2)
        updated.Individuals.Item(2) |> should equal (testBatsman, IndividualInnings.create)

    [<Test>]
    member _x.``new batsman added correctly to innings at end 2`` ()=
        let updated = Innings.sendInBatsman testBatsman inningsWithNoBatsmanAtEnd2
        updated.IndexOfBatsmanAtEnd2 |> should equal (Some 2)
        updated.Individuals.Item(2) |> should equal (testBatsman, IndividualInnings.create)