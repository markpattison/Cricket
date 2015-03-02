namespace Cricket.CricketEngine.UnitTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine

module EndChangeTests =

    let sampleInnings =
        {
            IndividualInnings = [ NewIndividualInnings; NewIndividualInnings ];
            IsDeclared = false;
            IndexOfBatsmanAtEnd1 = 0;
            IndexOfBatsmanAtEnd2 = 1;
            EndFacingNext = End1;
            OversCompleted = 0;
            BallsSoFarThisOver = 0;
        }

//    [<Test>]
//    let ``batsmen change ends if odd number of runs scored`` ()=
//        let updated = Update sampleInnings (ScoreRuns 1)
//        updated.I

