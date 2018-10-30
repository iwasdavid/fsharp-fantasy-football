module Domain
    type Player = { FirstName:string; LastName:string; Team: string; Position: string; Rating:int }
    type PlayerCounts = { Goalkeepers: int; Defenders: int; Midfielders: int; Attackers: int }

    type Position = | Attacker | Midfielder | Defender | Goalkeeper

    // After you match on this DU, you'll get back a PlayerCounts and lose that information
    // I would expect the types wrapped here to encode that somehow
    type Formation =
        | FourFourTwo of PlayerCounts
        | FourThreeThree of PlayerCounts

    type Team = {
        Goalkeeper: Player
        Defenders: Player[]
        Midfielder: Player[]
        Attacker: Player[]
    }