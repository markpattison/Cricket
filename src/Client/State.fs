module Cricket.Client.State

open Browser
open Elmish
open Elmish.UrlParser

open Cricket.CricketEngine
open Cricket.MatchRunner
open Cricket.Client.Extensions
open Cricket.Client.Lobby
open Cricket.Client.Types

let init () =
    { OuterState = Lobby StartPage }, Cmd.none

let update msg model =
    match model.OuterState, msg with
    | Lobby, LobbyMsg StartOnClient ->
        let cricketModel, cmd = CricketState.initClient()
        { model with OuterState = Playing cricketModel }, Cmd.map CricketMsg cmd
    | Lobby, LobbyMsg StartOnServer ->
        let cricketModel, cmd = CricketState.initServer()
        { model with OuterState = Playing cricketModel }, Cmd.map CricketMsg cmd
    | Lobby, LobbyMsg (SwitchPage page) ->
        { model with OuterState = Lobby page }, Cmd.none
    | Lobby, _ ->
        model, Cmd.none
    
    | Playing cricketModel, CricketMsg cricketMsg ->
        let updatedCricketModel, cmd = CricketState.update cricketMsg cricketModel
        { model with OuterState = Playing updatedCricketModel }, Cmd.map CricketMsg cmd
    | Playing _, _ ->
        model, Cmd.none
