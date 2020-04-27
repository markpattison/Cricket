namespace Cricket.MatchRunner

open Cricket.CricketEngine

module RandomBall =

    let random = System.Random()

    let twoNormalSamples mu1 sigma1 mu2 sigma2 =
        // Box-Muller
        let u = random.NextDouble()
        let v = random.NextDouble()
        let c = sqrt (-2.0 * log u)
        let x = c * cos (2.0 * System.Math.PI * v)
        let y = c * sin (2.0 * System.Math.PI * v)
        (mu1 + sigma1 * x, mu2 + sigma2 * y)

    let ball attributes batsman bowler =
        let batSkill = PlayerAttributes.battingSkill attributes batsman
        let bowlSkill = PlayerAttributes.bowlingSkill attributes bowler

        let batSigma = 1.291 - 0.339 * batSkill
        let bowlSigma = 1.191 - 0.314 * bowlSkill

        let xBat, xBowl = twoNormalSamples batSkill batSigma bowlSkill bowlSigma

        let xBall = xBat - xBowl

        match xBall with
        | x when x <= -3.00 -> Bowled
        | x when x <= -2.80 -> LBW
        | x when x <= -2.70 -> RunOutStriker (0, false)
        | x when x >=  3.20 -> Six
        | x when x >=  2.40 -> Four
        | x when x >=  2.00 -> ScoreRuns 2
        | x when x >=  0.90 -> ScoreRuns 1
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