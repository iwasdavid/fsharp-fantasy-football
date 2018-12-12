module FantasyFootball

open FSharp.Data
open Helper
open Domain

type Players = JsonProvider<"https://www.easports.com/fifa/ultimate-team/api/fut/item?page=%i">
type JsonPlayer = Players.Item

let createPlayerFromJson (player: JsonPlayer) : Player =
    { FirstName = player.FirstName
      LastName = player.LastName
      Team = player.Club.Name
      Rating = player.Rating
      Position =
          match player.Position with
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
          | _ -> "Unknown position!" }

let getAllPlayersInPosition (position: Position) (players: JsonPlayer list) =
    let findPosition (position: string) =
        match position.ToUpper().Trim() with
        | "GK" -> Goalkeeper
        | "RWB" | "RB" | "CB" | "LB" | "LWB" -> Defender
        | "RW" | "RM" | "LW" | "LM" | "CM" | "CDM" | "CAM" -> Midfielder
        | "ST" | "LF" | "CF" | "RF" -> Attacker
        | _ -> failwith "No known position :("
    
    players
    |> List.filter (fun x -> x.Club.Name <> "Icons" && findPosition x.Position = position)
    |> List.distinctBy (fun x -> x.FirstName + " " + x.LastName)

let getAllGoalkeepers = getAllPlayersInPosition Goalkeeper
let getAllDefenders = getAllPlayersInPosition Defender
let getAllMidfielders = getAllPlayersInPosition Midfielder
let getAllAttackers = getAllPlayersInPosition Attacker

let getStartingPlayers players findBest numberToTake =
    players
    |> List.sortByDescending findBest
    |> List.truncate numberToTake
    |> List.map createPlayerFromJson

let findBestGoalkeeper (p: JsonPlayer) =
    p.Rating +
    p.Composure * 10 +
    p.Gkdiving * 10 +
    p.Gkhandling * 10 +
    p.Gkkicking * 10 +
    p.Gkpositioning * 10 +
    p.Gkreflexes * 10 +
    p.Longpassing +
    p.Agility +
    p.Freekickaccuracy +
    p.Jumping +
    p.Positioning +
    p.Strength +
    p.Vision +
    p.WeakFoot

let findBestDefenders (p: JsonPlayer) =
    p.Rating +
    p.Composure +
    p.Acceleration +
    p.Aggression +
    p.Ballcontrol +
    p.Headingaccuracy +
    p.Interceptions +
    p.Jumping +
    p.Marking +
    p.Positioning +
    p.Shortpassing +
    p.Slidingtackle +
    p.Sprintspeed +
    p.Standingtackle +
    p.Strength +
    p.Vision +
    p.WeakFoot

let findBestMidfielders (p: JsonPlayer) =
    p.Rating +
    p.Acceleration +
    p.Aggression +
    p.Agility +
    p.Balance +
    p.Ballcontrol +
    p.SkillMoves +
    p.Crossing +
    p.Interceptions +
    p.Longpassing +
    p.Longshots +
    p.Positioning +
    p.Shortpassing +
    p.Standingtackle +
    p.Stamina +
    p.Strength +
    p.Vision

let findBestAttackers (player: JsonPlayer) =
    player.Rating +
    player.Composure +
    player.Acceleration +
    player.Aggression +
    player.Agility +
    player.Balance +
    player.Ballcontrol +
    player.SkillMoves +
    player.Curve +
    player.Dribbling +
    player.Finishing +
    player.Freekickaccuracy +
    player.Longshots +
    player.Penalties +
    player.Shotpower +
    player.Sprintspeed

let createTeam (pickedFormation: Formation) (players: JsonPlayer list) =
    let numberOfGoalkeepers, numberOfDefenders, numberOfMidfielders, numberOfAttackers =
        match pickedFormation with
        | FourFourTwo -> 1, 4, 4, 2
        | FourThreeThree -> 1, 4, 3, 3
        | ThreeFiveTwo -> 1, 3, 5, 2
        | FourFiveOne -> 1, 4, 5, 1

    let goalkeeper =
        getStartingPlayers (getAllGoalkeepers players) findBestGoalkeeper numberOfGoalkeepers
        |> List.tryHead
        |> Option.defaultWith (fun _ -> failwith "Goalkeeper was not found")
    
    { Goalkeeper = goalkeeper
      Defenders = getStartingPlayers (getAllDefenders players) findBestDefenders numberOfDefenders
      Midfielder = getStartingPlayers (getAllMidfielders players) findBestMidfielders numberOfMidfielders
      Attacker = getStartingPlayers (getAllAttackers players) findBestAttackers numberOfAttackers }

let findBestTeam formation =
    [| 1..150 |]
    |> Array.map (sprintf "https://www.easports.com/fifa/ultimate-team/api/fut/item?page=%i")
    |> Array.map Players.AsyncLoad
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Array.collect (fun x -> x.Items)
    |> Array.toList
    |> createTeam formation
    |> printTeam

let pickTeam = function
    | "442" -> findBestTeam FourFourTwo
    | "433" -> findBestTeam FourThreeThree
    | "352" -> findBestTeam ThreeFiveTwo
    | "451" -> findBestTeam FourFiveOne
    | _ -> failwith "Unknown formation, please try again!"
