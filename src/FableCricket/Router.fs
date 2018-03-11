module FableCricket.Router

open Elmish.Browser.Navigation
open Fable.Helpers.React.Props

type Page =
  | CricketPage
  | AveragesPage
  | AboutPage

let toHash page =
  match page with
  | AboutPage -> "#about"
  | AveragesPage -> "#averages"
  | CricketPage -> "#cricket"

let href route =
  Href (toHash route)

let modifyUrl route =
    route |> toHash |> Navigation.modifyUrl

let newUrl route =
    route |> toHash |> Navigation.newUrl

let modifyLocation route =
    Fable.Import.Browser.window.location.href <- toHash route