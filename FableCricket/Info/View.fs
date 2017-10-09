module Info.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

let root =
  div
    [ ClassName "content" ]
    [ h1
        [ ]
        [ str "About FableCricket" ]
      p
        [ ]
        [ str "This is a simple cricket game built with F# + Fable + Elmish + React." ] ]
