using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex02
{
    internal class Player
    {
        private readonly string m_PlayerName;

        private int m_Score;
        private bool m_IsComputerPlayer { get; set; }

        private int[] m_PositionForExtraEating;

        private e_PlayerGameStatus m_PlayerStatus;

        public void SetScoreForPlayer(int i_Score)
        {
            m_Score += i_Score;
        }

        public int GetPlayerScore()
        {
            return m_Score;
        }

        public void SetPositionForExtraEating(ref int[] i_PositionForExtraEating)
        {
            m_PositionForExtraEating[0] = i_PositionForExtraEating[0];
            m_PositionForExtraEating[1] = i_PositionForExtraEating[1];
        }

        public int[] GetPositionForExtraEating()
        {
            return m_PositionForExtraEating;
        }

        public e_PlayerGameStatus GetPlayerGameStatus()
        {
            return m_PlayerStatus;
        }

        public void SetPlayerGameStatus(e_PlayerGameStatus i_PlayerGameStatus)
        { 
            m_PlayerStatus = i_PlayerGameStatus;
        }

        public string GetPlayerName()
        {
            return m_PlayerName;
        }

        public Player(string i_PlayerName)
        {
            m_PlayerName = i_PlayerName;
            m_Score = 0;
            if (i_PlayerName.Equals("Computer"))
            {
                m_IsComputerPlayer = true;
                m_PositionForExtraEating = new int[2];
                m_PositionForExtraEating[0] = 0;
                m_PositionForExtraEating[1] = 0;
            }
            else
            {
                m_IsComputerPlayer = false;
                m_PositionForExtraEating = null;
            }
        }
    }
}