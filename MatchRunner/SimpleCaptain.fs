namespace Cricket.MatchRunner

open Cricket.CricketEngine

module SimpleCaptain =

    let replyModal ((team, msg), match') =
        let teamName =
            match team with
            | TeamA -> match'.TeamA
            | TeamB -> match'.TeamB
        match msg with
        | FollowOnDecision -> EnforceFollowOn
        | NewBatsmanRequired n -> SendInBatsman { Name = sprintf "%s Batsman %i" teamName n } |> UpdateInnings
        | NewBowlerRequiredTo _ ->
            let innings = match'.State |> MatchState.currentInnings
            let overs = innings.OversCompleted
            let bowler =
                match (overs / 8) % 2, overs % 2 with
                | 0, 0 -> { Name = sprintf "%s Bowler %i" teamName 10 }
                | 0, 1 -> { Name = sprintf "%s Bowler %i" teamName 11 }
                | 1, 0 -> { Name = sprintf "%s Bowler %i" teamName 8 }
                | 1, 1 -> { Name = sprintf "%s Bowler %i" teamName 9 }
                | _ -> { Name = "???" }
            SendInBowler bowler |> UpdateInnings

    let replyOptional ((team, msg), match') =
        let teamName =
            match team with
            | TeamA -> match'.TeamA
            | TeamB -> match'.TeamB
        match msg with
        | EndOfOver ->
            let innings = match'.State |> MatchState.currentInnings
            let overs = innings.OversCompleted
            match (overs / 8) % 2, overs % 2 with
            | 0, 0 -> { Name = sprintf "%s Bowler %i" teamName 10 } |> SendInBowler |> UpdateInnings |> Some
            | 0, 1 -> { Name = sprintf "%s Bowler %i" teamName 11 } |> SendInBowler |> UpdateInnings |> Some
            | 1, 0 -> { Name = sprintf "%s Bowler %i" teamName 8 } |> SendInBowler |> UpdateInnings |> Some
            | 1, 1 -> { Name = sprintf "%s Bowler %i" teamName 9 } |> SendInBowler |> UpdateInnings |> Some
            | _ -> None
        | CanDeclare -> None

