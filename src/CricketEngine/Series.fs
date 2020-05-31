namespace Cricket.CricketEngine

type Series =
    {
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
            Team1 = team1
            Team2 = team2
            Team1Wins = 0
            Team2Wins = 0
            Draws = 0
            Ties = 0
        }
    

    let update series result batFirst =
        match result, batFirst with
        | WinTeamA, Team1 | WinTeamB, Team2 -> { series with Team1Wins = series.Team1Wins + 1 }
        | WinTeamB, Team1 | WinTeamA, Team2 -> { series with Team2Wins = series.Team2Wins + 1 }
        | Draw, _ -> { series with Draws = series.Draws + 1 }
        | Tie, _ -> { series with Ties = series.Ties + 1 }
        | NoResult, _ -> series
    
    let totalMatches series =
        series.Team1Wins + series.Team2Wins + series.Draws + series.Ties

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

