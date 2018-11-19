# F# Fantasy Football

First off, I want to give credit to Martin Andersen and his awesome [post](https://martinand.net/2017/12/11/the-soccer-player-best-suited-to-be-santa-claus/) from last year's F# Advent Calendar, without which, I wouldn't have found the [The EA SPORTS FUT Database](https://www.easports.com/fifa/ultimate-team/fut/database) which I use for this post.

### The Idea

What I'm going to do in this blog post, is create a simple program which, given a valid football formation (442,452,352) will query the EA SPORTS FUT Database and return the best 11 players given a set of player characteristics. While this isn't particulary tricky, it does cover a lot of the basic F# idioms such as Record Types, Discriminated Unions, Pattern Matching and Partial Application.

### The Domain

I have an F# file called `Domain.fs` that has my Discriminated Union and Record Types which contains all the information about my players, team, possible formations and player positions:

```csharp
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
```

### Main Program

95% of the rest of the code is in a file called `FantasyFootball.fs`. What we will do for this part, is work through the code in logical steps until we have our team selected for us.

I suppose the first step in all of this is selecting the formation you would like. I'm quite a traditional when it comes to football, so for this blog post I'm going to pick `442`.

The first 2 functions that we'll look are:

```csharp
let findBestTeam formation =
	let players = getPlayers
	let pickedFormation = match formation with
	                      | FourFourTwo -> FourFourTwo
	                      | FourThreeThree -> FourThreeThree
	                      | ThreeFiveTwo -> ThreeFiveTwo
	                      | FourFiveOne -> FourFiveOne
	
	createTeam players pickedFormation |> printTeam
    
let pickTeam (stringFormation:string) =
	match stringFormation with
	| "442" -> findBestTeam FourFourTwo
	| "433" -> findBestTeam FourThreeThree
	| "352" -> findBestTeam ThreeFiveTwo
	| "451" -> findBestTeam FourFiveOne
	| _ -> failwith "Unknown formation, please try again!!"
```

Firstly, we call the `pickTeam` function passing in a string such as "442" which then uses pattern matching to determine which named case to pass to the `findBestTeam` function.

Once we are in the `findBestTeam` function, we first called the `getPlayers` (which we will look at shortly) and then we extract the type from `formation`. After we have done those 2 things, we have all the players in database and we have our chosen formation. The next step is to call the `createTeam` function:

```csharp
let createTeam (players:JsonValue[]) (pickedFormation:Formation) =
    let g,d,m,a = match pickedFormation with
                  | FourFourTwo -> 1,4,4,2
                  | FourThreeThree -> 1,4,3,3
                  | ThreeFiveTwo -> 1,3,5,2
                  | FourFiveOne -> 1,4,5,1

    let goalKeeper =  getStartingPlayers (getAllGoalkeepers players) findBestGoalkeeper g |> Array.head
    let defenders =  getStartingPlayers (getAllDefenders players) findBestDefenders d
    let midfielders =  getStartingPlayers (getAllMidfielders players) findBestMidfielders m
    let attackers =  getStartingPlayers (getAllAttackers players) findBestAttackers a
    
    {
        Goalkeeper = goalKeeper
        Defenders = defenders
        Midfielder = midfielders
        Attacker = attackers
    }
    
    let getStartingPlayers players findBest numberToTake =
    players
    |> Array.sortByDescending findBest
    |> Array.take numberToTake
    |> Array.map createPlayerFromJson
```
The first line of the `createTeam` function pattern matches over the `pickedFormation` which is passed in and then return a tuple containing the number of players that are needed in each position which is deconstructed using a let binding (I've changed the value names to g,d,m,a to save on space for the sake of the blog post). Once we have the number of players needed for each position we can call the `getStartingPlayers` to get the players best suited for our team and then we just return the `Team` record type, which is then printed to the console using a method in our `Helper.fs` file.

The `getStartingPlayers` then takes in the players, a function to sort the players by and then the number of players to take.

The next part of code that we'll look through is in charge of actually finding the best players for each position:

```csharp
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
```

The EA SPORTS FUT Database has retired and classic players which they call Icon players. As nice as it would be to include these in our fantasy team, the Icon players don't play much football nowadays so they wouldn't score us many points.

The `removeIconPlayers` function in the section above is very simple and is just passed to `Array.filter` to remove icon players and the `removeDupliactePlayers` function is passed to `Array.distinctBy` to remove duplicate players.

The next function: `getAllPlayersInPosition` is the function that we are going to use [partial application](https://fsharpforfunandprofit.com/posts/partial-application/) on, so we are making `pos:Position` the first argument so that when we call the function with 1 argument (instead of the 2 that it's declared with) we get back a function that requires 1 argument `(players:JsonValue[])` but will use the implmentation from `getAllPlayersInPosition`.

The next function is the `getPlayers` function that we use to call The EA SPORTS FUT Database API and retrieve the players:

```csharp
let getPlayers =
    let getPage page = 
        async {
            let! data = JsonValue.AsyncLoad (sprintf "https://www.easports.com/fifa/ultimate-team/api/fut/item?page=%i" page)
            return [| for item in data?items -> item |] 
        }

    let value = JsonValue.Load ("https://www.easports.com/fifa/ultimate-team/api/fut/item")
    let totalPages = value?totalPages.AsInteger()

    [|1..totalPages|] 
    |> Array.map getPage
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Array.concat
```



### Github

Source code is availble on Github: [https://github.com/iwasdavid/fsharp-fantasy-football](https://github.com/iwasdavid/fsharp-fantasy-football)

### Thanks

I would like to thanks [Stuart Lang](https://stuartlang.uk/) for taking a look at my code and making some suggestions which improved it.

Merry Christmas and a Happy New Year!
