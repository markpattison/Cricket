module App.Types

open Global

type Msg =
  | CricketMsg of Cricket.Types.Msg

type Model = {
    currentPage: Page
    cricket: Cricket.Types.Model
  }
