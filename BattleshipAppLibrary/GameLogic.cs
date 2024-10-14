using BattleshipAppLibrary.Models;
using BattleshipAppLibrary.Process;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BattleshipAppLibrary
{
    public static class GameLogic
    {
        public const ushort MaxNumberOfPlayers = 2;
        public const ushort MinNumberOfColumns = 1;
        public const ushort MaxNumberOfColumns = 5;
        public const char MinNumberOfLines = 'A';
        public const char MaxNumberOfLines = 'E';
        public static readonly string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const char Hit = 'X';
        public const char Empty = 'o';
        public const char Ship = '#';
        public const char Missed = '*';

        public static ushort TurnNumber { get; set; }
        public static bool IsGameOver { get; set; }
        public static PlayerModel Winner { get; set; }
        public static List<PlayerModel> Players { get; set; }
        public static bool HasShips (PlayerModel currentPlayer)
        {
            ushort shipsRemaining = PlayerModel.MaxNumberOfShips;
            foreach (GridSpotModel ship in currentPlayer.FriendlyShips)
            {
                if (ship.IsHit)
                {
                    shipsRemaining--;
                }
            }
            if (shipsRemaining == 0)
            {
                return false;
            }

            return true;
        }

        public static (bool isValid, char letter, ushort number) IsValidSpot(string input)
        {
            bool isValid = false;
            char letter = '\0';
            ushort number = 0;

            string regexPattern = $@"\[?\s*([{char.ToUpper(GameLogic.MinNumberOfLines)}-{char.ToUpper(GameLogic.MaxNumberOfLines)}{char.ToLower(GameLogic.MinNumberOfLines)}-{char.ToLower(GameLogic.MaxNumberOfLines)}])\s*\]?\s*-?\s*\[?\s*([{GameLogic.MinNumberOfColumns}-{GameLogic.MaxNumberOfColumns}])\s*\]?";
            Match match = Regex.Match(input, regexPattern);

            if (match.Success)
            {
                isValid = true;
                letter = char.ToUpper(match.Groups[1].Value[0]);
                number = ushort.Parse(match.Groups[2].Value);
            }

            return (isValid, letter, number);
        }

        public static bool TakeShot(PlayerModel currentPlayer, GridSpotModel strikeSpot)
        {
            bool isHit = false;
            currentPlayer.ShotsTaken.Add(strikeSpot);
            List<PlayerModel> enemies = Players.Where(obj => obj != currentPlayer).ToList();
            foreach (PlayerModel enemyPlayer in enemies)
            {
                foreach (GridSpotModel enemyShip in enemyPlayer.FriendlyShips)
                {
                    isHit = ProcessGridSpot.IsHit(strikeSpot, enemyShip);
                    if (isHit)
                    {
                        if (!HasShips(enemyPlayer))
                        {
                            IsGameOver = true;
                        }
                        return isHit;
                    }
                }
            }
            return isHit;
        }
    }
}
