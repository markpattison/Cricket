module FableCricket.LiveMatch.Types

open Cricket.CricketEngine
open Cricket.MatchRunner

type Model =
    {
        Match: Match
        PlayerRecords: Map<Player, PlayerRecord>
        LivePlayerRecords: Map<Player, PlayerRecord>
        InningsExpanded: bool list
        Series: Series
        PlayerAttributes: PlayerAttributes
    }

type Msg =
    | StartMatchMessage
    | StartNextInningsMessage
    | ContinueInningsBallMessage
    | ContinueInningsOverMessage
    | ContinueInningsInningsMessage
    | ResetMatchMessage
    | ToggleInningsExpandedMessage of int