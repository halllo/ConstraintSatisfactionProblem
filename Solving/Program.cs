using Microsoft.SolverFoundation.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Combinations;

namespace Solving
{
    /* Assigning 60 players to 10 stations over 10 round, using the Microsoft Solver Foundation.
     * However 60 players seem to be too many variables, so i solve with 30 and clone them.
     */
    internal class Program
    {
        static void Main(string[] args)
        {
            var assignments = GetAssignments(players: 30, stations: 10);
            assignments.Items.AddRange(assignments.Items.Select(i => new Assignment
            {
                Player = assignments.Players + i.Player,
                Round = i.Round,
                Station = i.Station,
                Slot = i.Slot
            }));
            Export(assignments);

            Console.WriteLine("DONE");
            Console.ReadLine();
        }

        private static Assignments GetAssignments(int players, int stations)
        {
            int rounds = stations;
            int playersPerStationPerRound = players / stations;

            var context = SolverContext.GetContext();
            var model = context.CreateModel();

            var playersDomain = Domain.IntegerRange(0, players - 1);
            var roundDecisions = new Dictionary<int, List<Decision>>();
            var stationDecisions = new Dictionary<int, List<Decision>>();
            var slotPairs = new List<(Decision first, Decision second)>();
            for (int round = 0; round < rounds; round++)
            {
                for (int station = 0; station < stations; station++)
                {
                    var slots = new List<Decision>();
                    for (int slot = 0; slot < playersPerStationPerRound; slot++)
                    {
                        var decision = new Decision(playersDomain, $"round{round}_station{station}_slot{slot}");
                        model.AddDecision(decision);
                        {
                            slots.Add(decision);
                        }
                        {
                            if (!roundDecisions.ContainsKey(round)) roundDecisions.Add(round, new List<Decision>());
                            roundDecisions[round].Add(decision);
                        }
                        {
                            if (!stationDecisions.ContainsKey(station)) stationDecisions.Add(station, new List<Decision>());
                            stationDecisions[station].Add(decision);
                        }
                    }
                    var pairs = slots.Pairs();
                    slotPairs.AddRange(pairs.Skip(1));
                }
            }

            foreach (var round in roundDecisions)
            {
                model.AddConstraint($"round{round.Key}_players_all_different", Model.AllDifferent(round.Value.ToArray()));
            }

            foreach (var station in stationDecisions)
            {
                model.AddConstraint($"station{station.Key}_players_all_different", Model.AllDifferent(station.Value.ToArray()));
            }

            {
                var slotPairTerms = slotPairs.Select(p => (p.first * 1000) + p.second).ToArray();
                model.AddConstraint($"unique_pairs", Model.AllDifferent(slotPairTerms));
            }

            var solution = context.Solve(new ConstraintProgrammingDirective());
            while (solution.Quality != SolverQuality.Infeasible)
            {
                var report = solution.GetReport();
                Console.WriteLine(report);
                var assignments = Parse(report.ToString());
                return new Assignments
                {
                    Players = players,
                    Stations = stations,
                    Rounds = rounds,
                    Items = assignments,
                };

                //solution.GetNext();
            }
            throw new Exception("no solution found");
        }

        private static List<Assignment> Parse(string report)
        {
            var decisionParser = new Regex("^round(?<round>.*?)_station(?<station>.*?)_slot(?<slot>.*?): (?<player>.*?)$");
            var assignments = report.ToString()
                .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => l.StartsWith("round"))
                .Select(l => decisionParser.Match(l))
                .Select(m => new Assignment
                {
                    Round = int.Parse(m.Groups["round"].Value),
                    Station = int.Parse(m.Groups["station"].Value),
                    Slot = int.Parse(m.Groups["slot"].Value),
                    Player = int.Parse(m.Groups["player"].Value)
                })
                .ToList();
            return assignments;
        }

        private static void Export(Assignments assignments)
        {
            var players = assignments.Players;
            var stations = assignments.Stations;
            var rounds = assignments.Rounds;

            var assignment = new StringBuilder();
            assignment.Append($"round;");
            for (int station = 0; station < stations; station++)
            {
                assignment.Append($"station{station};");
            }
            assignment.AppendLine();
            for (int round = 0; round < rounds; round++)
            {
                assignment.Append($"{round};");
                for (int station = 0; station < stations; station++)
                {
                    var ps = assignments.Items.Where(a => a.Round == round && a.Station == station).Select(a => a.Player).ToArray();
                    assignment.Append($"{string.Join(",", ps.Select(p => "p" + p))};");
                }
                assignment.AppendLine();
            }
            File.WriteAllText("assignment.csv", assignment.ToString());
        }

        public class Assignments
        {
            public int Players { get; set; }
            public int Stations { get; set; }
            public int Rounds { get; set; }
            public List<Assignment> Items { get; set; }
        }

        public class Assignment
        {
            public int Round { get; set; }
            public int Station { get; set; }
            public int Slot { get; set; }
            public int Player { get; set; }
        }
    }
}
