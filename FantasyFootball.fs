module FantasyFootball

    open FSharp.Data
    open FSharp.Data.JsonExtensions
    open FSharp.Data
    open FSharp.Data
    open System
    open Helper
    open Domain

    let getPlayers =
        let getPage page = 
            async {
              let! data = JsonValue.AsyncLoad ("https://www.easports.com/fifa/ultimate-team/api/fut/item?page=" + page.ToString())
              let items = 
                [| for item in data?items -> item |] 
                //|> Array.filter (fun item -> item?playerType.AsString() = "rare" || item?playerType.AsString() = "standard")
              return items
            }

        let value = 100//JsonValue.Load ("https://www.easports.com/fifa/ultimate-team/api/fut/item")
        let totalPages = value//?totalPages.AsInteger()

        //let para = [|1..totalPages|] |> Array.map getPage |> Async.Parallel
        //let para2 = [|1..totalPages|] |> Array.map getPage |> Async.Parallel |> Async.RunSynchronously

        [|1..totalPages|] 
        |> Array.map getPage
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Array.concat

    let findPosition (position:string) =
        match position.ToUpper().Trim() with
        | p when p = "GK" -> Goalkeeper
        | p when p = "RWB" || p = "RB" || p = "CB" || p = "LB" || p = "LWB" -> Defender
        | p when p = "RW" || p = "RM" || p = "LW" || p = "LM" || p = "CM" || p = "CDM" || p = "CAM"  -> Midfielder
        | p when p = "ST" || p = "LF" || p = "CF" || p = "RF" -> Attacker
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

    let getAllPlayersInPosition (pos:Position) (players:JsonValue[]) =

        let position = match pos with
                       | Goalkeeper -> Goalkeeper
                       | Defender -> Defender
                       | Midfielder -> Midfielder
                       | Attacker -> Attacker

        players |> Array.filter (findPositionFromString position)

    let getAllGoalkeepers = getAllPlayersInPosition Goalkeeper
    let getAllDefenders = getAllPlayersInPosition Defender
    let getAllMidfielders = getAllPlayersInPosition Midfielder
    let getAllAttackers = getAllPlayersInPosition Attacker

    let createPlayerFromJson (jsonPlayer:JsonValue) : Player =
        { FirstName = jsonPlayer?firstName.AsString() ; LastName = jsonPlayer?lastName.AsString() }

    let createTeam players pickedFormation =

        let numberOfGoalkeepers,numberOfDefenders,numberOfMidfielders,numberOfAttackers = pickedFormation.Goalkeepers, pickedFormation.Defenders, pickedFormation.Midfielders, pickedFormation.Attackers

        let goalKeepers =  getAllGoalkeepers players |> Array.take numberOfGoalkeepers |> Array.map createPlayerFromJson
        let defenders =  getAllDefenders players |> Array.take numberOfDefenders |> Array.map createPlayerFromJson
        let midfielders =  getAllMidfielders players |> Array.take numberOfMidfielders |> Array.map createPlayerFromJson
        let attackers =  getAllAttackers players |> Array.take numberOfAttackers |> Array.map createPlayerFromJson
        
        { Goalkeeper = goalKeepers.[0]
          Defenders = defenders
          Midfielder = midfielders
          Attacker = attackers }

    let findBestTeam formation =
        let players = getPlayers
        let pickedFormation = match formation with
                              | FourFourTwo x -> x
                              | FourThreeThree x -> x

        createTeam players pickedFormation |> Helper.printTeam
        
    let pickTeam (stringFormation:string) =
        match stringFormation with
        | "442" -> findBestTeam (FourFourTwo {Goalkeepers = 1; Defenders = 4; Midfielders = 4; Attackers = 2})
        | "433" -> findBestTeam (FourThreeThree {Goalkeepers = 1; Defenders = 4; Midfielders = 3; Attackers = 3})
        | _ -> failwith "Unknown formation, please try again!!"
