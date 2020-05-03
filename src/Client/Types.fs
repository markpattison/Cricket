module Cricket.Client.Types

open Router

type Msg =
  | CricketMsg of LiveMatch.Types.Msg

type Model = {
    currentPage: Page
    cricket: LiveMatch.Types.Model
  }
