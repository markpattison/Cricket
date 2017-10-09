module Global

type Page =
  | Cricket
  | About

let toHash page =
  match page with
  | About -> "#about"
  | Cricket -> "#cricket"
