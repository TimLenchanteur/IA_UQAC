using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacuumAgentWPF
{
    class BFSAlgo
    {
        // To keep track of state visited i guess it would be easier to keep track of only the position 
        //(Maybe need to override the equality not sure how contain work)
        class BFSNode
        {
            // Would have probably been best to find a way to deal with this with polymorphism but overkill for this tp
            public CustomEnvState _state;
            public VacuumAgent.VacuumAction _action;
            public int _depth = 0;
            public BFSNode _parentNode = null;

            public BFSNode(CustomEnvState initialState)
            {
                _state = initialState;
            }

            public BFSNode(CustomEnvState previousState, VacuumAgent.VacuumAction action)
            {
                _action = action;
                _state = new CustomEnvState(previousState, action);
            }
        }

        public static Stack<VacuumAgent.VacuumAction> ExecuteFor(Problem problem)
        {
            Stack<VacuumAgent.VacuumAction> vacuumActions = new Stack<VacuumAgent.VacuumAction>();
            // Run the algorithm to find solution node
            BFSNode lastNode = RunAlgo(problem);
            if(lastNode == null)
            {
                return new Stack<VacuumAgent.VacuumAction>();
            }

            // Get all actions leading to this node
            while(lastNode._parentNode != null)
            {
                vacuumActions.Push(lastNode._action);
                lastNode = lastNode._parentNode;
            }
            return vacuumActions;
        }

        private static BFSNode RunAlgo(Problem problem)
        {
            // Closed set of agent position for memory optimisation
            List<CustomEnvState> closed = new List<CustomEnvState>();
            // Fringe of nodes
            Queue<BFSNode> fringe = new Queue<BFSNode>();
            fringe.Enqueue(new BFSNode(problem._initialState));

            while (true)
            {
                if (fringe.Count == 0) return null;
                BFSNode currentNode = fringe.Dequeue();
                if (problem.HasBeenSolved(currentNode._state)) return currentNode;
                // Check if state in closed set
                if (!closed.Contains(currentNode._state))
                {
                    closed.Add(currentNode._state);
                    // Insert following nodes
                    List<VacuumAgent.VacuumAction> vacuumActions = VacuumAgent.PossibleActionFromThere(currentNode._state);
                    foreach (var action in vacuumActions)
                    {
                        BFSNode newNode = new BFSNode(currentNode._state, action);
                        newNode._parentNode = currentNode;
                        newNode._depth = currentNode._depth + 1;
                        fringe.Enqueue(newNode);
                    }
                }
            }
        }
    }
}
