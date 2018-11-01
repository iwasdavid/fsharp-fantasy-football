module Helper
    open FSharp.Data
    open FSharp.Data.JsonExtensions
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

    let findBestGoalkeeper player =
        player?rating.AsInteger() +
        (player?composure.AsInteger() * 10) +
        (player?gkdiving.AsInteger() * 10) +
        (player?gkhandling.AsInteger() * 10) +
        (player?gkkicking.AsInteger() * 10) +
        (player?gkpositioning.AsInteger() * 10) +
        (player?gkreflexes.AsInteger() * 10) +
        player?longpassing.AsInteger() +
        player?agility.AsInteger() +
        player?freekickaccuracy.AsInteger() +
        player?jumping.AsInteger() +
        player?positioning.AsInteger() +
        player?strength.AsInteger() +
        player?vision.AsInteger() +
        player?weakFoot.AsInteger()

    let findBestDefenders player =
        player?rating.AsInteger() +
        player?composure.AsInteger() +
        player?acceleration.AsInteger() +
        player?aggression.AsInteger() +
        player?ballcontrol.AsInteger() +
        player?headingaccuracy.AsInteger() +
        player?interceptions.AsInteger() +
        player?jumping.AsInteger() +
        player?marking.AsInteger() +
        player?positioning.AsInteger() +
        player?shortpassing.AsInteger() +
        player?slidingtackle.AsInteger() +
        player?sprintspeed.AsInteger() +
        player?standingtackle.AsInteger() +
        player?strength.AsInteger() +
        player?vision.AsInteger() +
        player?weakFoot.AsInteger()

    let findBestMidfielders player =
        player?rating.AsInteger() +
        player?acceleration.AsInteger() +
        player?aggression.AsInteger() +
        player?agility.AsInteger() +
        player?balance.AsInteger() +
        player?ballcontrol.AsInteger() +
        player?skillMoves.AsInteger() +
        player?crossing.AsInteger() +
        player?interceptions.AsInteger() +
        player?longpassing.AsInteger() +
        player?longshots.AsInteger() +
        player?positioning.AsInteger() +
        player?shortpassing.AsInteger() +
        player?standingtackle.AsInteger() +
        player?stamina.AsInteger() +
        player?strength.AsInteger() +
        player?vision.AsInteger()

    let findBestAttackers player =
        player?rating.AsInteger() +
        player?composure.AsInteger() +
        player?acceleration.AsInteger() +
        player?aggression.AsInteger() +
        player?agility.AsInteger() +
        player?balance.AsInteger() +
        player?ballcontrol.AsInteger() +
        player?skillMoves.AsInteger() +
        player?curve.AsInteger() +
        player?dribbling.AsInteger() +
        player?finishing.AsInteger() +
        player?freekickaccuracy.AsInteger() +
        player?longshots.AsInteger() +
        player?penalties.AsInteger() +
        player?shotpower.AsInteger() +
        player?sprintspeed.AsInteger()