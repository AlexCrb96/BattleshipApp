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
            for (ushort playerNo = 1; playerNo <= GameLogic.MaxNumberOfPlayers; playerNo++)
            {
                PlayerModel newPlayer = CreatePlayer(playerNo);
                GameLogic.Players.Add(newPlayer);
                Console.Clear();
            }
            
        }

        public static bool RunGame()
        {
            ushort turnNumber = 1;
            bool isGameOver = false;

            while (!GameLogic.IsDone() && !isGameOver)
            {
                for (ushort playerNo = 0; playerNo < GameLogic.MaxNumberOfPlayers; playerNo++)
                {
                    PlayerModel currentPlayer = GameLogic.Players[playerNo];
                    Console.WriteLine($"Player{playerNo + 1} turn {turnNumber}.\n");

                    DisplayPlayGround(currentPlayer.ShotsTaken, true, "target ground:");
                    DisplayPlayGround(currentPlayer.FriendlyShips, false, "home ground:");
                    // ask for coordinates to take the shot                    
                    GridSpotModel strikeSpot = AskForCoordinates(currentPlayer.ShotsTaken, true);
                    bool isHit = GameLogic.TakeShot(currentPlayer, strikeSpot);
                    if (isHit)
                    {
                        //Console.Clear();
                        Console.WriteLine("\nCongratulations! That's a HIT!\n");
                        DisplayPlayGround(currentPlayer.ShotsTaken, true, "new target ground:");
                        isGameOver = EndTurn();
                    }
                    else
                    {
                        //Console.Clear();
                        Console.WriteLine("\nBummer! That's a MISS!\n");
                        DisplayPlayGround(currentPlayer.ShotsTaken, true, "new target ground:");
                        isGameOver = EndTurn();
                    }

                    //PrintFriendlyShips(currentPlayer, playerNo);

                    if (isGameOver)
                    {
                        break;
                    }
                }
                turnNumber++;
            }

            ShowVictoryMessage();
            //DisplayTargetGround(GameLogic.Players[0].ShotsTaken);
            return isGameOver;
        }

        private static bool EndTurn()
        {
            Console.Write("\nPress enter to end turn.");
            Console.ReadLine();
            Console.Clear();
            return GameLogic.IsPlayerDead;
        }

        private static void ShowVictoryMessage()
        {
            Console.Clear();
            Console.WriteLine("Congratulations! You WON!");
        }

        public static void ExitGame()
        {
            Console.WriteLine("Press enter to exit game.");
            Console.ReadLine();
        }

        private static void DisplayPlayGround(List<GridSpotModel> spots, bool isTarget, string message)
        {
            Console.WriteLine($"This is the {message}");            
            ushort i = 0;
            ushort currentNumber = 0;
            ushort numberOflines = ProcessGridSpot.FromLetterToNumber(GameLogic.MaxNumberOfLines);
            ushort numberOfColumns = GameLogic.MaxNumberOfColumns;
            for (i = 0; i <= numberOflines; i++)
            {
                char currentLetter = ProcessGridSpot.FromNumberToLetter(i);
                for (currentNumber = 0; currentNumber <= numberOfColumns; currentNumber++)
                {
                    if (i == 0)
                    {
                        if (currentNumber == 0)
                        {
                            Console.Write("  ");
                        }
                        else
                        {
                            Console.Write($"{currentNumber} ");
                        }
                    }
                    else
                    {
                        if (currentNumber == 0)
                        {
                            Console.Write($"{currentLetter} ");
                        }
                        else
                        {
                            //check if letter and number are found in list spot in targetedSpots
                            GridSpotModel currentSpot = ProcessGridSpot.SpotFound(currentLetter, currentNumber, spots);
                            {
                                // if current spot is hit
                                if (currentSpot.SpotLetter == currentLetter && currentSpot.SpotNumber == currentNumber)
                                {
                                    if (currentSpot.IsHit)
                                    {
                                        // display hit
                                        Console.Write($"{GameLogic.Hit} ");
                                        continue;
                                    }
                                    else
                                    {
                                        if (isTarget)
                                        {
                                            // display missed
                                            Console.Write($"{GameLogic.Missed} ");
                                            continue;
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
                                    continue;
                                }
                                    
                            }
                        }
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();

        }

        private static void PrintFriendlyShips(PlayerModel player, ushort playerNo)
        {
            string output = $"Player{playerNo + 1} ship positions are: ";
            foreach (GridSpotModel spot in player.FriendlyShips)
            {
                output += $"{spot.SpotLetter}{spot.SpotNumber}, ";
            }
            output = output.Substring(0, output.Length - 2);
            Console.WriteLine(output);
        }

        private static PlayerModel CreatePlayer(ushort playerNo)
        {
            PlayerModel newPlayer = new PlayerModel();

            Console.WriteLine($"Hello, Player{playerNo}!");
            // No shots taken yet
            newPlayer.ShotsTaken = new List<GridSpotModel>();
            // Ask player for their ship placements
            newPlayer.FriendlyShips = PlaceShips(playerNo);          
            
            return newPlayer;
        }

        private static List<GridSpotModel> PlaceShips(ushort playerNo)
        {
            List<GridSpotModel> friendlyShips = new List<GridSpotModel>(PlayerModel.MaxNumberOfShips);

            for (ushort battleshipNo = 1; battleshipNo <= PlayerModel.MaxNumberOfShips; battleshipNo++)
            {
                GridSpotModel newShip = AskForCoordinates(friendlyShips, false, battleshipNo);
                friendlyShips.Add(newShip);
            }

            return friendlyShips;
        }

        private static GridSpotModel AskForCoordinates(List<GridSpotModel> spots, bool isStrike, int shipNumber = 0)
        {
            GridSpotModel spot = new GridSpotModel();
            if (isStrike)
            {
                Console.Write($"Please input coordinates for the strike {letterRange}{numberRange}: ");
            }
            else
            {
                Console.Write($"Please input the location of your  battleship no. {shipNumber} {letterRange}{numberRange}: ");
            }
            string input = Console.ReadLine();
            spot = ExtractCoordinates(input);
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
            GridSpotModel output = new GridSpotModel();
            Match match = ProcessGridSpot.IsValidInput(input);
            while (!match.Success)
            {
                Console.Write("Invalid input. Please try again: ");
                input = Console.ReadLine();
                match = ProcessGridSpot.IsValidInput(input);
            }
            output.SpotLetter = char.ToUpper(match.Groups[1].Value[0]);
            output.SpotNumber = ushort.Parse(match.Groups[2].Value);
            output.IsHit = false;
            
            return output;
        }
    }
}
