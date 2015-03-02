namespace Cricket.CricketEngine

type IndividualInnings =
    {
        Score: int;
        HowOut: HowOut option;
        BallsFaced: int;
        Fours: int;
        Sixes: int;
    }

[<AutoOpen>]
module IndividualInningsFunctions =

    let NewIndividualInnings =
        {
            Score = 0;
            HowOut = None;
            BallsFaced = 0;
            Fours = 0;
            Sixes = 0;
        }

    let Update (bowler: Player) (innings: IndividualInnings) (ball: BallOutcome) =
        {
            Score = innings.Score + GetRuns ball;
            HowOut = GetHowOut bowler ball;
            BallsFaced = innings.BallsFaced + if (CountsAsBallFaced ball) then 1 else 0;
            Fours = innings.Fours + if (IsFour ball) then 1 else 0;
            Sixes = innings.Sixes + if (IsSix ball) then 1 else 0;
        }