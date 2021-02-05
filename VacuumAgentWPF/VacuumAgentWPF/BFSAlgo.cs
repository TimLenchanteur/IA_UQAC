using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacuumAgentWPF
{
    class BFSAlgo
    {

        class BFSGraph
        {
            // To keep track of state visited i guess it would be easier to keep track of only the position 
            //(Maybe need to override the equality not sure how contain work)
            class BFSNode
            {
                // Would have probably been best to find a way to deal with this with polymorphism but overkill for this tp
                public CustomEnvState _state;
                VacuumAgent.VacuumAction _action;
                int _depth;

                BFSNode(CustomEnvState previousState, VacuumAgent.VacuumAction action) {
                    _action = action;
                    _state = new CustomEnvState(previousState, action);
                }


            }
        }

        public static Stack<VacuumAgent.VacuumAction> ExecuteFor(Problem problem) {
            return new Stack<VacuumAgent.VacuumAction>();
        }
    }
}
