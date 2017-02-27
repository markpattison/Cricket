namespace Cricket.CricketEngine

type MessageToCaptain =
    | BatsmanRequired of int
    | BowlerRequired of End
    | FollowOnDecision
    | CanChangeBowler
    | CanDeclare

type CaptainAction =
    | Action of MatchUpdate
    | NoAction

let captain message =
    match message with
    | BatsmanRequired n -> { Name = sprintf "Batmsan %i" n }
    

module MatchRunner =
    
