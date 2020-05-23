namespace Cricket.Server

open Microsoft.Azure
open Microsoft.Azure.Cosmos.Table
open Microsoft.AspNetCore.Http

type CricketStore(sessionId: string, state: string) =
    inherit TableEntity(partitionKey = sessionId, rowKey = "state")
    new() = CricketStore(null, null)
    member val SessionId = sessionId with get, set
    member val State = state with get, set