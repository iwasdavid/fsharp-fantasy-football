module Domain

type Player =
    { FirstName: string
      LastName: string
      Team: string
      Position: string
      Rating: int }

type Team =
    { Goalkeeper : Player
      Defenders : Player list
      Midfielder : Player list
      Attacker : Player list }

type Position =
    | Attacker
    | Midfielder
    | Defender
    | Goalkeeper
    
type Formation =
    | FourFourTwo
    | FourThreeThree
    | ThreeFiveTwo
    | FourFiveOne
