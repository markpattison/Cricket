namespace Cricket.CricketEngine

type IndividualInnings =
    {
        Score: int;
        HowOut: HowOut option;
        BallsFaced: int;
        Fours: int;
        Sixes: int;
    }

module IndividualInnings =

    let create =
        {
            Score = 0;
            HowOut = None;
            BallsFaced = 0;
            Fours = 0;
            Sixes = 0;
        }

    let update (bowler: Player) (ball: BallOutcome) (innings: IndividualInnings) =
        {
            Score = innings.Score + BallOutcome.getRuns ball;
            HowOut = BallOutcome.howStrikerOut bowler ball;
            BallsFaced = innings.BallsFaced + if (BallOutcome.countsAsBallFaced ball) then 1 else 0;
            Fours = innings.Fours + if (BallOutcome.isAFour ball) then 1 else 0;
            Sixes = innings.Sixes + if (BallOutcome.isASix ball) then 1 else 0;
        }

    let updateNonStriker (ball: BallOutcome) (innings: IndividualInnings) =
        match ball with
        | RunOutNonStriker _ -> { innings with HowOut = Some HowOut.RunOut }
        | _ -> innings
