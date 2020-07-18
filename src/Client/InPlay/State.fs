module Cricket.Client.InPlay.State

open Elmish

open Cricket.CricketEngine
open Cricket.MatchRunner
open Cricket.Client
open Cricket.Client.Extensions
open Cricket.Client.InPlay.Types

let serverUpdate sessionId serverMsg = Cmd.OfAsync.perform Server.api.update (sessionId, serverMsg) NewStateReceived
let getAverages sessionId = Cmd.OfAsync.perform Server.api.getAverages sessionId AveragesReceived
let getSeries sessionId = Cmd.OfAsync.perform Server.api.getSeries sessionId SeriesReceived
let getCompletedMatch sessionId matchId = Cmd.OfAsync.perform Server.api.getCompletedMatch (sessionId, matchId) CompletedMatchReceived

let checkForNewInnings model =
    match model.Match with
    | Resolved mtch ->
        let numInnings = mtch |> Match.inningsList |> List.length
        let numExpanders = model.InningsExpanded |> List.length
        if numInnings <> numExpanders then
            let newExpanders = List.init numInnings (fun i -> i = numInnings - 1)
            { model with InningsExpanded = newExpanders }
        else
            model
    | _ -> model

let initClient () : Model * Cmd<Msg> =
    {
        CurrentPage = CricketPage
        Match = MatchData.newMatch |> Resolved
        Averages = Map.empty |> Resolved
        InningsExpanded = []
        Series = Series.create "England" "India" |> Resolved
        CompletedMatches = Map.empty
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
                CompletedMatches = Map.empty
            }            
    }, []

let initServer (sessionId, mtch) : Model * Cmd<Msg> =
    let model =
        {
            CurrentPage = CricketPage
            Match = Resolved mtch
            Averages = HasNotStartedYet
            InningsExpanded = []
            Series = HasNotStartedYet
            CompletedMatches = Map.empty
            RunOption = OnServer (Resolved sessionId)
        }
    model |> checkForNewInnings, Cmd.none

let update msg model =
    match model.RunOption, msg with
    | _, SwitchPage AveragesPage ->
        match model.RunOption, model.Averages with
        | (OnServer (Resolved sessionId)), HasNotStartedYet ->
            { model with CurrentPage = AveragesPage; Averages = InProgress None }, getAverages sessionId
        | _ ->
            { model with CurrentPage = AveragesPage }, Cmd.none
    
    | _, SwitchPage (SeriesPage ListMatches) ->
        match model.RunOption, model.Series with
        | (OnServer (Resolved sessionId)), HasNotStartedYet ->
            { model with CurrentPage = SeriesPage ListMatches; Series = InProgress None }, getSeries sessionId
        | _ ->
            { model with CurrentPage = SeriesPage ListMatches }, Cmd.none
    
    | _, SwitchPage (SeriesPage (ShowMatch matchId)) ->
        match model.RunOption, Map.tryFind matchId model.CompletedMatches with
        | (OnServer (Resolved sessionId)), None
        | (OnServer (Resolved sessionId)), Some HasNotStartedYet ->
            { model with
                CurrentPage = SeriesPage (ShowMatch matchId)
                CompletedMatches = Map.add matchId (InProgress None) model.CompletedMatches }, getCompletedMatch sessionId matchId
        | _ ->
            { model with CurrentPage = SeriesPage (ShowMatch matchId) }, Cmd.none
    
    | _, SwitchPage page ->
        { model with CurrentPage = page }, Cmd.none
        
    | OnClient serverModel, ServerMsg serverMsg ->
        let updatedServerModel =
            MatchRunner.update serverMsg serverModel
        let updatedModel =
            { model with
                RunOption = OnClient updatedServerModel
                Match = updatedServerModel.Match |> Resolved
                Averages = updatedServerModel.LivePlayerRecords |> Resolved
                Series = updatedServerModel.Series |> Resolved
                CompletedMatches = updatedServerModel.CompletedMatches |> Map.map (fun _ m -> Resolved m)
            } |> checkForNewInnings
        updatedModel, Cmd.none

    | OnClient _, AveragesReceived _
    | OnClient _, SeriesReceived _
    | OnClient _, CompletedMatchReceived _
    | OnClient _, NewStateReceived _ ->
        model, Cmd.none   
    
    | OnServer (Resolved sessionId), ServerMsg serverMsg ->
        let updatedModel =
            { model with
                Match = updateInProgress model.Match
                Averages = updateInProgress model.Averages
                Series = updateInProgress model.Series
                CompletedMatches = model.CompletedMatches |> Map.map (fun _ m -> updateInProgress m)
            }
        updatedModel, serverUpdate sessionId serverMsg

    | OnServer (Resolved _), NewStateReceived (Ok mtch) ->
        let updatedModel =
            { model with
                Match = Resolved mtch
                Averages = HasNotStartedYet
                Series = HasNotStartedYet
            } |> checkForNewInnings
        updatedModel, Cmd.none       

    | OnServer (Resolved _), NewStateReceived (Error error) ->
        printfn "Error: %s" error
        model, Cmd.none

    | OnServer (Resolved _), AveragesReceived (Ok averages) ->
        let updatedModel = { model with Averages = Resolved averages }
        updatedModel, Cmd.none

    | OnServer (Resolved _), AveragesReceived (Error error) ->
        printfn "Error: %s" error
        model, Cmd.none

    | OnServer (Resolved _), SeriesReceived (Ok series) ->
        let updatedModel = { model with Series = Resolved series }
        updatedModel, Cmd.none

    | OnServer (Resolved _), SeriesReceived (Error error) ->
        printfn "Error: %s" error
        model, Cmd.none
    
    | OnServer (Resolved _), CompletedMatchReceived (Ok completedMatch) ->
        let matchId, mtch = completedMatch
        let updatedModel = { model with CompletedMatches = Map.add matchId (Resolved mtch) model.CompletedMatches }
        updatedModel, Cmd.none
    
    | OnServer (Resolved _), CompletedMatchReceived (Error error) ->
        printfn "Error: %s" error
        model, Cmd.none

    | OnServer _, ServerMsg _
    | OnServer _, NewStateReceived _
    | OnServer _, AveragesReceived _
    | OnServer _, SeriesReceived _
    | OnServer _, CompletedMatchReceived _ ->
        model, Cmd.none

    | _, ToggleInningsExpandedMessage index ->
        { model with
            InningsExpanded = model.InningsExpanded |> List.mapi
                (fun i expanded -> if i = index then not expanded else expanded)
        }, Cmd.none
