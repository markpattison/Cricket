module FableCricket.App.Types

open FableCricket.Global

type Msg =
  | CricketMsg of FableCricket.LiveMatch.Types.Msg

type Model = {
    currentPage: Page
    cricket: FableCricket.LiveMatch.Types.Model
  }
