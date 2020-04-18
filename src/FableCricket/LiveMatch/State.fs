module FableCricket.LiveMatch.State

open Cricket.CricketEngine
open Cricket.MatchRunner

open Elmish

open Types

let init () : Model * Cmd<Msg> =
    {
        Match = MatchData.newMatch
        LivePlayerRecords = Map.empty
        InningsExpanded = []
        Series = Series.create "England" "India"
        ServerModel =
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
    let numInnings = model.Match |> Match.inningsList |> List.length
    let numExpanders = model.InningsExpanded |> List.length
    if numInnings > numExpanders then
        let newExpanders = List.init numInnings (fun i -> i = numInnings - 1)
        { model with InningsExpanded = newExpanders }
    else
        model

let update msg model =
    match msg with
    | ServerMsg serverMsg ->
        let updatedServerModel =
            MatchRunner.update serverMsg model.ServerModel
        let updatedModel =
            {
                model with
                    ServerModel = updatedServerModel
                    Match = updatedServerModel.Match
                    LivePlayerRecords = updatedServerModel.LivePlayerRecords
                    Series = updatedServerModel.Series
            } |> checkForNewInnings
        updatedModel, Cmd.none
    
    | ToggleInningsExpandedMessage index ->
        { model with
            InningsExpanded = model.InningsExpanded |> List.mapi
                (fun i expanded -> if i = index then not expanded else expanded)
        }, Cmd.none
