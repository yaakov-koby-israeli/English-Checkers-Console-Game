using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ex02.ConsoleUtils;

namespace Ex02
{
    internal class UserInterface
    {
        public void StartGame()
        {
            Console.WriteLine("Please enter your name (up to 20 characters, no spaces):");

            string firstPlayerName = Console.ReadLine();

            while (!(IsValidPlayerName(firstPlayerName)))
            {
                Console.WriteLine("Please enter your name again (up to 20 characters, no spaces):");
                firstPlayerName = Console.ReadLine();
            }

            Screen.Clear();

            PrintOpponentChoiceMenu();
            string inputChoice = Console.ReadLine();
            int opponentChoice = 0;

            while (!int.TryParse(inputChoice, out opponentChoice) || !IsValidOpponentChoice(opponentChoice))
            {
                Screen.Clear();
                Console.WriteLine("Invalid input. Please enter a valid choice.");
                PrintOpponentChoiceMenu();
                inputChoice = Console.ReadLine();
            }

            string secondPlayerName;

            if (opponentChoice == 1) 
            {
                secondPlayerName = "Computer";
            }
            else 
            {
                Console.WriteLine("Please enter the name of the second player (up to 20 characters, no spaces):");
                secondPlayerName = Console.ReadLine();
                while (!(IsValidPlayerName(secondPlayerName)))
                {
                    Console.WriteLine("Please enter your name again (up to 20 characters, no spaces):");
                    secondPlayerName = Console.ReadLine();
                }
            }

            Screen.Clear();

            PrintBoardSizeMenu();
            string inputSize = Console.ReadLine();
            int boardSize = 0;

            while (!IsValidBoardSize(out  boardSize,  inputSize))
            {
                
                Screen.Clear();

                Console.WriteLine("Please choose again !");
                Console.WriteLine();

                PrintBoardSizeMenu();
                inputSize = Console.ReadLine();
                
            }

            Screen.Clear();

            GameLogic gamePlay = new GameLogic(boardSize, firstPlayerName, secondPlayerName);

            PrintBoard(gamePlay.GetMainBoard());

            while (true)
            {
                string currentPlayer = gamePlay.GetCurrentPlayerName();
                string currentPiece = gamePlay.GetCurrentPlayerPiece();
                e_PlayerGameStatus gameStatus = e_PlayerGameStatus.Active;
                e_PlayerGameStatus invalidInput = e_PlayerGameStatus.Active;
                
                if (currentPlayer == "Computer")
                {
                    Console.Write($"{currentPlayer}'s Turn (press ‘enter’ to see it’s move):");       
                }
                else
                {
                    Console.Write($"{currentPlayer}'s turn ({currentPiece}):");
                }

                string playerMoveString = Console.ReadLine(); 

                if(PlayerQuit(playerMoveString))
                {                         
                    gamePlay.CalculateScore();
                    Console.WriteLine($"Congratulations, {gamePlay.GetOtherPlayerName()}!"
                                      + $" You won the game with a score of {gamePlay.GetOtherPlayer().GetPlayerScore()}!");

                    if (IsContinue())
                    {
                        Screen.Clear();
                        gamePlay.GetMainBoard().InitializeBoard();
                        PrintBoard(gamePlay.GetMainBoard());
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                if (!gamePlay.GetIsPlayerOneMove() && currentPlayer == "Computer")
                {
                    gamePlay.ComputerGameManager(ref playerMoveString);

                    if(gamePlay.GetPlayerTow().GetPlayerGameStatus() == e_PlayerGameStatus.NoMoreMoves)
                    {
                        Console.WriteLine("No valid moves for the computer.");
                        break;
                    }
                }
                else
                {
                    while (!(IsValidMoveInput(playerMoveString, boardSize, ref invalidInput) && gamePlay.MakeMove(playerMoveString, ref gameStatus)))
                    {
                        if (gameStatus == e_PlayerGameStatus.Error || invalidInput == e_PlayerGameStatus.Error)
                        {
                            Console.WriteLine("Invalid input format. Please enter the move in the format ROWcol>ROWcol.");
                        }
                        if (gameStatus == e_PlayerGameStatus.MissedCapture)
                        {
                            Console.WriteLine("Invalid move! A capture is mandatory. Please try again.");
                        }
                        if (gameStatus == e_PlayerGameStatus.ExtraCapture)
                        {
                            Console.WriteLine("Another capture is available! Please continue.");
                        }

                        playerMoveString = Console.ReadLine(); 
                    }
                    
                }

                Screen.Clear();
                
                PrintBoard(gamePlay.GetMainBoard());
                
                Console.WriteLine($"{currentPlayer}'s turn was ({currentPiece}): {playerMoveString}");

                if (PrintGameStatus(ref gamePlay))
                {
                    if (IsContinue())
                    {
                        Screen.Clear();
                        gamePlay.GetMainBoard().InitializeBoard();
                        PrintBoard(gamePlay.GetMainBoard());
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            Screen.Clear();
        }
        public bool PrintGameStatus(ref GameLogic i_GamePlay)
        {
            bool IsGameOver = false;

            if(i_GamePlay.GameOver() || i_GamePlay.IsGameOverDueToNoMoves())
            {
                if(i_GamePlay.GetPlayerOne().GetPlayerGameStatus() == e_PlayerGameStatus.NoMoreMoves
                   && i_GamePlay.GetPlayerTow().GetPlayerGameStatus() == e_PlayerGameStatus.NoMoreMoves)
                {
                    Console.WriteLine("Tie !");
                     IsGameOver = true;
                }

                else if(i_GamePlay.GetPlayerOne().GetPlayerGameStatus() == e_PlayerGameStatus.NoMoreMoves)
                {
                    Console.WriteLine($"Congratulations, {i_GamePlay.GetPlayerTow().GetPlayerName()}!"
                                      + $" You won the game with a score of {i_GamePlay.GetPlayerTow().GetPlayerScore()}!");
                     IsGameOver = true;
                }

                else
                {
                    Console.WriteLine($"Congratulations, {i_GamePlay.GetPlayerOne().GetPlayerName()}!"
                                      + $" You won the game with a score of {i_GamePlay.GetPlayerOne().GetPlayerScore()}!");
                     IsGameOver = true;
                }
            }

            return IsGameOver;
        }
        public bool IsContinue()
        {
            bool v_IsContinue = false;

            Console.WriteLine("Press 1 To Continue 0 To Exit !");
            string userInput = Console.ReadLine();

            while(!(userInput.Equals("0") || userInput.Equals("1")))
            {
                Console.WriteLine("Press 1 To Continue 0 To Exit !");
                userInput = Console.ReadLine();
            }

            if(userInput.Equals("1"))
            {
                v_IsContinue = true;
            }

            return v_IsContinue;

        }
        public bool PlayerQuit(string i_InputMove)
        {
            bool v_IsGameOver = false;

            if (i_InputMove.Equals("Q") || i_InputMove.Equals("q"))
            {
                v_IsGameOver = true;
            }

            return v_IsGameOver;
        }
        public bool IsValidMoveInput(string i_MoveInput, int i_BoardSize, ref e_PlayerGameStatus o_InvalidInput)
        {
            bool v_IsValidMoveInput = true;
            o_InvalidInput = e_PlayerGameStatus.Active;

            if (!(i_MoveInput.Length == 5) || string.IsNullOrEmpty(i_MoveInput))
            {
                o_InvalidInput = e_PlayerGameStatus.Error;
                v_IsValidMoveInput = false;
            }
            else
            {
                if (!(i_MoveInput[0] >= 'A' && i_MoveInput[0] <= ('A' + i_BoardSize - 1)))
                {
                    o_InvalidInput = e_PlayerGameStatus.Error;
                    v_IsValidMoveInput = false;
                }
                if (!(i_MoveInput[1] >= 'a' && i_MoveInput[1] <= ('a' + i_BoardSize - 1)))
                {
                    o_InvalidInput = e_PlayerGameStatus.Error;
                    v_IsValidMoveInput = false;
                }
                if (i_MoveInput[2] != '>')
                {
                    o_InvalidInput = e_PlayerGameStatus.Error;
                    v_IsValidMoveInput = false;
                }
                if (!(i_MoveInput[3] >= 'A' && i_MoveInput[3] <= ('A' + i_BoardSize - 1)))
                {
                    o_InvalidInput = e_PlayerGameStatus.Error;
                    v_IsValidMoveInput = false;
                }
                if (!(i_MoveInput[4] >= 'a' && i_MoveInput[4] <= ('a' + i_BoardSize - 1)))
                {
                    o_InvalidInput = e_PlayerGameStatus.Error;
                    v_IsValidMoveInput = false;
                }
            }
            
            return v_IsValidMoveInput;
        }
        public void PrintBoardSizeMenu()
        {
            Console.WriteLine("Please select the board size:");
            Console.WriteLine("6 - Small board");
            Console.WriteLine("8 - Medium board");
            Console.WriteLine("10 - Large board");
            Console.Write("Enter your choice (6, 8, or 10): ");
        }
        public void PrintOpponentChoiceMenu()
        {
            Console.WriteLine("Who would you like to play against?");
            Console.WriteLine("1 - Play against the computer");
            Console.WriteLine("2 - Play against another player");
            Console.Write("Enter your choice (1 or 2): ");
        }
        public bool IsValidBoardSize(out int i_BoardSize, string i_UserInput)
        {
            bool v_IsValidBoardSize = false;

            i_BoardSize = 0;  

            if (!string.IsNullOrEmpty(i_UserInput) && int.TryParse(i_UserInput, out int number))
            {
                if (number == 6 || number == 8 || number == 10)
                {
                    i_BoardSize = number;
                    v_IsValidBoardSize = true;
                }
            }

            return v_IsValidBoardSize;
        }
        public bool IsValidOpponentChoice(int opponentChoice)
        {
            bool v_IsValidPlayerName = false;

            if (opponentChoice == 1 || opponentChoice == 2)
            {
                v_IsValidPlayerName = true;
            }

            return v_IsValidPlayerName;

        }
        public bool IsValidPlayerName(string i_PlayerName)
        {
            bool v_IsValidPlayerName = true;

            if (string.IsNullOrEmpty(i_PlayerName))
            {
                v_IsValidPlayerName = false;
            }

            if (i_PlayerName.Length > 20)
            {
                v_IsValidPlayerName = false;
            }

            if (i_PlayerName.Contains(" "))
            {
                v_IsValidPlayerName = false;
            }

            return v_IsValidPlayerName;
        }
        public void PrintBoard(Board i_MainBoard)
        {
            char[] boardRows = null;
            boardRows = new char[4 * i_MainBoard.GetBoardSize() + 2]; 
            boardRows[0] = ' ';

            for (int i = 1; i < 4 * i_MainBoard.GetBoardSize() + 2; i++)
            {
                boardRows[i] = '=';
            }

            for (int i = 0, j = 0; i < 4 * i_MainBoard.GetBoardSize() + 2; i++)
            {
                if ((i + 1) % 4 == 0)
                {
                    Console.Write((char)('a' + j));
                    j++;
                }
                else
                {
                    Console.Write(' ');
                }
            }

            Console.WriteLine();

            for (int i = 0; i < i_MainBoard.GetBoardSize(); i++)
            {
                Console.WriteLine(boardRows);
                Console.Write((char)('A' + i)); 

                for (int j = 0; j < i_MainBoard.GetBoardSize(); j++)
                {
                    Console.Write("|" + i_MainBoard.GetBoardMatrix()[i, j].GetPieceContext());
                }

                Console.Write("|");
                Console.WriteLine();
            }

            Console.WriteLine(boardRows);
        }
    }
}