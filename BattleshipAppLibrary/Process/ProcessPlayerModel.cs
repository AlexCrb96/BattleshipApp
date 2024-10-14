using BattleshipAppLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipAppLibrary.Process
{
    public static class ProcessPlayerModel
    {
        public static PlayerModel InitPlayer (List<GridSpotModel> friendlyShips)
        {
            PlayerModel newPlayer = new PlayerModel();

            // No shots taken yet
            newPlayer.ShotsTaken = new List<GridSpotModel>();
            // A new player must always have friendly ships
            newPlayer.FriendlyShips = friendlyShips;

            return newPlayer;
        }

        public static string SpotListToString(List<GridSpotModel> spots)
        {
            string output = "";

            foreach(GridSpotModel spot in spots)
            {
                output += $"{spot.SpotLetter}{spot.SpotNumber}, ";
            }

            output = output.Substring(0, output.Length - 2);

            return output;
        }
    }
}
