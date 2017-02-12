namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
[<TestFixture>]
type BowlingAnalysisBallsTests ()=

    let bowling = SampleData.sampleBowlingAnalysis

    static member TestData =
        [|
            DotBall;
            ScoreRuns 1;
            Four;
            ScoreRuns 4;
            Six;
            Bowled;
            LBW;
            HitWicket;
            Caught (SampleData.sampleFielder, false);
            Stumped SampleData.sampleFielder;
            RunOutStriker (0, false);
            RunOutStriker (1, true);
            RunOutNonStriker (0, false);
            RunOutNonStriker (1, true);
        |]

    [<TestCaseSource("TestData")>]
    member _x.``balls bowled increases correctly`` testData =
        let ball = testData
        (BowlingAnalysis.update ball bowling).Balls |> should equal (bowling.Balls + 1)

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
[<TestFixture>]
type BowlingAnalysisRunsConcededTests ()=

    let bowling = SampleData.sampleBowlingAnalysis

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
            RunOutNonStriker (1, true), 1;
        |]

    [<TestCaseSource("TestData")>]
    member _x.``runs conceded increases correctly`` testData =
        let ball, runsFromBall = testData
        (BowlingAnalysis.update ball bowling).RunsConceded |> should equal (bowling.RunsConceded + runsFromBall)

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
[<TestFixture>]
type BowlingAnalysisWicketsTests ()=

    let bowling = SampleData.sampleBowlingAnalysis

    static member TestData =
        [|
            DotBall, 0;
            ScoreRuns 1, 0;
            Four, 0;
            ScoreRuns 4, 0;
            Six, 0;
            Bowled, 1;
            LBW, 1;
            HitWicket, 1;
            Caught (SampleData.sampleFielder, false), 1;
            Stumped SampleData.sampleFielder, 1;
            RunOutStriker (0, false), 0;
            RunOutStriker (1, true), 0;
            RunOutNonStriker (0, false), 0;
            RunOutNonStriker (1, true), 0;
        |]

    [<TestCaseSource("TestData")>]
    member _x.``wickets increases correctly`` testData =
        let ball, wicketForBowler = testData
        (BowlingAnalysis.update ball bowling).Wickets |> should equal (bowling.Wickets + wicketForBowler)

[<System.Diagnostics.CodeAnalysis.SuppressMessage("NameConventions", "MemberNamesMustBePascalCase")>]
[<TestFixture>]
type BowlingAnalysisMaidensTests ()=
    
    let bowling = SampleData.sampleBowlingAnalysis

    static member TestData =
        [|
            ScoreRuns 1;
            Four;
            ScoreRuns 4;
            Six;
            RunOutStriker (1, false);
            RunOutNonStriker (1, false);
            RunOutStriker (1, true);
            RunOutNonStriker (1, true);
        |]

    [<TestCaseSource("TestData")>]
    member _x.``maidens count does not increase when runs scored in over`` testData =
        let ball = testData
        let over =
            [ DotBall; DotBall; DotBall; DotBall; DotBall; ball ]

        (BowlingAnalysis.updateAfterOver over bowling).Maidens |> should equal bowling.Maidens

    [<Test>]
    member _x.``maidens count increases when no runs scored in over`` ()=
        let over1 =
            [
                DotBall;
                Bowled;
                LBW;
                HitWicket;
                Caught (SampleData.sampleFielder, false);
                Stumped SampleData.sampleFielder;
            ]
        let over2 =
            [
                DotBall;
                Caught (SampleData.sampleFielder, true);
                RunOutStriker (0, false);
                RunOutNonStriker (0, false);
                RunOutStriker (0, true);
                RunOutNonStriker (0, true);
            ]

        (BowlingAnalysis.updateAfterOver over1 bowling).Maidens |> should equal (bowling.Maidens + 1)
        (BowlingAnalysis.updateAfterOver over2 bowling).Maidens |> should equal (bowling.Maidens + 1)
