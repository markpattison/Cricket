module Cricket.Client.Lobby.View

open Fable.Core.JsInterop
open Fable.React
open Fulma

open Cricket.Shared

let simpleButton dispatch isDisabled txt action =
  Control.div []
    [ Button.button
        [ Button.OnClick (fun e ->
                            e.preventDefault()
                            action |> dispatch)
          Button.Disabled isDisabled ]
        [ str txt ] ]

let view lobbyModel dispatch =
  let buttonsDisabled =
    match lobbyModel.State with
    | Ready -> false
    | _ -> true
  
  Content.content []
    [ h1 [] [ str "Play cricket!" ]
      Field.div []
        [ simpleButton dispatch buttonsDisabled "Play in browser" ClientSessionInitiated
          br []
          simpleButton dispatch buttonsDisabled "New game on server" RequestNewServerGame
          match lobbyModel.State with
          | NewServerGameReady (SessionId guid, _) ->
            str (sprintf "Keep a copy of this game ID for future access: %O" guid)
            simpleButton dispatch false "Start!" StartNewServerGame
          | _ -> ()
          br []
          simpleButton dispatch buttonsDisabled "Load game on server" RequestLoadServerGame
          Input.text
            [ Input.OnChange (fun ev -> dispatch (SessionIdTextChanged !!ev.target?value))
              Input.Value lobbyModel.SessionIdText ]
          if lobbyModel.LoadSessionError then str "Invalid game ID" ]
      hr []
      Cricket.Client.About.view ""
    ]