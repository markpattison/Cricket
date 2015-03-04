namespace Cricket.CricketEngine

type End =
    | End1
    | End2

type Innings =
    {
        Individuals: IndividualInnings list;
        IsDeclared: bool;
        IndexOfBatsmanAtEnd1: int;
        IndexOfBatsmanAtEnd2: int;
        EndFacingNext: End;
        OversCompleted: int;
        BallsSoFarThisOver: int
    }
    member _this.GetRuns =
        _this.Individuals |> List.sumBy (fun ii -> ii.Score)
    member _this.GetWickets =
        _this.Individuals |> List.filter (fun ii -> ii.HowOut.IsSome) |> List.length

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
            Individuals = [ NewIndividualInnings; NewIndividualInnings ];
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

    let UpdateInningsWithBall state (ballOutcome: BallOutcome) =
        let swapEnds = ballOutcome.HasChangedEnds
        let countsAsBallFaced = ballOutcome.CountsAsBallFaced
        let (overs, balls) =
            match countsAsBallFaced, state.BallsSoFarThisOver with
            | false, _ -> state.OversCompleted, state.BallsSoFarThisOver
            | true, 5 -> state.OversCompleted + 1, 0
            | true, n -> state.OversCompleted, n + 1
        {
            state with
                IndexOfBatsmanAtEnd1 = if swapEnds then state.IndexOfBatsmanAtEnd2 else state.IndexOfBatsmanAtEnd1;
                IndexOfBatsmanAtEnd2 = if swapEnds then state.IndexOfBatsmanAtEnd1 else state.IndexOfBatsmanAtEnd2;
                OversCompleted = overs;
                BallsSoFarThisOver = balls;
        }

