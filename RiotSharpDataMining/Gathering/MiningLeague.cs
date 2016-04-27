using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotSharp;
using RiotSharp.LeagueEndpoint.Enums;

namespace RiotSharpDataMining.Gathering
{
    public class MiningLeague
    {
        public Tier LeagueLevel { get; set; }
        public Region Region { get; set; }
        public string LeagueName { get; set; }

    }
}
