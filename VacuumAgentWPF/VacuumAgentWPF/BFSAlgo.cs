using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacuumAgentWPF
{
    class BFSAlgo
    {
        class BFSNode
        {
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
            // Lancement de l'algorithme pour trouver un noeud solution du problème
            BFSNode lastNode = RunAlgo(problem);
            if(lastNode == null)
            {
                return new Stack<VacuumAgent.VacuumAction>();
            }

            // Récupération de toutes les actions conduisant à ce noeud
            while(lastNode._parentNode != null)
            {
                vacuumActions.Push(lastNode._action);
                lastNode = lastNode._parentNode;
            }
            return vacuumActions;
        }

        private static BFSNode RunAlgo(Problem problem)
        {
            // Liste des états déjà visités pour optimisation
            List<CustomEnvState> closed = new List<CustomEnvState>();
            // Frontière des noeuds
            Queue<BFSNode> fringe = new Queue<BFSNode>();
            fringe.Enqueue(new BFSNode(problem._initialState));

            while (true)
            {
                if (fringe.Count == 0) return null;
                BFSNode currentNode = fringe.Dequeue();
                if (problem.HasBeenSolved(currentNode._state)) return currentNode;
                // Vérification de l'absence de l'état dans les états visités
                if (!closed.Contains(currentNode._state))
                {
                    closed.Add(currentNode._state);
                    // Ajout des noeuds suivants
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
