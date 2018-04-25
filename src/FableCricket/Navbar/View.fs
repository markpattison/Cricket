module FableCricket.Navbar.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma
open Fulma.FontAwesome

let navButton classy href faClass txt =
  Control.div []
    [ Button.a 
        [ Button.CustomClass (sprintf "button %s" classy)
          Button.Props [ Href href ] ]
        [ Icon.faIcon [] [ Fa.icon faClass ]
          span [] [ str txt ] ] ]

let navButtons =
  Navbar.Item.div []
    [ Field.div
        [ Field.IsGrouped ]
        [ navButton "twitter" "https://twitter.com/mark_pattison" Fa.I.Twitter "Twitter"
          navButton "github" "https://github.com/markpattison/Cricket" Fa.I.Github "GitHub" ] ]

let root =
  Navbar.navbar [ Navbar.Color IsPrimary ]
    [ Navbar.Brand.div []
        [ Navbar.Item.div []
            [ Heading.h4 [] [ str "FableCricket" ] ] ]
      Navbar.End.div []
        [ navButtons ] ]
