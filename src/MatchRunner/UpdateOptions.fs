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
    | ContinueInnings
    | StartMatch
    | StartNextInnings
    | MatchOver

type UpdateOptionsForUI =
    | StartMatchUI
    | StartNextInningsUI
    | ContinueInningsBallUI
    | ContinueInningsOverUI
    | ContinueInningsInningsUI
    | MatchOverUI

type OptionalMessageResult =
    | StateChanged of Match
    | StateUnchanged
