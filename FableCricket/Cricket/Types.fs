module Cricket.Types

open Cricket.CricketEngine

type Model =
    {
        Match: Match
    }

type Msg =
    | StartMatch
    | StartNextInnings
    | ContinueInningsBall
    | ContinueInningsOver
    | ResetMatch