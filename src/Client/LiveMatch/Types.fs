module Cricket.Client.LiveMatch.Types

open Cricket.CricketEngine
open Cricket.MatchRunner
open Cricket.Shared
open Cricket.Client.Extensions

type RunOption =
    | OnClient of ServerModel
    | OnServer of Deferred<SessionId>

type Model =
    {
        RunOption: RunOption
        Match: Deferred<Match>
        LivePlayerRecords: Deferred<Map<Player, PlayerRecord>>
        InningsExpanded: bool list
        Series: Deferred<Series>
    }

type Msg =
    | ServerSessionInitiated of SessionId * DataFromServer
    | ServerMsg of ServerMsg
    | NewStateReceived of Result<DataFromServer, string>
    | ToggleInningsExpandedMessage of int
