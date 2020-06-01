namespace Cricket.Shared

open System

open Cricket.CricketEngine
open Cricket.MatchRunner

type SessionId =
    | SessionId of Guid
    override this.ToString() =
        match this with | SessionId guid -> guid.ToString()

type DataFromServer = Match
type Averages = Map<Player, PlayerRecord>

module Route =
    /// Defines how routes are generated on server and mapped from client
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

/// A type that specifies the communication protocol between client and server
/// to learn more, read the docs at https://zaid-ajaj.github.io/Fable.Remoting/src/basics.html
type ICricketApi =
    { newSession : unit -> Async<SessionId * DataFromServer>
      loadSession: SessionId -> Async<Result<DataFromServer, string>>
      update : (SessionId * ServerMsg) -> Async<Result<DataFromServer, string>>
      getAverages : SessionId -> Async<Result<Averages, string>>
      getSeries : SessionId -> Async<Result<Series, string>>
    }
