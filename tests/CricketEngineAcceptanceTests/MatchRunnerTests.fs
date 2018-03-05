namespace Cricket.CricketEngine.AcceptanceTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine

module ``MatchRunner acceptance tests`` =

    let sampleMatchRules = { FollowOnMargin = 200; }

    let teamA =
        {
            TeamName = "Team A"
            Players = [| 1 .. 11 |] |> Array.map (fun n -> { Name = sprintf "batsman %i" n; ID = n })
        }

    let teamB =
        {
            TeamName = "Team B"
            Players = [| 1 .. 11 |] |> Array.map (fun n -> { Name = sprintf "bowler %i" n; ID = 20 + n })
        }

    let dot = Match.updateCurrentInnings (UpdateForBall DotBall)
    let score1 = Match.updateCurrentInnings (UpdateForBall (ScoreRuns 1))

    [<Test>]
    let ``start first innings`` ()=
        let matchState =
            Match.newMatch sampleMatchRules teamA teamB
            |> Match.updateMatchState StartMatch
            |> Match.updateMatchState (SendInBatsman teamA.Players.[0] |> UpdateInnings)
            |> Match.updateMatchState (SendInBatsman teamA.Players.[1] |> UpdateInnings)
            |> Match.updateMatchState (SendInBowler teamB.Players.[0] |> UpdateInnings)
            |> Match.updateMatchState (UpdateForBall Bowled |> UpdateInnings)

        //let options = Cricket.MatchRunner.MatchRunner.getOptionsUI matchState

        ()