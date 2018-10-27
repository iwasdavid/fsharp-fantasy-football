module Domain
    type Player = { FirstName:string; LastName:string }
    type PlayerCounts = { Goalkeepers: int; Defenders: int; Midfielders: int; Attackers: int }

    type Position = | Attacker | Midfielder | Defender | Goalkeeper

    type Formation =
        | FourFourTwo of PlayerCounts
        | FourThreeThree of PlayerCounts

    type Team = {
        Goalkeeper: Player
        Defenders: Player[]
        Midfielder: Player[]
        Attacker: Player[]
    }