using MySql.Data.MySqlClient;
using RiotSharp.MatchEndpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotSharpDataMining.DataSource
{
    public class DatabaseFacade
    {
        private readonly string _connectionString;
        private const string InsertTeamPreparedStatement =
            "INSERT INTO Teams(ChampionIdBan1, ChampionIdBan2, ChampionIdBan3, Winner,BaronKills,DragonKills,InhibitorKills,TowerKills,FirstDragonKill,FirstBaronKill,FirstInhibitorKill,FirstTowerKill,FirstBlood) VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10})";
        private const string InsertMatctPreparedStatement =
            "INSERT INTO MatchStatistics(Region, WinningTeamId, LosingTeamId, MatchDuration, MatchVersion) VALUES ({0},{1},{2},{3},{4})";

        public DatabaseFacade(string connectionString)
        {
            _connectionString = connectionString;
        }


        public void SaveMatch(MatchDetail matchDetail, int leagueLevel)
        {
            var winningTeam = matchDetail.Teams.First(x => x.Winner);

            var bannedSql = GenerateBansSql(winningTeam.Bans);
            // SQL SPAS
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                // Team queries
                var teamQueries = matchDetail.Teams.Select(x => string.Format(InsertTeamPreparedStatement,
                    GenerateBansSql(x.Bans),
                    x.Winner,
                    x.BaronKills,
                    x.DragonKills,
                    x.InhibitorKills,
                    x.TowerKills,
                    x.FirstDragon,
                    x.FirstBaron,
                    x.FirstInhibitor,
                    x.FirstTower,
                    x.FirstBlood));


                // Match query
                var matchQuery = string.Format("INSERT INTO MatchStatistics(Region, WinningTeamId, LosingTeamId, MatchDuration, MatchVersion) VALUES ({0},{1},{2},{3},{4})");
                // Participnt query.
                foreach (var cmd in teamQueries.Select(query => new MySqlCommand(query, conn)))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static string GenerateBansSql(List<BannedChampion> bannedChampions)
        {
            if (bannedChampions == null) { bannedChampions = new List<BannedChampion>(); }

            var bannedSql = bannedChampions.Select(x => "" + x.ChampionId).Aggregate((x, y) => y + "," + x);

            for (var i = 0; i < 3 - bannedChampions.Count; i++)
            {
                bannedSql += ",NULL";
            }

            return bannedSql;
        }
    }
}
