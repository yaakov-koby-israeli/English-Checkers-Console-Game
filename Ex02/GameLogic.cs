using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;


namespace Ex02
{
    internal class GameLogic
    {  
        private Board m_MainBoard;
        private readonly Player m_FirstPlayer; 
        private readonly Player m_SecondPlayer;
        private bool m_IsPlayerOneMove; 
        public GameLogic(int i_BoardSize, string i_FirstPlayerName, string i_SecondPlayerName)
        {
            m_MainBoard = new Board(i_BoardSize);
            m_FirstPlayer = new Player(i_FirstPlayerName);
            m_SecondPlayer = new Player(i_SecondPlayerName); 
            m_IsPlayerOneMove = true;

            // עדכון סטטוס משחק לשחקנים
            m_FirstPlayer.SetPlayerGameStatus(e_PlayerGameStatus.Active);
            m_SecondPlayer.SetPlayerGameStatus(e_PlayerGameStatus.Waiting);
        }
        public Player GetPlayerOne()
        {
            return m_FirstPlayer;
        }
        public Player GetPlayerTow()
        {
            return m_SecondPlayer;
        }
        public bool GetIsPlayerOneMove()
        {
            return m_IsPlayerOneMove;
        }
        public string GetCurrentPlayerName()
        {
            return m_IsPlayerOneMove ? m_FirstPlayer.GetPlayerName() : m_SecondPlayer.GetPlayerName();
        }
        public string GetOtherPlayerName()
        {
            return m_IsPlayerOneMove ? m_SecondPlayer.GetPlayerName() : m_FirstPlayer.GetPlayerName();
        }
        public Player GetOtherPlayer()
        {
            return m_IsPlayerOneMove ? m_SecondPlayer : m_FirstPlayer;
        }

        public string GetCurrentPlayerPiece()
        {
            return m_IsPlayerOneMove ? "O" : "X";
        }
        public Board GetMainBoard()
        {
            return m_MainBoard;
        }
        public bool MakeMove(string i_PlayerMove, ref e_PlayerGameStatus i_GameStatus)
        {
            bool v_MakeMove = false;
            i_GameStatus = e_PlayerGameStatus.Active;

            int[] currentPosition = ConvertStringToPosition(i_PlayerMove, 0);
            int[] nextStepPosition = ConvertStringToPosition(i_PlayerMove, 3);

            if(!m_MainBoard.IsValidBoardPosition(currentPosition[0], currentPosition[1]))
            {
                i_GameStatus = e_PlayerGameStatus.Error; 
            }

            if(!m_MainBoard.IsValidBoardPosition(nextStepPosition[0], nextStepPosition[1]))
            {
                i_GameStatus = e_PlayerGameStatus.Error; 
            }

            if(m_IsPlayerOneMove && i_GameStatus == e_PlayerGameStatus.Active) 
            {
                if(m_MainBoard.IsCellContainOPiece(currentPosition[0], currentPosition[1]))
                {
                    if(ValidateRegularMove(ref currentPosition, ref nextStepPosition))
                    {
                        if(CheckForMandatoryCaptureOPiece())
                        {
                            i_GameStatus = e_PlayerGameStatus.MissedCapture;
                            m_IsPlayerOneMove = true; 
                        }
                        else
                        {
                            m_MainBoard.UpdateBoard(ref currentPosition, ref nextStepPosition);
                            m_IsPlayerOneMove = false;
                            v_MakeMove = true;
                        }
                    }
                    else if(IsValidCaptureForOPiece(ref currentPosition, ref nextStepPosition))
                    {
                        m_MainBoard.UpdateBoard(ref currentPosition, ref nextStepPosition);     
                        if(CheckMultipleJumpsForRegularPiece(ref nextStepPosition) || CheckMultipleJumpsForKingPiece(ref nextStepPosition))
                        {
                            m_IsPlayerOneMove = true;
                            i_GameStatus = e_PlayerGameStatus.ExtraCapture;
                        }
                        else
                        {
                            m_IsPlayerOneMove = false;
                        }

                        v_MakeMove = true;
                    }
                    else
                    {
                        i_GameStatus = e_PlayerGameStatus.Error;
                    }
                }
                else if(m_MainBoard.IsCellContainUPiece(currentPosition[0], currentPosition[1]))
                {
                    if(IsValidKingMove(ref currentPosition, ref nextStepPosition))
                    {
                        if(CheckForMandatoryCaptureOPiece())
                        { 
                            i_GameStatus = e_PlayerGameStatus.MissedCapture;
                            m_IsPlayerOneMove = true; 
                        }
                        else
                        {
                            m_MainBoard.UpdateBoard(ref currentPosition, ref nextStepPosition);
                            m_IsPlayerOneMove = false;
                            v_MakeMove = true;
                        }
                    }
                    else if(IsValidCaptureForUPiece(ref currentPosition, ref nextStepPosition))
                    {
                        m_MainBoard.UpdateBoard(ref currentPosition, ref nextStepPosition);
                        if(CheckMultipleJumpsForKingPiece(ref nextStepPosition))
                        {
                            m_IsPlayerOneMove = true;
                            i_GameStatus = e_PlayerGameStatus.ExtraCapture;
                        }
                        else
                        {
                            m_IsPlayerOneMove = false;
                        }

                        v_MakeMove = true;
                    }
                    else
                    {
                        i_GameStatus = e_PlayerGameStatus.Error;
                    }
                }
                else
                {
                    i_GameStatus = e_PlayerGameStatus.Error;
                }
            }

            if(!m_IsPlayerOneMove && i_GameStatus == e_PlayerGameStatus.Active)
            {
                if(m_MainBoard.IsCellContainXPiece(currentPosition[0], currentPosition[1]))
                {
                    if(ValidateRegularMove(ref currentPosition, ref nextStepPosition))
                    {
                        if(CheckForMandatoryCaptureXPiece())
                        {
                            i_GameStatus = e_PlayerGameStatus.MissedCapture;
                            m_IsPlayerOneMove = false;
                        }
                        else
                        {
                            m_MainBoard.UpdateBoard(ref currentPosition, ref nextStepPosition);
                            m_IsPlayerOneMove = true;
                            v_MakeMove = true;
                        }
                    }
                    else if(IsValidCaptureForXPiece(ref currentPosition, ref nextStepPosition))
                    {
                        m_MainBoard.UpdateBoard(ref currentPosition, ref nextStepPosition);
                        if(CheckMultipleJumpsForRegularPiece(ref nextStepPosition)
                           || CheckMultipleJumpsForKingPiece(ref nextStepPosition))
                        {
                            m_IsPlayerOneMove = false;
                            i_GameStatus = e_PlayerGameStatus.ExtraCapture;
                        }
                        else
                        {
                            m_IsPlayerOneMove = true;
                        }

                        v_MakeMove = true;

                    }
                    else
                    {
                        i_GameStatus = e_PlayerGameStatus.Error;
                    }
                }
                else if(m_MainBoard.IsCellContainKPiece(currentPosition[0], currentPosition[1]))
                {
                    if(IsValidKingMove(ref currentPosition, ref nextStepPosition))
                    {
                        if(CheckForMandatoryCaptureXPiece())
                        {
                            i_GameStatus = e_PlayerGameStatus.MissedCapture;
                            m_IsPlayerOneMove = false;
                        }
                        else
                        {
                            m_MainBoard.UpdateBoard(ref currentPosition, ref nextStepPosition);
                            m_IsPlayerOneMove = true;
                            v_MakeMove = true;
                        }
                    }
                    else if(IsValidCaptureForKPiece(ref currentPosition, ref nextStepPosition))
                    {
                        m_MainBoard.UpdateBoard(ref currentPosition, ref nextStepPosition);
                        if(CheckMultipleJumpsForKingPiece(ref nextStepPosition))
                        {
                            m_IsPlayerOneMove = false;
                            i_GameStatus = e_PlayerGameStatus.ExtraCapture;
                            ;
                        }
                        else
                        {
                            m_IsPlayerOneMove = true;
                        }

                        v_MakeMove = true;
                    }
                    else
                    {
                        i_GameStatus = e_PlayerGameStatus.Error;
                    }
                }
                else
                {
                    i_GameStatus = e_PlayerGameStatus.Error;
                }
            }

            return v_MakeMove;
        }

        public int[] ConvertStringToPosition(string i_PlayerMove, int i_Index)
        {
            int[] currentPosition = new int[2];

            currentPosition[0] = i_PlayerMove[i_Index] - 'A';
            currentPosition[1] = i_PlayerMove[i_Index + 1] - 'a';

            return currentPosition;

        }
        public bool ValidateRegularMove(ref int[] i_CurrentPosition, ref int[] i_NextPosition)
        {
            bool v_ValidateRegularMove = false;
    
            if (i_CurrentPosition[0] == i_NextPosition[0] + 1 && IsCellEmpty(i_NextPosition[0], i_NextPosition[1]) && IsCellContainXPiece(i_CurrentPosition[0], i_CurrentPosition[1])) // בדיקה שהוא חייב לזוז קדימה כלומר שורה 1 למעלה במטריצה!
            {
                if (i_CurrentPosition[1] == i_NextPosition[1] + 1) 
                {
                    v_ValidateRegularMove = !v_ValidateRegularMove;
                }
                else if (i_CurrentPosition[1] == i_NextPosition[1] - 1) 
                {
                    v_ValidateRegularMove = !v_ValidateRegularMove;
                }
            }

            if (i_CurrentPosition[0] == i_NextPosition[0] - 1 && IsCellEmpty(i_NextPosition[0], i_NextPosition[1]) && IsCellContainOPiece(i_CurrentPosition[0], i_CurrentPosition[1])) // בדיקה שהוא חייב לזוז קדימה כלומר שורה 1 למעלה במטריצה!
            {
                if (i_CurrentPosition[1] == i_NextPosition[1] + 1) 
                {
                    return v_ValidateRegularMove = !v_ValidateRegularMove;
                }
                else if (i_CurrentPosition[1] == i_NextPosition[1] - 1) 
                {
                    return v_ValidateRegularMove = !v_ValidateRegularMove;
                }
            }

            return v_ValidateRegularMove;
        }
        public bool IsValidKingMove(ref int[] i_CurrentPosition, ref int[] i_NextStepPosition)
        {
            bool v_IsValidKingMove = false;

            if (i_CurrentPosition[0] == i_NextStepPosition[0] - 1 && IsCellEmpty(i_NextStepPosition[0], i_NextStepPosition[1])) 
            {
                if (i_CurrentPosition[1] == i_NextStepPosition[1] + 1) 
                {
                    v_IsValidKingMove = !v_IsValidKingMove;
                }
                else if (i_CurrentPosition[1] == i_NextStepPosition[1] - 1) 
                {
                    v_IsValidKingMove = !v_IsValidKingMove;
                }
            }

            if (i_CurrentPosition[0] == i_NextStepPosition[0] + 1 && IsCellEmpty(i_NextStepPosition[0], i_NextStepPosition[1])) 
            {
                if (i_CurrentPosition[1] == i_NextStepPosition[1] + 1)
                {
                    return v_IsValidKingMove = !v_IsValidKingMove;
                }
                else if (i_CurrentPosition[1] == i_NextStepPosition[1] - 1) 
                {
                    return v_IsValidKingMove = !v_IsValidKingMove;
                }
            }

            return v_IsValidKingMove;
            
        }
        public bool CheckForMandatoryCaptureXPiece() 
        {
            bool v_CheckForMandatoryCaptureXPiece = false;

            for (int row = 0; row < m_MainBoard.GetBoardSize(); row++)
            {
                for (int col = 0; col < m_MainBoard.GetBoardSize(); col++)
                {
                    e_PieceType e_CurrentPieceType = m_MainBoard.GetPieceAtPosition(row, col); 
                    int[] currentPosition = new int[2];

                    currentPosition[0] = row;
                    currentPosition[1] = col;


                    if (e_CurrentPieceType == e_PieceType.X) 
                    {
                        if (CanCaptureXPiece(ref currentPosition))
                        {
                            v_CheckForMandatoryCaptureXPiece = true;
                        }
                    }
                    if (e_CurrentPieceType == e_PieceType.K)
                    {
                        if (CanCaptureKPiece(ref currentPosition))
                        {
                            v_CheckForMandatoryCaptureXPiece = true;
                        }
                    }

                }
            }

            return v_CheckForMandatoryCaptureXPiece;
        }
        public bool CheckMultipleJumpsForRegularPiece(ref int[] i_NextPosition)
        {
            bool v_CheckMultipleJumpsForRegularPiece = false;
            e_PieceType currentPiectType = m_MainBoard.GetBoardMatrix()[i_NextPosition[0], i_NextPosition[1]].GetPieceType();

            if (currentPiectType == e_PieceType.O)
            {
                if (CanCaptureOPiece(ref i_NextPosition))
                {
                    v_CheckMultipleJumpsForRegularPiece = !v_CheckMultipleJumpsForRegularPiece;
                }
            }
            if (currentPiectType == e_PieceType.X)
            {
                if (CanCaptureXPiece(ref i_NextPosition))
                {
                    v_CheckMultipleJumpsForRegularPiece = !v_CheckMultipleJumpsForRegularPiece;
                }
            }
            return v_CheckMultipleJumpsForRegularPiece;
        }
        public bool CheckMultipleJumpsForKingPiece(ref int[] i_NextPosition)
        {
            bool v_CheckMultipleJumpsForKingPiece = false;

            e_PieceType currentPiectType = m_MainBoard.GetBoardMatrix()[i_NextPosition[0], i_NextPosition[1]].GetPieceType();

            if (currentPiectType == e_PieceType.U)
            {
                if (CanCaptureUPiece(ref i_NextPosition))
                {
                    v_CheckMultipleJumpsForKingPiece = !v_CheckMultipleJumpsForKingPiece;
                }
            }
            if (currentPiectType == e_PieceType.K)
            {
                if (CanCaptureKPiece(ref i_NextPosition))
                {
                    v_CheckMultipleJumpsForKingPiece = true;
                }
            }
            return v_CheckMultipleJumpsForKingPiece;
        }
        public bool CanCaptureXPiece(ref int[] i_CurrentPosition)
        {
            bool v_CanCaptureXPiece = false;
            int[] optionToEatFromTopLeft = new int[2];
            int[] optionToEatFromTopRight = new int[2];

            optionToEatFromTopLeft[0] = i_CurrentPosition[0] - 2;
            optionToEatFromTopLeft[1] = i_CurrentPosition[1] - 2;
            optionToEatFromTopRight[0] = i_CurrentPosition[0] - 2;
            optionToEatFromTopRight[1] = i_CurrentPosition[1] + 2;

            if (m_MainBoard.IsValidBoardPosition(optionToEatFromTopLeft[0], optionToEatFromTopLeft[1]))
            {
                if (IsCellEmpty(optionToEatFromTopLeft[0], optionToEatFromTopLeft[1]) &&
                    IsCellContainOPiece(i_CurrentPosition[0] - 1, i_CurrentPosition[1] - 1) || IsCellContainUPiece(i_CurrentPosition[0] - 1, i_CurrentPosition[1] - 1))
                {
                    v_CanCaptureXPiece = !v_CanCaptureXPiece;
                }
            }
            if (m_MainBoard.IsValidBoardPosition(optionToEatFromTopRight[0], optionToEatFromTopRight[1]))
            {
                if (IsCellEmpty(optionToEatFromTopRight[0], optionToEatFromTopRight[1]) &&
                    IsCellContainOPiece(i_CurrentPosition[0] - 1, i_CurrentPosition[1] + 1) || IsCellContainUPiece(i_CurrentPosition[0] - 1, i_CurrentPosition[1] + 1))
                {
                    v_CanCaptureXPiece = !v_CanCaptureXPiece;
                }
            }

            return v_CanCaptureXPiece;

        }
        public bool CheckForMandatoryCaptureOPiece() 
        {
            bool v_CheckForMandatoryCaptureOPiece = false;

            for (int row = 0; row < m_MainBoard.GetBoardSize(); row++)
            {
                for (int col = 0; col < m_MainBoard.GetBoardSize(); col++)
                {
                    e_PieceType e_CurrentPieceType = m_MainBoard.GetPieceAtPosition(row, col); 
                    int[] currentPosition = new int[2];

                    currentPosition[0] = row;
                    currentPosition[1] = col;

                    if (e_CurrentPieceType == e_PieceType.O) 
                    { 
                        if (CanCaptureOPiece(ref currentPosition))
                        {
                            v_CheckForMandatoryCaptureOPiece = true;
                        }
                    }
                    if (e_CurrentPieceType == e_PieceType.U)
                    {
                        if (CanCaptureUPiece(ref currentPosition))
                        {
                            v_CheckForMandatoryCaptureOPiece = true;
                        }
                    }
                }
            }

            return v_CheckForMandatoryCaptureOPiece;
        }
        public bool CanCaptureUPiece(ref int[] i_NextPosition)
        {
            bool v_CanCaptureUPiece = false;
            int[] optionToEatFromTopLeft = new int[2];
            int[] optionToEatFromTopRight = new int[2];

            optionToEatFromTopLeft[0] = i_NextPosition[0] - 2;
            optionToEatFromTopLeft[1] = i_NextPosition[1] - 2;
            optionToEatFromTopRight[0] = i_NextPosition[0] - 2;
            optionToEatFromTopRight[1] = i_NextPosition[1] + 2;

            if (CanCaptureOPiece(ref i_NextPosition))
            {
                v_CanCaptureUPiece = true;
            }

            if (m_MainBoard.IsValidBoardPosition(optionToEatFromTopLeft[0], optionToEatFromTopLeft[1]))
            {
                if (IsCellEmpty(optionToEatFromTopLeft[0], optionToEatFromTopLeft[1]) &&
                    IsCellContainXPiece(i_NextPosition[0] - 1, i_NextPosition[1] - 1) || IsCellContainKPiece(i_NextPosition[0] - 1, i_NextPosition[1] - 1))
                {
                    v_CanCaptureUPiece = !v_CanCaptureUPiece;
                }
            }
            if (m_MainBoard.IsValidBoardPosition(optionToEatFromTopRight[0], optionToEatFromTopRight[1]))
            {
                if (IsCellEmpty(optionToEatFromTopRight[0], optionToEatFromTopRight[1]) &&
                    IsCellContainXPiece(i_NextPosition[0] - 1, i_NextPosition[1] + 1) || IsCellContainKPiece(i_NextPosition[0] - 1, i_NextPosition[1] + 1))
                {
                    v_CanCaptureUPiece = !v_CanCaptureUPiece;
                }
            }
            return v_CanCaptureUPiece;
        }
        public bool CanCaptureOPiece(ref int[] i_CurrentPosition)
        {
            bool v_CanCaptureOPiece = false;
            int[] optionToEatFromButtomLeft = new int[2];
            int[] optionToEatFromButtomRight = new int[2];

            optionToEatFromButtomLeft[0] = i_CurrentPosition[0] + 2;
            optionToEatFromButtomLeft[1] = i_CurrentPosition[1] - 2;
            optionToEatFromButtomRight[0] = i_CurrentPosition[0] + 2;
            optionToEatFromButtomRight[1] = i_CurrentPosition[1] + 2;

            if (m_MainBoard.IsValidBoardPosition(optionToEatFromButtomLeft[0], optionToEatFromButtomLeft[1]))
            {
                if (IsCellEmpty(optionToEatFromButtomLeft[0], optionToEatFromButtomLeft[1]) &&
                    (IsCellContainXPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] - 1) || IsCellContainKPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] - 1)))
                {
                    v_CanCaptureOPiece = true;
                }
            }
            if (m_MainBoard.IsValidBoardPosition(optionToEatFromButtomRight[0], optionToEatFromButtomRight[1]))
            {
                if (IsCellEmpty(optionToEatFromButtomRight[0], optionToEatFromButtomRight[1]) &&
                    IsCellContainXPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] + 1) || IsCellContainKPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] + 1))
                {
                    v_CanCaptureOPiece = true;
                }
            }

            return v_CanCaptureOPiece;
        }
        public bool CanCaptureKPiece(ref int[] i_CurrentPosition)
        {
            bool v_CanCaptureKPiece = false;
            int[] optionToEatFromButtomLeft = new int[2];
            int[] optionToEatFromButtomRight = new int[2];

            optionToEatFromButtomLeft[0] = i_CurrentPosition[0] + 2;
            optionToEatFromButtomLeft[1] = i_CurrentPosition[1] - 2;
            optionToEatFromButtomRight[0] = i_CurrentPosition[0] + 2;
            optionToEatFromButtomRight[1] = i_CurrentPosition[1] + 2;

            if (CanCaptureXPiece(ref i_CurrentPosition))
            {
                v_CanCaptureKPiece = true;
            }

            if (m_MainBoard.IsValidBoardPosition(optionToEatFromButtomLeft[0], optionToEatFromButtomLeft[1]))
            {
                if (IsCellEmpty(optionToEatFromButtomLeft[0], optionToEatFromButtomLeft[1]) &&
                    (IsCellContainOPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] - 1) || IsCellContainUPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] - 1)))
                {
                    v_CanCaptureKPiece = true;
                }
            }
            if (m_MainBoard.IsValidBoardPosition(optionToEatFromButtomRight[0], optionToEatFromButtomRight[1]))
            {
                if (IsCellEmpty(optionToEatFromButtomRight[0], optionToEatFromButtomRight[1]) &&
                    IsCellContainOPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] + 1) || IsCellContainUPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] + 1))
                {
                    v_CanCaptureKPiece = true;
                }
            }

            return v_CanCaptureKPiece;
        }
        public bool IsValidCaptureForOPiece(ref int[] i_CurrentPosition, ref int[] i_NextPosition)
        {
            bool v_IsValidCaptureForOPiece = false;

            if (i_CurrentPosition[0] == i_NextPosition[0] - 2 && i_CurrentPosition[1] == i_NextPosition[1] - 2)
            {
                if (IsCellEmpty(i_NextPosition[0], i_NextPosition[1]) &&
                    (IsCellContainXPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] + 1) || IsCellContainKPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] + 1)))
                {
                    v_IsValidCaptureForOPiece = true;
                    m_MainBoard.GetBoardMatrix()[i_CurrentPosition[0] + 1, i_CurrentPosition[1] + 1] = new Piece(e_PieceType.E);
                }
            }

            if (i_CurrentPosition[0] == i_NextPosition[0] - 2 && i_CurrentPosition[1] == i_NextPosition[1] + 2)
            {
                if (IsCellEmpty(i_NextPosition[0], i_NextPosition[1]) &&
                    (IsCellContainXPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] - 1) || IsCellContainKPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] - 1)))
                {
                    v_IsValidCaptureForOPiece = true;
                    m_MainBoard.GetBoardMatrix()[i_CurrentPosition[0] + 1, i_CurrentPosition[1] - 1] = new Piece(e_PieceType.E);
                }
            }

            return v_IsValidCaptureForOPiece;
        }
        public bool IsValidCaptureForXPiece(ref int[] i_CurrentPosition, ref int[] i_NextPosition)
        {
            bool v_IsValidCaptureForXPiece = false;

            if (i_CurrentPosition[0] == i_NextPosition[0] + 2 && i_CurrentPosition[1] == i_NextPosition[1] - 2)
            {
                if (IsCellEmpty(i_NextPosition[0], i_NextPosition[1]) &&
                    (IsCellContainOPiece(i_CurrentPosition[0] - 1, i_CurrentPosition[1] + 1) || IsCellContainUPiece(i_CurrentPosition[0] - 1, i_CurrentPosition[1] + 1)))
                {
                    v_IsValidCaptureForXPiece = true;
                    m_MainBoard.GetBoardMatrix()[i_CurrentPosition[0] - 1, i_CurrentPosition[1] + 1] = new Piece(e_PieceType.E);
                }
            }

            if (i_CurrentPosition[0] == i_NextPosition[0] + 2 && i_CurrentPosition[1] == i_NextPosition[1] + 2)
            {
                if (IsCellEmpty(i_NextPosition[0], i_NextPosition[1]) &&
                    (IsCellContainOPiece(i_CurrentPosition[0] - 1, i_CurrentPosition[1] - 1) || IsCellContainUPiece(i_CurrentPosition[0] - 1, i_CurrentPosition[1] - 1)))
                {
                    v_IsValidCaptureForXPiece = true;
                    m_MainBoard.GetBoardMatrix()[i_CurrentPosition[0] - 1, i_CurrentPosition[1] - 1] = new Piece(e_PieceType.E);
                }
            }
            return v_IsValidCaptureForXPiece;
        }
        public bool IsValidCaptureForKPiece(ref int[] i_CurrentPosition, ref int[] i_NextPosition)
        {
            bool v_IsValidCaptureForKPiece = false;

            if (IsValidCaptureForXPiece(ref i_CurrentPosition, ref i_NextPosition))
            {
                v_IsValidCaptureForKPiece = true;
            }

            if (i_CurrentPosition[0] == i_NextPosition[0] - 2 && i_CurrentPosition[1] == i_NextPosition[1] - 2)
            {
                if (IsCellEmpty(i_NextPosition[0], i_NextPosition[1]) &&
                    (IsCellContainOPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] + 1) || IsCellContainUPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] + 1)))
                {
                    v_IsValidCaptureForKPiece = true;
                    m_MainBoard.GetBoardMatrix()[i_CurrentPosition[0] + 1, i_CurrentPosition[1] + 1] = new Piece(e_PieceType.E);
                }
            }

            if (i_CurrentPosition[0] == i_NextPosition[0] - 2 && i_CurrentPosition[1] == i_NextPosition[1] + 2)
            {
                if (IsCellEmpty(i_NextPosition[0], i_NextPosition[1]) &&
                    (IsCellContainOPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] - 1) || IsCellContainUPiece(i_CurrentPosition[0] + 1, i_CurrentPosition[1] - 1)))
                {
                    v_IsValidCaptureForKPiece = true;
                    m_MainBoard.GetBoardMatrix()[i_CurrentPosition[0] + 1, i_CurrentPosition[1] - 1] = new Piece(e_PieceType.E);
                }
            }

            return v_IsValidCaptureForKPiece;
        }
        public bool IsValidCaptureForUPiece(ref int[] i_CurrentPosition, ref int[] i_NextPosition)
        {
            bool v_IsValidCaptureForUPiece = false;

            if (IsValidCaptureForOPiece(ref i_CurrentPosition, ref i_NextPosition))
            {
                v_IsValidCaptureForUPiece = !v_IsValidCaptureForUPiece;
            }

            if (i_CurrentPosition[0] == i_NextPosition[0] + 2 && i_CurrentPosition[1] == i_NextPosition[1] - 2) 
            {
                if (IsCellEmpty(i_NextPosition[0], i_NextPosition[1]) &&
                    (IsCellContainXPiece(i_CurrentPosition[0] - 1, i_CurrentPosition[1] + 1) || IsCellContainKPiece(i_CurrentPosition[0] - 1, i_CurrentPosition[1] + 1)))
                {
                    v_IsValidCaptureForUPiece = true;
                    m_MainBoard.GetBoardMatrix()[i_CurrentPosition[0] - 1, i_CurrentPosition[1] + 1] = new Piece(e_PieceType.E);
                }
            }

            if (i_CurrentPosition[0] == i_NextPosition[0] + 2 && i_CurrentPosition[1] == i_NextPosition[1] + 2)
            {
                if (IsCellEmpty(i_NextPosition[0], i_NextPosition[1]) &&
                    (IsCellContainXPiece(i_CurrentPosition[0] - 1, i_CurrentPosition[1] - 1) || IsCellContainKPiece(i_CurrentPosition[0] - 1, i_CurrentPosition[1] - 1)))
                {
                    v_IsValidCaptureForUPiece = true;
                    m_MainBoard.GetBoardMatrix()[i_CurrentPosition[0] - 1, i_CurrentPosition[1] - 1] = new Piece(e_PieceType.E);
                }
            }

            return v_IsValidCaptureForUPiece;
        }
        public bool IsCellEmpty(int i_Row, int i_Col)
        {
            bool v_IsCellEmpty = false;

            if (m_MainBoard.IsValidBoardPosition(i_Row, i_Col))
            {
                if (m_MainBoard.GetBoardMatrix()[i_Row, i_Col].GetPieceType() == e_PieceType.E)
                {
                    v_IsCellEmpty = !v_IsCellEmpty;
                }
            }

            return v_IsCellEmpty;
        }
        public bool IsCellContainXPiece(int i_Row, int i_Col)
        {
            bool v_IsCellContainXPiece = false;

            if (m_MainBoard.IsValidBoardPosition(i_Row, i_Col))
            {

                if (m_MainBoard.GetBoardMatrix()[i_Row, i_Col].GetPieceType() == e_PieceType.X)
                {
                    v_IsCellContainXPiece = !v_IsCellContainXPiece;
                }
            }

            return v_IsCellContainXPiece;
        }
        public bool IsCellContainOPiece(int i_Row, int i_Col)
        {
            bool v_IsCellContainOPiece = false;

            if (m_MainBoard.IsValidBoardPosition(i_Row, i_Col))
            {
                if (m_MainBoard.GetBoardMatrix()[i_Row, i_Col].GetPieceType() == e_PieceType.O)
                {
                    v_IsCellContainOPiece = !v_IsCellContainOPiece;
                }
            }

            return v_IsCellContainOPiece;
        }
        public bool IsCellContainKPiece(int i_Row, int i_Col)
        {
            bool v_IsCellContainKPiece = false;

            if (m_MainBoard.IsValidBoardPosition(i_Row, i_Col))
            {
                if (m_MainBoard.GetBoardMatrix()[i_Row, i_Col].GetPieceType() == e_PieceType.K)
                {
                    v_IsCellContainKPiece = !v_IsCellContainKPiece;
                }
            }

            return v_IsCellContainKPiece;
        }
        public bool IsCellContainUPiece(int i_Row, int i_Col)
        {
            bool v_IsCellContainKPiece = false;

            if (m_MainBoard.IsValidBoardPosition(i_Row, i_Col))
            {
                if (m_MainBoard.GetBoardMatrix()[i_Row, i_Col].GetPieceType() == e_PieceType.U)
                {
                    v_IsCellContainKPiece = !v_IsCellContainKPiece;
                }
            }

            return v_IsCellContainKPiece;
        }
        public bool GameOver(string i_CurrentPlayer)
        {
            bool v_GameOver = false;
            int numXPiece = 0;
            int numOPiece = 0;

            for (int row = 0; row < m_MainBoard.GetBoardSize(); row++)
            {
                for (int col = 0; col < m_MainBoard.GetBoardSize(); col++)
                {

                    e_PieceType currentPiece = m_MainBoard.GetPieceAtPosition(row, col);

                    if (currentPiece == e_PieceType.X)
                    {
                        numXPiece++;
                    }
                    if (currentPiece == e_PieceType.K)
                    {
                        numXPiece += 4;
                    }
                    if (currentPiece == e_PieceType.O)
                    {
                        numOPiece++;
                    }
                    if (currentPiece == e_PieceType.U)
                    {
                        numOPiece += 4;
                    }
                }
            }
            
            return v_GameOver;
        }
        public void ComputerGameManager(ref string o_ComputerMoveString)
        {
            if (m_SecondPlayer.GetPlayerGameStatus() == e_PlayerGameStatus.ExtraCapture && !(m_IsPlayerOneMove))
            {
                int[] positionArray = m_SecondPlayer.GetPositionForExtraEating();
                ExtraEatingForComputer(ref positionArray, m_MainBoard.GetBoardMatrix()[positionArray[0], positionArray[1]].GetPieceType(), ref o_ComputerMoveString);
            }
            else
            {
                InitComputerGame(ref  o_ComputerMoveString);
            }

        }
        public void InitComputerGame(ref string o_ComputerMoveString)
        {
            List<(int, int, int, int)> normalMoves = new List<(int, int, int, int)>();
            List<(int, int, int, int)> eatingMoves = new List<(int, int, int, int)>();

            List<(int, int, int, int)> ExtraEatingMoves = new List<(int, int, int, int)>();

            int IsEatingMove = 0; 

            for (int rows = 0; rows < m_MainBoard.GetBoardSize(); rows++)
            {
                for(int cols = 0; cols < m_MainBoard.GetBoardSize(); cols++)
                {
                    if(m_MainBoard.GetBoardMatrix()[rows, cols].GetPieceType() == e_PieceType.X)
                    {
                        CheckIfNextStepValidForComputer(rows, cols, ref normalMoves, e_PieceType.X);
                        CheckIfEatingValidForComputer(rows, cols, ref eatingMoves, e_PieceType.X);
                    }

                    if (m_MainBoard.GetBoardMatrix()[rows, cols].GetPieceType() == e_PieceType.K)
                    {
                        CheckIfNextStepValidForComputer(rows, cols, ref normalMoves, e_PieceType.K);
                        CheckIfEatingValidForComputer(rows, cols, ref eatingMoves, e_PieceType.K);
                    }
                }
            }

            (int, int, int, int) chosenMove = ComputerChosenMove(ref normalMoves, ref eatingMoves , ref IsEatingMove);

            if(chosenMove != (-1, -1, -1, -1))
            {
                int[] currentPosition = new int[2];
                int[] nextPosition = new int[2];

                currentPosition[0] = chosenMove.Item1;
                currentPosition[1] = chosenMove.Item2;

                nextPosition[0] = chosenMove.Item3;
                nextPosition[1] = chosenMove.Item4;

                if (IsEatingMove == 1)
                {
                    int midRow = (chosenMove.Item1 + chosenMove.Item3) / 2;
                    int midCol = (chosenMove.Item2 + chosenMove.Item4) / 2;
                    m_MainBoard.GetBoardMatrix()[midRow, midCol] = new Piece(e_PieceType.E);  
                    m_MainBoard.UpdateBoard(ref currentPosition, ref nextPosition);
                    ConvertComputerPlayToString(ref currentPosition, ref nextPosition, ref o_ComputerMoveString);

                    CheckIfEatingValidForComputer(nextPosition[0], nextPosition[1],
                                                  ref ExtraEatingMoves, m_MainBoard.GetBoardMatrix()[nextPosition[0], nextPosition[1]].GetPieceType());

                    if (ExtraEatingMoves.Count>0)
                    {
                        m_IsPlayerOneMove = false;
                        m_SecondPlayer.SetPlayerGameStatus(e_PlayerGameStatus.ExtraCapture);
                        m_FirstPlayer.SetPlayerGameStatus(e_PlayerGameStatus.Waiting);
                        m_SecondPlayer.SetPositionForExtraEating(ref nextPosition);
                    }
                    else
                    {
                        m_SecondPlayer.SetPlayerGameStatus(e_PlayerGameStatus.Waiting);
                        m_FirstPlayer.SetPlayerGameStatus(e_PlayerGameStatus.Active);
                        m_IsPlayerOneMove = true;
                    }
                   
                }
              
                else if(IsEatingMove == 0)
                {
                    m_SecondPlayer.SetPlayerGameStatus(e_PlayerGameStatus.Waiting);
                    m_FirstPlayer.SetPlayerGameStatus(e_PlayerGameStatus.Active);
                    m_MainBoard.UpdateBoard(ref currentPosition, ref nextPosition);
                    ConvertComputerPlayToString(ref currentPosition, ref nextPosition, ref o_ComputerMoveString);
                    m_IsPlayerOneMove = true; 

                }
            }
            else
            {
                m_SecondPlayer.SetPlayerGameStatus(e_PlayerGameStatus.NoMoreMoves);
                m_FirstPlayer.SetPlayerGameStatus(e_PlayerGameStatus.Active);
                m_IsPlayerOneMove = true;
            }

        }

        public void CheckIfNextStepValidForComputer(int i_Rows, int i_Cols,ref List<(int, int, int, int)> i_NormalMovesList,e_PieceType i_EnumPieceType)
        { 

           if(m_MainBoard.IsValidPosition(i_Rows - 1, i_Cols - 1))
           {
               if (m_MainBoard.GetBoardMatrix()[i_Rows - 1, i_Cols - 1].GetPieceType() == e_PieceType.E)
               {
                   i_NormalMovesList.Add((i_Rows, i_Cols, i_Rows - 1, i_Cols - 1));
               }
           }

           if(m_MainBoard.IsValidPosition(i_Rows - 1, i_Cols + 1))
           {
               if (m_MainBoard.GetBoardMatrix()[i_Rows - 1, i_Cols + 1].GetPieceType() == e_PieceType.E)
               {
                   i_NormalMovesList.Add((i_Rows, i_Cols, i_Rows - 1, i_Cols + 1));
               }
           }
           
           
           if(i_EnumPieceType == e_PieceType.K)
           {
               if(m_MainBoard.IsValidPosition(i_Rows + 1, i_Cols - 1))
               {
                   if (m_MainBoard.GetBoardMatrix()[i_Rows + 1, i_Cols - 1].GetPieceType() == e_PieceType.E)
                   {
                       i_NormalMovesList.Add((i_Rows, i_Cols, i_Rows + 1, i_Cols - 1));
                   }
               }

               if(m_MainBoard.IsValidPosition(i_Rows + 1, i_Cols + 1))
               {
                   if (m_MainBoard.GetBoardMatrix()[i_Rows + 1, i_Cols + 1].GetPieceType() == e_PieceType.E)
                   {
                       i_NormalMovesList.Add((i_Rows, i_Cols, i_Rows + 1, i_Cols + 1));
                   }
               }
           }
        }

        public void CheckIfEatingValidForComputer(int i_Rows, int i_Cols, ref List<(int, int, int, int)> i_EatingMovesList, e_PieceType i_EnumPieceType)
        {

            if(m_MainBoard.IsValidPosition(i_Rows - 2, i_Cols - 2))
            {
                if (m_MainBoard.GetBoardMatrix()[i_Rows - 2, i_Cols - 2].GetPieceType() == e_PieceType.E)
                {
                    if (m_MainBoard.GetBoardMatrix()[i_Rows - 1, i_Cols - 1].GetPieceType() == e_PieceType.O
                        || m_MainBoard.GetBoardMatrix()[i_Rows - 1, i_Cols - 1].GetPieceType() == e_PieceType.U)
                    {
                        i_EatingMovesList.Add((i_Rows, i_Cols, i_Rows - 2, i_Cols - 2));
                    }

                }
            }

            if(m_MainBoard.IsValidPosition(i_Rows - 2, i_Cols + 2))
            {
                if (m_MainBoard.GetBoardMatrix()[i_Rows - 2, i_Cols + 2].GetPieceType() == e_PieceType.E)
                {
                    if (m_MainBoard.GetBoardMatrix()[i_Rows - 1, i_Cols + 1].GetPieceType() == e_PieceType.O
                        || m_MainBoard.GetBoardMatrix()[i_Rows - 1, i_Cols + 1].GetPieceType() == e_PieceType.U)
                    {
                        i_EatingMovesList.Add((i_Rows, i_Cols, i_Rows - 2, i_Cols + 2));
                    }
                }
            }
           
            if(i_EnumPieceType == e_PieceType.K)
            {
                if(m_MainBoard.IsValidPosition(i_Rows + 2, i_Cols - 2))
                {
                    if (m_MainBoard.GetBoardMatrix()[i_Rows + 2, i_Cols - 2].GetPieceType() == e_PieceType.E)
                    {
                        if (m_MainBoard.GetBoardMatrix()[i_Rows + 1, i_Cols - 1].GetPieceType() == e_PieceType.O
                            || m_MainBoard.GetBoardMatrix()[i_Rows + 1, i_Cols - 1].GetPieceType() == e_PieceType.U)
                        {
                            i_EatingMovesList.Add((i_Rows, i_Cols, i_Rows + 2, i_Cols - 2));
                        }
                    }
                }

                if(m_MainBoard.IsValidPosition(i_Rows + 2, i_Cols + 2))
                {
                    if (m_MainBoard.GetBoardMatrix()[i_Rows + 2, i_Cols + 2].GetPieceType() == e_PieceType.E)
                    {
                        if (m_MainBoard.GetBoardMatrix()[i_Rows + 1, i_Cols + 1].GetPieceType() == e_PieceType.O
                            || m_MainBoard.GetBoardMatrix()[i_Rows + 1, i_Cols + 1].GetPieceType() == e_PieceType.U)
                        {
                            i_EatingMovesList.Add((i_Rows, i_Cols, i_Rows + 2, i_Cols + 2));
                        }
                    }
                }
            }
        }

        public (int, int, int, int) ComputerChosenMove(ref List<(int, int, int, int)> i_NormalMovesList,
                                                       ref List<(int, int, int, int)> i_EatingMovesList, ref int i_IsEatingMove)
        {
            Random random = new Random();

            if (i_EatingMovesList.Count > 0)
            {
                i_IsEatingMove = 1;
                return i_EatingMovesList[random.Next(i_EatingMovesList.Count)];
            }
            else if (i_NormalMovesList.Count > 0)
            {
                i_IsEatingMove = 0;
                return i_NormalMovesList[random.Next(i_NormalMovesList.Count)];
            }
            return (-1, -1, -1, -1);
        }
        public void ExtraEatingForComputer(ref int[]  i_PositionForEating, e_PieceType i_PieceType, ref string o_ComputerMoveString)
        {
            List<(int, int, int, int)> ExtraEatingMove = new List<(int, int, int, int)>();

            CheckIfEatingValidForComputer(i_PositionForEating[0], i_PositionForEating[1],
                                          ref ExtraEatingMove, m_MainBoard.GetBoardMatrix()[i_PositionForEating[0], i_PositionForEating[1]].GetPieceType());
            
            int [] nextPosition = new int [2];
            nextPosition[0] = ExtraEatingMove[0].Item3;
            nextPosition[1] = ExtraEatingMove[0].Item4;

            int midRow = (i_PositionForEating[0] + nextPosition[0]) / 2;
            int midCol = (i_PositionForEating[1] + nextPosition[1]) / 2;

            m_MainBoard.GetBoardMatrix()[midRow, midCol] = new Piece(e_PieceType.E);  
            m_MainBoard.UpdateBoard(ref i_PositionForEating, ref nextPosition); 
            ConvertComputerPlayToString(ref i_PositionForEating, ref nextPosition, ref o_ComputerMoveString);

            ExtraEatingMove.Clear();
 
            CheckIfEatingValidForComputer(nextPosition[0], nextPosition[1],
                ref ExtraEatingMove, m_MainBoard.GetBoardMatrix()[nextPosition[0], nextPosition[1]].GetPieceType());

            if (ExtraEatingMove.Count > 0)
            {
                m_SecondPlayer.SetPlayerGameStatus(e_PlayerGameStatus.ExtraCapture);
                m_FirstPlayer.SetPlayerGameStatus(e_PlayerGameStatus.Waiting);
                m_SecondPlayer.SetPositionForExtraEating(ref nextPosition);
                m_IsPlayerOneMove = false;
            }
            else
            {
                m_SecondPlayer.SetPlayerGameStatus(e_PlayerGameStatus.Waiting);
                m_FirstPlayer.SetPlayerGameStatus(e_PlayerGameStatus.Active);
                m_IsPlayerOneMove = true;
            }

        }
        public void ConvertComputerPlayToString(ref int[] i_CurrentPosition , ref int[] i_NextPosition, ref string o_ComputerMoveString)
        {
            char currentRow = (char)('A' + i_CurrentPosition[0]); 
            char nextRow = (char)('A' + i_NextPosition[0]);               
            char currentColumn = (char)('a' + i_CurrentPosition[1]); 
            char nextColumn = (char)('a' + i_NextPosition[1]);       

            o_ComputerMoveString = $"{currentRow}{currentColumn}>{nextRow}{nextColumn}";
        }
        public bool GameOver()
        {     
            bool v_GameOver = false;
            int numXPiece = 0;
            int numOPiece = 0;

            CalculateAmountOfPieces(ref numXPiece, ref numOPiece);

            if (numOPiece == 0)
            {
                m_FirstPlayer.SetPlayerGameStatus(e_PlayerGameStatus.NoMoreMoves);
                v_GameOver = true;
                m_SecondPlayer.SetScoreForPlayer(numXPiece);
            }
            else if (numXPiece == 0)
            {
                m_SecondPlayer.SetPlayerGameStatus(e_PlayerGameStatus.NoMoreMoves);
                v_GameOver = true;
                m_FirstPlayer.SetScoreForPlayer(numOPiece);
            }

            return v_GameOver;
        }
        public bool IsGameOverDueToNoMoves()
        {
            bool v_IsGameOverDueToNoMoves = false;
            int sumOfMovesXPiece = 0;
            int sumOfMovesOPiece = 0;
            int numXPiece = 0;
            int numOPiece = 0;

            for (int row = 0; row < m_MainBoard.GetBoardSize(); row++)
            {
                for (int col = 0; col < m_MainBoard.GetBoardSize(); col++)
                {
                    e_PieceType currentPiece = m_MainBoard.GetPieceAtPosition(row, col);
                    int[] currentPosition = new int[2];
                    currentPosition[0] = row;
                    currentPosition[1] = col;

                    if (currentPiece == e_PieceType.O)
                    {
                        if (CanRegularMoveOPiece(ref currentPosition) || CanCaptureOPiece(ref currentPosition))
                        {
                            sumOfMovesOPiece++;
                        }
                    }
                    if (currentPiece == e_PieceType.U)
                    {
                        if (CanRegularMoveKingPiece(ref currentPosition) || CanCaptureUPiece(ref currentPosition))
                        {
                            sumOfMovesOPiece++;
                        }
                    }

                    if (currentPiece == e_PieceType.X)
                    {
                        if (CanRegularMoveXPiece(ref currentPosition) || CanCaptureXPiece(ref currentPosition))
                        {
                            sumOfMovesXPiece++;
                        }
                    }
                    if (currentPiece == e_PieceType.K)
                    {
                        if (CanRegularMoveKingPiece(ref currentPosition) || CanCaptureKPiece(ref currentPosition))
                        {
                            sumOfMovesXPiece++;
                        }
                    }

                }
            }

            if (sumOfMovesOPiece == 0 && sumOfMovesXPiece == 0) 
            {
                CalculateAmountOfPieces(ref numXPiece, ref numOPiece);
                v_IsGameOverDueToNoMoves = true;
                m_FirstPlayer.SetPlayerGameStatus(e_PlayerGameStatus.NoMoreMoves);
                m_SecondPlayer.SetPlayerGameStatus(e_PlayerGameStatus.NoMoreMoves);
            }
            else if (sumOfMovesOPiece == 0)
            {
                m_FirstPlayer.SetPlayerGameStatus(e_PlayerGameStatus.NoMoreMoves);
                v_IsGameOverDueToNoMoves = true;
                CalculateAmountOfPieces(ref numXPiece, ref numOPiece);
                m_SecondPlayer.SetScoreForPlayer(Math.Abs(numXPiece - numOPiece));

            }
            else if (sumOfMovesXPiece == 0)
            {
                m_SecondPlayer.SetPlayerGameStatus(e_PlayerGameStatus.NoMoreMoves);
                v_IsGameOverDueToNoMoves = true;
                CalculateAmountOfPieces(ref numXPiece, ref numOPiece);
                m_FirstPlayer.SetScoreForPlayer(Math.Abs(numXPiece - numOPiece));
            }

            return v_IsGameOverDueToNoMoves;
        }
        public bool CanRegularMoveOPiece(ref int[] i_CurrentPosition)
        {
            bool v_CanRegularMoveOPiece = false;

            int[] optionToMoveButtomLeft = new int[2];
            int[] optionToMoveButtomRight = new int[2];

            optionToMoveButtomLeft[0] = i_CurrentPosition[0] + 1;
            optionToMoveButtomLeft[1] = i_CurrentPosition[1] - 1;
            optionToMoveButtomRight[0] = i_CurrentPosition[0] + 1;
            optionToMoveButtomRight[1] = i_CurrentPosition[1] + 1;

            if (m_MainBoard.IsValidBoardPosition(optionToMoveButtomLeft[0], optionToMoveButtomLeft[1]))
            {
                if (ValidateRegularMove(ref i_CurrentPosition, ref optionToMoveButtomLeft))
                {
                    v_CanRegularMoveOPiece = true;
                }
            }

            if (m_MainBoard.IsValidBoardPosition(optionToMoveButtomRight[0], optionToMoveButtomRight[1]))
            {
                if (ValidateRegularMove(ref i_CurrentPosition, ref optionToMoveButtomRight))
                {
                    v_CanRegularMoveOPiece = true;
                }
            }

            return v_CanRegularMoveOPiece;
        }
        public bool CanRegularMoveXPiece(ref int[] i_CurrentPosition)
        {
            bool v_CanRegularMoveXPiece = false;

            int[] optionToMoveTopLeft = new int[2];
            int[] optionToMoveTopRight = new int[2];

            optionToMoveTopLeft[0] = i_CurrentPosition[0] - 1;
            optionToMoveTopLeft[1] = i_CurrentPosition[1] - 1;
            optionToMoveTopRight[0] = i_CurrentPosition[0] - 1;
            optionToMoveTopRight[1] = i_CurrentPosition[1] + 1;

            if (m_MainBoard.IsValidBoardPosition(optionToMoveTopLeft[0], optionToMoveTopLeft[1]))
            {
                if (ValidateRegularMove(ref i_CurrentPosition, ref optionToMoveTopLeft))
                {
                    v_CanRegularMoveXPiece = true;
                }
            }

            if (m_MainBoard.IsValidBoardPosition(optionToMoveTopRight[0], optionToMoveTopRight[1]))
            {
                if (ValidateRegularMove(ref i_CurrentPosition, ref optionToMoveTopRight))
                {
                    v_CanRegularMoveXPiece = true;
                }
            }

            return v_CanRegularMoveXPiece;

        }
        public bool CanRegularMoveKingPiece(ref int[] i_CurrentPosition)
        {
            bool v_CanRegularMoveKingPiece = false;
            int[] optionToMoveTopLeft = new int[2];
            int[] optionToMoveTopRight = new int[2];
            int[] optionToMoveButtomLeft = new int[2];
            int[] optionToMoveButtomRight = new int[2];

            optionToMoveTopLeft[0] = i_CurrentPosition[0] - 1;
            optionToMoveTopLeft[1] = i_CurrentPosition[1] - 1;
            optionToMoveTopRight[0] = i_CurrentPosition[0] - 1;
            optionToMoveTopRight[1] = i_CurrentPosition[1] + 1;


            optionToMoveButtomLeft[0] = i_CurrentPosition[0] + 1;
            optionToMoveButtomLeft[1] = i_CurrentPosition[1] - 1;
            optionToMoveButtomRight[0] = i_CurrentPosition[0] + 1;
            optionToMoveButtomRight[1] = i_CurrentPosition[1] + 1;

            if (m_MainBoard.IsValidBoardPosition(optionToMoveTopLeft[0], optionToMoveTopLeft[1]))
            {
                if (IsValidKingMove(ref i_CurrentPosition, ref optionToMoveTopLeft))
                {
                    v_CanRegularMoveKingPiece = true;
                }
            }

            if (m_MainBoard.IsValidBoardPosition(optionToMoveTopRight[0], optionToMoveTopRight[1]))
            {
                if (IsValidKingMove(ref i_CurrentPosition, ref optionToMoveTopRight))
                {
                    v_CanRegularMoveKingPiece = true;
                }
            }

            if (m_MainBoard.IsValidBoardPosition(optionToMoveButtomLeft[0], optionToMoveButtomLeft[1]))
            {
                if (IsValidKingMove(ref i_CurrentPosition, ref optionToMoveButtomLeft))
                {
                    v_CanRegularMoveKingPiece = true;
                }
            }

            if (m_MainBoard.IsValidBoardPosition(optionToMoveButtomRight[0], optionToMoveButtomRight[1]))
            {
                if (IsValidKingMove(ref i_CurrentPosition, ref optionToMoveButtomRight))
                {
                    v_CanRegularMoveKingPiece = true;
                }
            }

            return v_CanRegularMoveKingPiece;
        }

        public void CalculateAmountOfPieces(ref int i_NumXPiece, ref int i_NumOPiece)
        {
            for (int row = 0; row < m_MainBoard.GetBoardSize(); row++)
            {
                for (int col = 0; col < m_MainBoard.GetBoardSize(); col++)
                {
                    e_PieceType currentPiece = m_MainBoard.GetPieceAtPosition(row, col);

                    if (currentPiece == e_PieceType.X)
                    {
                        i_NumXPiece++;
                    }
                    if (currentPiece == e_PieceType.K)
                    {
                        i_NumXPiece += 4;
                    }
                    if (currentPiece == e_PieceType.O)
                    {
                        i_NumOPiece++;
                    }
                    if (currentPiece == e_PieceType.U)
                    {
                        i_NumOPiece += 4;
                    }
                }
            }
        }
        public void CalculateScore()
        {
            int numOPiece = 0;
            int numXPiece = 0;
            Player  currentPlayer = GetOtherPlayer();

            CalculateAmountOfPieces(ref numXPiece,ref numOPiece);

            currentPlayer.SetScoreForPlayer(Math.Abs(numOPiece- numXPiece));
        }
    }
}