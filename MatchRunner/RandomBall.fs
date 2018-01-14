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


// Thoughts on generating the ball outcome
// ***************************************
//
// Could depend on:
//  - batsman's skill
//  - batsman's consistency
//  - batsman's long-term form
//  - batsman's short-term form e.g. has got his eye in
//  - batsman's tiredness
//  - batsman's natural level of aggression
//  - batsman's level of intended aggression based on match situation
//
//  - bowler's skill
//  - bowler's consistency
//  - bowler's long-term form
//  - bowler's short-term form
//  - bowler's tiredness (greater effect than for batsmen)
//  - bowler's natural level of aggression
//  - bowler's level of intended aggression based on match situation
//
//  Initially, could just use skill levels
//
// Calibration thoughts:
//  - E(runs per ball) increases (slowly) with batsman's skill and decreases (quickly) with bowler's skill
//  - E(wickets per ball) decreases (quickly) with batsman's skill and increases (slowly) with bowler's skill
//  - E(runs per wicket) must increase with batsman's skill and decrease with bowler's skill
//
//  - E(runs per ball) increases with aggression
//  - E(wickets per ball) increases with aggression
//  - E(runs per wicket) is concave with aggression