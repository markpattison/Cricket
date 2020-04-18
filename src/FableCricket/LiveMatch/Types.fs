module FableCricket.LiveMatch.Types

open Cricket.CricketEngine
open Cricket.MatchRunner

type Model =
    {
        ServerModel: ServerModel // TODO: eventually remove
        Match: Match // TODO: defer
        LivePlayerRecords: Map<Player, PlayerRecord> // TODO: defer
        InningsExpanded: bool list
        Series: Series // TODO: defer
    }

type Msg =
    | ServerMsg of ServerMsg
    | ToggleInningsExpandedMessage of int