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
        public static bool IsSpotTaken(List<GridSpotModel> spots, GridSpotModel newSpot)
        {
            return spots.Any(spot => char.ToUpper(spot.SpotLetter) == char.ToUpper(newSpot.SpotLetter) && spot.SpotNumber == newSpot.SpotNumber);
        }

        public static GridSpotModel InitSpot(char letter, ushort number)
        {
            GridSpotModel output = new GridSpotModel();
            output.SpotLetter = letter;
            output.SpotNumber = number;
            output.IsHit = false;

            return output;
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

        public static GridSpotModel FindSpot(char spotLetter, ushort spotNumber, List<GridSpotModel> spots)
        {
            GridSpotModel currentSpot = null;

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
