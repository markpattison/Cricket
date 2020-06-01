namespace Cricket.CricketEngine

type MatchRecord =
    {
        Index: int
        Summary: string
    }

type Series =
    {
        CompletedMatches: MatchRecord list
        Team1: string
        Team2: string
        Team1Wins: int
        Team2Wins: int
        Draws: int
        Ties: int
    }

type BatFirst = Team1 | Team2

module Series =

    let create team1 team2 =
        {
            CompletedMatches = []
            Team1 = team1
            Team2 = team2
            Team1Wins = 0
            Team2Wins = 0
            Draws = 0
            Ties = 0
        }
    
    let totalMatches series =
        series.Team1Wins + series.Team2Wins + series.Draws + series.Ties    

    let update series mtch batFirst =
        let summary = Match.summaryStatus mtch
        let index = 1 + totalMatches series

        let completed = List.append series.CompletedMatches [ { Index = index; Summary = summary } ]

        let result =
            match MatchState.summaryState mtch.State with
            | MatchCompleted result -> result
            | _ -> NoResult
        
        match result, batFirst with
        | WinTeamA, Team1 | WinTeamB, Team2 -> { series with CompletedMatches = completed; Team1Wins = series.Team1Wins + 1 }
        | WinTeamB, Team1 | WinTeamA, Team2 -> { series with CompletedMatches = completed; Team2Wins = series.Team2Wins + 1 }
        | Draw, _ -> { series with CompletedMatches = completed; Draws = series.Draws + 1 }
        | Tie, _ -> { series with CompletedMatches = completed; Ties = series.Ties + 1 }
        | NoResult, _ -> { series with CompletedMatches = completed }

    let summary series =
        let wins =
            match series.Team1Wins - series.Team2Wins with
            | x when x > 0 -> sprintf "%s lead %s %i-%i" series.Team1 series.Team2 series.Team1Wins series.Team2Wins
            | x when x < 0 -> sprintf "%s lead %s %i-%i" series.Team2 series.Team1 series.Team2Wins series.Team1Wins
            | _ -> sprintf "%s and %s are level %i-%i" series.Team1 series.Team2 series.Team1Wins series.Team2Wins
        let drawsTies =
            match series.Draws, series.Ties with
            | 0, 0 -> ""
            | 0, t -> sprintf " with %i ties" t
            | d, 0 -> sprintf " with %i draws" d
            | d, t -> sprintf " with %i ties and %i draws" t d
        wins + drawsTies

