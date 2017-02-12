namespace Cricket.CricketEngine.AcceptanceTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine

module ``Acceptance tests`` =

    let sampleMatchRules = { FollowOnMargin = 200; }

    let batsman1 = { Name = "batsman 1" }
    let batsman2 = { Name = "batsman 2" }
    let batsman3 = { Name = "batsman 3" }

    let dot = Match.updateCurrentInnings (Innings.updateForBall DotBall)
    let score1 = Match.updateCurrentInnings (Innings.updateForBall (ScoreRuns 1))

    [<Test>]
    let ``start first innings`` ()=
        let match' =
            Match.newMatch sampleMatchRules "Team A" "Team B"
            |> Match.updateMatchState StartMatch
            |> Match.updateCurrentInnings (Innings.sendInBatsman batsman1)
            |> Match.updateCurrentInnings (Innings.sendInBatsman batsman2)

        Match.summaryStatus match' |> should equal "Team A are 0 for 0 in their first innings"

    [<Test>]
    let ``one Boycott over`` ()=
        let match' =
            Match.newMatch sampleMatchRules "Team A" "Team B"
            |> Match.updateMatchState StartMatch
            |> Match.updateCurrentInnings (Innings.sendInBatsman batsman1)
            |> Match.updateCurrentInnings (Innings.sendInBatsman batsman2)
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
                        Individuals =
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
                        IsDeclared = false;
                        BatsmanAtEnd1 = Some batsman2;
                        BatsmanAtEnd2 = Some batsman1;
                        EndFacingNext = End2;
                        OversCompleted = 1;
                        BallsThisOver = [];
                    }
            }

        match' |> should equal expected

    [<Test>]
    let ``non-striker run out`` ()=
        let match' =
            Match.newMatch sampleMatchRules "Team A" "Team B"
            |> Match.updateMatchState StartMatch
            |> Match.updateCurrentInnings (Innings.sendInBatsman batsman1)
            |> Match.updateCurrentInnings (Innings.sendInBatsman batsman2)
            |> dot
            |> score1
            |> score1
            |> Match.updateCurrentInnings (Innings.updateForBall (RunOutNonStriker (1, false)))
            |> Match.updateCurrentInnings (Innings.sendInBatsman batsman3)

        let expected =
            {
                TeamA = "Team A";
                TeamB = "Team B";
                Rules = sampleMatchRules;
                State = A'Ongoing
                    {
                        Individuals =
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
                        IsDeclared = false;
                        BatsmanAtEnd1 = Some batsman3;
                        BatsmanAtEnd2 = Some batsman1;
                        EndFacingNext = End1;
                        OversCompleted = 0;
                        BallsThisOver = [ DotBall; ScoreRuns 1; ScoreRuns 1; RunOutNonStriker (1, false) ]
                    }
            }

        match' |> should equal expected