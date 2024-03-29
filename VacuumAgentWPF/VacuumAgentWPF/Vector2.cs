﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacuumAgentWPF
{
    class Vector2
    {
        private int _x;
        public int X {
            get => _x;
            set => _x = value;
        }

        private int _y;
        public int Y
        {
            get => _y;
            set => _y = value;
        }

        Vector2() {
            _x = 0;
            _y = 0;
        }

        public Vector2(in int x, in int y) {
            _x = x;
            _y = y;
        }

        public override bool Equals(Object obj)
        {
            Vector2 vector2 = obj as Vector2;
            if (vector2 == null)
            {
                return false;
            }
            else
            {
                return _x == vector2.X && _y == vector2.Y;
            }
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2) => new Vector2(v1._x - v2._x, v1._y - v2._y);

        public float Magnitude() {
            return (float)Math.Sqrt(_x*_x + _y*_y);
        }
    }
}
