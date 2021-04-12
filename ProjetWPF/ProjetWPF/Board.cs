using System;
using System.Collections.Generic;
using System.Text;

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
                            token.Position = new Vector2(i, j);
                            m_tokens[i, j] = token;
                        }
                        // White
                        else if (i >= 6)
                        {
                            Token token = new Token(Token.TokenColor.White);
                            token.Position = new Vector2(i, j);
                            m_tokens[i, j] = token;
                        }
                    }
                }
            }
        }
    }
}
