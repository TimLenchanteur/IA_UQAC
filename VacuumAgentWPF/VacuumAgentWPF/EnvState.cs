using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacuumAgentWPF
{
    class EnvState
    {
        protected int _markedState;

        public EnvState() {
            _markedState = 0;
        }

        public void MarkAttributeForEquality(int state) {
            _markedState = state;
        }

        public virtual bool IsEqualRelativeToTestedAttribute(EnvState otherState) {
            return false;
        }

    }
}
