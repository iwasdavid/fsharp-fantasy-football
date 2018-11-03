module FantasyFootball

    open FSharp.Data
    open FSharp.Data.JsonExtensions
    open Helper
    open Domain

    let getPlayers =
        let getPage page = 
            async {
                let! data = JsonValue.AsyncLoad (sprintf "https://www.easports.com/fifa/ultimate-team/api/fut/item?page=%i" page)
                let items = [| for item in data?items -> item |] 
                return items
            }

        [|1..150|] 
        |> Array.map getPage
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Array.concat

    let findPosition (position:string) =
        match position.ToUpper().Trim() with
        | "GK" -> Goalkeeper
        | "RWB" | "RB" | "CB" | "LB" | "LWB" -> Defender
        | "RW" | "RM" | "LW" | "LM" | "CM" | "CDM" | "CAM"  -> Midfielder
        | "ST" | "LF" | "CF" | "RF" -> Attacker
        | _ -> failwith "No known position :("

    let findPositionFromString (position:Position) (player:JsonValue)  =
        let passedInPosition = match position with
                               | Goalkeeper -> Goalkeeper
                               | Defender -> Defender
                               | Midfielder -> Midfielder
                               | Attacker -> Attacker

        let stringPosition = player?position.AsString()
        let foundPosition = findPosition stringPosition
        
        passedInPosition = foundPosition

    let createPlayerFromJson (jsonPlayer:JsonValue) : Player =
        {
            FirstName = jsonPlayer?firstName.AsString()
            LastName = jsonPlayer?lastName.AsString()
            Team = jsonPlayer?club?name.AsString()
            Rating = jsonPlayer?rating.AsInteger()
            Position = match jsonPlayer?position.AsString() with
                       | "GK" -> "Goalkeeper"
                       | "RWB" -> "Right Wing Back"
                       | "RB" -> "Right Back"
                       | "LWB" -> "Left Wing Back"
                       | "LB" -> "Left Back"
                       | "CB" -> "Centre Back"
                       | "RW" -> "Right Wing"
                       | "RM" -> "Right Midfield"
                       | "LW" -> "Left Wing"
                       | "LM" -> "Left Midfield"
                       | "CM" -> "Centre Midfield"
                       | "CDM" -> "Centre Defensive Midfielder"
                       | "CAM" -> "Centre Atacking Midfielder"
                       | "ST" -> "Striker"
                       | "LF" -> "Left Forward"
                       | "RF" -> "Right Forward"
                       | "CF" -> "Centre Forward"
                       | _ -> "Unknown position!"
        }

    let removeIconPlayers (jsonPlayer:JsonValue) = jsonPlayer?club?name.AsString() <> "Icons"
        
    let removeDupliactePlayers (player:JsonValue) = player?firstName.AsString() + " " + player?lastName.AsString()

    let getAllPlayersInPosition (pos:Position) (players:JsonValue[]) =

        let position = match pos with
                       | Goalkeeper -> Goalkeeper
                       | Defender -> Defender
                       | Midfielder -> Midfielder
                       | Attacker -> Attacker

        players
        |> Array.filter (findPositionFromString position)
        |> Array.distinctBy removeDupliactePlayers
        |> Array.filter removeIconPlayers

    let getAllGoalkeepers = getAllPlayersInPosition Goalkeeper
    let getAllDefenders = getAllPlayersInPosition Defender
    let getAllMidfielders = getAllPlayersInPosition Midfielder
    let getAllAttackers = getAllPlayersInPosition Attacker

    let getStartingPlayers players findBest numberToTake =
        players
        |> Array.sortByDescending findBest
        |> Array.take numberToTake
        |> Array.map createPlayerFromJson

    let createTeam (players:JsonValue[]) (pickedFormation:Formation) =
        let numberOfGoalkeepers,numberOfDefenders,numberOfMidfielders,numberOfAttackers = match pickedFormation with
                                                                                          | FourFourTwo -> 1,4,4,2
                                                                                          | FourThreeThree -> 1,4,3,3
                                                                                          | ThreeFiveTwo -> 1,3,5,2

        let goalKeeper =  getStartingPlayers (getAllGoalkeepers players) findBestGoalkeeper numberOfGoalkeepers |> Array.head
        let defenders =  getStartingPlayers (getAllDefenders players) findBestDefenders numberOfDefenders
        let midfielders =  getStartingPlayers (getAllMidfielders players) findBestMidfielders numberOfMidfielders
        let attackers =  getStartingPlayers (getAllAttackers players) findBestAttackers numberOfAttackers
        
        {
            Goalkeeper = goalKeeper
            Defenders = defenders
            Midfielder = midfielders
            Attacker = attackers
        }

    let findBestTeam formation =
        let players = getPlayers
        let pickedFormation = match formation with
                              | FourFourTwo -> FourFourTwo
                              | FourThreeThree -> FourThreeThree
                              | ThreeFiveTwo -> ThreeFiveTwo

        createTeam players pickedFormation |> printTeam
        
    let pickTeam (stringFormation:string) =
        match stringFormation with
        | "442" -> findBestTeam FourFourTwo
        | "433" -> findBestTeam FourThreeThree
        | "352" -> findBestTeam ThreeFiveTwo
        | _ -> failwith "Unknown formation, please try again!!"
