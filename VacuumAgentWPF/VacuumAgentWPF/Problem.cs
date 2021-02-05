using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacuumAgentWPF
{
    class Problem
    {
        EnvState _initialState;
        EnvState InitialState
        {
            get => _initialState;
        }

        EnvState _wishedState;

        public Problem(EnvState initialState, EnvState wishedState) {
            _initialState = initialState;
            _wishedState = wishedState;
        }

        public bool HasBeenSolved(EnvState resultState) {
            return _wishedState.IsEqual(resultState);
        }
    }
}
