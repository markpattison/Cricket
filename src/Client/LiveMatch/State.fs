module Cricket.Client.LiveMatch.State

open Elmish

open Cricket.CricketEngine
open Cricket.MatchRunner
open Cricket.Client.Extensions
open Types

module Server =

    open Cricket.Shared
    open Fable.Remoting.Client

    let api : ICricketApi =
      Remoting.createApi()
      |> Remoting.withRouteBuilder Route.builder
      |> Remoting.buildProxy<ICricketApi>

let initiateSession = Cmd.OfAsync.perform Server.api.newSession () ServerSessionInitiated
let serverUpdate sessionId serverMsg = Cmd.OfAsync.perform Server.api.update (sessionId, serverMsg) NewStateReceived

let initClient () : Model * Cmd<Msg> =
    {
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
        Match = HasNotStartedYet
        LivePlayerRecords = HasNotStartedYet
        InningsExpanded = []
        Series = HasNotStartedYet
        RunOption = OnServer (InProgress None)
    }, initiateSession

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
    | OnClient _, NewStateReceived _ ->
        model, Cmd.none   
     
    | OnServer InProgress, ServerSessionInitiated (sessionId, (mtch, livePlayerRecords, series)) ->
        let updatedModel =
            { model with
                RunOption = OnServer (Resolved sessionId)
                Match = Resolved mtch
                LivePlayerRecords = Resolved livePlayerRecords
                Series = Resolved series
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

    | OnServer (Resolved _), NewStateReceived (Ok (mtch, livePlayerRecords, series)) ->
        let updatedModel =
            { model with
                Match = Resolved mtch
                LivePlayerRecords = Resolved livePlayerRecords
                Series = Resolved series
            } |> checkForNewInnings
        updatedModel, Cmd.none       

    | OnServer (Resolved _), NewStateReceived (Error error) ->
        printfn "Error: %s" error
        model, Cmd.none

    | OnServer _, ServerSessionInitiated _
    | OnServer _, ServerMsg _
    | OnServer _, NewStateReceived _ ->
        model, Cmd.none

    | _, ToggleInningsExpandedMessage index ->
        { model with
            InningsExpanded = model.InningsExpanded |> List.mapi
                (fun i expanded -> if i = index then not expanded else expanded)
        }, Cmd.none
