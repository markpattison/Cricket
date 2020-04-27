module FableCricket.LiveMatch.Types

open Cricket.CricketEngine
open Cricket.MatchRunner
open FableCricket.Extensions

type RunOption =
    | OnClient of ServerModel

type Model =
    {
        RunOption: RunOption
        Match: Deferred<Match>
        LivePlayerRecords: Deferred<Map<Player, PlayerRecord>>
        InningsExpanded: bool list
        Series: Deferred<Series>
    }

type Msg =
    | ServerMsg of ServerMsg
    | ToggleInningsExpandedMessage of int
