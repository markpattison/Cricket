module Cricket.Types

open Cricket.CricketEngine

type Model =
    {
        Match: Match
        PlayerRecords: Map<Player, PlayerRecord>
        InningsExpanded: bool list
    }

type Msg =
    | StartMatch
    | StartNextInnings
    | ContinueInningsBall
    | ContinueInningsOver
    | ResetMatch
    | ToggleInningsExpanded of int