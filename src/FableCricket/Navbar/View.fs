module FableCricket.Navbar.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma.Color
open Fulma.Components
open Fulma.Elements
open Fulma.Elements.Form
open Fulma.Extra.FontAwesome

let navButton classy href faClass txt =
  Control.div
    [
      Control.CustomClass (sprintf "button %s" classy)
      Control.Props [ Href href ]
    ]
    [
      Icon.faIcon [] [ Fa.icon faClass ]
      span [] [ str txt ]
    ]

let navButtons =
  Navbar.Item.div []
    [ Field.div
        [ Field.IsGrouped ]
        [
          navButton "twitter" "https://twitter.com/mark_pattison" Fa.I.Twitter "Twitter"
          navButton "github" "https://github.com/markpattison/Cricket" Fa.I.Github "Github"
        ]
    ]

let root =
  Navbar.navbar [ Navbar.Color IsPrimary ]
    [
      Navbar.Brand.div []
        [
          Navbar.Item.div []
            [
              Heading.h4 [] [ str "FableCricket" ]
            ]
        ]
      Navbar.End.div []
        [ navButtons ]
    ]
