using Microsoft.SolverFoundation.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Solving
{
    /* Assigning 60 players to 10 stations over 10 round, using the Microsoft Solver Foundation.
     * However 60 players seem to be too many variables, so i solve with 30 and clone them.
     */
    internal class Program
    {
        static int players = 60;
        static int stations = 10;
        static int rounds = stations;
        static int playersPerStationPerRound = players / stations;

        static void Main(string[] args)
        {
            var context = SolverContext.GetContext();
            var model = context.CreateModel();

            var playersDomain = Domain.IntegerRange(0, players - 1);
            var roundDecisions = new Dictionary<int, List<Decision>>();
            var stationDecisions = new Dictionary<int, List<Decision>>();
            for (int round = 0; round < rounds; round++)
            {
                for (int station = 0; station < stations; station++)
                {
                    for (int slot = 0; slot < playersPerStationPerRound; slot++)
                    {
                        var decision = new Decision(playersDomain, $"round{round}_station{station}_slot{slot}");
                        model.AddDecision(decision);
                        {
                            if (!roundDecisions.ContainsKey(round)) roundDecisions.Add(round, new List<Decision>());
                            roundDecisions[round].Add(decision);
                        }
                        {
                            if (!stationDecisions.ContainsKey(station)) stationDecisions.Add(station, new List<Decision>());
                            stationDecisions[station].Add(decision);
                        }
                    }
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

            var solution = context.Solve(new ConstraintProgrammingDirective());
            while (solution.Quality != SolverQuality.Infeasible)
            {
                var report = solution.GetReport();
                Console.WriteLine(report);
                Export(report.ToString());

                //solution.GetNext();
                break;
            }

            Console.WriteLine("DONE");
            Console.ReadLine();
        }

        private static void Export(string report)
        {
            var decisionParser = new Regex("^round(?<round>.*?)_station(?<station>.*?)_slot(?<slot>.*?): (?<player>.*?)$");
            var assignments = report.ToString()
                .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => l.StartsWith("round"))
                .Select(l => decisionParser.Match(l))
                .Select(m => new
                {
                    round = int.Parse(m.Groups["round"].Value),
                    station = int.Parse(m.Groups["station"].Value),
                    slot = int.Parse(m.Groups["slot"].Value),
                    player = int.Parse(m.Groups["player"].Value)
                })
                .ToList();

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
                    var ps = assignments.Where(a => a.round == round && a.station == station).Select(a => a.player).ToArray();
                    assignment.Append($"{string.Join(",", ps.Select(p => "p" + p))},{string.Join(",", ps.Select(p => "p" + (players + p)))};");
                }
                assignment.AppendLine();
            }
            File.WriteAllText("assignment.csv", assignment.ToString());
        }
    }
}
