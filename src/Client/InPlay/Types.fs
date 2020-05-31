module Cricket.Client.InPlay.Types

open Cricket.CricketEngine
open Cricket.MatchRunner
open Cricket.Shared
open Cricket.Client.Extensions

type RunOption =
    | OnClient of ServerModel
    | OnServer of Deferred<SessionId>

type Page =
    | CricketPage
    | AveragesPage
    | AboutPage

type Msg =
    | SwitchPage of Page
    | ServerMsg of ServerMsg
    | NewStateReceived of Result<DataFromServer, string>
    | StatisticsReceived of Result<Statistics, string>
    | ToggleInningsExpandedMessage of int

type Model =
    {
        CurrentPage: Page
        RunOption: RunOption
        Match: Deferred<Match>
        LivePlayerRecords: Deferred<Map<Player, PlayerRecord>>
        InningsExpanded: bool list
        Series: Deferred<Series>
    }
