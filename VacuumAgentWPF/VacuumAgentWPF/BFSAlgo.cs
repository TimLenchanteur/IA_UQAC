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
                int depth;

            }
        }

        public static Stack<VacuumAgent.VacuumAction> ExecuteFor(Problem problem) {
            return new Stack<VacuumAgent.VacuumAction>();
        }
    }
}
