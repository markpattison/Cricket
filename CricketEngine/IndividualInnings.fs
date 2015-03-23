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

    let Update (bowler: Player) (ball: BallOutcome) (innings: IndividualInnings) =
        {
            Score = innings.Score + ball.GetRuns;
            HowOut = ball.GetHowStrikerOut bowler;
            BallsFaced = innings.BallsFaced + if (ball.CountsAsBallFaced) then 1 else 0;
            Fours = innings.Fours + if (ball.IsAFour) then 1 else 0;
            Sixes = innings.Sixes + if (ball.IsASix) then 1 else 0;
        }

    let UpdateNonStriker (ball: BallOutcome) (innings: IndividualInnings) =
        match ball with
        | RunOutNonStriker _ -> { innings with HowOut = Some HowOut.RunOut }
        | _ -> innings
