namespace Cricket.CricketEngine.AcceptanceTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine

module ``Acceptance tests`` =

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
            |> Match.updateCurrentInnings (SendInBatsman batsman1)
            |> Match.updateCurrentInnings (SendInBatsman batsman2)

        Match.summaryStatus matchState |> should equal "Team A are 0 for 0 in their first innings"

    [<Test>]
    let ``one Boycott over`` ()=
        let matchState =
            Match.newMatch sampleMatchRules "Team A" "Team B"
            |> Match.updateMatchState StartMatch
            |> Match.updateCurrentInnings (SendInBatsman batsman1)
            |> Match.updateCurrentInnings (SendInBatsman batsman2)
            |> Match.updateCurrentInnings (SendInBowler bowler1)
            |> dot
            |> dot
            |> dot
            |> dot
            |> dot
            |> score1

        let expected =
            {
                TeamA = "Team A";
                TeamB = "Team B";
                Rules = sampleMatchRules;
                State = A'Ongoing
                    {
                        Batsmen =
                            [
                                batsman1, 
                                {
                                    Score = 1;
                                    HowOut = None;
                                    BallsFaced = 6;
                                    Fours = 0;
                                    Sixes = 0;
                                };
                                batsman2, 
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
                                bowler1,
                                {
                                    Balls = 6;
                                    Maidens = 0;
                                    RunsConceded = 1;
                                    Wickets = 0;
                                }
                            ]
                        IsDeclared = false;
                        BatsmanAtEnd1 = Some batsman2;
                        BatsmanAtEnd2 = Some batsman1;
                        EndFacingNext = End2;
                        OversCompleted = 1;
                        BallsThisOver = [];
                        BowlerToEnd1 = Some bowler1;
                        BowlerToEnd2 = None
                    }
            }

        matchState |> should equal expected

    [<Test>]
    let ``non-striker run out`` ()=
        let matchState =
            Match.newMatch sampleMatchRules "Team A" "Team B"
            |> Match.updateMatchState StartMatch
            |> Match.updateCurrentInnings (SendInBatsman batsman1)
            |> Match.updateCurrentInnings (SendInBatsman batsman2)
            |> Match.updateCurrentInnings (SendInBowler bowler1)
            |> dot
            |> score1
            |> score1
            |> Match.updateCurrentInnings (UpdateForBall (RunOutNonStriker (1, false)))
            |> Match.updateCurrentInnings (SendInBatsman batsman3)

        let expected =
            {
                TeamA = "Team A";
                TeamB = "Team B";
                Rules = sampleMatchRules;
                State = A'Ongoing
                    {
                        Batsmen =
                            [
                                batsman1, 
                                {
                                    Score = 2;
                                    HowOut = None;
                                    BallsFaced = 3;
                                    Fours = 0;
                                    Sixes = 0;
                                };
                                batsman2, 
                                {
                                    Score = 1;
                                    HowOut = Some RunOut;
                                    BallsFaced = 1;
                                    Fours = 0;
                                    Sixes = 0;
                                };
                                batsman3, 
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
                                bowler1,
                                {
                                    Balls = 4;
                                    Maidens = 0;
                                    RunsConceded = 3;
                                    Wickets = 0;
                                }
                            ]
                        IsDeclared = false;
                        BatsmanAtEnd1 = Some batsman3;
                        BatsmanAtEnd2 = Some batsman1;
                        EndFacingNext = End1;
                        OversCompleted = 0;
                        BallsThisOver = [ DotBall; ScoreRuns 1; ScoreRuns 1; RunOutNonStriker (1, false) ]
                        BowlerToEnd1 = Some bowler1;
                        BowlerToEnd2 = None
                    }
            }

        matchState |> should equal expected

    [<Test>]
    let ``four declarations`` ()=
        let matchState =
            Match.newMatch sampleMatchRules "Team A" "Team B"
            |> Match.updateMatchState StartMatch
            |> Match.updateCurrentInnings Declare
            |> Match.updateMatchState StartNextInnings
            |> Match.updateCurrentInnings Declare
            |> Match.updateMatchState StartNextInnings
            |> Match.updateCurrentInnings Declare
            |> Match.updateMatchState StartNextInnings
            |> Match.updateCurrentInnings Declare

        Match.summaryStatus matchState |> should equal "Match tied"
        