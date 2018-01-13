module FableCricket.LiveMatch.Types

open Cricket.CricketEngine

type Model =
    {
        Match: Match
        PlayerRecords: Map<Player, PlayerRecord>
        LivePlayerRecords: Map<Player, PlayerRecord>
        InningsExpanded: bool list
        Series: Series
    }

type Msg =
    | StartMatchMessage
    | StartNextInningsMessage
    | ContinueInningsBallMessage
    | ContinueInningsOverMessage
    | ResetMatchMessage
    | ToggleInningsExpandedMessage of int