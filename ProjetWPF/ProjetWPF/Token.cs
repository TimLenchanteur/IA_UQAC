using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetWPF
{
    class Token
    {
        public enum TokenColor { White, Black };
        protected TokenColor m_color;
        public TokenColor Color { get { return m_color; } }

        public string image;

        private Vector2 m_position;
        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        public Token(TokenColor color)
        {
            SetColor(color);
        }

        public virtual void SetColor(TokenColor color)
        {
            m_color = color;
            if (m_color == TokenColor.Black)
            {
                image = "tokenblack.png";
            }
            else
            {
                image = "tokenwhite.png";
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Token)) return false;
            Token token = obj as Token;
            return m_color == token.m_color && m_position.Equals(token.m_position);
        }
    }
}
