using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipAppLibrary.Models
{
    public class GridSpotModel
    {
        public char SpotLetter { get; set; }
        public ushort SpotNumber { get; set; }
        public bool IsHit { get; set; }

    }
}
