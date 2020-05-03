namespace Cricket.MatchRunner

open Cricket.CricketEngine

module MatchData =

    let players =
        [
            "GA Gooch",      1.00, 0.50
            "MA Atherton",   1.05, 0.60
            "DI Gower",      1.00, 0.50
            "AJ Lamb",       0.90, 0.50
            "RA Smith",      0.95, 0.50
            "JE Morris",     0.90, 0.50
            "RC Russell",    0.85, 0.50
            "CC Lewis",      0.80, 0.70
            "EE Hemmings",   0.70, 0.65
            "ARC Fraser",    0.65, 0.75
            "DE Malcolm",    0.60, 0.70
            "RJ Shastri",    0.95, 0.70
            "NS Sidhu",      0.90, 0.70
            "SV Manjrekar",  1.00, 0.70
            "DB Vengsarkar", 0.90, 0.70
            "M Azharuddin",  1.00, 0.70
            "SR Tendulkar",  1.05, 0.70
            "M Prabhakar",   0.90, 0.70
            "N Kapil Dev",   0.85, 0.75
            "KS More",       0.80, 0.70
            "SK Sharma",     0.70, 0.65
            "ND Hirwani",    0.65, 0.70
        ]

    let newMatch =
        {
            TeamA =
                {
                    TeamName = "England"
                    Players =
                        players
                        |> List.take 11
                        |> List.mapi (fun n (name, _, _) -> { Name = name; ID = n })
                        |> List.toArray
                }
            TeamB =
                {
                    TeamName = "India"
                    Players =
                        players
                        |> List.skip 11
                        |> List.take 11
                        |> List.mapi (fun n (name, _, _) -> { Name = name; ID = 11 + n })
                        |> List.toArray
                }
            State = NotStarted
            Rules = { FollowOnMargin = 200 }
        }
    