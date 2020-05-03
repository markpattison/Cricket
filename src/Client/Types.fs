module Cricket.Client.Types

open Router

type Msg =
  | CricketMsg of LiveMatch.Types.Msg

type Model = {
    CurrentPage: Page
    Cricket: LiveMatch.Types.Model
  }
