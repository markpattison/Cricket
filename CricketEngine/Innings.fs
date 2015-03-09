namespace Cricket.CricketEngine

type End =
    | End1
    | End2
    member _this.OtherEnd = if _this = End1 then End2 else End1

type Innings =
    {
        Individuals: (Player * IndividualInnings) list;
        IsDeclared: bool;
        IndexOfBatsmanAtEnd1: int option;
        IndexOfBatsmanAtEnd2: int option;
        EndFacingNext: End;
        OversCompleted: int;
        BallsSoFarThisOver: int
    }
    member _this.GetRuns =
        _this.Individuals |> List.sumBy (fun (_, ii) -> ii.Score)
    member _this.GetWickets =
        _this.Individuals |> List.filter (fun (_, ii) -> ii.HowOut.IsSome) |> List.length

type InningsStatus =
    | InningsCompleted of Innings
    | InningsOngoing of Innings
    member x.GetInnings =
        match x with
        | InningsCompleted innings -> innings
        | InningsOngoing innings -> innings

[<AutoOpen>]
module InningsFunctions =

    let NewInnings batsman1 batsman2 =
        {
            Individuals = [ (batsman1, NewIndividualInnings); (batsman2, NewIndividualInnings) ];
            IsDeclared = false;
            IndexOfBatsmanAtEnd1 = Some 0;
            IndexOfBatsmanAtEnd2 = Some 1;
            EndFacingNext = End1;
            OversCompleted = 0;
            BallsSoFarThisOver = 0;
        }

    let UpdateAtN f n list =
        let updateFunction i x = if i = n then f x else x
        List.mapi updateFunction list

    let swap (a, b) = (b, a)
    let tempBowler = Name "tempBowler" // TODO

    let UpdateInningsWithBall state (ballOutcome: BallOutcome) =
        let swapEnds = ballOutcome.HasChangedEnds
        let countsAsBallFaced = ballOutcome.CountsAsBallFaced
        let (overs, balls, endFacing) =
            match countsAsBallFaced, state.BallsSoFarThisOver with
            | false, _ -> state.OversCompleted, state.BallsSoFarThisOver, state.EndFacingNext
            | true, 5 -> state.OversCompleted + 1, 0, state.EndFacingNext.OtherEnd
            | true, n -> state.OversCompleted, n + 1, state.EndFacingNext
        let isStrikerOut = Option.isSome (ballOutcome.GetHowStrikerOut tempBowler)
        let isNonStrikerOut = Option.isSome ballOutcome.GetHowNonStrikerOut
        let unswappedAssumingEnd1 =
            match isStrikerOut, isNonStrikerOut with
            | false, false -> (state.IndexOfBatsmanAtEnd1, state.IndexOfBatsmanAtEnd2)
            | false, true -> (state.IndexOfBatsmanAtEnd1, None)
            | true, false -> (None, state.IndexOfBatsmanAtEnd2)
            | true, true -> failwith "both batsman cannot be out on the same ball"
        let (batsmanAtEnd1, batsmanAtEnd2) =
            match state.EndFacingNext, swapEnds with
            | End1, false | End2, true -> unswappedAssumingEnd1
            | End1, true | End2, false -> swap unswappedAssumingEnd1
        {
            state with
                IndexOfBatsmanAtEnd1 = batsmanAtEnd1;
                IndexOfBatsmanAtEnd2 = batsmanAtEnd2;
                EndFacingNext = endFacing;
                OversCompleted = overs;
                BallsSoFarThisOver = balls;
        }

    let SendInNewBatsman state (nextBatsman: Player) =
        let nextIndex = List.length state.Individuals
        match state.IndexOfBatsmanAtEnd1, state.IndexOfBatsmanAtEnd2 with
        | None, None -> failwith "cannot have two batsmen out at the same time"
        | Some _, Some _ -> failwith "cannot send in new batsman unless one is out"
        | None, Some _ ->
            {
                state with
                    Individuals = List.append state.Individuals [(nextBatsman, NewIndividualInnings)];
                    IndexOfBatsmanAtEnd1 = Some nextIndex;
            }
        | Some _, None ->
            {
                state with
                    Individuals = List.append state.Individuals [(nextBatsman, NewIndividualInnings)];
                    IndexOfBatsmanAtEnd2 = Some nextIndex;
            }
