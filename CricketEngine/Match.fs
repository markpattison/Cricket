namespace Match

open MatchState

type Match =
    {
        Info: string;
        State: MatchState
    }
    member x.UpdateState (update: MatchState -> MatchState) = { x with State = (update x.State) }
