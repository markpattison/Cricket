namespace Cricket.CricketEngine.AcceptanceTests

open FsUnit
open NUnit.Framework

open Cricket.CricketEngine

module ``Acceptance tests`` =

    let sampleMatchRules = { FollowOnMargin = 200; }

    let batsman1 = Name "batsman 1"
    let batsman2 = Name "batsman 2"

    [<Test>]
    let ``acceptance test`` ()=
        let match' =
            NewMatch sampleMatchRules "Team A" "Team B"
            |> UpdateMatchState StartMatch
            |> UpdateCurrentInnings (SendInNewBatsman batsman1)
            |> UpdateCurrentInnings (SendInNewBatsman batsman2)

        SummaryStatus match' |> should equal "Team A are 0 for 0 in their first innings"



