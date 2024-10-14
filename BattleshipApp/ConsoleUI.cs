using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BattleshipAppLibrary;
using BattleshipAppLibrary.Models;
using BattleshipAppLibrary.Process;


namespace BattleshipApp
{
    internal static class ConsoleUI
    {
        private static readonly string letterRange = $"[{char.ToUpper(GameLogic.MinNumberOfLines)}-{char.ToUpper(GameLogic.MaxNumberOfLines)}]";
        private static readonly string numberRange = $"[{GameLogic.MinNumberOfColumns}-{GameLogic.MaxNumberOfColumns}]";

        public static void InitGame()
        {
            GameLogic.Players = new List<PlayerModel>(GameLogic.MaxNumberOfPlayers);
            for (ushort playerNo = 0; playerNo < GameLogic.MaxNumberOfPlayers; playerNo++)
            {
                PlayerModel newPlayer = CreatePlayer(playerNo);
                WaitForInput("Press enter to continue.");
            }
            
        }

        public static void RunGame()
        {
            GameLogic.TurnNumber = 1;
            GameLogic.IsGameOver = false;
            ushort playerNo = 0;
            while (!GameLogic.IsGameOver)
            {
                for (playerNo = 0; playerNo < GameLogic.MaxNumberOfPlayers; playerNo++)
                {
                    PlayerModel currentPlayer = GameLogic.Players[playerNo];
                    Console.WriteLine($"Player{playerNo + 1} turn {GameLogic.TurnNumber}.");
                    DisplayLegend();
                    
                    Console.WriteLine();
                    DisplayPlayGround(currentPlayer.ShotsTaken, true, "This is the target ground:");
                    Console.WriteLine();
                    DisplayPlayGround(currentPlayer.FriendlyShips, false, "This is the home ground:");
                    Console.WriteLine();

                    // ask for coordinates to take the shot                    
                    GridSpotModel strikeSpot = AskForCoordinates(currentPlayer.ShotsTaken, true);
                    bool isHit = GameLogic.TakeShot(currentPlayer, strikeSpot);

                    WaitForInput("Press enter to continue.");

                    if (isHit)
                    {
                        // display feedback
                        Console.WriteLine("Congratulations! That's a HIT!");
                    }
                    else
                    {
                        // display feedback
                        Console.WriteLine("Bummer! That's a MISS!");
                    }

                    DisplayLegend();
                    Console.WriteLine();

                    // display new target ground
                    DisplayPlayGround(currentPlayer.ShotsTaken, true, "This is the updated target ground:");

                    // end turn and check if the game is over
                    WaitForInput("Press enter to end turn.");
                    if (GameLogic.IsGameOver)
                    {
                        GameLogic.Winner = currentPlayer;
                        break;
                    }
                }

                // only increment TurnNumber if the game is still running after all players have taken their turn
                if (!GameLogic.IsGameOver)
                {
                    GameLogic.TurnNumber++;
                }
            }

            ShowVictoryMessage(GameLogic.Winner, playerNo);

        }

        private static void WaitForInput(string message)
        {
            Console.WriteLine();
            Console.Write(message);
            Console.ReadLine();
            Console.Clear();
        }

        private static void ShowVictoryMessage(PlayerModel winner, ushort playerNo)
        {
            Console.Clear();
            Console.WriteLine($"Congratulations, Player{playerNo + 1}! You WON!\n");
            Console.WriteLine($"The game lasted {GameLogic.TurnNumber} turns.");
            PrintFriendlyShips(winner, playerNo);
            PrintShotsTaken(winner, playerNo);
        }

        public static void ExitGame()
        {
            WaitForInput("Press enter to exit game.");
        }

        private static void DisplayPlayGround(List<GridSpotModel> spots, bool isTarget, string message)
        {
            Console.WriteLine(message);            
            ushort currentLine = 0;
            ushort currentColumn = 0;
            ushort numberOflines = ProcessGridSpot.FromLetterToNumber(GameLogic.MaxNumberOfLines);
            ushort numberOfColumns = GameLogic.MaxNumberOfColumns;
            for (currentLine = 0; currentLine <= numberOflines; currentLine++)
            {
                char currentLetter = ProcessGridSpot.FromNumberToLetter(currentLine);
                for (currentColumn = 0; currentColumn <= numberOfColumns; currentColumn++)
                {
                    if (currentLine == 0)
                    {
                        if (currentColumn == 0)
                        {
                            // matrix [0][0] displays whitspace
                            Console.Write("  ");
                        }
                        else
                        {
                            // matrix [0][1...n] displays the number of columns
                            Console.Write($"{currentColumn} ");
                        }
                    }
                    else
                    {
                        if (currentColumn == 0)
                        {
                            // matrix [1...n][0] displays the number of lines as letters
                            Console.Write($"{currentLetter} ");
                        }
                        else
                        {
                            //check if current letter and number are found in spots list (friendlyShips or shotsTaken)
                            GridSpotModel currentSpot = ProcessGridSpot.FindSpot(currentLetter, currentColumn, spots);
                            {
                                // if current spot is found in the list, it means that it's either: a hit, a ship or a miss
                                if (currentSpot != null)
                                {
                                    if (currentSpot.IsHit)
                                    {
                                        // display hit
                                        Console.Write($"{GameLogic.Hit} ");
                                    }
                                    else
                                    {
                                        if (isTarget)
                                        {
                                            // display missed
                                            Console.Write($"{GameLogic.Missed} ");
                                        }
                                        else
                                        {
                                            // display ship
                                            Console.Write($"{GameLogic.Ship} ");
                                        }
                                    }
                                }
                                else
                                {
                                    //display empty
                                    Console.Write($"{GameLogic.Empty} ");
                                }  
                            }
                        }
                    }
                }
                // end of the line
                Console.WriteLine();
            }
        }

        private static void DisplayLegend()
        {
           Console.WriteLine($"{GameLogic.Ship} - ship, {GameLogic.Hit} - hit, {GameLogic.Missed} - missed, {GameLogic.Empty} - Empty.");
        }

        private static void PrintFriendlyShips(PlayerModel player, ushort playerNo)
        {
            string output = $"Player{playerNo + 1} ship positions are: ";
            output += ProcessPlayerModel.SpotListToString(player.FriendlyShips);
            Console.WriteLine(output);
        }

        private static void PrintShotsTaken(PlayerModel player, ushort playerNo)
        {
            string output = $"Player{playerNo + 1} shots taken are: ";
            output += ProcessPlayerModel.SpotListToString(player.ShotsTaken);
            Console.WriteLine(output);
        }

        private static PlayerModel CreatePlayer(ushort playerNo)
        {
            Console.WriteLine($"Hello, Player{playerNo + 1}!");
            Console.WriteLine();
            List<GridSpotModel> friendlyShips = PlaceShips();

            PlayerModel newPlayer = ProcessPlayerModel.InitPlayer(friendlyShips);
            GameLogic.Players.Add(newPlayer);

            Console.WriteLine();
            DisplayPlayGround(newPlayer.FriendlyShips, false, "This will be your homeground: ");

            return newPlayer;
        }

        private static List<GridSpotModel> PlaceShips()
        {
            List<GridSpotModel> friendlyShips = new List<GridSpotModel>(PlayerModel.MaxNumberOfShips);

            for (ushort battleshipNo = 0; battleshipNo < PlayerModel.MaxNumberOfShips; battleshipNo++)
            {
                GridSpotModel newShip = AskForCoordinates(friendlyShips, false, battleshipNo);
                friendlyShips.Add(newShip);
            }

            return friendlyShips;
        }

        private static GridSpotModel AskForCoordinates(List<GridSpotModel> spots, bool isStrike, int shipNumber = 0)
        {
            if (isStrike)
            {
                Console.Write($"Please input coordinates for the strike {letterRange}{numberRange}: ");
            }
            else
            {
                Console.Write($"Please input the location of your  battleship no. {shipNumber + 1} {letterRange}{numberRange}: ");
            }

            string input = Console.ReadLine();
            GridSpotModel spot = ExtractCoordinates(input);

            while (ProcessGridSpot.IsSpotTaken(spots, spot))
            {
                if (isStrike)
                {
                    Console.Write("This spot is already hit. Please try another: ");
                }
                else
                {
                    Console.Write("This spot is already taken. Please try another: ");
                }                
                input = Console.ReadLine();
                spot = ExtractCoordinates(input);
            }

            return spot;
        }

        private static GridSpotModel ExtractCoordinates (string input)
        {
            var result = GameLogic.IsValidSpot(input);

            while (!result.isValid)
            {
                Console.Write("Invalid input. Please try again: ");
                input = Console.ReadLine();
                result = GameLogic.IsValidSpot(input);
            }

            GridSpotModel output = ProcessGridSpot.InitSpot(result.letter, result.number);

            return output;
        }


    }
}
