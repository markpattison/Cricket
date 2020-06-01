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
    | SeriesPage
    | AboutPage

type Msg =
    | SwitchPage of Page
    | ServerMsg of ServerMsg
    | NewStateReceived of Result<DataFromServer, string>
    | AveragesReceived of Result<Averages, string>
    | SeriesReceived of Result<Series, string>
    | ToggleInningsExpandedMessage of int

type Model =
    {
        CurrentPage: Page
        RunOption: RunOption
        Match: Deferred<Match>
        Averages: Deferred<Averages>
        InningsExpanded: bool list
        Series: Deferred<Series>
    }
