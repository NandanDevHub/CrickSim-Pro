using System.Collections.Generic;

namespace CrickSimPro.API.Models
{
    public class MatchScenario
    {
        public required string GameType { get; set; }
        public required string PitchType { get; set; }
        public required string Weather { get; set; }
        public int CurrentDay { get; set; }
        public int Overs { get; set; }

        public required string BattingFirst { get; set; }
        public required string BattingSecond { get; set; }

        public required List<PlayerProfile> TeamAPlayers { get; set; }
        public required List<PlayerProfile> TeamBPlayers { get; set; }

        // Legacy backward compatibility properties
        public required List<BatterProfile> BattingFirstPlayers { get; set; }
        public required List<BatterProfile> BattingSecondPlayers { get; set; }
        public List<BatterProfile>? Batters { get; set; }
        public List<string>? BowlerTypes { get; set; }

        public int BattingAggression { get; set; }
        public int BowlingAggression { get; set; }
        public int? TargetScore { get; set; }

        public MatchScenario CloneForSecondInnings(int target)
        {
            return new MatchScenario
            {
                GameType = this.GameType,
                PitchType = this.PitchType,
                Weather = this.Weather,
                CurrentDay = this.CurrentDay,
                Overs = this.Overs,
                BattingFirst = this.BattingSecond,
                BattingSecond = this.BattingFirst,

                // Do NOT swap teams here! Keep team assignments fixed.
                TeamAPlayers = this.TeamAPlayers != null ? new List<PlayerProfile>(this.TeamAPlayers) : null,
                TeamBPlayers = this.TeamBPlayers != null ? new List<PlayerProfile>(this.TeamBPlayers) : null,

                BattingFirstPlayers = this.BattingSecondPlayers,
                BattingSecondPlayers = this.BattingFirstPlayers,
                Batters = this.BattingSecondPlayers != null
                    ? this.BattingSecondPlayers.ConvertAll(bp => new BatterProfile { Name = bp.Name, Type = bp.Type })
                    : new List<BatterProfile>(),
                BowlerTypes = new List<string>(this.BowlerTypes ?? new List<string>()),

                BattingAggression = this.BattingAggression,
                BowlingAggression = this.BowlingAggression,
                TargetScore = target
            };
        }

        public MatchScenario CloneForThirdInnings(int lead, bool forceTeam2Batting = false)
        {
            if (forceTeam2Batting)
            {
                // Follow-on: Team 2 bats again, Team 1 fields
                return new MatchScenario
                {
                    GameType = this.GameType,
                    PitchType = this.PitchType,
                    Weather = this.Weather,
                    CurrentDay = this.CurrentDay + 1,
                    Overs = this.Overs,
                    BattingFirst = this.BattingSecond,
                    BattingSecond = this.BattingFirst,

                    TeamAPlayers = this.TeamBPlayers != null ? new List<PlayerProfile>(this.TeamBPlayers) : null,
                    TeamBPlayers = this.TeamAPlayers != null ? new List<PlayerProfile>(this.TeamAPlayers) : null,

                    BattingFirstPlayers = this.BattingSecondPlayers,
                    BattingSecondPlayers = this.BattingFirstPlayers,
                    Batters = this.BattingSecondPlayers != null
                        ? this.BattingSecondPlayers.ConvertAll(bp => new BatterProfile { Name = bp.Name, Type = bp.Type })
                        : new List<BatterProfile>(),
                    BowlerTypes = new List<string>(this.BowlerTypes ?? new List<string>()),

                    BattingAggression = this.BattingAggression,
                    BowlingAggression = this.BowlingAggression,
                    TargetScore = null
                };
            }
            else
            {
                // Normal 3rd innings: Team 1 bats again
                return new MatchScenario
                {
                    GameType = this.GameType,
                    PitchType = this.PitchType,
                    Weather = this.Weather,
                    CurrentDay = this.CurrentDay + 1,
                    Overs = this.Overs,
                    BattingFirst = this.BattingFirst,
                    BattingSecond = this.BattingSecond,

                    TeamAPlayers = this.TeamAPlayers != null ? new List<PlayerProfile>(this.TeamAPlayers) : null,
                    TeamBPlayers = this.TeamBPlayers != null ? new List<PlayerProfile>(this.TeamBPlayers) : null,

                    BattingFirstPlayers = this.BattingFirstPlayers,
                    BattingSecondPlayers = this.BattingSecondPlayers,
                    Batters = this.BattingFirstPlayers != null
                        ? this.BattingFirstPlayers.ConvertAll(bp => new BatterProfile { Name = bp.Name, Type = bp.Type })
                        : new List<BatterProfile>(),
                    BowlerTypes = new List<string>(this.BowlerTypes ?? new List<string>()),

                    BattingAggression = this.BattingAggression,
                    BowlingAggression = this.BowlingAggression,
                    TargetScore = null
                };
            }
        }

        public MatchScenario CloneForFourthInnings(int target)
        {
            return new MatchScenario
            {
                GameType = this.GameType,
                PitchType = this.PitchType,
                Weather = this.Weather,
                CurrentDay = this.CurrentDay + 1,
                Overs = this.Overs,
                BattingFirst = this.BattingSecond,
                BattingSecond = this.BattingFirst,

                TeamAPlayers = this.TeamAPlayers != null ? new List<PlayerProfile>(this.TeamAPlayers) : null,
                TeamBPlayers = this.TeamBPlayers != null ? new List<PlayerProfile>(this.TeamBPlayers) : null,

                BattingFirstPlayers = this.BattingSecondPlayers,
                BattingSecondPlayers = this.BattingFirstPlayers,
                Batters = this.BattingSecondPlayers != null
                    ? this.BattingSecondPlayers.ConvertAll(bp => new BatterProfile { Name = bp.Name, Type = bp.Type })
                    : new List<BatterProfile>(),
                BowlerTypes = new List<string>(this.BowlerTypes ?? new List<string>()),

                BattingAggression = this.BattingAggression,
                BowlingAggression = this.BowlingAggression,
                TargetScore = target
            };
        }

        public MatchScenario CloneForSuperOver(bool isTeamA)
        {
            return new MatchScenario
            {
                GameType = this.GameType,
                PitchType = this.PitchType,
                Weather = this.Weather,
                CurrentDay = this.CurrentDay,
                Overs = 1,
                BattingFirst = isTeamA ? this.BattingFirst : this.BattingSecond,
                BattingSecond = isTeamA ? this.BattingSecond : this.BattingFirst,
                TeamAPlayers = this.TeamAPlayers != null ? [.. this.TeamAPlayers] : null,
                TeamBPlayers = this.TeamBPlayers != null ? [.. this.TeamBPlayers] : null,
                BattingFirstPlayers = isTeamA ? this.BattingFirstPlayers : this.BattingSecondPlayers,
                BattingSecondPlayers = isTeamA ? this.BattingSecondPlayers : this.BattingFirstPlayers,
                Batters = isTeamA && this.BattingFirstPlayers != null
                    ? this.BattingFirstPlayers.ConvertAll(bp => new BatterProfile { Name = bp.Name, Type = bp.Type })
                    : this.BattingSecondPlayers != null
                        ? this.BattingSecondPlayers.ConvertAll(bp => new BatterProfile { Name = bp.Name, Type = bp.Type })
                        : [],
                BowlerTypes = new List<string>(this.BowlerTypes ?? []),
                BattingAggression = this.BattingAggression,
                BowlingAggression = this.BowlingAggression,
                TargetScore = null
            };
#pragma warning restore CS8601 // Possible null reference assignment.
        }

    }
}
