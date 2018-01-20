namespace Cricket.CricketEngine.AcceptanceTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine

module ``Acceptance tests`` =

    let sampleMatchRules = { FollowOnMargin = 200; }

    let teamA =
        {
            Name = "Team A"
            Players = [| 1 .. 11 |] |> Array.map (fun n -> { Name = sprintf "batsman %i" n; ID = n })
        }

    let teamB =
        {
            Name = "Team B"
            Players = [| 1 .. 11 |] |> Array.map (fun n -> { Name = sprintf "bowler %i" n; ID = 20 + n })
        }

    let dot = Match.updateCurrentInnings (UpdateForBall DotBall)
    let score1 = Match.updateCurrentInnings (UpdateForBall (ScoreRuns 1))

    [<Test>]
    let ``start first innings`` ()=
        let matchState =
            Match.newMatch sampleMatchRules teamA teamB
            |> Match.updateMatchState StartMatch
            |> Match.updateCurrentInnings (SendInBatsman teamA.Players.[0])
            |> Match.updateCurrentInnings (SendInBatsman teamA.Players.[1])

        Match.summaryStatus matchState |> should equal "Team A are 0 for 0 in their first innings"

    [<Test>]
    let ``one Boycott over`` ()=
        let matchState =
            Match.newMatch sampleMatchRules teamA teamB
            |> Match.updateMatchState StartMatch
            |> Match.updateCurrentInnings (SendInBatsman teamA.Players.[0])
            |> Match.updateCurrentInnings (SendInBatsman teamA.Players.[1])
            |> Match.updateCurrentInnings (SendInBowler teamB.Players.[0])
            |> dot
            |> dot
            |> dot
            |> dot
            |> dot
            |> score1

        let expected =
            {
                TeamA = teamA;
                TeamB = teamB;
                Rules = sampleMatchRules;
                State = A'Ongoing
                    {
                        Batsmen =
                            [
                                teamA.Players.[0], 
                                {
                                    Score = 1;
                                    HowOut = None;
                                    BallsFaced = 6;
                                    Fours = 0;
                                    Sixes = 0;
                                };
                                teamA.Players.[1], 
                                {
                                    Score = 0;
                                    HowOut = None;
                                    BallsFaced = 0;
                                    Fours = 0;
                                    Sixes = 0;
                                };
                            ];
                        Bowlers =
                            [
                                teamB.Players.[0],
                                {
                                    Balls = 6;
                                    Maidens = 0;
                                    RunsConceded = 1;
                                    Wickets = 0;
                                }
                            ]
                        IsDeclared = false;
                        BatsmanAtEnd1 = Some teamA.Players.[1];
                        BatsmanAtEnd2 = Some teamA.Players.[0];
                        EndFacingNext = End2;
                        OversCompleted = 1;
                        BallsThisOver = [];
                        BowlerToEnd1 = Some teamB.Players.[0];
                        BowlerToEnd2 = None;
                        FallOfWickets = []
                    }
            }

        matchState |> should equal expected

    [<Test>]
    let ``non-striker run out`` ()=
        let matchState =
            Match.newMatch sampleMatchRules teamA teamB
            |> Match.updateMatchState StartMatch
            |> Match.updateCurrentInnings (SendInBatsman teamA.Players.[0])
            |> Match.updateCurrentInnings (SendInBatsman teamA.Players.[1])
            |> Match.updateCurrentInnings (SendInBowler teamB.Players.[0])
            |> dot
            |> score1
            |> score1
            |> Match.updateCurrentInnings (UpdateForBall (RunOutNonStriker (1, false)))
            |> Match.updateCurrentInnings (SendInBatsman teamA.Players.[2])

        let expected =
            {
                TeamA = teamA;
                TeamB = teamB;
                Rules = sampleMatchRules;
                State = A'Ongoing
                    {
                        Batsmen =
                            [
                                teamA.Players.[0], 
                                {
                                    Score = 2;
                                    HowOut = None;
                                    BallsFaced = 3;
                                    Fours = 0;
                                    Sixes = 0;
                                };
                                teamA.Players.[1], 
                                {
                                    Score = 1;
                                    HowOut = Some RunOut;
                                    BallsFaced = 1;
                                    Fours = 0;
                                    Sixes = 0;
                                };
                                teamA.Players.[2], 
                                {
                                    Score = 0;
                                    HowOut = None;
                                    BallsFaced = 0;
                                    Fours = 0;
                                    Sixes = 0;
                                };
                            ];
                        Bowlers =
                            [
                                teamB.Players.[0],
                                {
                                    Balls = 4;
                                    Maidens = 0;
                                    RunsConceded = 3;
                                    Wickets = 0;
                                }
                            ]
                        IsDeclared = false;
                        BatsmanAtEnd1 = Some teamA.Players.[2];
                        BatsmanAtEnd2 = Some teamA.Players.[0];
                        EndFacingNext = End1;
                        OversCompleted = 0;
                        BallsThisOver = [ DotBall; ScoreRuns 1; ScoreRuns 1; RunOutNonStriker (1, false) ]
                        BowlerToEnd1 = Some teamB.Players.[0];
                        BowlerToEnd2 = None;
                        FallOfWickets = [ { Wicket = 1; Runs = 3; BatsmanOut = teamA.Players.[1]; Overs = 0; BallsWithinOver = 4 } ]
                    }
            }

        matchState |> should equal expected

    [<Test>]
    let ``four declarations`` ()=
        let matchState =
            Match.newMatch sampleMatchRules teamA teamB
            |> Match.updateMatchState StartMatch
            |> Match.updateCurrentInnings Declare
            |> Match.updateMatchState StartNextInnings
            |> Match.updateCurrentInnings Declare
            |> Match.updateMatchState StartNextInnings
            |> Match.updateCurrentInnings Declare
            |> Match.updateMatchState StartNextInnings
            |> Match.updateCurrentInnings Declare

        Match.summaryStatus matchState |> should equal "Match tied"
        