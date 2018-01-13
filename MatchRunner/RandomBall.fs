namespace Cricket.MatchRunner

open Cricket.CricketEngine

module RandomBall =

    let random = System.Random()

    let ball batsman bowler =
        match random.Next(0, 100) with
        | x when x <= 2 -> Bowled
        | x when x <= 4 -> LBW
        | x when x <= 5 -> RunOutStriker (0, false)
        | x when x >= 99 -> Six
        | x when x >= 97 -> Four
        | x when x >= 95 -> ScoreRuns 2
        | x when x >= 3 -> ScoreRuns 1
        | _ -> DotBall
