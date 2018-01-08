namespace Cricket.CricketEngine

type HighScore =
    | NoHighScore
    | HighScore of Score: int * Out: bool

type BestBowling =
    | NoBestBowling
    | BestBowling of Wickets: int * Runs: int

type BattingRecord =
    {
        BattingInnings: int
        NotOuts: int
        Runs: int
        HighScore: HighScore
        BallsFaced: int
        Hundreds: int
        Fifties: int
        Ducks: int
        Fours: int
        Sixes: int
    }

type BowlingRecord =
    {
        BowlingInnings: int
        BallsBowled: int
        Maidens: int
        RunsConceded: int
        Wickets: int
        BestInnings: BestBowling
        BestMatch: BestBowling
        FiveWicketInnings: int
        TenWicketMatches: int
    }

type PlayerRecord =
    {
        Matches: int
        MatchInProgress: bool
        Batting: BattingRecord
        Bowling: BowlingRecord
        Catches: int
        Stumpings: int
    }

[<CustomEquality; CustomComparison>]
type BattingAverage =
    {
        Player: Player
        InProgress: bool
        Matches: int
        Innings: int
        NotOuts: int
        Runs: int
        HighScore: HighScore
        Average: float option
        BallsFaced: int
        StrikeRate: float option
        Hundreds: int
        Fifties: int
        Ducks: int
        Fours: int
        Sixes: int
    }

    override _this.Equals(obj2) =
        match obj2 with
        | :? BattingAverage as otherRecord -> (_this.Player.ID = otherRecord.Player.ID)
        | _ -> false
    
    override _this.GetHashCode() = _this.Player.ID
    
    interface System.IComparable with
        member _this.CompareTo otherObj =
            match otherObj with
            | :? BattingAverage as otherRecord ->
                match _this.Average, otherRecord.Average with
                | Some thisAve, Some otherAve ->
                    if thisAve > otherAve then
                        -1
                    elif thisAve < otherAve then
                        1
                    else
                        otherRecord.Runs - _this.Runs
                | Some _, None -> -1
                | None, Some _ -> 1
                | None, None -> otherRecord.Runs - _this.Runs
            | _ -> 0

[<CustomEquality; CustomComparison>]
type BowlingAverage =
    {
        Player: Player
        InProgress: bool
        Matches: int
        Innings: int
        BallsBowled: int
        Maidens: int
        RunsConceded: int
        Wickets: int
        BestInnings: BestBowling
        BestMatch: BestBowling
        Average: float option
        Economy: float option
        StrikeRate: float option
        FiveWicketInnings: int
        TenWicketMatches: int
        Catches: int
        Stumpings: int
    }

    override _this.Equals(obj2) =
        match obj2 with
        | :? BattingAverage as otherRecord -> (_this.Player.ID = otherRecord.Player.ID)
        | _ -> false
    override _this.GetHashCode() = _this.Player.ID

    interface System.IComparable with
        member _this.CompareTo otherObj =
            match otherObj with
            | :? BowlingAverage as otherRecord ->
                match _this.Average, otherRecord.Average with
                | Some thisAve, Some otherAve ->
                    if thisAve < otherAve then
                        -1
                    elif thisAve > otherAve then
                        1
                    else
                        _this.RunsConceded - otherRecord.RunsConceded
                | Some _, None -> -1
                | None, Some _ -> 1
                | None, None -> _this.RunsConceded - otherRecord.RunsConceded
            | _ -> 0

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Averages =

    let createBatting =
        {
            BattingInnings = 0
            NotOuts = 0
            Runs = 0
            HighScore = NoHighScore
            BallsFaced = 0
            Hundreds = 0
            Fifties = 0
            Ducks = 0
            Fours = 0
            Sixes = 0
        }
    
    let createBowling =
        {
            BowlingInnings = 0
            BallsBowled = 0
            Maidens = 0
            RunsConceded = 0
            Wickets = 0
            BestInnings = NoBestBowling
            BestMatch = NoBestBowling
            FiveWicketInnings = 0
            TenWicketMatches = 0
        }

    let create =
        {
            Matches = 0
            MatchInProgress = false
            Batting = createBatting
            Bowling = createBowling
            Catches = 0
            Stumpings = 0
        }
    
    let oneIf predicate = if predicate then 1 else 0

    let updateHighScore previousBest innings =
        match previousBest with
        | HighScore (Score = score; Out = out) when innings.Score < score || (innings.Score = score && not out)
            -> previousBest
        | _ -> HighScore (Score = innings.Score, Out = innings.HowOut.IsSome)

    let updateBattingForInnings batting innings =
        {
            BattingInnings = batting.BattingInnings + 1
            NotOuts = batting.NotOuts + oneIf (innings.HowOut.IsNone)
            Runs = batting.Runs + innings.Score
            HighScore = updateHighScore batting.HighScore innings
            BallsFaced = batting.BallsFaced + innings.BallsFaced
            Hundreds = batting.Hundreds + oneIf (innings.Score >= 100)
            Fifties = batting.Fifties + oneIf (innings.Score >= 50 && innings.Score < 100)
            Ducks = batting.Ducks + oneIf (innings.Score = 0 && innings.HowOut.IsSome)
            Fours = batting.Fours + innings.Fours
            Sixes = batting.Sixes + innings.Sixes
        }
    
    let updateBestBowling previousBest wickets runs =
        match previousBest with
        | BestBowling (Wickets = bestWickets; Runs = bestRuns) when wickets < bestWickets || (wickets = bestWickets && runs > bestRuns)
            -> previousBest
        | _ -> BestBowling (Wickets = wickets, Runs = runs)     

    let updateBowlingForInnings bowling analysis =
        {
            BowlingInnings = bowling.BowlingInnings + 1
            BallsBowled = bowling.BallsBowled + analysis.Balls
            Maidens = bowling.Maidens + analysis.Maidens
            RunsConceded = bowling.RunsConceded + analysis.RunsConceded
            Wickets = bowling.Wickets + analysis.Wickets
            BestInnings = updateBestBowling bowling.BestInnings analysis.Wickets analysis.RunsConceded
            BestMatch = bowling.BestMatch // have to update this at match level
            FiveWicketInnings = bowling.FiveWicketInnings + oneIf (analysis.Wickets >= 5)
            TenWicketMatches = bowling.TenWicketMatches // have to update this at match level
        }
    
    let private updatePlayerForAllMatchInnings player playerRecord mtch isInProgress =
        let allInnings = Match.inningsList mtch |> List.map (fun (_, _, inn) -> inn)
        let allBatting =
            allInnings
            |> List.collect (fun inn -> inn.Batsmen)
        let battingInnings =
            allBatting
            |> List.filter (fun (batsman, _) -> batsman = player)
            |> List.map snd
        let bowlingAnalyses =
            allInnings
            |> List.collect (fun inn -> inn.Bowlers)
            |> List.filter (fun (bowler, _) -> bowler = player)
            |> List.map snd
        let matchWickets = bowlingAnalyses |> List.sumBy (fun ba -> ba.Wickets)
        let matchRunsCondeded = bowlingAnalyses |> List.sumBy (fun ba -> ba.RunsConceded)
        let bowlingRecordUpdatedForMatch =
            { playerRecord.Bowling with
                BestMatch = (updateBestBowling playerRecord.Bowling.BestMatch matchWickets matchRunsCondeded)
                TenWicketMatches = playerRecord.Bowling.TenWicketMatches + oneIf (matchWickets >= 10)
            }            
        let catches =
            allBatting
            |> List.sumBy (fun (_, ii) ->
                match ii.HowOut with
                | Some (OutCaught (BowledBy = _; CaughtBy = fielder)) when fielder = player -> 1
                | _ -> 0)
        let stumpings =
            allBatting
            |> List.sumBy (fun (_, ii) ->
                match ii.HowOut with
                | Some (OutStumped (BowledBy = _; StumpedBy = fielder)) when fielder = player -> 1
                | _ -> 0)
        {
            Matches = playerRecord.Matches + 1
            MatchInProgress = isInProgress
            Batting = List.fold updateBattingForInnings playerRecord.Batting battingInnings
            Bowling = List.fold updateBowlingForInnings bowlingRecordUpdatedForMatch bowlingAnalyses
            Catches = playerRecord.Catches + catches
            Stumpings = playerRecord.Stumpings + stumpings
        }

    let updatePlayerForMatch player playerRecord mtch =
        match playerRecord.MatchInProgress, MatchState.summaryStateForPlayerRecords mtch.State with
        | _, NoMatch
            -> playerRecord
        | true, _
            -> failwith "Player cannot have two matches in progress"
        | false, InProgress
            -> updatePlayerForAllMatchInnings player playerRecord mtch true
        | false, Completed
            -> updatePlayerForAllMatchInnings player playerRecord mtch false
    
    let updatePlayersForMatch playerRecords mtch =
        match MatchState.summaryStateForPlayerRecords mtch.State with
        | NoMatch -> playerRecords
        | _ ->
            let matchPlayers =
                Array.append mtch.TeamA.Players mtch.TeamB.Players
                |> Array.toList
            let newPlayers =
                matchPlayers
                |> List.filter (fun player -> not (Map.containsKey player playerRecords))
                |> List.map (fun player -> (player, create))
            let recordsWithNewPlayers =
                List.concat [ Map.toList playerRecords; newPlayers ]
                |> Map.ofList
            recordsWithNewPlayers
            |> Map.map (fun player record ->
                if List.contains player matchPlayers then
                    updatePlayerForMatch player record mtch
                else
                    record)

    let createBattingAverage (player, playerRecord) =
        let batting = playerRecord.Batting
        let outs = batting.BattingInnings - batting.NotOuts
        {
            Player = player
            InProgress = playerRecord.MatchInProgress
            BattingAverage.Matches = playerRecord.Matches
            Innings = batting.BattingInnings
            NotOuts = batting.NotOuts
            Runs = batting.Runs
            HighScore = batting.HighScore
            Average = if outs = 0 then None else Some (float batting.Runs / float outs)
            BallsFaced = batting.BallsFaced
            StrikeRate = if batting.BallsFaced = 0 then None else Some (100.0 * float batting.Runs / float batting.BallsFaced)
            Hundreds = batting.Hundreds
            Fifties = batting.Fifties
            Ducks = batting.Ducks
            Fours = batting.Fours
            Sixes = batting.Sixes
        }
    
    let createBowlingAverage (player, playerRecord) =
        let bowling = playerRecord.Bowling
        {
            Player = player
            InProgress = playerRecord.MatchInProgress
            Matches = playerRecord.Matches
            Innings = bowling.BowlingInnings
            BallsBowled = bowling.BallsBowled
            Maidens = bowling.Maidens
            RunsConceded = bowling.RunsConceded
            Wickets = bowling.Wickets
            BestInnings = bowling.BestInnings
            BestMatch = bowling.BestMatch
            Average = if bowling.Wickets = 0 then None else Some (float bowling.RunsConceded / float bowling.Wickets)
            Economy = if bowling.BallsBowled = 0 then None else Some (6.0 * float bowling.RunsConceded / float bowling.BallsBowled)
            StrikeRate = if bowling.Wickets = 0 then None else Some (float bowling.BallsBowled / float bowling.Wickets)
            FiveWicketInnings = bowling.FiveWicketInnings
            TenWicketMatches = bowling.TenWicketMatches
            Catches = playerRecord.Catches
            Stumpings = playerRecord.Stumpings
        }
    
    let blank = "-"

    let formatHighScore highScore =
        match highScore with
        | NoHighScore -> blank
        | HighScore (score, true) -> sprintf "%i" score
        | HighScore (score, false) -> sprintf "%i*" score
    
    let formatAverage average =
        match average with
        | None -> blank
        | Some ave -> sprintf "%.2f" ave
    
    let formatBestBowling bestBowling =
        match bestBowling with
        | NoBestBowling -> blank
        | BestBowling (wickets, runs) -> sprintf "%i/%i" wickets runs
    
    let formatMatches matches isInProgress =
        match isInProgress with
        | false -> sprintf "%i" matches
        | true -> sprintf "%i*" matches
