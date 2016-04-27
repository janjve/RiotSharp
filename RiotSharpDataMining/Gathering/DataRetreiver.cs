using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MySql.Data.MySqlClient;
using RiotSharp;
using RiotSharp.GameEndpoint.Enums;
using RiotSharp.LeagueEndpoint;
using RiotSharp.LeagueEndpoint.Enums;
using RiotSharpDataMining.Constants;
using RiotSharpDataMining.DataSource;
using RiotSharpDataMining.Gathering;

namespace RiotSharpDataMining.DataCollection
{
    public class DataRetreiver
    {

        public static void Run2()
        {
            var api = RiotApi.GetInstance(ConnectionConstant.RiotApiKey);

            //Try get summoners from db
            List<int> summonerIds;
            bool isDbEmpty;
            using (var conn = new MySqlConnection(ConnectionConstant.MySqlConnectionString))
            {
                conn.Open();
                string query = string.Format("Select * FROM Participants LIMIT 1");
                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteReader();
                isDbEmpty = result.HasRows;
            }
            if (true)
            {
                summonerIds = new List<int> { DataConstants.InitialSummoner };
            }
            else
            {
                summonerIds = GetSummoners();
            }
            var summoners = api.GetLeagues(DataConstants.Region, summonerIds.Take(1).ToList());

            //Loops through summoner seeds
            foreach (var summoner in summoners)
            {
                var summonerLeague = api.GetEntireLeaguesAsync(DataConstants.Region, new List<int> { (int)summoner.Key }).Result;
                //Loops through all summoners in league
                foreach (var summonerInLeague in summonerLeague)
                {
                    var recentMatches = api.GetRecentGames(DataConstants.Region, summonerInLeague.Key);
                    //Loops through all matches for that summoner
                    foreach (var recentMatch in recentMatches.Where(x => x.GameType == GameType.MatchedGame
                        && x.GameMode == GameMode.Classic
                        && x.MapType == MapType.SummonersRift
                        && x.GameSubType == GameSubType.RankedSolo5x5))
                    {
                        try
                        {
                            var matchDetails = api.GetMatch(DataConstants.Region, recentMatch.GameId);
                            var league = summonerLeague[summoner.Key]
                                .Where(x => x.Queue == DataConstants.Queue)
                                .Select(x => x.Tier)
                                .First();
                            new DatabaseFacade(ConnectionConstant.MySqlConnectionString).SaveMatch(matchDetails, (int)league);
                        }
                        catch (Exception) { /*Ignore if error*/}
                    }
                }
                //Insert the current division into Leagues!!
            }
        }




        private static List<int> GetSummoners()
        {
            var api = RiotApi.GetInstance(ConnectionConstant.RiotApiKey);
            var leagues = new List<MiningLeague>();
            var summoners = new List<int>();

            using (var conn = new MySqlConnection(ConnectionConstant.MySqlConnectionString))
            {
                conn.Open();
                var query = "SELECT * FROM Leagues";
                var cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteReader();
                var something = result;
                while (result.NextResult())
                {
                    leagues.Add(new MiningLeague
                    {
                        LeagueLevel = (Tier)result.GetInt32(0),
                        Region = (Region)result.GetInt32(1),
                        LeagueName = result.GetString(2)
                    });
                }

                foreach (var league in (Tier[])Enum.GetValues(typeof(Tier)))
                {
                    for (var i = 0; true; i++)
                    {
                        var ids = new List<int>();
                        query = string.Format("Select summonerId FROM Participants WHERE LeagueLevel = " + (int)league
                            + " OFFSET " + i * 1000 + " LIMIT 1000");
                        cmd = new MySqlCommand(query, conn);
                        result = cmd.ExecuteReader();
                        while (result.NextResult())
                        {
                            ids.Add(result.GetInt32(0));
                        }
                        var summonerLeagues = api.GetLeagues(DataConstants.Region, ids);
                        foreach (var summonerLeague in summonerLeagues)
                        {
                            //summonerLeague.Value.
                        }

                    }
                }
                return summoners;
            }
        }
    }
}
