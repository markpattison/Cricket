namespace Cricket.MatchRunner

open Cricket.CricketEngine

module SimpleCaptain =

    let replyModal ((teamChoice, msg), match') =
        let team =
            match teamChoice with
            | TeamA -> match'.TeamA
            | TeamB -> match'.TeamB
        match msg with
        | FollowOnDecision -> EnforceFollowOn
        | NewBatsmanRequired n -> SendInBatsman team.Players.[n - 1] |> UpdateInnings
        | NewBowlerRequiredTo _ ->
            let innings = match'.State |> MatchState.currentInnings
            let overs = innings.OversCompleted
            let bowler =
                match (overs / 8) % 2, overs % 2 with
                | 0, 0 -> team.Players.[9]
                | 0, 1 -> team.Players.[10]
                | 1, 0 -> team.Players.[7]
                | 1, 1 -> team.Players.[8]
                | _ -> { Name = "???" }
            SendInBowler bowler |> UpdateInnings

    let replyOptional ((teamChoice, msg), match') =
        let team =
            match teamChoice with
            | TeamA -> match'.TeamA
            | TeamB -> match'.TeamB
        match msg with
        | EndOfOver ->
            let innings = match'.State |> MatchState.currentInnings
            let overs = innings.OversCompleted
            match (overs / 8) % 2, overs % 2 with
            | 0, 0 -> team.Players.[9] |> SendInBowler |> UpdateInnings |> Some
            | 0, 1 -> team.Players.[10] |> SendInBowler |> UpdateInnings |> Some
            | 1, 0 -> team.Players.[7] |> SendInBowler |> UpdateInnings |> Some
            | 1, 1 -> team.Players.[8] |> SendInBowler |> UpdateInnings |> Some
            | _ -> None
        | CanDeclare -> None

