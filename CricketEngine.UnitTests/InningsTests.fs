namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
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
        let updated = Innings.update (UpdateForBall ball) innings
        if shouldChangeEnds then
            updated.BatsmanAtEnd1 |> should equal innings.BatsmanAtEnd2
            updated.BatsmanAtEnd2 |> should equal innings.BatsmanAtEnd1
        else
            updated.BatsmanAtEnd1 |> should equal innings.BatsmanAtEnd1
            updated.BatsmanAtEnd2 |> should equal innings.BatsmanAtEnd2

    [<TestCaseSource("TestDataStrikerOut")>]
    member _x.``non-striker changes ends correctly when striker is out`` testData =
        let ball, shouldChangeEnds = testData
        let updated = Innings.update (UpdateForBall ball) innings
        if shouldChangeEnds then
            updated.BatsmanAtEnd1 |> should equal innings.BatsmanAtEnd2
        else
            updated.BatsmanAtEnd2 |> should equal innings.BatsmanAtEnd2

    [<TestCaseSource("TestDataNonStrikerOut")>]
    member _x.``striker change ends correctly when non-striker is out`` testData =
        let ball, shouldChangeEnds = testData
        let updated = Innings.update (UpdateForBall ball) innings
        if shouldChangeEnds then
            updated.BatsmanAtEnd2 |> should equal innings.BatsmanAtEnd1
        else
            updated.BatsmanAtEnd1 |> should equal innings.BatsmanAtEnd1

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
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
        let updated = Innings.update (UpdateForBall ball) innings
        if shouldChangeEnds then
            updated.BatsmanAtEnd2 |> should equal None
        else
            updated.BatsmanAtEnd1 |> should equal None

    [<TestCaseSource("TestDataNonStrikerOut")>]
    member _x.``non-striker is out correctly`` testData =
        let ball, shouldChangeEnds = testData
        let updated = Innings.update (UpdateForBall ball) innings
        if shouldChangeEnds then
            updated.BatsmanAtEnd1 |> should equal None
        else
            updated.BatsmanAtEnd2 |> should equal None

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
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
        let updated = Innings.update (UpdateForBall ball) testInnings
        let striker = (if currentEnd = End1 then innings.BatsmanAtEnd1 else innings.BatsmanAtEnd2).Value
        let testIndividualInnings = innings |> Innings.forPlayer striker
        let expectedIndividualInnings = IndividualInnings.update SampleData.sampleBowler ball testIndividualInnings

        updated |> Innings.forPlayer striker |> should equal expectedIndividualInnings

    [<Test>]
    member _x.``non-striker's individual innings is updated correctly when he is run out`` ([<ValueSource("NonStrikerRunOutTestData")>] testData) ([<ValueSource("Ends")>] currentEnd) =
        let ball = testData
        let testInnings = { innings with EndFacingNext = currentEnd }
        let updated = Innings.update (UpdateForBall ball) testInnings
        let nonStriker = (if currentEnd = End1 then innings.BatsmanAtEnd2 else innings.BatsmanAtEnd1).Value
        let testIndividualInnings = innings |> Innings.forPlayer nonStriker
        let expectedIndividualInnings = IndividualInnings.updateNonStriker ball testIndividualInnings

        updated |> Innings.forPlayer nonStriker |> should equal expectedIndividualInnings

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
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
        let testInnings = { innings with BallsThisOver = dotBalls ballsFaced }
        let updated = Innings.update (UpdateForBall ball) testInnings
        if shouldIncrementBalls then
            updated.BallsSoFarThisOver |> should equal (ballsFaced + 1)
        else
            updated.BallsSoFarThisOver |> should equal ballsFaced

    [<TestCaseSource("TestData")>]
    member _x.``balls faced should be reset to zero at the end of an over`` testData =
        let ball, shouldIncrementBalls = testData
        let testInnings = { innings with BallsThisOver = dotBalls 5 }
        let updated = Innings.update (UpdateForBall ball) testInnings
        if shouldIncrementBalls then
            updated.BallsSoFarThisOver |> should equal 0
        else
            updated.BallsSoFarThisOver |> should equal 5 

    [<TestCaseSource("TestData")>]
    member _x.``overs completed should be incremented at the end of an over`` testData =
        let ball, shouldIncrementBalls = testData
        let testInnings = { innings with OversCompleted = 10; BallsThisOver = dotBalls 5 }
        let updated = Innings.update (UpdateForBall ball) testInnings
        if shouldIncrementBalls then
            updated.OversCompleted |> should equal 11
        else
            updated.OversCompleted |> should equal 10 

    [<Test>]
    member _x.``end facing next should not change before end of over`` ([<ValueSource("TestData")>] testData) ([<ValueSource("BallsFaced")>] ballsFaced) ([<ValueSource("Ends")>] currentEnd) =
        let ball, (_: bool) = testData
        let testInnings = { innings with BallsThisOver = dotBalls ballsFaced; EndFacingNext = currentEnd }
        let updated = Innings.update (UpdateForBall ball) testInnings
        updated.EndFacingNext |> should equal currentEnd

    [<Test>]
    member _x.``end facing next should change at the end of an over`` ([<ValueSource("TestData")>] testData) ([<ValueSource("Ends")>] currentEnd) =
        let ball, (_: bool) = testData
        let testInnings = { innings with BallsThisOver = dotBalls 5; EndFacingNext = currentEnd }
        let updated = Innings.update (UpdateForBall ball) testInnings
        updated.EndFacingNext |> should not' (equal currentEnd)

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
[<TestFixture>]
type SendInNewBatsmanTests ()=

    let innings, _, _ = SampleData.sampleInningsData

    let inningsWithNoBatsmanAtEnd1 = { innings with BatsmanAtEnd1 = None }
    let inningsWithNoBatsmanAtEnd2 = { innings with BatsmanAtEnd2 = None }
    let inningsWithNoBatsmen = { innings with BatsmanAtEnd1 = None; BatsmanAtEnd2 = None }

    let testBatsman = { Name = "sentInBatsman" }
    let testBatsman2 = { Name = "sentInBatsman2" }

    [<Test>]
    member _x.``cannot send in a new batsman to an already-started innings with no batsmen`` ()=
        (fun () -> (Innings.update (SendInBatsman testBatsman) inningsWithNoBatsmen) |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    member _x.``first opener in is added correctly to innings`` ()=
        let updated = Innings.update (SendInBatsman testBatsman) Innings.create

        updated.BatsmanAtEnd1 |> should equal (Some testBatsman)
        updated.Batsmen |> should haveLength 1
        (updated.Batsmen.Item 0) |> should equal (testBatsman, IndividualInnings.create)

    [<Test>]
    member _x.``second opener in is added correctly to innings`` ()=
        let updated1 = Innings.update (SendInBatsman testBatsman) Innings.create
        let updated2 = Innings.update (SendInBatsman testBatsman2) updated1

        updated2.BatsmanAtEnd2 |> should equal (Some testBatsman2)
        updated2.Batsmen |> should haveLength 2
        (updated2.Batsmen.Item 1) |> should equal (testBatsman2, IndividualInnings.create)

    [<Test>]
    member _x.``cannot send in a new batsman to an innings with two batsmen`` ()=
        (fun () -> (Innings.update (SendInBatsman testBatsman) innings) |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    member _x.``new batsman added correctly to innings at end 1`` ()=
        let updated = Innings.update (SendInBatsman testBatsman) inningsWithNoBatsmanAtEnd1

        updated.BatsmanAtEnd1 |> should equal (Some testBatsman)
        updated.Batsmen.Item(2) |> should equal (testBatsman, IndividualInnings.create)

    [<Test>]
    member _x.``new batsman added correctly to innings at end 2`` ()=
        let updated = Innings.update (SendInBatsman testBatsman) inningsWithNoBatsmanAtEnd2

        updated.BatsmanAtEnd2 |> should equal (Some testBatsman)
        updated.Batsmen.Item(2) |> should equal (testBatsman, IndividualInnings.create)

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
[<TestFixture>]
type DeclarationTests ()=

    [<Test>]
    member _x.``innings declared`` ()=
        let innings = 50 %/ 5
        let declared = innings |> Innings.update Declare

        declared.GetRuns |> should equal 50
        declared.GetWickets |> should equal 5
        declared.IsDeclared |> should be True
        declared.IsCompleted |> should be True

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
[<TestFixture>]
type BowlingAnalysesUpdatedCorrectly ()=

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

    static member Ends = [| End1; End2 |]

    [<Test>]
    member _x.``bowler's analysis is updated correctly`` ([<ValueSource("TestData")>] testData) ([<ValueSource("Ends")>] currentEnd) =
        let ball = testData
        let testInnings = { innings with EndFacingNext = currentEnd }
        let updated = Innings.update (UpdateForBall ball) testInnings
        let bowler = (if currentEnd = End1 then innings.BowlerToEnd1 else innings.BowlerToEnd2).Value
        let testBowlingAnalysis = innings |> Innings.forBowler bowler
        let expectedBowlingAnalysis = BowlingAnalysis.update ball testBowlingAnalysis

        updated |> Innings.forBowler bowler |> should equal expectedBowlingAnalysis

    [<Test>]
    member _x.``new bowling analysis is created correctly`` ([<ValueSource("TestData")>] testData) =
        let ball = testData
        let newBowler = { Name = "new bowler" }
        let testInnings = { innings with EndFacingNext = End1 } |> Innings.update (SendInBowler newBowler)
        let updated = Innings.update (UpdateForBall ball) testInnings
        let expectedBowlingAnalysis = BowlingAnalysis.update ball BowlingAnalysis.create

        updated |> Innings.forBowler newBowler |> should equal expectedBowlingAnalysis

    [<Test>]
    member _x.``bowling analysis is correct after a maiden over`` ()=
        let newBowler = { Name = "new bowler" }
        let testInnings = { innings with EndFacingNext = End1 } |> Innings.update (SendInBowler newBowler)
        let updated =
            testInnings
            |> Innings.update (UpdateForBall DotBall)
            |> Innings.update (UpdateForBall DotBall)
            |> Innings.update (UpdateForBall DotBall)
            |> Innings.update (UpdateForBall DotBall)
            |> Innings.update (UpdateForBall DotBall)
            |> Innings.update (UpdateForBall DotBall)

        let expectedBowlingAnalysis = { Balls = 6; Maidens = 1; RunsConceded = 0; Wickets = 0 }

        updated |> Innings.forBowler newBowler |> should equal expectedBowlingAnalysis

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
[<TestFixture>]
type SendInNewBowlerTests ()=

    let innings, _, _ = SampleData.sampleInningsData

    let newBowler = { Name = "new bowler" }

    [<Test>]
    member _x.``new bowler added correctly for new over to end 1`` ()=
        let testInnings = { innings with BallsThisOver = []; EndFacingNext = End1 }
        let updated = testInnings |> Innings.update (SendInBowler newBowler)

        updated.BowlerToEnd1 |> should equal (Some newBowler)

    [<Test>]
    member _x.``new bowler added correctly for new over to end 2`` ()=
        let testInnings = { innings with BallsThisOver = []; EndFacingNext = End2 }
        let updated = testInnings |> Innings.update (SendInBowler newBowler)

        updated.BowlerToEnd2 |> should equal (Some newBowler)

    [<Test>]
    member _x.``new bowler cannot be added during over to end 1`` ()=
        let testInnings = { innings with BallsThisOver = [ DotBall ]; EndFacingNext = End1 }
        (fun () -> (testInnings |> Innings.update (SendInBowler newBowler)) |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    member _x.``new bowler cannot be added during over to end 2`` ()=
        let testInnings = { innings with BallsThisOver = [ DotBall; DotBall; DotBall; DotBall; DotBall ]; EndFacingNext = End2 }
        (fun () -> (testInnings |> Innings.update (SendInBowler newBowler)) |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    member _x.``same bowler cannot bowl to end 1 then end 2`` ()=
        let testInnings = { innings with BallsThisOver = []; EndFacingNext = End2; BowlerToEnd1 = Some newBowler; OversCompleted = 1 }
        (fun () -> (testInnings |> Innings.update (SendInBowler newBowler)) |> ignore) |> should throw typeof<System.Exception>

    [<Test>]
    member _x.``same bowler cannot bowl to end 2 then end 1`` ()=
        let testInnings = { innings with BallsThisOver = []; EndFacingNext = End1; BowlerToEnd2 = Some newBowler; OversCompleted = 1 }
        (fun () -> (testInnings |> Innings.update (SendInBowler newBowler)) |> ignore) |> should throw typeof<System.Exception>
