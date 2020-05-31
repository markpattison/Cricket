module Cricket.Client.InPlay.State

open Elmish

open Cricket.CricketEngine
open Cricket.MatchRunner
open Cricket.Client
open Cricket.Client.Extensions
open Cricket.Client.InPlay.Types

let serverUpdate sessionId serverMsg = Cmd.OfAsync.perform Server.api.update (sessionId, serverMsg) NewStateReceived
let getStatistics sessionId = Cmd.OfAsync.perform Server.api.getStatistics sessionId StatisticsReceived

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

let initServer (sessionId, mtch) : Model * Cmd<Msg> =
    let model =
        {
            CurrentPage = CricketPage
            Match = Resolved mtch
            LivePlayerRecords = HasNotStartedYet
            InningsExpanded = []
            Series = HasNotStartedYet
            RunOption = OnServer (Resolved sessionId)
        }
    model |> checkForNewInnings, Cmd.none

let update msg model =
    match model.RunOption, msg with
    | _, SwitchPage page ->
        match model.RunOption, page, model.LivePlayerRecords with
        | (OnServer (Resolved sessionId)), AveragesPage, HasNotStartedYet ->
            { model with CurrentPage = page; LivePlayerRecords = InProgress None; Series = InProgress None }, getStatistics sessionId
        | _ ->
            { model with CurrentPage = page }, Cmd.none
    
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

    | OnClient _, StatisticsReceived _
    | OnClient _, NewStateReceived _ ->
        model, Cmd.none   
    
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

    | OnServer _, ServerMsg _
    | OnServer _, NewStateReceived _
    | OnServer _, StatisticsReceived _ ->
        model, Cmd.none

    | _, ToggleInningsExpandedMessage index ->
        { model with
            InningsExpanded = model.InningsExpanded |> List.mapi
                (fun i expanded -> if i = index then not expanded else expanded)
        }, Cmd.none
