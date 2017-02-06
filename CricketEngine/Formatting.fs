namespace Cricket.CricketEngine

module Formatting =

    let formatRuns runs =
        match runs with
        | 1 -> "1 run"
        | n when n > 1 -> sprintf "%i runs" n
        | _ -> failwith "Invalid number of runs"

    let formatWickets wickets =
        match wickets with
        | 1 -> "1 wicket"
        | n when n > 1 -> sprintf "%i wickets" n
        | _ -> failwith "Invalid number of wickets"

    let formatWicketsLeft wickets =
        formatWickets (10 - wickets)
