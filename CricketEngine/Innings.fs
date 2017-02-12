﻿namespace Cricket.CricketEngine

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
        Batsmen: (Player * IndividualInnings) list;
        IsDeclared: bool;
        BatsmanAtEnd1: Player option;
        BatsmanAtEnd2: Player option;
        EndFacingNext: End;
        OversCompleted: int;
        BallsThisOver: BallOutcome list;
    }
    member _this.GetRuns =
        _this.Batsmen |> List.sumBy (fun (_, ii) -> ii.Score)
    member _this.GetWickets =
        _this.Batsmen |> List.filter (fun (_, ii) -> ii.HowOut.IsSome) |> List.length
    member _this.IsCompleted =
        _this.IsDeclared || _this.GetWickets = 10
    member _this.BallsSoFarThisOver =
        _this.BallsThisOver |> List.length

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
            Batsmen = [];
            IsDeclared = false;
            BatsmanAtEnd1 = None;
            BatsmanAtEnd2 = None;
            EndFacingNext = End1;
            OversCompleted = 0;
            BallsThisOver = [];
        }

    let forPlayer player state =
        List.find (fun (p, _) -> p = player) state.Batsmen |> snd

    let private updateBatsmen f batsman (list: (Player * IndividualInnings) list) =
        list
        |> List.map (fun (player, indInnings) ->
            (player, if player = batsman then f indInnings else indInnings))

    let private tempBowler = { Name = "testBowler" } // TODO

    let updateForBall (ballOutcome: BallOutcome) (state: Innings) =
        let swapEnds = BallOutcome.changedEnds ballOutcome
        let countsAsBallFaced = BallOutcome.countsAsBallFaced ballOutcome
        let overCompleted = countsAsBallFaced && state.BallsSoFarThisOver = 5

        let striker, nonStriker =
            match state.EndFacingNext, state.BatsmanAtEnd1, state.BatsmanAtEnd2 with
            | End1, Some bat1, Some bat2 -> bat1, bat2
            | End2, Some bat1, Some bat2 -> bat2, bat1
            | _ -> failwith "cannot bowl ball without two batsmen"

        let whoOut = BallOutcome.whoOut ballOutcome

        let (batsmanAtFacingEnd, batsmanAtNonFacingEnd) =
            match whoOut, swapEnds with
            | Striker, false -> None, Some nonStriker
            | Striker, true -> Some nonStriker, None
            | NonStriker, false -> Some striker, None
            | NonStriker, true -> None, Some striker
            | Nobody, false -> Some striker, Some nonStriker
            | Nobody, true -> Some nonStriker, Some striker

        let (batsmanAtEnd1, batsmanAtEnd2) =
            match state.EndFacingNext with
            | End1 -> batsmanAtFacingEnd, batsmanAtNonFacingEnd
            | End2 -> batsmanAtNonFacingEnd, batsmanAtFacingEnd

        {
            state with
                Batsmen =
                    state.Batsmen
                    |> updateBatsmen (IndividualInnings.update tempBowler ballOutcome) striker
                    |> updateBatsmen (IndividualInnings.updateNonStriker ballOutcome) nonStriker;
                BatsmanAtEnd1 = batsmanAtEnd1;
                BatsmanAtEnd2 = batsmanAtEnd2;
                EndFacingNext = if overCompleted then state.EndFacingNext.OtherEnd else state.EndFacingNext;
                OversCompleted = if overCompleted then state.OversCompleted + 1 else state.OversCompleted;
                BallsThisOver = if overCompleted then [] else state.BallsThisOver @ [ ballOutcome ];
        }

    let sendInBatsman (nextBatsman: Player) state =
        let nextIndex = List.length state.Batsmen
        match state.BatsmanAtEnd1, state.BatsmanAtEnd2, nextIndex with
        | None, None, 0 ->
                {
                    state with
                        Batsmen = List.append state.Batsmen [(nextBatsman, IndividualInnings.create)];
                        BatsmanAtEnd1 = Some nextBatsman;
                }
        | None, None, _ -> failwith "cannot have two batsmen out at the same time"
        | Some _, Some _, _ -> failwith "cannot send in new batsman unless one is out"
        | None, Some _, _ ->
            {
                state with
                    Batsmen = List.append state.Batsmen [(nextBatsman, IndividualInnings.create)];
                    BatsmanAtEnd1 = Some nextBatsman;
            }
        | Some _, None, _ ->
            {
                state with
                    Batsmen = List.append state.Batsmen [(nextBatsman, IndividualInnings.create)];
                    BatsmanAtEnd2 = Some nextBatsman;
            }
