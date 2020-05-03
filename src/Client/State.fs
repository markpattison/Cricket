module Cricket.Client.State

open Browser
open Elmish
open Elmish.UrlParser

open Cricket.Client.Router
open Cricket.Client.Types

let pageParser: Parser<Page->Page,Page> =
  oneOf [
    map AboutPage (s "about")
    map AveragesPage (s "averages")
    map CricketPage (s "cricket")
  ]

let urlUpdate (result: Option<Page>) model =
  match result with
  | None ->
    console.error("Error parsing url")
    model, modifyUrl model.currentPage
  | Some page ->
      { model with currentPage = page }, []

let init result =
  let (cricket, cricketCmd) = LiveMatch.State.initServer()
  let (model, cmd) =
    urlUpdate result
      { currentPage = CricketPage
        cricket = cricket }
  model, Cmd.batch [ cmd
                     Cmd.map CricketMsg cricketCmd ]

let update msg model =
  match msg with
  | CricketMsg msg ->
      let (cricket, cricketCmd) = LiveMatch.State.update msg model.cricket
      { model with cricket = cricket }, Cmd.map CricketMsg cricketCmd
