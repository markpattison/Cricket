namespace Cricket.MatchRunner

open Cricket.CricketEngine

module SimpleCaptain =

    let replyModal (team, match', msg) =
        let innings = match'.State |> MatchState.currentInnings
        let teamName =
            match team with
            | TeamA -> match'.TeamA
            | TeamB -> match'.TeamB
        match msg with
        | FollowOnDecision -> EnforceFollowOn
        | NewBatsmanRequired n -> SendInBatsman { Name = sprintf "%s Batsman %i" teamName n } |> UpdateInnings
        | NewBowlerRequiredTo _ ->
            let overs = innings.OversCompleted
            let bowler =
                match (overs / 8) % 2, overs % 2 with
                | 0, 0 -> { Name = sprintf "%s Bowler %i" teamName 10 }
                | 0, 1 -> { Name = sprintf "%s Bowler %i" teamName 11 }
                | 1, 0 -> { Name = sprintf "%s Bowler %i" teamName 8 }
                | 1, 1 -> { Name = sprintf "%s Bowler %i" teamName 9 }
                | _ -> { Name = "???" }
            SendInBowler bowler |> UpdateInnings

    let replyOptional (team, match', msg) =
        let innings = match'.State |> MatchState.currentInnings
        let teamName =
            match team with
            | TeamA -> match'.TeamA
            | TeamB -> match'.TeamB
        match msg with
        | EndOfOver ->
            let overs = innings.OversCompleted
            let bowler =
                match (overs / 8) % 2, overs % 2 with
                | 0, 0 -> { Name = sprintf "%s Bowler %i" teamName 10 }
                | 0, 1 -> { Name = sprintf "%s Bowler %i" teamName 11 }
                | 1, 0 -> { Name = sprintf "%s Bowler %i" teamName 8 }
                | 1, 1 -> { Name = sprintf "%s Bowler %i" teamName 9 }
                | _ -> { Name = "???" }
            SendInBowler bowler |> UpdateInnings |> Some
        | CanDeclare -> None

