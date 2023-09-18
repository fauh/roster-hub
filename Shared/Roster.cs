using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorApp.Shared
{
    public class Roster
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string[] Tags { get; set; }
        public string RosterBody { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Faction { get; set; }        
        public int? Wins { get; set; }
        public int? Losses { get; set; }
        public int? TotalGames { get; set; }
        public bool WasWinner { get { return Wins == TotalGames; } }
    }
}
