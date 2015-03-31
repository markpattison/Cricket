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
    member _this.IsCompleted =
        _this.IsDeclared || _this.GetWickets = 10

//    member _this.GetFacingBatsman =
//        let indexOfFacingBatsman =
//            match _this.IsDeclared, _this.GetWickets, _this.EndFacingNext with
//            | true, _, _ | _, 10, _ -> failwith "innings over"
//            | _, _, End1 -> _this.IndexOfBatsmanAtEnd1
//            | _, _, End2 -> _this.IndexOfBatsmanAtEnd2
//        match indexOfFacingBatsman with
//        | None -> failwith "facing batsman not found"
//        | Some n -> _this.Individuals.Item n

[<AutoOpen>]
module InningsFunctions =

    let (|InningsOngoing|InningsCompleted|) (innings: Innings) =
        if innings.IsCompleted then InningsCompleted innings else InningsOngoing innings

    let NewInnings =
        {
            Individuals = [];
            IsDeclared = false;
            IndexOfBatsmanAtEnd1 = None;
            IndexOfBatsmanAtEnd2 = None;
            EndFacingNext = End1;
            OversCompleted = 0;
            BallsSoFarThisOver = 0;
        }

    let UpdateIndividuals updateStriker updateNonStriker indexStriker indexNonStriker list =
        let updateFunction i x =
            match i with
            | a when a = indexStriker -> updateStriker x
            | a when a = indexNonStriker -> updateNonStriker x
            | _ -> x
        List.mapi updateFunction list

    let swap (a, b) = (b, a)
    let tempBowler = Name "testBowler" // TODO

    let UpdateInningsWithBall (ballOutcome: BallOutcome) state =
        let swapEnds = ballOutcome.HasChangedEnds
        let countsAsBallFaced = ballOutcome.CountsAsBallFaced
        let (overs, balls, endFacing) =
            match countsAsBallFaced, state.BallsSoFarThisOver with
            | false, _ -> state.OversCompleted, state.BallsSoFarThisOver, state.EndFacingNext
            | true, 5 -> state.OversCompleted + 1, 0, state.EndFacingNext.OtherEnd
            | true, n -> state.OversCompleted, n + 1, state.EndFacingNext
        let indexOfStriker = (if state.EndFacingNext = End1 then state.IndexOfBatsmanAtEnd1 else state.IndexOfBatsmanAtEnd2).Value
        let indexOfNonStriker = (if state.EndFacingNext = End1 then state.IndexOfBatsmanAtEnd2 else state.IndexOfBatsmanAtEnd1).Value
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
        let updateStriker (p, ii) = (p, Update tempBowler ballOutcome ii)
        let updateNonStriker (p, ii) = (p, UpdateNonStriker ballOutcome ii)
        {
            state with
                Individuals = (UpdateIndividuals updateStriker updateNonStriker indexOfStriker indexOfNonStriker state.Individuals);
                IndexOfBatsmanAtEnd1 = batsmanAtEnd1;
                IndexOfBatsmanAtEnd2 = batsmanAtEnd2;
                EndFacingNext = endFacing;
                OversCompleted = overs;
                BallsSoFarThisOver = balls;
        }

    let SendInNewBatsman (nextBatsman: Player) state =
        let nextIndex = List.length state.Individuals
        let updatedState =
            match state.IndexOfBatsmanAtEnd1, state.IndexOfBatsmanAtEnd2 with
            | None, None ->
                if nextIndex = 0 then
                    {
                        state with
                            Individuals = List.append state.Individuals [(nextBatsman, NewIndividualInnings)];
                            IndexOfBatsmanAtEnd1 = Some nextIndex;
                    }
                else failwith "cannot have two batsmen out at the same time"
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
        updatedState
