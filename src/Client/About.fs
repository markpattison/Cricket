module Cricket.Client.About

open Fable.React

open Fulma

open Cricket.Client

let view extraText =
  Content.content []
    [ h1 [] [ str "About FableCricket" ]
      p [] [ str "This is a simple cricket game built with F# + Fable + Elmish + React + Fulma." ]
      p [] [ str extraText ]
      p [] [ str ("Version " + Version.VersionNumber) ] ]
