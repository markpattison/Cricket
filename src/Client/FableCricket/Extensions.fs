module FableCricket.Extensions

type Deferred<'t> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 't
