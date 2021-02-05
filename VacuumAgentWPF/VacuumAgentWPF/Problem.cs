using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacuumAgentWPF
{
    class Problem
    {
        public CustomEnvState _initialState { get; }

        CustomEnvState _wishedState;

        public Problem(CustomEnvState initialState, CustomEnvState wishedState) {
            _initialState = initialState;
            _wishedState = wishedState;
        }

        public bool HasBeenSolved(CustomEnvState resultState) {
            return _wishedState.IsEqual(resultState);
        }
    }
}
