module App.Types

open Global

type Msg =
  | CounterMsg of Counter.Types.Msg
  | CricketMsg of Cricket.Types.Msg
  | HomeMsg of Home.Types.Msg

type Model = {
    currentPage: Page
    counter: Counter.Types.Model
    cricket: Cricket.Types.Model
    home: Home.Types.Model
  }
