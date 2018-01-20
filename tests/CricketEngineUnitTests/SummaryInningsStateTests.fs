﻿namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine
open TestHelpers

module ``SummaryInningsState tests`` =

    let summary innings = Innings.summaryState innings

    [<Test>]
    let ``innings declared`` ()=
        let innings = 100 %/% 5
        (summary innings) |> should equal Completed

    [<Test>]
    let ``innings all out`` ()=
        let innings = 100 %/ 10
        (summary innings) |> should equal Completed

    [<Test>]
    let ``new innings`` ()=
        let innings = Innings.create
        (summary innings) |> should equal (BatsmanRequired 1)

    [<Test>]
    let ``new innings, one batsman in`` ()=
        let innings =
            Innings.create
            |> Innings.update (SendInBatsman SampleData.sampleBatsman1) None
        (summary innings) |> should equal (BatsmanRequired 2)

    [<Test>]
    let ``new innings, both batsman in`` ()=
        let innings =
            Innings.create
            |> Innings.update (SendInBatsman SampleData.sampleBatsman1) None
            |> Innings.update (SendInBatsman SampleData.sampleBatsman2) None
        (summary innings) |> should equal (BowlerRequiredTo End1)

    [<Test>]
    let ``new innings, both batsman in and bowler ready`` ()=
        let innings =
            Innings.create
            |> Innings.update (SendInBatsman SampleData.sampleBatsman1) None
            |> Innings.update (SendInBatsman SampleData.sampleBatsman2) None
            |> Innings.update (SendInBowler SampleData.sampleBowler1) None
        (summary innings) |> should equal EndOver

    [<Test>]
    let ``new innings, after one over`` ()=
        let innings =
            Innings.create
            |> Innings.update (SendInBatsman SampleData.sampleBatsman1) None
            |> Innings.update (SendInBatsman SampleData.sampleBatsman2) None
            |> Innings.update (SendInBowler SampleData.sampleBowler1) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
        (summary innings) |> should equal (BowlerRequiredTo End2)

    [<Test>]
    let ``batsman out`` ()=
        let innings =
            Innings.create
            |> Innings.update (SendInBatsman SampleData.sampleBatsman1) None
            |> Innings.update (SendInBatsman SampleData.sampleBatsman2) None
            |> Innings.update (SendInBowler SampleData.sampleBowler1) None
            |> Innings.update (UpdateForBall LBW) None
        (summary innings) |> should equal (BatsmanRequired 3)

    [<Test>]
    let ``after one ball`` ()=
        let innings =
            Innings.create
            |> Innings.update (SendInBatsman SampleData.sampleBatsman1) None
            |> Innings.update (SendInBatsman SampleData.sampleBatsman2) None
            |> Innings.update (SendInBowler SampleData.sampleBowler1) None
            |> Innings.update (UpdateForBall DotBall) None
        (summary innings) |> should equal MidOver

    [<Test>]
    let ``after five balls`` ()=
        let innings =
            Innings.create
            |> Innings.update (SendInBatsman SampleData.sampleBatsman1) None
            |> Innings.update (SendInBatsman SampleData.sampleBatsman2) None
            |> Innings.update (SendInBowler SampleData.sampleBowler1) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
        (summary innings) |> should equal MidOver

    [<Test>]
    let ``end of over`` ()=
        let innings =
            Innings.create
            |> Innings.update (SendInBatsman SampleData.sampleBatsman1) None
            |> Innings.update (SendInBatsman SampleData.sampleBatsman2) None
            |> Innings.update (SendInBowler SampleData.sampleBowler1) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (SendInBowler SampleData.sampleBowler2) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
            |> Innings.update (UpdateForBall DotBall) None
        (summary innings) |> should equal EndOver
