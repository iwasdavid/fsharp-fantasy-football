module Helper
    open Domain
    let printLine item =
        printf "%s\n" item

    let printTeam team =
        printf ""

        printLine "-----------TEAM------------"
        printLine ""

        printLine "GOALKEEPERS"
        printLine ""
        printLine (team.Goalkeeper.FirstName + " " + team.Goalkeeper.LastName)
        printLine ""

        printLine "DEFENDERS"
        printLine ""
        team.Defenders |> Array.iter (fun x -> printLine (x.FirstName + " " + x.LastName))
        printLine ""

        printLine "MIDFIELDERS"
        printLine ""
        team.Midfielder |> Array.iter (fun x -> printLine (x.FirstName + " " + x.LastName))
        printLine ""

        printLine "ATTACKERS"
        printLine ""
        team.Attacker |> Array.iter (fun x -> printLine (x.FirstName + " " + x.LastName))
        printLine ""
        printLine "------------END------------"