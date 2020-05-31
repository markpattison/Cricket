namespace Cricket.CricketEngine

type BowlingAnalysis =
    {
        Balls: int;
        Maidens: int;
        RunsConceded: int;
        Wickets: int;
    }

module BowlingAnalysis =

    let create =
        {
            Balls = 0;
            Maidens = 0;
            RunsConceded = 0;
            Wickets = 0;
        }

    let update ball bowling =
        {
            Balls = bowling.Balls + 1;
            Maidens = bowling.Maidens;
            RunsConceded = bowling.RunsConceded + BallOutcome.getRuns ball
            Wickets = bowling.Wickets + if (BallOutcome.isWicketForBowler ball) then 1 else 0
        }

    let updateAfterOver (balls: BallOutcome list) bowling =
        let runsFromOver = balls |> List.sumBy BallOutcome.getRuns
        if runsFromOver > 0 then bowling else { bowling with Maidens = bowling.Maidens + 1 }