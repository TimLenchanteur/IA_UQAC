using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuAppWPF
{
    /// <summary>
    /// Contraintes attribue aux variables d'un sudoku
    /// </summary>
    public class SudokuCSP
    {
        // Taille d'une ligne du sudoku
        int m_size;
        // Valeurs legales courante de chaque variable
        List<int>[,] m_variablesDomain;
        // Voisins de chaque variables sur le graphe csp
        // Chaque couple variable, voisin represente une contrainte binaire
        List<SudokuSolver.SudokuVariable>[,] m_variablesNeighbors;
        // Voisins non attribue de chaque variables sur le graphe csp 
        List<SudokuSolver.SudokuVariable>[,] m_variableRemainingNeighbor;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="assignment">Attribution initiale du sudoku</param>
        public SudokuCSP(SudokuSolver.SudokuAssignment assignment) {
            m_size = assignment.Size;
            int[,] grid = assignment.Grid;
            m_variablesDomain = new List<int>[m_size, m_size];
            m_variablesNeighbors = new List<SudokuSolver.SudokuVariable>[m_size,m_size];
            m_variableRemainingNeighbor = new List<SudokuSolver.SudokuVariable>[m_size, m_size];

            SudokuSolver.SudokuVariable[,] tempVariableGrid = new SudokuSolver.SudokuVariable[m_size, m_size];

            // Creer les variables
            for (int x = 0; x < m_size; x++)
            {
                for (int y = 0; y < m_size; y++)
                {
                    tempVariableGrid[x, y] = new SudokuSolver.SudokuVariable(x,y);
                    m_variablesDomain[x,y] = new List<int>(Enumerable.Range(1, m_size));
                    m_variablesNeighbors[x,y] = new List<SudokuSolver.SudokuVariable>();
                    m_variableRemainingNeighbor[x,y] = new List<SudokuSolver.SudokuVariable>();
                }
            }

            int squareSize = (int)Math.Sqrt(m_size);
            // Creer les contraintes
            for (int x = 0; x < m_size; x++)
            {
                for (int y = 0; y < m_size; y++)
                {

                    // Si la variable a deja ete assigne on peut mettre a jour son domaine et ne pas chercher ses voisins
                    if (grid[x,y] != -1) {
                        m_variablesDomain[x,y].Clear();
                        m_variablesDomain[x, y].Add(grid[x, y]);
                        continue;
                    }

                    // Sinon on l'ajoute a la liste des variable non assignes
                    assignment.AddRemainingVariable(tempVariableGrid[x, y]);
                    
                    // On va aussi chercher tous ses voisins 
                    // Si l'un d'entre eux a ete assigne on enleve sa valeur des valeurs legal de la variable courente, il n'est aussi pas
                    // necessaire de l'ajouter a la liste des voisins restant de cette variables
                    // Colonnes
                    for (int k = 0; k < m_size; k++)
                    {
                        if (k != y)
                        {
                            m_variablesNeighbors[x, y].Add(tempVariableGrid[x, k]);
                            if (grid[x, k] != -1) m_variablesDomain[x, y].Remove(grid[x, k]);
                            else m_variableRemainingNeighbor[x, y].Add(tempVariableGrid[x, k]);
                        }
                    }

                    // Lignes 
                    for (int k = 0; k < m_size; k++)
                    {
                        if (k != x)
                        {
                            m_variablesNeighbors[x, y].Add(tempVariableGrid[k,y]);
                            if (grid[k, y] != -1) m_variablesDomain[x, y].Remove(grid[k, y]);
                            else m_variableRemainingNeighbor[x, y].Add(tempVariableGrid[k, y]);
                        }
                    }

                    // Carre
                    for (int k = (x / squareSize) * squareSize; k < (x / squareSize) * squareSize + squareSize; k++)
                    {
                        for (int l = (y / squareSize) * squareSize; l < (y / squareSize) * squareSize + squareSize; l++)
                        {
                            if (x != k && y != l)
                            {
                                m_variablesNeighbors[x, y].Add(tempVariableGrid[k, l]);
                                if (grid[k, l] != -1) m_variablesDomain[x, y].Remove(grid[k, l]);
                                else m_variableRemainingNeighbor[x, y].Add(tempVariableGrid[k, l]);
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Constructeur de copie
        /// </summary>
        /// <param name="otherCSP">Le CSP a copie</param>
        public SudokuCSP(SudokuCSP otherCSP) {
            m_size = otherCSP.m_size;
            m_variablesNeighbors = otherCSP.m_variablesNeighbors;
            m_variablesDomain = new List<int>[m_size, m_size];
            m_variableRemainingNeighbor = new List<SudokuSolver.SudokuVariable>[m_size, m_size];

            for (int x = 0; x < m_size; x++)
            {
                for (int y = 0; y < m_size; y++)
                {
                    m_variablesDomain[x, y] = new List<int>(otherCSP.m_variablesDomain[x,y]);
                    m_variableRemainingNeighbor[x, y] = new List<SudokuSolver.SudokuVariable>(otherCSP.m_variableRemainingNeighbor[x, y]);
                }
            }
        }

        /// <summary>
        /// La valeur qui vient d'etre attribue est t'elle consistante avec les contrainte ?
        /// </summary>
        /// <param name="variable">Variable attribue</param>
        /// <param name="value">Valeur attribue</param>
        /// <returns>vrai si c'est la cas, faux sinon</returns>
        public bool IsValueConsistent(SudokuSolver.SudokuVariable variable, int value) {
            // Regarde si la valeur appartient au valeur legale de la variable
            // On peut se permettre de faire ca car on utilise du forward checking, sinon il faudrait tester les contraintes une par une a partir de l'assignement
            return m_variablesDomain[variable.X, variable.Y].Contains(value);
        }

        /// <summary>
        /// Retourne une liste des variables auquel il reste le moins de variables legales
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns>Les variables qui contiennent le moins de valeurs legales</returns>
        public List<SudokuSolver.SudokuVariable> MinimumRemainingValues(SudokuSolver.SudokuAssignment assignment)
        {
            int mrv = 10;
            List<SudokuSolver.SudokuVariable> values = new List<SudokuSolver.SudokuVariable>();
            foreach (SudokuSolver.SudokuVariable variable in assignment.RemainingVariable)
            {
                // Recupere le nombre de valeurs restante de la variable
                // On peut se permettre de faire ca car on utilise le forward checking
                // Dans le cas contraire il aurait fallu regarder tous les voisin pour chaque variables
                int domainSize = m_variablesDomain[variable.X, variable.Y].Count;
                if(domainSize < mrv)
                {
                    mrv = domainSize;
                    values.Clear();
                    values.Add(variable);
                }
                else if (domainSize == mrv) values.Add(variable);
            }
            return values;
        }

        /// <summary>
        /// Selectionne les variables qui sont le moins contrainte parmis celle restant a attribues
        /// </summary>
        /// <param name="mrvValues">Une liste de variables</param>
        /// <returns>Une liste des variables les moins contraintes</returns>
        public List<SudokuSolver.SudokuVariable> DegreeHeuristic(List<SudokuSolver.SudokuVariable> mrvValues)
        {
            int maxConstraints = 0;
            List<SudokuSolver.SudokuVariable> degreeValues = new List<SudokuSolver.SudokuVariable>();
            // Pour le moment les voisins restant sont gerer dans le forward checking pour eviter certaines operations (reinserer la variable non assigne dans les voisins restant)
            foreach (var variable in mrvValues)
            {
                int remainingConstraints = m_variableRemainingNeighbor[variable.X, variable.Y].Count;
                if (remainingConstraints > maxConstraints)
                {
                    maxConstraints = remainingConstraints;
                    degreeValues.Clear();
                    degreeValues.Add(variable);
                }
                else if (remainingConstraints == maxConstraints)
                {
                    degreeValues.Add(variable);
                }
            }
            return degreeValues;
        }

        /// <summary>
        /// Compte le nombre de contraintes pour chaque valeurs possible d'une variables
        /// </summary>
        /// <param name="variable">La variable</param>
        /// <returns>Nombre de contraintes pour chaque valeur possible dune variable</returns>
        public int[] NeighborsDomains(SudokuSolver.SudokuVariable variable) {
            int[] neighborsDomains = new int[m_size];

            for (int i = 0; i < m_size; i++) {
                neighborsDomains[i] = 0;
            }

            // Cela marche seulement parce que les domaines sont mis a jour dans le forward checking sans lui
            // il faudrait passer l'assignement et verifier pour tout les voisins et negligeant les voisins assigne
            foreach (SudokuSolver.SudokuVariable neighbors in m_variableRemainingNeighbor[variable.X, variable.Y]) {
                foreach (int value in m_variablesDomain[neighbors.X, neighbors.Y]) {
                    neighborsDomains[value - 1]++;
                }
            }
            return neighborsDomains;
        }

        /// <summary>
        /// Les valeurs legales restantes d'une variable
        /// </summary>
        /// <param name="variable">La variable</param>
        /// <returns>Une liste des valeurs legales restante de la variable</returns>
        public List<int> VariableDomains(SudokuSolver.SudokuVariable variable) {
            return m_variablesDomain[variable.X, variable.Y];
        }

        /// <summary>
        /// Retourne la valeur la moins contraignante parmi celle renseigne
        /// </summary>
        /// <param name="currentDomain">Les valeurs legale a regarder</param>
        /// <param name="neighbourDomain">Un tableau liant les valeurs legales au nombre de contraintes exerces sur celle-ci</param>
        /// <returns>La valeur la moins contraignante </returns>
        public int LeastConstrainingValue(List<int> currentDomain, int[] neighbourDomain)
        {
            int minConstraint = int.MaxValue;
            int bestValue = -1;
            foreach (int value in currentDomain)
            {
                if (minConstraint > neighbourDomain[value-1])
                {
                    minConstraint = neighbourDomain[value-1];
                    bestValue = value;
                }
            }
            return bestValue;
        }

        /// <summary>
        /// Algorithme permettant de verifier avec un temps d'avance si des valeurs legales ne pourront jamais etre assigne a une variable
        /// </summary>
        /// <param name="cspBase">Les contraintes actuelles sur le sudoku</param>
        /// <param name="assignement">L'attribution actuelle des variables</param>
        /// <param name="lastAssignedVariable">La derniere variable attribue</param>
        /// <returns>Les contraintes possiblement avec des valeurs legales en moins</returns>
        public static SudokuCSP ACThree(SudokuCSP cspBase, SudokuSolver.SudokuAssignment assignement, SudokuSolver.SudokuVariable lastAssignedVariable) {
            // Le fait d'effectuer une copie permet de ne pas avoir a s'inquieter de reset les changements qui on ete effectue 
            // dans l'algorithme si le chemin n'est pas retenue
            // Cela amene cependant une complexite suplementaire dans l'algorithme
            SudokuCSP newCsp = new SudokuCSP(cspBase);

            // On modifie a jour le domaine de la derniere variable assigne 
            // Cela est fait dans cette fonction car cela permet de ne pas avoir a se souvenir du domaine modifie
            // si jamais l'assignement de cette variable ne permet pas de resoudre le sudoku
            newCsp.m_variablesDomain[lastAssignedVariable.X, lastAssignedVariable.Y].Clear();
            newCsp.m_variablesDomain[lastAssignedVariable.X, lastAssignedVariable.Y].Add(assignement.Grid[lastAssignedVariable.X, lastAssignedVariable.Y]);
            foreach (SudokuSolver.SudokuVariable neighbor in newCsp.m_variableRemainingNeighbor[lastAssignedVariable.X, lastAssignedVariable.Y]) {
                newCsp.m_variableRemainingNeighbor[neighbor.X, neighbor.Y].Remove(lastAssignedVariable);
            }

            Queue<Tuple<SudokuSolver.SudokuVariable, SudokuSolver.SudokuVariable>> arcs = new Queue<Tuple<SudokuSolver.SudokuVariable, SudokuSolver.SudokuVariable>>();
            
            // On fait d'abord un tour sur les arcs entre toute les variables restantes et leur voisin
            foreach (SudokuSolver.SudokuVariable remainingVariable in assignement.RemainingVariable)
            {
                // Ici du fait que l'on sache dans quel variable on va modifier les domaines, on peut se permettre de ne pas rajouter ses voisins
                // a chaque fois qu'on lui enleve un domaine 
                bool remove = false;
                foreach (SudokuSolver.SudokuVariable neighbor in newCsp.m_variablesNeighbors[remainingVariable.X, remainingVariable.Y]) {
                    remove |= RemoveInconsistentValue(newCsp, remainingVariable, neighbor);
                }
                if (remove)
                {
                    foreach (SudokuSolver.SudokuVariable remainingNeighbor in newCsp.m_variableRemainingNeighbor[remainingVariable.X, remainingVariable.Y])
                    {
                        arcs.Enqueue(new Tuple<SudokuSolver.SudokuVariable, SudokuSolver.SudokuVariable>(remainingNeighbor, remainingVariable));
                    }
                }
            }

            // On effectue ensuite les operations necessaire sur la queue jusqu'a ce que celle-ci soit vide
            while (arcs.Count != 0)
            {
                Tuple<SudokuSolver.SudokuVariable, SudokuSolver.SudokuVariable> arc = arcs.Dequeue();
                if (RemoveInconsistentValue(newCsp, arc.Item1, arc.Item2))
                {
                    foreach (SudokuSolver.SudokuVariable remainingNeighbor in newCsp.m_variableRemainingNeighbor[arc.Item1.X, arc.Item1.Y])
                    {
                        arcs.Enqueue(new Tuple<SudokuSolver.SudokuVariable, SudokuSolver.SudokuVariable>(remainingNeighbor, arc.Item1));
                    }
                }
            }

            return newCsp;
        }

        /// <summary>
        /// Enleve les valeurs inconsistante des valeurs legales d'une variable
        /// </summary>
        /// <param name="csp">Les contraintes actuelle sur le sudoku</param>
        /// <param name="variableToCheck">La variables dont on verifie les valeurs legales</param>
        /// <param name="variableNeighbor">Un voisin de la variable</param>
        /// <returns></returns>
        static bool RemoveInconsistentValue(SudokuCSP csp, SudokuSolver.SudokuVariable variableToCheck, SudokuSolver.SudokuVariable variableNeighbor)
        {
            bool removed = false;
            List<int> domainsNode1 = new List<int>(csp.m_variablesDomain[variableToCheck.X, variableToCheck.Y]);
            List<int> domaineNode2 = csp.m_variablesDomain[variableNeighbor.X, variableNeighbor.Y];
            foreach (int x in domainsNode1)
            {
                // Si le domaine de la variable voisine ne contient qu'une seule valeur c'est qu'elle a soit ete assigne
                // soit c'est la seule variable que l'on peut lui assigne. 
                // Dans tous les cas on ne pourra pas donner cette valeur a la variable courante
                if (domaineNode2.Contains(x) && domaineNode2.Count == 1) {
                    csp.m_variablesDomain[variableToCheck.X, variableToCheck.Y].Remove(x);
                    removed = true;
                    break;
                }
            }
            return removed;
        }

    }
}

