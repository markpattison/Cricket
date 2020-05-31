namespace Cricket.MatchRunner

type PlayerAttribute =
    {
        BattingSkill: float
        BowlingSkill: float
    }

type PlayerAttributes =
    {
        Attributes: Map<int, PlayerAttribute>
    }

module PlayerAttributes =

    let battingSkill attributes (player: Cricket.CricketEngine.Player) =
        let playerId = player.ID
        attributes.Attributes.[playerId].BattingSkill

    let bowlingSkill attributes (player: Cricket.CricketEngine.Player) =
        let playerId = player.ID
        attributes.Attributes.[playerId].BowlingSkill