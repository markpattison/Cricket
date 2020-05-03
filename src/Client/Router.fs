module Cricket.Client.Router

open Browser
open Elmish.Navigation
open Fable.React.Props

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
    window.location.href <- toHash route