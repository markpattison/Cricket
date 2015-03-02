namespace Cricket.CricketEngine

type End =
    | End1
    | End2

type Innings =
    {
        IndividualInnings: IndividualInnings list;
        IsDeclared: bool;
        IndexOfBatsmanAtEnd1: int;
        IndexOfBatsmanAtEnd2: int;
        EndFacingNext: End;
        OversCompleted: int;
        BallsSoFarThisOver: int
    }
    member _this.GetRuns =
        _this.IndividualInnings |> List.sumBy (fun ii -> ii.Score)
    member _this.GetWickets =
        _this.IndividualInnings |> List.filter (fun ii -> ii.HowOut.IsSome) |> List.length

type InningsStatus =
    | InningsCompleted of Innings
    | InningsOngoing of Innings
    member x.GetInnings =
        match x with
        | InningsCompleted innings -> innings
        | InningsOngoing innings -> innings

[<AutoOpen>]
module InningsFunctions =

    let NewInnings =
        {
            IndividualInnings = [ NewIndividualInnings; NewIndividualInnings ];
            IsDeclared = false;
            IndexOfBatsmanAtEnd1 = 0;
            IndexOfBatsmanAtEnd2 = 1;
            EndFacingNext = End1;
            OversCompleted = 0;
            BallsSoFarThisOver = 0;
        }

    let UpdateAtN f n list =
        let updateFunction i x = if i = n then f x else x
        List.mapi updateFunction list

    let Update state (ballOutcome: BallOutcome) =
        state
//        let swapEnds = ballOutcome.HasChangedEnds
//        let completedOver = state.BallsSoFarThisOver >= 5
//        {
//            IndividualInnings = state.IndividualInnings |> UpdateAtN (Update (Name "test") ballOutcome) state.IndexOfBatsmanAtEnd1;
//            IsDeclared = false;
//            IndexOfBatsmanAtEnd1 = if swapEnds then state.IndexOfBatsmanAtEnd2 else state.IndexOfBatsmanAtEnd1;
//            IndexOfBatsmanAtEnd2 = if swapEnds then state.IndexOfBatsmanAtEnd1 else state.IndexOfBatsmanAtEnd2;
//            EndFacingNext = state.EndFacingNext;
//            OversCompleted = state.OversCompleted + if completedOver then 1 else 0;
//            BallsSoFarThisOver = if completedOver then 0 else state.BallsSoFarThisOver + 1;
//        }
