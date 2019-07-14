module FableCricket.Info.View

open Fable.React

open Fulma

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
