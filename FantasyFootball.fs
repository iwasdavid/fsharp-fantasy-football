module FantasyFootball

    // Overall this looks great, you clearly understand what you are doing and know the F# idioms
    // I'll just add some minor comments and trivia

    open FSharp.Data
    open FSharp.Data.JsonExtensions
    open Helper
    open Domain

    let getPlayers =
        let getPage page = 
            async {
              let! data = JsonValue.AsyncLoad (sprintf "https://www.easports.com/fifa/ultimate-team/api/fut/item?page=%i" page)
              let items = 
                [| for item in data?items -> item |] 
                //|> Array.filter (fun item -> item?playerType.AsString() = "rare" || item?playerType.AsString() = "standard")
              return items
            }

        let value = 10//JsonValue.Load ("https://www.easports.com/fifa/ultimate-team/api/fut/item")
        let totalPages = value//?totalPages.AsInteger()

        [|600..800|] 
        |> Array.map getPage
        |> Async.Parallel
        // Here you passing 200 Asyncs to be ran in parallel. This might work, but you might want to batch/throttle this work.
        // There is a nice library for handling this sort of stuff; AsyncSeq: https://github.com/fsprojects/FSharp.Control.AsyncSeq
        // Here's the API http://fsprojects.github.io/FSharp.Control.AsyncSeq/reference/fsharp-control-asyncseq.html
        |> Async.RunSynchronously
        |> Array.concat

    let findPosition (position:string) =
        // Suggestions:
        match position.ToUpper().Trim() with
        | "GK" -> Goalkeeper
        | "RWB" | "RB" | "CB" | "LB" | "LWB" -> Defender
        | "RW" | "RM" | "LW" | "LM" | "CM" | "CDM" | "CAM"  -> Midfielder
        | "ST" | "LF" | "CF" | "RF" -> Attacker
        | _ -> failwith "No known position :("

    let findPositionFromString (position:Position) (player:JsonValue)  =

        // I presume this is still a work in progress
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

    let removeIconPlayers (jsonPlayer:JsonValue) =
        // Alternative:
        jsonPlayer?club?name.AsString() <> "Icons"
    
    let removeDupliactePlayers (player:JsonValue) =
        player?firstName.AsString() + " " + player?lastName.AsString()

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

    // Partial application 👍
    let getAllGoalkeepers = getAllPlayersInPosition Goalkeeper
    let getAllDefenders = getAllPlayersInPosition Defender
    let getAllMidfielders = getAllPlayersInPosition Midfielder
    let getAllAttackers = getAllPlayersInPosition Attacker

    

    let createTeam players pickedFormation =

        let numberOfGoalkeepers,numberOfDefenders,numberOfMidfielders,numberOfAttackers = pickedFormation.Goalkeepers, pickedFormation.Defenders, pickedFormation.Midfielders, pickedFormation.Attackers

        let goalKeepers =  getAllGoalkeepers players
                           |> Array.sortByDescending (fun x -> x?rating) //add function to sort these properly
                           |> Array.take numberOfGoalkeepers
                           |> Array.map createPlayerFromJson

        let defenders =  getAllDefenders players
                         |> Array.sortByDescending (fun x -> x?rating)
                         |> Array.take numberOfDefenders
                         |> Array.map createPlayerFromJson

        let midfielders =  getAllMidfielders players
                           |> Array.sortByDescending (fun x -> x?rating)
                           |> Array.take numberOfMidfielders
                           |> Array.map createPlayerFromJson

        let attackers =  getAllAttackers players
                          |> Array.sortByDescending (fun x -> x?rating)
                          |> Array.take numberOfAttackers
                          |> Array.map createPlayerFromJson
        
        { Goalkeeper = goalKeepers |> Array.head // Alternative again 🤷
          Defenders = defenders
          Midfielder = midfielders
          Attacker = attackers }

    let findBestTeam formation =
        // getPlayers here is just players, either change getPlayers to take unit, or rename it and remove this redundant variable
        let players = getPlayers
        let pickedFormation = match formation with
                              | FourFourTwo x -> x
                              | FourThreeThree x -> x
        createTeam players pickedFormation |> printTeam
        
    let pickTeam (stringFormation:string) =
        match stringFormation with
        | "442" -> findBestTeam (FourFourTwo {Goalkeepers = 1; Defenders = 4; Midfielders = 4; Attackers = 2}) // It feels like the record created here belongs inside of the domain, otherwise FourFourTwo is a name with no meaning
        | "433" -> findBestTeam (FourThreeThree {Goalkeepers = 1; Defenders = 4; Midfielders = 3; Attackers = 3})
        | _ -> failwith "Unknown formation, please try again!!"
