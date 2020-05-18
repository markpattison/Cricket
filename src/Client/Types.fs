module Cricket.Client.Types

open Cricket.CricketEngine
open Cricket.MatchRunner
open Cricket.Shared
open Cricket.Client.Extensions

type RunOption =
    | OnClient of ServerModel
    | OnServer of Deferred<SessionId>

type Msg =
    | ServerSessionInitiated of SessionId * DataFromServer
    | ServerMsg of ServerMsg
    | NewStateReceived of Result<DataFromServer, string>
    | StatisticsReceived of Result<Statistics, string>
    | ToggleInningsExpandedMessage of int

type Page =
    | CricketPage
    | AveragesPage
    | AboutPage

type Model =
    {
        CurrentPage: Page
        RunOption: RunOption
        Match: Deferred<Match>
        LivePlayerRecords: Deferred<Map<Player, PlayerRecord>>
        InningsExpanded: bool list
        Series: Deferred<Series>
    }
