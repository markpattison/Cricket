namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
[<TestFixture>]
type IndividualInningsScoreTests ()=

    let innings, bowler, _ = SampleData.sampleIndividualData

    static member TestData =
        [|
            DotBall, 0;
            ScoreRuns 1, 1;
            Four, 4;
            ScoreRuns 4, 4;
            Six, 6;
            Bowled, 0;
            LBW, 0;
            HitWicket, 0;
            Caught (SampleData.sampleFielder, false), 0;
            Stumped SampleData.sampleFielder, 0;
            RunOutStriker (0, false), 0;
            RunOutStriker (1, true), 1;
            RunOutNonStriker (0, false), 0;
            RunOutNonStriker (1, true), 1;        |]

    [<TestCaseSource("TestData")>]
    member _x.``batsman's score increases correctly`` testData =
        let ball, scoreIncreasesBy = testData
        (IndividualInnings.update bowler ball innings).Score |> should equal (innings.Score + scoreIncreasesBy)

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
[<TestFixture>]
type IndividualInningsHowOutTests ()=

    let innings, bowler, _ = SampleData.sampleIndividualData

    static member TestData =
        [|
            DotBall, None;
            ScoreRuns 1, None;
            Four, None;
            ScoreRuns 4, None;
            Six, None;
            Bowled, Some (HowOut.Bowled SampleData.sampleBowler);
            LBW, Some (HowOut.LBW SampleData.sampleBowler);
            HitWicket, Some (HowOut.HitWicket SampleData.sampleBowler);
            Caught (SampleData.sampleFielder, false), Some (HowOut.Caught (SampleData.sampleBowler, SampleData.sampleFielder));
            Caught (SampleData.sampleFielder, true), Some (HowOut.Caught (SampleData.sampleBowler, SampleData.sampleFielder));
            Stumped SampleData.sampleFielder, Some (HowOut.Stumped (SampleData.sampleBowler, SampleData.sampleFielder));
            RunOutStriker (0, false), Some (HowOut.RunOut);
            RunOutStriker (1, true), Some (HowOut.RunOut);
            RunOutNonStriker (0, false), None;
            RunOutNonStriker (1, true), None;       |]

    [<TestCaseSource("TestData")>]
    member _x.``batsman is out correctly`` (testData: BallOutcome * HowOut option) =
        let ball, howOut = testData
        (IndividualInnings.update bowler ball innings).HowOut |> should equal howOut

[<TestFixture>]
type IndividualInningsBallsFacedTests ()=

    let innings, bowler, _ = SampleData.sampleIndividualData

    static member TestData =
        [|
            DotBall, true;
            ScoreRuns 1, true;
            Four, true;
            ScoreRuns 4, true;
            Six, true;
            Bowled, true;
            LBW, true;
            HitWicket, true;
            Caught (SampleData.sampleFielder, false), true;
            Stumped SampleData.sampleFielder, true;
            RunOutStriker (0, true), true;
            RunOutStriker (1, false), true;
            RunOutNonStriker (0, true), true;
            RunOutNonStriker (1, false), true;        |]

    [<TestCaseSource("TestData")>]
    member _x.``batsman's balls faced update correctly`` testData =
        let ball, ballsFacedIncreased = testData
        (IndividualInnings.update bowler ball innings).BallsFaced |> should equal (innings.BallsFaced + if ballsFacedIncreased then 1 else 0)

[<TestFixture>]
type IndividualInningsFoursTests ()=

    let innings, bowler, _ = SampleData.sampleIndividualData

    static member TestData =
        [|
            DotBall, false;
            ScoreRuns 1, false;
            Four, true;
            ScoreRuns 4, true;
            Six, false;
            Bowled, false;
            LBW, false;
            HitWicket, false;
            Caught (SampleData.sampleFielder, false), false;
            Stumped SampleData.sampleFielder, false;
            RunOutStriker (3, true), false;
            RunOutStriker (3, false), false;
            RunOutStriker (4, true), true;
            RunOutStriker (4, false), true;
            RunOutNonStriker (3, true), false;
            RunOutNonStriker (3, false), false;
            RunOutNonStriker (4, true), true;
            RunOutNonStriker (4, false), true;        |]

    [<TestCaseSource("TestData")>]
    member _x.``batsman's fours update correctly`` testData =
        let ball, foursIncreased = testData
        (IndividualInnings.update bowler ball innings).Fours |> should equal (innings.Fours + if foursIncreased then 1 else 0)

[<TestFixture>]
type IndividualInningsSixesTests ()=

    let innings, bowler, _ = SampleData.sampleIndividualData

    static member TestData =
        [|
            DotBall, false;
            ScoreRuns 1, false;
            Four, false;
            Six, true;
            ScoreRuns 6, true;
            Bowled, false;
            LBW, false;
            HitWicket, false;
            Caught (SampleData.sampleFielder, false), false;
            Stumped SampleData.sampleFielder, false;
            RunOutStriker (5, true), false;
            RunOutStriker (5, false), false;
            RunOutStriker (6, true), true;
            RunOutStriker (6, false), true;
            RunOutNonStriker (5, true), false;
            RunOutNonStriker (5, false), false;
            RunOutNonStriker (6, true), true;
            RunOutNonStriker (6, false), true;
        |]

    [<TestCaseSource("TestData")>]
    member _x.``batsman's sixes update correctly`` testData =
        let ball, sixesIncreased = testData
        (IndividualInnings.update bowler ball innings).Sixes |> should equal (innings.Sixes + if sixesIncreased then 1 else 0)