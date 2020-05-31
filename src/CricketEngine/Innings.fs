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
        Batsmen: (Player * IndividualInnings) list
        Bowlers: (Player * BowlingAnalysis) list
        IsDeclared: bool
        BatsmanAtEnd1: Player option
        BatsmanAtEnd2: Player option
        EndFacingNext: End
        OversCompleted: int
        BallsThisOver: BallOutcome list
        BowlerToEnd1: Player option
        BowlerToEnd2: Player option
        FallOfWickets: FallOfWicket list
    }
    member _this.GetRuns =
        _this.Batsmen |> List.sumBy (fun (_, ii) -> ii.Score)
    member _this.GetWickets =
        _this.Batsmen |> List.filter (fun (_, ii) -> ii.HowOut.IsSome) |> List.length
    member _this.IsCompleted =
        _this.IsDeclared || _this.GetWickets = 10
    member _this.BallsSoFarThisOver =
        _this.BallsThisOver |> List.length

type SummaryInningsState =
    | Completed
    | BatsmanRequired of int
    | BowlerRequiredTo of End
    | EndOver
    | MidOver

type InningsUpdate =
    | Declare
    | SendInBatsman of Player
    | SendInBowler of Player
    | UpdateForBall of BallOutcome

module Innings =

    type ReadyForBall =
        | Ready of striker : Player * nonStriker : Player * bowler : Player
        | NotReady of string

    let (|InningsOngoing|InningsCompleted|) (innings: Innings) =
        if innings.IsCompleted then InningsCompleted innings else InningsOngoing innings

    let create =
        {
            Batsmen = []
            Bowlers = []
            IsDeclared = false
            BatsmanAtEnd1 = None
            BatsmanAtEnd2 = None
            EndFacingNext = End1
            OversCompleted = 0
            BallsThisOver = []
            BowlerToEnd1 = None
            BowlerToEnd2 = None
            FallOfWickets = []
        }

    let summary (state: Innings) =
        match state.GetWickets, state.IsDeclared with
        | 10, _ -> sprintf "%i all out" state.GetRuns
        | w, true -> sprintf "%i/%i declared" state.GetRuns w
        | w, false -> sprintf "%i/%i" state.GetRuns w

    let private readyForBall (state: Innings) =
        match state.IsCompleted, state.EndFacingNext, state.BatsmanAtEnd1, state.BatsmanAtEnd2, state.BowlerToEnd1, state.BowlerToEnd2 with
        | false, End1, Some bat1, Some bat2, Some bowl1, _ -> Ready(striker = bat1, nonStriker = bat2, bowler = bowl1)
        | false, End2, Some bat1, Some bat2, _, Some bowl2 -> Ready(striker = bat2, nonStriker = bat1, bowler = bowl2)
        | false, _, None, _, _, _ | false, _, _, None, _, _ -> NotReady "batsman missing"
        | false, End1, _, _, None, _ | false, End2, _, _, _, None -> NotReady "bowler missing"
        | true, _, _, _, _, _ -> NotReady "innings over"

    let batsmanFacingNext (state: Innings) =
        match state |> readyForBall with
        | Ready (striker = s) -> s
        | NotReady msg -> failwith msg
    
    let bowlerBowlingNext (state: Innings) =
        match state |> readyForBall with
        | Ready (bowler = b) -> b
        | NotReady msg -> failwith msg

    let forPlayer player state =
        List.find (fun (p, _) -> p = player) state.Batsmen |> snd

    let forBowler player state =
        List.find (fun (p, _) -> p = player) state.Bowlers |> snd

    let private updateBatsmen f batsman (list: (Player * IndividualInnings) list) =
        list
        |> List.map (fun (player, indInnings) ->
            (player, if player = batsman then f indInnings else indInnings))

    let private addBowlerIfNeeded bowler bowlers =
        if List.exists (fun (p, _) -> p = bowler) bowlers then
            bowlers
        else
            bowlers @ [ (bowler, BowlingAnalysis.create) ]

    let private updateBowlers ball bowler (list: (Player * BowlingAnalysis) list) =
        list
        |> List.map (fun (player, bowling) ->
            (player, if player = bowler then BowlingAnalysis.update ball bowling else bowling))

    let private updateBowlersForEndOverIfNeeded overCompleted ballsThisOver bowler (list: (Player * BowlingAnalysis) list) =
        if overCompleted then
            list
            |> List.map (fun (player, bowling) ->
                (player, if player = bowler then BowlingAnalysis.updateAfterOver ballsThisOver bowling else bowling))
        else
            list

    let private updateForBall (ballOutcome: BallOutcome) (state: Innings) =
        let swapEnds = BallOutcome.changedEnds ballOutcome
        let countsAsBallFaced = BallOutcome.countsAsBallFaced ballOutcome
        let overCompleted = countsAsBallFaced && state.BallsSoFarThisOver = 5

        let striker, nonStriker, bowler =
            match state |> readyForBall with
            | Ready (striker = s; nonStriker = ns; bowler = b) -> s, ns, b
            | NotReady msg -> failwith msg

        let whoOut = BallOutcome.whoOut ballOutcome

        let (batsmanAtFacingEnd, batsmanAtNonFacingEnd) =
            match whoOut, swapEnds with
            | Striker, false -> None, Some nonStriker
            | Striker, true -> Some nonStriker, None
            | NonStriker, false -> Some striker, None
            | NonStriker, true -> None, Some striker
            | Nobody, false -> Some striker, Some nonStriker
            | Nobody, true -> Some nonStriker, Some striker

        let updatedBatsmen =
            state.Batsmen
            |> updateBatsmen (IndividualInnings.update bowler ballOutcome) striker
            |> updateBatsmen (IndividualInnings.updateNonStriker ballOutcome) nonStriker

        let (batsmanAtEnd1, batsmanAtEnd2) =
            match state.EndFacingNext with
            | End1 -> batsmanAtFacingEnd, batsmanAtNonFacingEnd
            | End2 -> batsmanAtNonFacingEnd, batsmanAtFacingEnd

        let ballsThisOver = state.BallsThisOver @ [ ballOutcome ]

        let interimState =
            {
                state with
                    Batsmen =
                        state.Batsmen
                        |> updateBatsmen (IndividualInnings.update bowler ballOutcome) striker
                        |> updateBatsmen (IndividualInnings.updateNonStriker ballOutcome) nonStriker
                    Bowlers =
                        state.Bowlers
                        |> addBowlerIfNeeded bowler
                        |> updateBowlers ballOutcome bowler
                        |> updateBowlersForEndOverIfNeeded overCompleted ballsThisOver bowler
                    BatsmanAtEnd1 = batsmanAtEnd1;
                    BatsmanAtEnd2 = batsmanAtEnd2;
                    EndFacingNext = if overCompleted then state.EndFacingNext.OtherEnd else state.EndFacingNext;
                    OversCompleted = if overCompleted then state.OversCompleted + 1 else state.OversCompleted;
                    BallsThisOver = if overCompleted then [] else ballsThisOver;
            }
        
        let newWicket =
            match whoOut, striker, nonStriker with
            | Striker, batsmanOut, _ | NonStriker, _, batsmanOut ->
                Some { Wicket = 1 + state.GetWickets; Runs = updatedBatsmen |> List.sumBy (fun (_, ii) -> ii.Score); BatsmanOut = batsmanOut; Overs = state.OversCompleted; BallsWithinOver = 1 + state.BallsSoFarThisOver }
            | Nobody, _, _ -> None
        
        match newWicket with
            | Some w -> { interimState with FallOfWickets = state.FallOfWickets @ [ w ] }
            | None -> interimState
        
    let private sendInBatsman (nextBatsman: Player) state =
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

    let private sendInBowler (nextBowler: Player) (state: Innings) =
        if state.BallsSoFarThisOver <> 0 then failwith "cannot change bowler during an over"
        let lastBowler =
            match state.EndFacingNext.OtherEnd with
            | End1 -> state.BowlerToEnd1
            | End2 -> state.BowlerToEnd2
        if state.OversCompleted > 0 && lastBowler = Some nextBowler then failwith "bowler cannot bowl consecutive overs"

        match state.EndFacingNext with
            | End1 -> { state with BowlerToEnd1 = Some nextBowler }
            | End2 -> { state with BowlerToEnd2 = Some nextBowler }

    let private declare state =
        { state with IsDeclared = true }

    let update transition toWin state =
        match transition with
        | Declare -> declare state
        | SendInBatsman nextBatsman -> sendInBatsman nextBatsman state
        | SendInBowler nextBowler -> sendInBowler nextBowler state
        | UpdateForBall ball ->
            let ballRestricedForMatchEnd = BallOutcome.restrictForEndMatch toWin ball
            updateForBall ballRestricedForMatchEnd state

    let summaryState (state: Innings) =
        if state.IsCompleted then
            Completed
        else
            match state.BatsmanAtEnd1, state.BatsmanAtEnd2 with
            | None, None -> BatsmanRequired 1
            | None, Some _ | Some _, None -> BatsmanRequired (state.GetWickets + 2)
            | _ ->
                match state.EndFacingNext, state.BowlerToEnd1, state.BowlerToEnd2 with
                | End1, None, _ -> BowlerRequiredTo End1
                | End2, _, None -> BowlerRequiredTo End2
                | _ -> if state.BallsSoFarThisOver = 0 then EndOver else MidOver
