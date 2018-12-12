module Helper

open Domain

let private formatPlayer player =
    sprintf
        "Name: %s %s. Team: %s. Position: %s. Rating: %O"
        player.FirstName
        player.LastName
        player.Team
        player.Position
        player.Rating
    
let private printPlayers = List.map formatPlayer >> List.iter (printfn "%s")    
    
let printTeam team =
    printfn "-----------TEAM------------\n"

    printfn "GOALKEEPERS\n"
    printfn "%s\n" (formatPlayer team.Goalkeeper)

    printfn "DEFENDERS\n"
    printPlayers team.Defenders
    printfn ""

    printfn "MIDFIELDERS\n"
    printPlayers team.Midfielder
    printfn ""

    printfn "ATTACKERS\n"
    printPlayers team.Attacker
    printfn "\n------------END------------"
