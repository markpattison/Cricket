module FableCricket.Info.View

open Fable.Helpers.React

open Fulma.Elements

open FableCricket

let root =
  Content.content []
    [ h1
        [ ]
        [ str "About FableCricket" ]
      p
        [ ]
        [ str "This is a simple cricket game built with F# + Fable + Elmish + React + Fulma." ]
      p
        [ ]
        [ str ("Version " + App.Version.VersionNumber) ] ]
