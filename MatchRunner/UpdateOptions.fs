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
    | ModalMessageToCaptain of (TeamChoice * ModalMessageToCaptain)
    | ContinueInnings of (TeamChoice * OptionalMessageToCaptain) list
    | StartMatch
    | StartNextInnings
    | MatchOver

type UpdateOptionsForUI =
    | StartMatchUI
    | StartNextInningsUI
    | ContinueInningsBallUI
    | ContinueInningsOverUI
    | MatchOverUI

