using Combinations;

// Assigning 60 players to 10 stations over 10 round, by chance

var players = 60;
var stations = 10;
var rounds = stations;
var playersPerStationPerRound = players / stations;

var validAssignments = Enumerable.Range(0, int.MaxValue)
    .Select(run => GetRandomAssignment())
    .Where(NoStationTwice)
    .Take(1);

foreach (var validAssignment in validAssignments)
{
    Console.WriteLine(validAssignment);
}

bool NoStationTwice(IReadOnlyList<PlayerAtStation> playerAtStationAtRounds)
{
    var stationsAndRoundsByPlayers = playerAtStationAtRounds.GroupBy(p => p.Player);
    return stationsAndRoundsByPlayers.All(player => player.GroupBy(p => p.Station).Count() == stations);
}

IReadOnlyList<PlayerAtStation> GetRandomAssignment()
{
    var stationsPerRound = new List<PlayerAtStation>();
    var playersPerRound = Enumerable.Range(0, players).Randomize().Chunk(6).ToArray();
    for (int station = 0; station < stations; station++)
    {
        foreach (var player in playersPerRound[station])
        {
            stationsPerRound.Add(new PlayerAtStation(player, station));
        }
    }
    return stationsPerRound.AsReadOnly();
}

public record PlayerAtStation(int Player, int Station);