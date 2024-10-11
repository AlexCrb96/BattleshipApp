using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipAppLibrary.Models
{
    public class PlayerModel
    {
        public const ushort MaxNumberOfShips = 3;
        public List<GridSpotModel> FriendlyShips { get; set; }
        public List<GridSpotModel> ShotsTaken { get; set; }

    }
}
