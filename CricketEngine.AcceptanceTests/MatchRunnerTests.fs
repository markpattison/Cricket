namespace Cricket.CricketEngine.AcceptanceTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine

module ``MatchRunner acceptance tests`` =

    let sampleMatchRules = { FollowOnMargin = 200; }

    let batsman1 = { Name = "batsman 1" }
    let batsman2 = { Name = "batsman 2" }
    let batsman3 = { Name = "batsman 3" }

    let bowler1 = { Name = "bowler 1" }
    let bowler2 = { Name = "bowler 2" }

    let dot = Match.updateCurrentInnings (UpdateForBall DotBall)
    let score1 = Match.updateCurrentInnings (UpdateForBall (ScoreRuns 1))

    [<Test>]
    let ``start first innings`` ()=
        let matchState =
            Match.newMatch sampleMatchRules "Team A" "Team B"
            |> Match.updateMatchState StartMatch

        let options = Cricket.MatchRunner.MatchRunner.updateForUI matchState

        ()