module FableCricket.Extensions

type Deferred<'t> =
    | HasNotStartedYet
    | InProgress of 't option
    | Resolved of 't

let updateInProgress deferred =
    match deferred with
    | HasNotStartedYet
    | InProgress None -> InProgress None
    | InProgress (Some oldState)
    | Resolved oldState -> InProgress (Some oldState)
