namespace Cricket.CricketEngine

type End =
    | End1
    | End2
    member _this.OtherEnd = if _this = End1 then End2 else End1
    override _this.ToString() =
        match _this with
        | End1 -> "end 1"
        | End2 -> "end 2"

type Innings =
    {
        Individuals: (Player * IndividualInnings) list;
        IsDeclared: bool;
        BatsmanAtEnd1: Player option;
        BatsmanAtEnd2: Player option;
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

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix )>]
module Innings =

    let (|InningsOngoing|InningsCompleted|) (innings: Innings) =
        if innings.IsCompleted then InningsCompleted innings else InningsOngoing innings

    let create =
        {
            Individuals = [];
            IsDeclared = false;
            BatsmanAtEnd1 = None;
            BatsmanAtEnd2 = None;
            EndFacingNext = End1;
            OversCompleted = 0;
            BallsSoFarThisOver = 0;
        }

    let forPlayer player state =
        List.find (fun (p, _) -> p = player) state.Individuals |> snd

    let private updateIndividuals f batsman (list: (Player * IndividualInnings) list) =
        list
        |> List.map (fun (player, indInnings) ->
            (player, if player = batsman then f indInnings else indInnings))

    let private swap (a, b) = (b, a)
    let private tempBowler = { Name = "testBowler" } // TODO

    let updateForBall (ballOutcome: BallOutcome) state =
        let swapEnds = BallOutcome.changedEnds ballOutcome
        let countsAsBallFaced = BallOutcome.countsAsBallFaced ballOutcome
        let (overs, balls, endFacing) =
            match countsAsBallFaced, state.BallsSoFarThisOver with
            | false, _ -> state.OversCompleted, state.BallsSoFarThisOver, state.EndFacingNext
            | true, 5 -> state.OversCompleted + 1, 0, state.EndFacingNext.OtherEnd
            | true, n -> state.OversCompleted, n + 1, state.EndFacingNext
        let striker = (if state.EndFacingNext = End1 then state.BatsmanAtEnd1 else state.BatsmanAtEnd2).Value
        let nonStriker = (if state.EndFacingNext = End1 then state.BatsmanAtEnd2 else state.BatsmanAtEnd1).Value
        let isStrikerOut = Option.isSome (BallOutcome.howStrikerOut tempBowler ballOutcome)
        let isNonStrikerOut = Option.isSome (BallOutcome.howNonStrikerOut ballOutcome)
        let unswappedAssumingEnd1 =
            match isStrikerOut, isNonStrikerOut with
            | false, false -> (state.BatsmanAtEnd1, state.BatsmanAtEnd2)
            | false, true -> (state.BatsmanAtEnd1, None)
            | true, false -> (None, state.BatsmanAtEnd2)
            | true, true -> failwith "both batsman cannot be out on the same ball"
        let (batsmanAtEnd1, batsmanAtEnd2) =
            match state.EndFacingNext, swapEnds with
            | End1, false | End2, true -> unswappedAssumingEnd1
            | End1, true | End2, false -> swap unswappedAssumingEnd1
        {
            state with
                Individuals =
                    state.Individuals
                    |> updateIndividuals (IndividualInnings.update tempBowler ballOutcome) striker
                    |> updateIndividuals (IndividualInnings.updateNonStriker ballOutcome) nonStriker;
                BatsmanAtEnd1 = batsmanAtEnd1;
                BatsmanAtEnd2 = batsmanAtEnd2;
                EndFacingNext = endFacing;
                OversCompleted = overs;
                BallsSoFarThisOver = balls;
        }

    let sendInBatsman (nextBatsman: Player) state =
        let nextIndex = List.length state.Individuals
        match state.BatsmanAtEnd1, state.BatsmanAtEnd2, nextIndex with
        | None, None, 0 ->
                {
                    state with
                        Individuals = List.append state.Individuals [(nextBatsman, IndividualInnings.create)];
                        BatsmanAtEnd1 = Some nextBatsman;
                }
        | None, None, _ -> failwith "cannot have two batsmen out at the same time"
        | Some _, Some _, _ -> failwith "cannot send in new batsman unless one is out"
        | None, Some _, _ ->
            {
                state with
                    Individuals = List.append state.Individuals [(nextBatsman, IndividualInnings.create)];
                    BatsmanAtEnd1 = Some nextBatsman;
            }
        | Some _, None, _ ->
            {
                state with
                    Individuals = List.append state.Individuals [(nextBatsman, IndividualInnings.create)];
                    BatsmanAtEnd2 = Some nextBatsman;
            }
