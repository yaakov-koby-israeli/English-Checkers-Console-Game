using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ex02
{
    internal class Board
    {
        private Piece[,] m_Board;
        private readonly int r_BoardSize; 

        public Board(int i_BoardSize)
        {
            this.r_BoardSize = i_BoardSize;
            this.m_Board = new Piece[i_BoardSize, i_BoardSize];
            InitializeBoard();
        }

        public Piece[,] GetBoardMatrix()
        {
            return m_Board;
        }

        public int GetBoardSize()
        {
            return this.r_BoardSize;
        }

        public void InitializeBoard()
        {
            int emptyRows = r_BoardSize / 2;

            for (int row = 0; row < r_BoardSize; row++)
            {
                for (int col = 0; col < r_BoardSize; col++)
                {
                    Piece currentPiece = new Piece(e_PieceType.E);
                    m_Board[row, col] = currentPiece;
                }
            }

            for (int row = 0; row < emptyRows - 1; row++)
            {
                for (int col = 0; col < r_BoardSize; col++)
                {
                    if (row % 2 == 0 && col % 2 == 1)
                    {
                        Piece currentPiece = new Piece(e_PieceType.O);
                        m_Board[row, col] = currentPiece;
                    }

                    if (row % 2 == 1 && col % 2 == 0)
                    {
                        Piece currentPiece = new Piece(e_PieceType.O);
                        m_Board[row, col] = currentPiece;
                    }
                }
            }

            for (int row = emptyRows + 1; row < r_BoardSize; row++)
            {
                for (int col = 0; col < r_BoardSize; col++)
                {
                    if (row % 2 == 0 && col % 2 == 1)
                    {
                        Piece currentPiece = new Piece(e_PieceType.X);
                        m_Board[row, col] = currentPiece;
                    }

                    if (row % 2 == 1 && col % 2 == 0)
                    {
                        Piece currentPiece = new Piece(e_PieceType.X);
                        m_Board[row, col] = currentPiece;
                    }
                }
            }
        }
     
        public bool IsValidBoardPosition(int i_Row, int i_Column)
        {
            bool v_IsValidBoardPosition = true;

            if (i_Row < 0 || i_Row >= r_BoardSize || i_Column < 0 || i_Column >= r_BoardSize)
            {
                v_IsValidBoardPosition = false;
            }
            return v_IsValidBoardPosition;
        }

        public bool IsCellContainOPiece(int i_Row, int i_Col)
        {
            bool v_IsCellContainOPiece = false;

            if (m_Board[i_Row, i_Col].GetPieceType() == e_PieceType.O)
            {
                v_IsCellContainOPiece = !v_IsCellContainOPiece;
            }

            return v_IsCellContainOPiece;
        }

        public bool IsCellContainXPiece(int i_Row, int i_Col)
        {
            bool v_IsCellContainXPiece = false;

            if (m_Board[i_Row, i_Col].GetPieceType() == e_PieceType.X)
            {
                v_IsCellContainXPiece = true;
            }

            return v_IsCellContainXPiece;
        }

        public bool IsCellContainKPiece(int i_Row, int i_Col)
        {
            const bool v_IsCellContainKPiece = true;

            if (m_Board[i_Row, i_Col].GetPieceType() == e_PieceType.K)
            {
                return v_IsCellContainKPiece;
            }

            return !v_IsCellContainKPiece;
        }

        public bool IsCellContainUPiece(int i_Row, int i_Col)
        {
            const bool v_IsCellContainUPiece = true;

            if (m_Board[i_Row, i_Col].GetPieceType() == e_PieceType.U)
            {
                return v_IsCellContainUPiece;
            }

            return !v_IsCellContainUPiece;
        }

        public bool IsValidPosition(int i_Row, int i_Col)
        {
            bool v_IsValidPosition = true;

            if (i_Row < 0 || i_Row >= r_BoardSize || i_Col < 0 || i_Col >= r_BoardSize)
            {
                v_IsValidPosition = false;
            }
            return v_IsValidPosition;
        }
  
        public e_PieceType GetPieceAtPosition(int i_Row, int i_Col)
        {
            return m_Board[i_Row, i_Col].GetPieceType();
        }
    
        public bool MakeKing(int i_Rows, int i_Cols)
        {
            bool v_IsKing = false;

            if (i_Rows == r_BoardSize - 1)
            {
                m_Board[i_Rows, i_Cols] = new Piece(e_PieceType.U);
                v_IsKing = true;
            }

            if (i_Rows == 0)
            {
                m_Board[i_Rows, i_Cols] = new Piece(e_PieceType.K);
                v_IsKing = true;
            }

            return v_IsKing;
        }

        public void UpdateBoard(ref int[] i_CurrentPosition, ref int[] i_NextPosition)
        {
            int currentRow = i_CurrentPosition[0];
            int currentCol = i_CurrentPosition[1];
            int nextRow = i_NextPosition[0];
            int nextCol = i_NextPosition[1];

            if (m_Board[currentRow, currentCol].GetPieceType() == e_PieceType.K
               || m_Board[currentRow, currentCol].GetPieceType() == e_PieceType.U)
            {
                e_PieceType currentPieceType = GetPieceAtPosition(currentRow, currentCol);
                Piece movingPiece = new Piece(currentPieceType);
                m_Board[nextRow, nextCol] = movingPiece;
            }    
            else if (!MakeKing(nextRow, nextCol))
            {
                e_PieceType currentPieceType = GetPieceAtPosition(currentRow, currentCol);
                Piece movingPiece = new Piece(currentPieceType);
                m_Board[nextRow, nextCol] = movingPiece;
            }     
            m_Board[currentRow, currentCol] = new Piece(e_PieceType.E);  
        }
        
    }
}