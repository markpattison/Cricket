module Cricket.Server.Storage

open Microsoft.Azure.Cosmos.Table
open Thoth.Json.Net

open Cricket.CricketEngine
open Cricket.MatchRunner
open Cricket.Shared

type ServerModelStorage =
    {
        Match: Match
        Series: Series
        CompletedMatches: int list
        CurrentMatchId: int
    }

type CricketStoreType =
    | State
    | PlayerRecords
    | PlayerAttributes
    | CompletedMatch of int
    member this.ToRowKey() =
        match this with
        | State -> "state"
        | PlayerRecords -> "player records"
        | PlayerAttributes -> "player attributes"
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
        Series = state.Series
        CompletedMatches = state.CompletedMatches |> Seq.map (fun kvp -> kvp.Key) |> Seq.toList
        CurrentMatchId = state.CurrentMatchId
    }

let fromServerModelStorage (storage: ServerModelStorage) (completedMatches: Map<int, Match>) (playerRecords: Map<Player, PlayerRecord>) (playerAttributes: PlayerAttributes) =
    {
        ServerModel.Match = storage.Match
        PlayerRecords = playerRecords
        LivePlayerRecords = Averages.updatePlayersForMatch playerRecords storage.Match
        PlayerAttributes = playerAttributes
        Series = storage.Series
        CompletedMatches = completedMatches
        CurrentMatchId = storage.CurrentMatchId
    }

let saveCompletedMatch (table: CloudTable) (sessionId: SessionId) (matchId: int) (mtch: Match) =
    let json = Encode.Auto.toString(0, mtch)
    let cricketStore = CricketStore(sessionId.ToString(), CompletedMatch matchId, json)
    let op = TableOperation.InsertOrReplace(cricketStore)
    table.Execute(op) |> ignore

let savePlayerRecords (table: CloudTable) (sessionId: SessionId) (playerRecords: Map<Player, PlayerRecord>) =
    let json = Encode.Auto.toString(0, playerRecords)
    let cricketStore = CricketStore(sessionId.ToString(), PlayerRecords, json)
    let op = TableOperation.InsertOrReplace(cricketStore)
    table.Execute(op) |> ignore

let savePlayerAttributes (table: CloudTable) (sessionId: SessionId) (playerAttributes: PlayerAttributes) =
    let json = Encode.Auto.toString(0, playerAttributes)
    let cricketStore = CricketStore(sessionId.ToString(), PlayerAttributes, json)
    let op = TableOperation.InsertOrReplace(cricketStore)
    table.Execute(op) |> ignore

let loadCompletedMatch (table: CloudTable) (sessionId: SessionId) (matchId: int) =
    let op = TableOperation.Retrieve<CricketStore>(sessionId.ToString(), State.ToRowKey())
    let opResult = table.Execute(op).Result  

    match opResult with
    | null -> Error (sprintf "Session could not be found for completed match %i" matchId)
    | :? CricketStore as cricketStore ->
        Decode.Auto.fromString<Match>(cricketStore.Data)
    | _ -> Error (sprintf "Error reading match %i" matchId)

let loadPlayerRecords (table: CloudTable) (sessionId: SessionId) =
    let op = TableOperation.Retrieve<CricketStore>(sessionId.ToString(), PlayerRecords.ToRowKey())
    let opResult = table.Execute(op).Result  

    match opResult with
    | null -> Error "Player records could not be found"
    | :? CricketStore as cricketStore ->
        Decode.Auto.fromString<Map<Player, PlayerRecord>>(cricketStore.Data)
    | _ -> Error (sprintf "Error reading player records")

let loadPlayerAttributes (table: CloudTable) (sessionId: SessionId) =
    let op = TableOperation.Retrieve<CricketStore>(sessionId.ToString(), PlayerAttributes.ToRowKey())
    let opResult = table.Execute(op).Result  

    match opResult with
    | null -> Error "Player attributes could not be found"
    | :? CricketStore as cricketStore ->
        Decode.Auto.fromString<PlayerAttributes>(cricketStore.Data)
    | _ -> Error (sprintf "Error reading player attributes")

let save (table: CloudTable) (sessionId: SessionId) (state: ServerModel) (previouslySavedState: ServerModel option) =
    let stateToStore = toServerModelStorage state
    let stateJson = Encode.Auto.toString(0, stateToStore)

    printfn "JSON length: %i" stateJson.Length // DEBUG

    let cricketStore = CricketStore(sessionId.ToString(), State, stateJson)
    let op = TableOperation.InsertOrReplace(cricketStore)
    table.Execute(op) |> ignore

    // only save new completed matches

    let completedMatchesToSave =
        match previouslySavedState with
        | Some prev ->
            state.CompletedMatches
            |> Map.toSeq
            |> Seq.filter (fun (matchId, _) -> Map.containsKey matchId prev.CompletedMatches |> not)
        | None ->
            state.CompletedMatches
            |> Map.toSeq

    completedMatchesToSave
    |> Seq.iter (fun (matchId, mtch) -> saveCompletedMatch table sessionId matchId mtch)

    // only save player records and attributes if changed
    let shouldSavePlayerRecords =
        match previouslySavedState with
        | Some prev -> state.PlayerRecords <> prev.PlayerRecords
        | None -> true
    
    let shouldSavePlayerAttributes =
        match previouslySavedState with
        | Some prev -> state.PlayerAttributes <> prev.PlayerAttributes
        | None -> true

    if shouldSavePlayerRecords then savePlayerRecords table sessionId state.PlayerRecords
    if shouldSavePlayerAttributes then savePlayerAttributes table sessionId state.PlayerAttributes

let load (table: CloudTable) (sessionId: SessionId) =
    let playerRecords = loadPlayerRecords table sessionId
    let playerAttributes = loadPlayerAttributes table sessionId
    
    let op = TableOperation.Retrieve<CricketStore>(sessionId.ToString(), State.ToRowKey())
    let opResult = table.Execute(op).Result
    
    match opResult with
    | null -> Error "Session could not be found"
    | :? CricketStore as cricketStore ->
        let loadResult = Decode.Auto.fromString<ServerModelStorage>(cricketStore.Data)
        match loadResult, playerRecords, playerAttributes with
        | Ok storage, Ok records, Ok attributes ->
            let completedMatchesLoadResults = storage.CompletedMatches |> List.map (fun matchId -> (matchId, loadCompletedMatch table sessionId matchId))
            let okCompletedMatches = completedMatchesLoadResults |> List.choose (fun (matchId, res) -> match res with | Ok mtch -> Some (matchId, mtch) | _ -> None)

            if List.length completedMatchesLoadResults = List.length okCompletedMatches then
                let completedMatches = Map.ofList okCompletedMatches
                let serverModel = fromServerModelStorage storage completedMatches records attributes
                Ok serverModel
            else
                Error "Error reading completed matches"

        | _ -> Error "Error reading session"
            
    | _ -> Error "Error reading session"
