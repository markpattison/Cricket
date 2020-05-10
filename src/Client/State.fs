module Cricket.Client.State

open Browser
open Elmish
open Elmish.UrlParser

open Cricket.CricketEngine
open Cricket.MatchRunner
open Cricket.Client.Extensions
open Cricket.Client.Router
open Cricket.Client.Types

let pageParser: Parser<Page -> Page, Page> =
  oneOf [
    map AboutPage (s "about")
    map AveragesPage (s "averages")
    map CricketPage (s "cricket")
  ]


module Server =

    open Cricket.Shared
    open Fable.Remoting.Client

    let api : ICricketApi =
      Remoting.createApi()
      |> Remoting.withRouteBuilder Route.builder
      |> Remoting.buildProxy<ICricketApi>

let initiateSession = Cmd.OfAsync.perform Server.api.newSession () ServerSessionInitiated
let serverUpdate sessionId serverMsg = Cmd.OfAsync.perform Server.api.update (sessionId, serverMsg) NewStateReceived
let getStatistics sessionId = Cmd.OfAsync.perform Server.api.getStatistics sessionId StatisticsReceived

let urlUpdate (result: Option<Page>) model =
  match result with
  | None ->
    console.error("Error parsing url")
    model, modifyUrl model.CurrentPage

  | Some page ->
      match model.RunOption, page, model.LivePlayerRecords with
      | (OnServer (Resolved sessionId)), AveragesPage, HasNotStartedYet ->
          { model with CurrentPage = page; LivePlayerRecords = InProgress None; Series = InProgress None }, getStatistics sessionId
      | _ ->
          { model with CurrentPage = page }, Cmd.none

let initClient () : Model * Cmd<Msg> =
    {
        CurrentPage = CricketPage
        Match = MatchData.newMatch |> Resolved
        LivePlayerRecords = Map.empty |> Resolved
        InningsExpanded = []
        Series = Series.create "England" "India" |> Resolved
        RunOption = OnClient
            {   
                Match = MatchData.newMatch
                PlayerRecords = Map.empty
                LivePlayerRecords = Map.empty
                PlayerAttributes =
                    {
                        Attributes =
                            MatchData.players
                            |> List.mapi (fun n (_, bat, bowl) -> n, { BattingSkill = bat; BowlingSkill = bowl })
                            |> Map.ofList
                    }
                Series = Series.create "England" "India"
            }            
    }, []

let initServer () : Model * Cmd<Msg> =
    {
        CurrentPage = CricketPage
        Match = HasNotStartedYet
        LivePlayerRecords = HasNotStartedYet
        InningsExpanded = []
        Series = HasNotStartedYet
        RunOption = OnServer (InProgress None)
    }, initiateSession

let init result =
  let (initialState, initialCommand) = initServer()
  let (model, cmd) = urlUpdate result initialState
  
  model, Cmd.batch [ cmd; initialCommand ]

let checkForNewInnings model =
    match model.Match with
    | Resolved mtch ->
        let numInnings = mtch |> Match.inningsList |> List.length
        let numExpanders = model.InningsExpanded |> List.length
        if numInnings > numExpanders then
            let newExpanders = List.init numInnings (fun i -> i = numInnings - 1)
            { model with InningsExpanded = newExpanders }
        else
            model
    | _ -> model

let update msg model =
    match model.RunOption, msg with
    | OnClient serverModel, ServerMsg serverMsg ->
        let updatedServerModel =
            MatchRunner.update serverMsg serverModel
        let updatedModel =
            { model with
                RunOption = OnClient updatedServerModel
                Match = updatedServerModel.Match |> Resolved
                LivePlayerRecords = updatedServerModel.LivePlayerRecords |> Resolved
                Series = updatedServerModel.Series |> Resolved
            } |> checkForNewInnings
        updatedModel, Cmd.none

    | OnClient _, ServerSessionInitiated _
    | OnClient _, StatisticsReceived _
    | OnClient _, NewStateReceived _ ->
        model, Cmd.none   
     
    | OnServer InProgress, ServerSessionInitiated (sessionId, mtch) ->
        let updatedModel =
            { model with
                RunOption = OnServer (Resolved sessionId)
                Match = Resolved mtch
                LivePlayerRecords = HasNotStartedYet
                Series = HasNotStartedYet
            }
        updatedModel |> checkForNewInnings, Cmd.none
    
    | OnServer (Resolved sessionId), ServerMsg serverMsg ->
        let updatedModel =
            { model with
                Match = updateInProgress model.Match
                LivePlayerRecords = updateInProgress model.LivePlayerRecords
                Series = updateInProgress model.Series
            }
        updatedModel, serverUpdate sessionId serverMsg

    | OnServer (Resolved _), NewStateReceived (Ok mtch) ->
        let updatedModel =
            { model with
                Match = Resolved mtch
                LivePlayerRecords = HasNotStartedYet
                Series = HasNotStartedYet
            } |> checkForNewInnings
        updatedModel, Cmd.none       

    | OnServer (Resolved _), NewStateReceived (Error error) ->
        printfn "Error: %s" error
        model, Cmd.none

    | OnServer (Resolved _), StatisticsReceived (Ok stats) ->
        let updatedModel = { model with LivePlayerRecords = Resolved stats.LivePlayerRecords; Series = Resolved stats.Series }
        updatedModel, Cmd.none

    | OnServer (Resolved _), StatisticsReceived (Error error) ->
        printfn "Error: %s" error
        model, Cmd.none

    | OnServer _, ServerSessionInitiated _
    | OnServer _, ServerMsg _
    | OnServer _, NewStateReceived _
    | OnServer _, StatisticsReceived _ ->
        model, Cmd.none

    | _, ToggleInningsExpandedMessage index ->
        { model with
            InningsExpanded = model.InningsExpanded |> List.mapi
                (fun i expanded -> if i = index then not expanded else expanded)
        }, Cmd.none
