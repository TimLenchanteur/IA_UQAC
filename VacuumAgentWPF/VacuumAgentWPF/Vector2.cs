using System;
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


    }
}
