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
            public CustomEnvState _state;
            public VacuumAgent.VacuumAction _action;
            public AStarNode _parentNode = null;
            float _cost = 0;

            public AStarNode(CustomEnvState initialState)
            {
                _state = initialState;
                _cost = _state.EuclidianActionHeuristic();
            }

            public AStarNode(AStarNode previousNode, VacuumAgent.VacuumAction action)
            {
                _action = action;
                _state = new CustomEnvState(previousNode._state, action);
                // 1 is the cost of Manhattan to attain this node from previous node or cost of the action, either way it is the same cost
                // Cost is at 0.25 to Clean just so that if the Robot need to grab he does it
                _cost = 1 + (action == VacuumAgent.VacuumAction.Clean ?  0 : (action == VacuumAgent.VacuumAction.GrabClean ? 1 : _state.EuclidianActionHeuristic()));
            }

            public int CompareTo(AStarNode other)
            {
                return _cost.CompareTo(other._cost);
            }
        }

        public static Stack<VacuumAgent.VacuumAction> ExecuteFor(Problem problem)
        {
            Stack<VacuumAgent.VacuumAction> vacuumActions = new Stack<VacuumAgent.VacuumAction>();

            // Lancement de l'algorithme pour trouver un noeud solution du problème
            AStarNode lastNode = RunAlgo(problem);
            if (lastNode == null)
            {
                return vacuumActions;
            }

            // Récupération de toutes les actions conduisant à ce noeud
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
            List<CustomEnvState> closed = new List<CustomEnvState>();
            // Fringe of nodes
            PriorityQueue<AStarNode> fringe = new PriorityQueue<AStarNode>();
            fringe.Enqueue(new AStarNode(problem._initialState));

            while (true)
            {
                if (fringe.Count == 0) return null;
                AStarNode currentNode = fringe.Dequeue();
                if (problem.HasBeenSolved(currentNode._state)) return currentNode;
                // Check if state in closed set
                if (!closed.Contains(currentNode._state))
                {
                    closed.Add(currentNode._state);
                    // Insert following nodes
                    List<VacuumAgent.VacuumAction> vacuumActions = VacuumAgent.PossibleActionFromThere(currentNode._state);
                    foreach (var action in vacuumActions)
                    {
                        AStarNode newNode = new AStarNode(currentNode, action);
                        newNode._parentNode = currentNode;
                        fringe.Enqueue(newNode);
                    }
                }
            }
        }
    }
}
