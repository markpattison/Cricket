module Cricket.Types

open Cricket.CricketEngine

type Model =
    {
        Match: Match
        InningsExpanded: bool list
    }

type Msg =
    | StartMatch
    | StartNextInnings
    | ContinueInningsBall
    | ContinueInningsOver
    | ResetMatch
    | ToggleInningsExpanded of int