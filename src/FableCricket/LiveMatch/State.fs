module FableCricket.LiveMatch.State

open Elmish

open Cricket.CricketEngine
open Cricket.MatchRunner
open FableCricket.Extensions
open Types

let init () : Model * Cmd<Msg> =
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
    match msg, model.RunOption with
    | ServerMsg serverMsg, OnClient serverModel ->
        let updatedServerModel =
            MatchRunner.update serverMsg serverModel
        let updatedModel =
            {
                model with
                    RunOption = OnClient updatedServerModel
                    Match = updatedServerModel.Match |> Resolved
                    LivePlayerRecords = updatedServerModel.LivePlayerRecords |> Resolved
                    Series = updatedServerModel.Series |> Resolved
            } |> checkForNewInnings
        updatedModel, Cmd.none
    
    | ToggleInningsExpandedMessage index, _ ->
        { model with
            InningsExpanded = model.InningsExpanded |> List.mapi
                (fun i expanded -> if i = index then not expanded else expanded)
        }, Cmd.none
