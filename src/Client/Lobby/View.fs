module Cricket.Client.Lobby.View

open Fable.Core.JsInterop
open Fable.React
open Fulma

open Cricket.Shared

let simpleButton dispatch isDisabled txt action =
  Control.div []
    [ Button.button
        [ if not isDisabled then Button.Color IsInfo
          Button.OnClick (fun e ->
                            e.preventDefault()
                            action |> dispatch)
          Button.Disabled isDisabled ]
        [ str txt ] ]

let view lobbyModel dispatch =
  let content =
    match lobbyModel.State with
    | NewServerGameReady (SessionId guid, _) ->
      Field.div []
        [ h2 [] [ str "New game on server" ]
          str (sprintf "Keep a copy of the following game ID for future access (this can be found later in the About tab).")
          Field.div []
            [ Label.label [] [ str "Game ID" ]
              Input.text
                [ Input.IsReadOnly true
                  Input.Value (guid.ToString()) ] ]
          br []
          simpleButton dispatch false "Start" StartNewServerGame ]
    | _ ->
      let buttonsDisabled =
        match lobbyModel.State with
        | Ready -> false
        | _ -> true
      
      Field.div []
        [ h1 [] [ str "Play cricket!" ]
          p [] [ simpleButton dispatch buttonsDisabled "Play in browser" ClientSessionInitiated; str "Game will be lost once browser tab is closed" ]
          p [] [ simpleButton dispatch buttonsDisabled "New game on server" RequestNewServerGame; str "Create a new persistent game" ]
          p [] [ simpleButton dispatch buttonsDisabled "Load game on server" RequestLoadServerGame; str "Continue a persistent game using the game ID below" ]
          Field.div []
            [ Label.label [] [ str "Game ID" ]
              Input.text
                [ Input.OnChange (fun ev -> dispatch (SessionIdTextChanged !!ev.target?value))
                  Input.Value lobbyModel.SessionIdText ] ]
          if lobbyModel.LoadSessionError then str "Invalid game ID"
          br []
          br []
          Cricket.Client.About.view "" ]
  
  Content.content []
    [ content ]