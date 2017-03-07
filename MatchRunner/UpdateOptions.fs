namespace Cricket.MatchRunner

open Cricket.CricketEngine

type ModalMessageToCaptain =
    | FollowOnDecision
    | NewBatsmanRequired of int
    | NewBowlerRequiredTo of End

type OptionalMessageToCaptain =
    | EndOfOver
    | CanDeclare

type UpdateOptions =
    | ModalMessageToCaptain of (Team * Match * ModalMessageToCaptain)
    | ContinueInnings of (Team * Match * OptionalMessageToCaptain) list
    | StartMatch
    | StartNextInnings
    | MatchOver

type UpdateOptionsForUI =
    | StartMatchUI
    | StartNextInningsUI
    | ContinueInningsUI
    | MatchOverUI

