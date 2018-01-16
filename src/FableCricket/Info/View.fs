module FableCricket.Info.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

open FableCricket

let root =
  div
    [ ClassName "content" ]
    [ h1
        [ ]
        [ str "About FableCricket" ]
      p
        [ ]
        [ str "This is a simple cricket game built with F# + Fable + Elmish + React." ]
      p
        [ ]
        [ str ("Version " + App.Version.VersionNumber) ] ]
