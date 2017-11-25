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
        TenWicketMatch: int
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

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix )>]
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
            TenWicketMatch = 0
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
            TenWicketMatch = bowling.TenWicketMatch // have to update this at match level
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
                TenWicketMatch = playerRecord.Bowling.TenWicketMatch + oneIf (matchWickets >= 10)
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
