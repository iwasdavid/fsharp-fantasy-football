module Domain
    type Player = {
        FirstName:string
        LastName:string
        Team: string
        Position: string
        Rating:int 
    }

    type Team = {
        Goalkeeper: Player
        Defenders: Player[]
        Midfielder: Player[]
        Attacker: Player[]
    }

    type Position = Attacker | Midfielder | Defender | Goalkeeper
    type Formation = FourFourTwo | FourThreeThree | ThreeFiveTwo | FourFiveOne
    