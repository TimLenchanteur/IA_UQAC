using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetWPF
{
    class Queen : Token
    {
        public Queen(TokenColor color) : base(color)
        {
        }

        public override void SetColor(TokenColor color)
        {
            m_color = color;
            if (m_color == TokenColor.Black)
            {
                image = "queenblack.png";
            }
            else
            {
                image = "queenwhite.png";
            }
        }
    }
}
