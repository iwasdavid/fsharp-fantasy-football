module Helper
    open Domain
    let printLine item =
        printf "%s\n" item

    let whatToPrint player =
        "Name: " + player.FirstName + " " + player.LastName + ". Team: " + player.Team + ". Position: " + player.Position + ". Rating: " + player.Rating.ToString()
    let printTeam team =
        printf ""

        printLine "-----------TEAM------------"
        printLine ""

        printLine "GOALKEEPERS"
        printLine ""
        printLine (whatToPrint team.Goalkeeper)
        printLine ""

        printLine "DEFENDERS"
        printLine ""
        team.Defenders |> Array.iter (fun x -> printLine (whatToPrint x))
        printLine ""

        printLine "MIDFIELDERS"
        printLine ""
        team.Midfielder |> Array.iter (fun x -> printLine (whatToPrint x))
        printLine ""

        printLine "ATTACKERS"
        printLine ""
        team.Attacker |> Array.iter (fun x -> printLine (whatToPrint x))
        printLine ""
        printLine "------------END------------"