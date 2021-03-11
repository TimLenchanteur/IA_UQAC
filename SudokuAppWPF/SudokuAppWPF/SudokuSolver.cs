using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SudokuAppWPF
{
    /// <summary>
    /// IA capable de resoudre un sudoku 
    /// </summary>
    public class SudokuSolver
    {
        /// <summary>
        /// Classe utilise pour modeliser une case de sudoku
        /// </summary>
        public class SudokuVariable {
            private int _x;
            public int X
            {
                get => _x;
                set => _x = value;
            }

            private int _y;
            public int Y
            {
                get => _y;
                set => _y = value;
            }

            SudokuVariable()
            {
                _x = 0;
                _y = 0;
            }

            public SudokuVariable(in int x, in int y)
            {
                _x = x;
                _y = y;
            }

            public override bool Equals(Object obj)
            {
                if (obj == this) return true;

                SudokuVariable vector2 = obj as SudokuVariable;
                if (vector2 == null)
                {
                    return false;
                }
                else
                {
                    return _x == vector2.X && _y == vector2.Y;
                }
            }
        }

        /// <summary>
        /// Classe utilise pour modeliser l'attribution de variable du sudoku a un moment T
        /// </summary>
        public class SudokuAssignment {
            // La grille de sudoku avec ses valeurs assignes
            int[,] m_grid;
            int m_size;
            public int[,] Grid {
                get => m_grid;
            }
            // Taille d'une ligne du sudoku
            public int Size {
                get => m_size;
            }

            // Variable qui non pas ete attribue pour le moment 
            List<SudokuVariable> m_remainingVariable;
            public List<SudokuVariable> RemainingVariable {
                get => m_remainingVariable;
            }

            public SudokuAssignment(int[,] grid, int size) {
                m_size = size;
                m_grid = new int[size, size];
                m_remainingVariable = new List<SudokuVariable>();
                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < size; j++) {
                        m_grid[i, j] = grid[i, j];
                    }
                }
            }

            /// <summary>
            /// Indique qu'une variable qui n'est pas encore connu de la classe n'a pas encore ete attribue
            /// </summary>
            /// <param name="variable">Variable a attribue</param>
            public void AddRemainingVariable(SudokuVariable variable) {
                m_remainingVariable.Add(variable);
            }

            /// <summary>
            /// Attribue une variable
            /// </summary>
            /// <param name="variable">Variable a attribue</param>
            /// <param name="value">Valeur attribue a la variable</param>
            public void AssignVariable(SudokuVariable variable, int value) {
                m_remainingVariable.Remove(variable);
                m_grid[variable.X, variable.Y] = value;
            }

            /// <summary>
            /// L'attribution est t'elle complete ? 
            /// </summary>
            /// <returns>vrai si toute les variable du sudoku sont attribue, faux sinon</returns>
            public bool IsComplete() {
                return m_remainingVariable.Count == 0; 
            }

            /// <summary>
            /// Desattribue une variable
            /// </summary>
            /// <param name="variable">Variable qui n'est plus attribue</param>
            public void UnassignVariable(SudokuVariable variable) {
                //Pas besoin d'enlever la valeur de la grille, elle sera eventuellement remplacer
                m_remainingVariable.Add(variable);
            }
        }

        // Attribution actuelle des variables du sudoku
        SudokuAssignment m_assignement;
        // Contrainte a applique sur les variables du sudoku
        SudokuCSP m_contraints;

        /// <summary>
        /// Constructeur 
        /// </summary>
        /// <param name="grid">La grille de sudoku avec ses valeurs initial</param>
        /// <param name="size">La taille d'une ligne de la grille</param>
        public SudokuSolver(int[,] grid, int size) {
            m_assignement = new SudokuAssignment(grid, size);
            m_contraints = new SudokuCSP(m_assignement);
        }

        /// <summary>
        /// Resoud le sudoku attache a l'IA
        /// </summary>
        /// <returns>Un tableau contenant les variables de sudoku attribue si l'Ia a reussi a resoudre celui-ci, null sinon</returns>
        public int[,] Solve() {
            return RecursiveBackTracking(m_contraints, m_assignement);
        }

        /// <summary>
        /// Algorithme de resolution du sudoku a partir des contraintes qui lui sont imposées
        /// </summary>
        /// <param name="csp">Contraintes sur le sudoku</param>
        /// <param name="assignement">Attribution initiale des variables du sudoku</param>
        /// <returns>Un tableau contenant les variables de sudoku attribue si l'algorithme a reussi a resoudre celui-ci, null sinon</returns>
        int[,] RecursiveBackTracking(SudokuCSP csp, SudokuAssignment assignement)
        {
            if (assignement.IsComplete()) return assignement.Grid;

            SudokuVariable selectedVariable = SelectUnassignedVariable(csp, assignement);

            foreach (int value in OrderDomainValue(csp, selectedVariable)) {
                if (csp.IsValueConsistent(selectedVariable, value)) {
                    assignement.AssignVariable(selectedVariable, value);
                    // Effectue un forward checking avant d'aller plus profond
                    int[,] res = RecursiveBackTracking(SudokuCSP.ACThree(csp, assignement, selectedVariable), assignement);
                    if (res != null) return res;
                    assignement.UnassignVariable(selectedVariable);
                }
            }
            return null;
        }

        /// <summary>
        /// Une liste des valeurs legales de la variable trier de facon a ce que la valeur la moins contraignante soit en tete de liste
        /// </summary>
        /// <param name="csp">Contrainte sur le sudoku</param>
        /// <param name="variableSelected">Variable a attribue</param>
        /// <returns>la liste correspondante</returns>
        private List<int> OrderDomainValue(SudokuCSP csp, SudokuVariable variableSelected) {
            List<int> orderedDomainValue = new List<int>();
            // On recupere un tableau qui compte l'apparition de chaque valeurs legale dans les voisins restant de la variable
            // (Ne marche que lors de l'utilisation d'un algorithme de forward checking)
            int[] neighborsDomains = csp.NeighborsDomains(variableSelected);
            // Recuperation des valeurs legales de la variable
            List<int> variableDomain = new List<int>(csp.VariableDomains(variableSelected));
            // Tant que l'on a pas parcouru toute les valeurs legales de la variable on regarde la moins contraignante et on l'ajoute 
            // a la liste resultat
            while (variableDomain.Count != 0) {
                int nextValue = csp.LeastConstrainingValue(variableDomain, neighborsDomains);
                variableDomain.Remove(nextValue);
                orderedDomainValue.Add(nextValue);
            }
            return orderedDomainValue;
        }

        /// <summary>
        /// Selectionne une variable non attribue
        /// </summary>
        /// <param name="csp">Contrainte sur le sudoku</param>
        /// <param name="assignement">Attribution des variables</param>
        /// <returns></returns>
        private SudokuVariable SelectUnassignedVariable(SudokuCSP csp, SudokuAssignment assignement)
        {
            var mrvValues = csp.MinimumRemainingValues(assignement);
            // Choisi la premiere variable rendu par degree heuristic
            SudokuVariable toAssign = csp.DegreeHeuristic(mrvValues)[0];

            return toAssign;
        }
    }
}
