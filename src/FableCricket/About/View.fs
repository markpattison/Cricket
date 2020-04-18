module FableCricket.About

open Fable.React

open Fulma

open FableCricket

let view =
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
