namespace Cricket.CricketEngine.AcceptanceTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine

module ``Acceptance tests`` =

    let sampleMatchRules = { FollowOnMargin = 200; }

    let batsman1 = Name "batsman 1"
    let batsman2 = Name "batsman 2"
    let batsman3 = Name "batsman 3"

    let dotBall = UpdateInningsWithBall DotBall
    let score1 = UpdateInningsWithBall (ScoreRuns 1)

    [<Test>]
    let ``start first innings`` ()=
        let match' =
            NewMatch sampleMatchRules "Team A" "Team B"
            |> UpdateMatchState StartMatch
            |> UpdateCurrentInnings (SendInNewBatsman batsman1)
            |> UpdateCurrentInnings (SendInNewBatsman batsman2)

        SummaryStatus match' |> should equal "Team A are 0 for 0 in their first innings"

//    [<Test>]
//    let ``one over`` ()=
//        let match' =
//            NewMatch sampleMatchRules "Team A" "Team B"
//            |> UpdateMatchState StartMatch
//            |> UpdateCurrentInnings (SendInNewBatsman batsman1)
//            |> UpdateCurrentInnings (SendInNewBatsman batsman2)
//            |> UpdateCurrentInnings dotBall
//            |> UpdateCurrentInnings dotBall
//            |> UpdateCurrentInnings dotBall
//            |> UpdateCurrentInnings dotBall
//            |> UpdateCurrentInnings dotBall
//            |> UpdateCurrentInnings score1
//
//        let firstInnings = CurrentInnings match'.State
//
//        SummaryStatus match' |> should equal "Team A are 1 for 0 in their first innings"
//        firstInnings.GetRuns |> should equal 1
//        firstInnings.GetWickets |> should equal 0
//        firstInnings.IndexOfBatsmanAtEnd1 |> should equal 1
//        firstInnings.IndexOfBatsmanAtEnd2 |> should equal 0
//        firstInnings.OversCompleted |> should equal 1
//        firstInnings.BallsSoFarThisOver |> should equal 0

