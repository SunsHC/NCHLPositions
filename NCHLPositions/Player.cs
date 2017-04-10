using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCHLPositions
{

    internal class Player
    {
        public string Name { get; set; }
        public PlayerPosition NCHLPos { get; set; }
        public PlayerPosition NHLPos { get; set; }
        public NCHLTeam NCHLTeam { get; set; }
        public NHLTeam NHLTeam { get; set; }
    }
}
