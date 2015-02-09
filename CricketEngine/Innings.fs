namespace Cricket.CricketEngine

type Innings =
    { IndividualInnings: IndividualInnings list; IsDeclared: bool}
    member _this.GetRuns =
        _this.IndividualInnings |> List.sumBy (fun ii -> ii.Score)
    member _this.GetWickets =
        _this.IndividualInnings |> List.filter (fun ii -> ii.HowOut.IsSome) |> List.length

type InningsStatus =
    | InningsCompleted of Innings
    | InningsOngoing of Innings
    member x.GetInnings =
        match x with
        | InningsCompleted innings -> innings
        | InningsOngoing innings -> innings

[<AutoOpen>]
module InningsFunctions =

    let NewInnings = { IndividualInnings = [ NewIndividualInnings; NewIndividualInnings ]; IsDeclared = false }
