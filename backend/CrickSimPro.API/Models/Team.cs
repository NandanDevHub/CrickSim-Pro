using System.Collections.Generic;

namespace CrickSimPro.API.Models
{
    public class Team
    {
        public string Name { get; set; }                  // "India"
        public List<PlayerProfile> Players { get; set; }  // 11 players
    }
}
