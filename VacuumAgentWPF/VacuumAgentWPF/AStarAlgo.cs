using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacuumAgentWPF
{
    class AStarAlgo
    {

        class AStarNode : IComparable<AStarNode>
        {

            // Would have probably been best to find a way to deal with this with polymorphism but overkill for this tp
            public CustomEnvState _state;
            public VacuumAgent.VacuumAction _action;
            public AStarNode _parentNode = null;
            float _cost = 0;

            public AStarNode(CustomEnvState initialState)
            {
                _state = initialState;
                // 0 + Heuristique car noeud racine
                _cost = _state.EuclidianActionHeuristic();
            }

            public AStarNode(CustomEnvState previousState, VacuumAgent.VacuumAction action)
            {
                _action = action;
                _state = new CustomEnvState(previousState, action);
                // 1 is the cost of Manhattan to attain this node from previous node or cost of the action, either way it is the same cost
                _cost = 1 + (action == VacuumAgent.VacuumAction.Clean ? 0 : _state.EuclidianActionHeuristic());
            }

            public int CompareTo(AStarNode other)
            {
                return _cost.CompareTo(other._cost);
            }
        }

        public static Stack<VacuumAgent.VacuumAction> ExecuteFor(Problem problem)
        {
            Stack<VacuumAgent.VacuumAction> vacuumActions = new Stack<VacuumAgent.VacuumAction>();

            // Run the algorithm to find solution node
            AStarNode lastNode = RunAlgo(problem);
            if (lastNode == null)
            {
                return vacuumActions;
            }

            // Get all actions leading to this node
            while (lastNode._parentNode != null)
            {
                vacuumActions.Push(lastNode._action);
                lastNode = lastNode._parentNode;
            }
            return vacuumActions;
        }

        private static AStarNode RunAlgo(Problem problem)
        {
            // Closed set of agent position for memory optimisation
            List<Vector2> closed = new List<Vector2>();
            // Fringe of nodes
            PriorityQueue<AStarNode> fringe = new PriorityQueue<AStarNode>();
            fringe.Enqueue(new AStarNode(problem._initialState));

            while (true)
            {
                if (fringe.Count == 0) return null;
                AStarNode currentNode = fringe.Dequeue();
                if (problem.HasBeenSolved(currentNode._state)) return currentNode;
                // Check if state in closed set
                if (!closed.Contains(currentNode._state.Agent_Pos))
                {
                    closed.Add(currentNode._state.Agent_Pos);
                    // Insert following nodes
                    List<VacuumAgent.VacuumAction> vacuumActions = VacuumAgent.PossibleActionFromThere(currentNode._state);
                    foreach (var action in vacuumActions)
                    {
                        AStarNode newNode = new AStarNode(currentNode._state, action);
                        newNode._parentNode = currentNode;
                        fringe.Enqueue(newNode);
                    }
                }
            }
        }
    }
}
