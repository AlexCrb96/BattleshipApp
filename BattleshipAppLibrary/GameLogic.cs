using BattleshipAppLibrary.Models;
using BattleshipAppLibrary.Process;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static List<PlayerModel> Players { get; set; }

        public static bool IsPlayerDead { get; private set; } = false;

        public static bool IsDone ()
        {

            foreach (PlayerModel currentPlayer in Players)
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
                    return true;
                }
            }
   
            return false;
        }

        public static bool TakeShot(PlayerModel currentPlayer, GridSpotModel strikeSpot)
        {
            bool isHit = false;
            // strikeSpot.IsHit = true; -> not sure if this is necessary
            currentPlayer.ShotsTaken.Add(strikeSpot);
            List<PlayerModel> enemies = Players.Where(obj => obj != currentPlayer).ToList();
            foreach (PlayerModel enemyPlayer in enemies)
            {
                foreach (GridSpotModel enemyShip in enemyPlayer.FriendlyShips)
                {
                    if (ProcessGridSpot.IsHit(strikeSpot, enemyShip))
                    {
                        isHit = true;
                        if (IsDone())
                        {
                            IsPlayerDead = true;
                        }
                        return isHit;
                    }
                }
            }
            return isHit;
        }
    }
}
