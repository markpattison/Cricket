module FableCricket.Global

type Page =
  | CricketPage
  | AveragesPage
  | AboutPage

let toHash page =
  match page with
  | AboutPage -> "#about"
  | AveragesPage -> "#averages"
  | CricketPage -> "#cricket"
