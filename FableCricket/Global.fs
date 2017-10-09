module Global

type Page =
  | Home
  | Counter
  | Cricket
  | About

let toHash page =
  match page with
  | About -> "#about"
  | Counter -> "#counter"
  | Cricket -> "#cricket"
  | Home -> "#home"
