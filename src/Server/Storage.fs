module Cricket.Server.Storage

open Microsoft.Azure.Cosmos.Table
open Thoth.Json.Net

open Cricket.CricketEngine
open Cricket.MatchRunner
open Cricket.Shared

type ServerModelStorage =
    {
        Match: Match
        PlayerRecords: Map<Player, PlayerRecord>
        PlayerAttributes: PlayerAttributes
        Series: Series
        CompletedMatches: int list
        CurrentMatchId: int
    }

type CricketStoreType =
    | State
    | CompletedMatch of int
    member this.ToRowKey() =
        match this with
        | State -> "state"
        | CompletedMatch matchId -> sprintf "match %i" matchId

type CricketStore(sessionId: string, storeType: CricketStoreType, data: string) =
    inherit TableEntity(partitionKey = sessionId, rowKey = storeType.ToRowKey())
    new() = CricketStore(null, State, null)
    member val SessionId = sessionId with get, set
    member val StoreType = storeType with get, set
    member val Data = data with get, set

let toServerModelStorage (state: ServerModel) =
    {
        ServerModelStorage.Match = state.Match
        PlayerRecords = state.PlayerRecords
        PlayerAttributes = state.PlayerAttributes
        Series = state.Series
        CompletedMatches = state.CompletedMatches |> Seq.map (fun kvp -> kvp.Key) |> Seq.toList
        CurrentMatchId = state.CurrentMatchId
    }

let fromServerModelStorage (storage: ServerModelStorage) (completedMatches: Map<int, Match>) =
    {
        ServerModel.Match = storage.Match
        PlayerRecords = storage.PlayerRecords
        LivePlayerRecords = Averages.updatePlayersForMatch storage.PlayerRecords storage.Match
        PlayerAttributes = storage.PlayerAttributes
        Series = storage.Series
        CompletedMatches = completedMatches
        CurrentMatchId = storage.CurrentMatchId
    }

let saveCompletedMatch (table: CloudTable) (sessionId: SessionId) (matchId: int) (mtch: Match) =
    let matchJson = Encode.Auto.toString(0, mtch)
    let cricketStore = CricketStore(sessionId.ToString(), CompletedMatch matchId, matchJson)
    let op = TableOperation.InsertOrReplace(cricketStore)
    table.Execute(op) |> ignore

let loadCompletedMatch (table: CloudTable) (sessionId: SessionId) (matchId: int) =
    let op = TableOperation.Retrieve<CricketStore>(sessionId.ToString(), State.ToRowKey())
    let opResult = table.Execute(op).Result  

    match opResult with
    | null -> Error "Session could not be found"
    | :? CricketStore as cricketStore ->
        Decode.Auto.fromString<Match>(cricketStore.Data)
    | _ -> Error (sprintf "Error reading match %i" matchId)

let save (table: CloudTable) (sessionId: SessionId) (state: ServerModel) (previouslySavedState: ServerModel) =
    let stateToStore = toServerModelStorage state
    let stateJson = Encode.Auto.toString(0, stateToStore)

    printfn "JSON length: %i" stateJson.Length // DEBUG

    let cricketStore = CricketStore(sessionId.ToString(), State, stateJson)
    let op = TableOperation.InsertOrReplace(cricketStore)
    table.Execute(op) |> ignore

    let completedMatchesToSave =
        state.CompletedMatches
        |> Map.toSeq
        |> Seq.filter (fun (matchId, _) -> Map.containsKey matchId previouslySavedState.CompletedMatches |> not)

    completedMatchesToSave
    |> Seq.iter (fun (matchId, mtch) -> saveCompletedMatch table sessionId matchId mtch)

let load (table: CloudTable) (sessionId: SessionId) =
    let op = TableOperation.Retrieve<CricketStore>(sessionId.ToString(), State.ToRowKey())
    let opResult = table.Execute(op).Result
    
    match opResult with
    | null -> Error "Session could not be found"
    | :? CricketStore as cricketStore ->
        let loadResult = Decode.Auto.fromString<ServerModelStorage>(cricketStore.Data)
        match loadResult with
        | Ok storage ->
            let completedMatchesLoadResults = storage.CompletedMatches |> List.map (fun matchId -> (matchId, loadCompletedMatch table sessionId matchId))
            let okCompletedMatches = completedMatchesLoadResults |> List.choose (fun (matchId, res) -> match res with | Ok mtch -> Some (matchId, mtch) | _ -> None)

            if List.length completedMatchesLoadResults = List.length okCompletedMatches then
                let completedMatches = Map.ofList okCompletedMatches
                let serverModel = fromServerModelStorage storage completedMatches
                Ok serverModel
            else
                Error "Error reading completed matches"

        | Error err -> Error "Error reading session"
            
    | _ -> Error "Error reading session"
