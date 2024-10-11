using BattleshipAppLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BattleshipAppLibrary.Process
{
    public static class ProcessGridSpot
    {
        public static bool IsSpotTaken(List<GridSpotModel> friendlyShips, GridSpotModel newShip)
        {
            return friendlyShips.Any(ship => char.ToUpper(ship.SpotLetter) == char.ToUpper(newShip.SpotLetter)) &&
                                friendlyShips.Any(ship => ship.SpotNumber == newShip.SpotNumber);
        }

        public static Match IsValidInput (string input)
        {
            string regexPattern = $@"\[?\s*([{char.ToUpper(GameLogic.MinNumberOfLines)}-{char.ToUpper(GameLogic.MaxNumberOfLines)}{char.ToLower(GameLogic.MinNumberOfLines)}-{char.ToLower(GameLogic.MaxNumberOfLines)}])\s*\]?\s*-?\s*\[?\s*([{GameLogic.MinNumberOfColumns}-{GameLogic.MaxNumberOfColumns}])\s*\]?";
            return Regex.Match(input, regexPattern);
        }

        public static bool IsHit (GridSpotModel strikeSpot, GridSpotModel enemyShip)
        {
            bool isHit = strikeSpot.SpotLetter == enemyShip.SpotLetter && strikeSpot.SpotNumber == enemyShip.SpotNumber;
            if (isHit)
            {
                enemyShip.IsHit = true;
                strikeSpot.IsHit = true;
            }
            return isHit;
        }

        public static ushort FromLetterToNumber (char spotLetter)
        {
            ushort i = 1;
            char currentLetter = char.ToUpper(spotLetter);
            for (i = 1; i <= GameLogic.Alphabet.Length; i++)
            {
                if (GameLogic.Alphabet[i-1] == currentLetter)
                {
                    return i;
                }
            }
            // should not reach here
            return 0;
        }

        public static char FromNumberToLetter (ushort spotNumber)
        {
            if (spotNumber >= 1 && spotNumber <= GameLogic.Alphabet.Length)
            {
                return GameLogic.Alphabet[spotNumber-1];
            }
            // should not reach here
            return '?';
        }

        public static GridSpotModel SpotFound(char spotLetter, ushort spotNumber, List<GridSpotModel> spots)
        {
            GridSpotModel currentSpot = new GridSpotModel();

            foreach (GridSpotModel spot in spots)
            {
                if (spot.SpotLetter == spotLetter && spot.SpotNumber == spotNumber)
                {
                    currentSpot = spot;
                }
            }

            return currentSpot;
        }
    }
}
