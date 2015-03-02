namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine

[<TestFixture>]
type InningsChangeEndsTests ()=

    let innings, _, _ = SampleData.sampleInningsData

    static member TestData =
        [|
            DotBall, false;
            ScoreRuns 1, true;
            ScoreRuns 2, false;
            Four, false;
            Six, false;
            Bowled, false;
            LBW, false;
            HitWicket, false;
            Caught (SampleData.sampleFielder, false), false;
            Caught (SampleData.sampleFielder, true), true;
            Stumped SampleData.sampleFielder, false;
            RunOut (2, false), false;
            RunOut (2, true), true;
            RunOut (1, false), true;
            RunOut (1, true), false;
        |]

    [<TestCaseSource("TestData")>]
    member _x.``batsmen change ends correctly`` testData =
        let ball, shouldChangeEnds = testData
        let updated = UpdateInningsWithBall innings ball
        if shouldChangeEnds then
            updated.IndexOfBatsmanAtEnd1 |> should equal innings.IndexOfBatsmanAtEnd2
            updated.IndexOfBatsmanAtEnd2 |> should equal innings.IndexOfBatsmanAtEnd1
        else
            updated.IndexOfBatsmanAtEnd1 |> should equal innings.IndexOfBatsmanAtEnd1
            updated.IndexOfBatsmanAtEnd2 |> should equal innings.IndexOfBatsmanAtEnd2
