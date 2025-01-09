using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex02
{
    internal class Piece 
    {
        private e_PieceType m_CurrentType;
        private string m_PieceContext;
        
        public Piece(e_PieceType i_PieceType)
        {
            m_CurrentType = i_PieceType;
            updatePieceContext(); 
        }
        
        private void updatePieceContext()
        {
            switch (m_CurrentType)
            {
                case e_PieceType.E:
                    m_PieceContext = "   ";
                    break;
                case e_PieceType.X:
                    m_PieceContext = " X ";
                    break;
                case e_PieceType.O:
                    m_PieceContext = " O ";
                    break;
                case e_PieceType.K:
                    m_PieceContext = " K ";
                    break;
                case e_PieceType.U:
                    m_PieceContext = " U ";
                    break;
            }
        }
        
        public e_PieceType GetPieceType()
        {
            return m_CurrentType;
        }

        public string GetPieceContext()
        {
            return m_PieceContext;
        }
    }
}
