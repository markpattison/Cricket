module Cricket.Server.Storage

open Microsoft.Azure.Cosmos.Table
open Thoth.Json.Net

open Cricket.Shared
open Cricket.MatchRunner

type CricketStore(sessionId: string, state: string) =
    inherit TableEntity(partitionKey = sessionId, rowKey = "state")
    new() = CricketStore(null, null)
    member val SessionId = sessionId with get, set
    member val State = state with get, set

let save (table: CloudTable) (sessionId: SessionId) (state: ServerModel) =
    let stateJson = Encode.Auto.toString(0, state)
    let cricketStore = CricketStore(sessionId.ToString(), stateJson)
    let op = TableOperation.InsertOrReplace(cricketStore)
    table.Execute(op) |> ignore

let load (table: CloudTable) (sessionId: SessionId) =
    let op = TableOperation.Retrieve<CricketStore>(sessionId.ToString(), "state")
    let opResult = table.Execute(op).Result
    
    match opResult with
    | null -> Error "Session could not be found"
    | :? CricketStore as cricketStore ->
        Decode.Auto.fromString<ServerModel>(cricketStore.State)
    | _ -> Error "Error reading session"
