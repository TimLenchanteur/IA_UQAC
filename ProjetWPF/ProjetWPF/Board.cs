using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ProjetWPF
{
    class Board
    {
        Token[,] m_tokens = new Token[10, 10];
        public Token[,] Tokens { get { return m_tokens; } }

        public void Initialize()
        {
            for(int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (i % 2 == 0 && j % 2 == 1 || i % 2 == 1 && j % 2 == 0)
                    {
                        // Black
                        if(i <= 3)
                        {
                            Token token = new Token(Token.TokenColor.Black);
                            token.Position = new Vector2(j, i);
                            m_tokens[i, j] = token;
                        }
                        // White
                        else if (i >= 6)
                        {
                            Token token = new Token(Token.TokenColor.White);
                            token.Position = new Vector2(j, i);
                            m_tokens[i, j] = token;
                        }
                        else
                        {
                            m_tokens[i, j] = null;
                        }
                    }
                }
            }
        }

        public List<Vector2> PossibleMoves(Token token)
        {
            List<Vector2> moves = new List<Vector2>();

            // Moves
            if (token.Color == Token.TokenColor.Black)
            {
                // Down left
                Vector2 downLeft = new Vector2(token.Position.X - 1, token.Position.Y + 1);
                if (downLeft.X >= 0 && downLeft.Y < 10 && m_tokens[downLeft.Y, downLeft.X] == null)
                {
                    moves.Add(downLeft);
                }
                // Down right
                Vector2 downRight = new Vector2(token.Position.X + 1, token.Position.Y + 1);
                if (downRight.X < 10 && downLeft.Y < 10 && m_tokens[downRight.Y, downRight.X] == null)
                {
                    moves.Add(downRight);
                }
            }
            else
            {
                // Top left
                Vector2 topLeft = new Vector2(token.Position.X - 1, token.Position.Y - 1);
                if (topLeft.X >= 0 && topLeft.Y >= 0 && m_tokens[topLeft.Y, topLeft.X] == null)
                {
                    moves.Add(topLeft);
                }
                // Top right
                Vector2 topRight = new Vector2(token.Position.X + 1, token.Position.Y - 1);
                if (topRight.X < 10 && topRight.Y >= 0 && m_tokens[topRight.Y, topRight.X] == null)
                {
                    moves.Add(topRight);
                }
            }

            // Capture
            // Top left
            // Top right
            // Down left
            // Down right

            return moves;
        }
    }
}
