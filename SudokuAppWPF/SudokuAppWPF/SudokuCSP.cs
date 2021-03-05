using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuAppWPF
{
    public class SudokuCSP
    {
        public class CSPVariable {
            Tuple<int, int> m_positionAttached;
            public Tuple<int, int> Position {
                get => m_positionAttached;
            }
            List<CSPVariable> m_neighbors;
            public List<CSPVariable> Neighbors {
                get => m_neighbors;
            }

            int m_currentValue;
            int m_remainingConstraint;
            public int RemainingConstraint
            {
                get => m_remainingConstraint;
            }

            int m_maxDomainValues;
            List<int> m_domains;
            public List<int> NodeDomain {
                get => m_domains;
            }
            public int DomainSize {
                get => m_domains.Count;
            }

            public CSPVariable(Tuple<int, int> position, int sizeSquare) {
                m_positionAttached = position;
                m_neighbors = new List<CSPVariable>();
                m_currentValue = -1;
                m_maxDomainValues = sizeSquare;
                m_domains = new List<int>(Enumerable.Range(1, sizeSquare));
                m_remainingConstraint = 0;
            }

            /// <summary>
            /// 
            /// Appelez cette fonction uniquement a la construction
            /// </summary>
            /// <param name="neighbor"></param>
            public void AddNeigbhor(CSPVariable neighbor) {
                if (!m_neighbors.Contains(neighbor))
                {
                    m_neighbors.Add(neighbor);
                    m_remainingConstraint++;
                    if (HasSetValue() && !neighbor.HasSetValue())
                    {
                        neighbor.RemoveValueFromDomain(m_currentValue);
                        neighbor.m_remainingConstraint--;
                    }
                }
            }

            public int[] NeighorsDomains() {
                int[] domains = new int[m_maxDomainValues + 1];

                for (int k = 0; k < domains.Length; k++)
                {
                    domains[k] = 0;
                }

                foreach (CSPVariable neighbors in m_neighbors) {
                    foreach (int nonConstrainedValue in neighbors.m_domains)
                    {
                        domains[nonConstrainedValue] += 1;
                    }
                }

                return domains;
            }

            public void AddValueInDomains(int value)
            {
                foreach (CSPVariable neighbor in m_neighbors) {
                    if (neighbor.m_currentValue == value) return;
                }
                m_domains.Add(value);
            }

            public void RemoveValueFromDomain(int value)
            {
                m_domains.Remove(value);
            }

            public void SetValue(int value)
            {
                m_currentValue = value;
                foreach (CSPVariable neighbor in m_neighbors)
                {
                    neighbor.RemoveValueFromDomain(value);
                    neighbor.m_remainingConstraint--;
                }
            }

            public bool HasSetValue() {
                return m_currentValue != -1;
            }

            public void ResetValue() {
                int resetedValue = m_currentValue;
                m_currentValue = -1;
                foreach (CSPVariable neighbor in m_neighbors)
                {
                    neighbor.AddValueInDomains(resetedValue);
                    neighbor.m_remainingConstraint++;
                }
            }
        }

        public List<CSPVariable> m_remainingVariable;
        List<Tuple<CSPVariable, CSPVariable>> m_constraints;
        public int RemainingVariable { 
            get => m_remainingVariable.Count;
        }


        public SudokuCSP(int[,] grid)
        {
            m_remainingVariable = new List<CSPVariable>();
            m_constraints = new List<Tuple<CSPVariable, CSPVariable>>();
            BuildGraph(grid);
            return;
        }

        void BuildGraph(int[,] grid) {
            int squareSize = (int)Math.Sqrt(grid.Length);
            CSPVariable[,] tempVariableGrid = new CSPVariable[squareSize, squareSize];

            for (int x = 0; x < squareSize; x++) {
                for (int y = 0; y < squareSize; y++) {
                    tempVariableGrid[x, y] = new CSPVariable(new Tuple<int, int>(x, y), squareSize);
                    if (grid[x, y] != -1) tempVariableGrid[x, y].SetValue(grid[x, y]);
                    else m_remainingVariable.Add(tempVariableGrid[x, y]);
                }
            }

            CreateConstraint(tempVariableGrid, squareSize);
        }

        void CreateConstraint(CSPVariable[,] grid, int gridSize) {

            for (int x = 0; x < gridSize; x++) {
                for (int y = 0; y < gridSize; y++) {

                    //Column
                    for (int k = 0; k < gridSize; k++)
                    {
                        if (k != y)
                        {
                            m_constraints.Add(new Tuple<CSPVariable, CSPVariable>(grid[x, y], grid[x, k]));
                            grid[x, y].AddNeigbhor(grid[x, k]);
                            grid[x, k].AddNeigbhor(grid[x,y]);
                        }
                    }

                    //Line
                    for (int k = 0; k < gridSize; k++)
                    {
                        if (k != x)
                        {
                            m_constraints.Add(new Tuple<CSPVariable, CSPVariable>(grid[x, y], grid[k,y]));
                            grid[x, y].AddNeigbhor(grid[k, y]);
                            grid[k, y].AddNeigbhor(grid[x, y]);
                        }
                    }

                    //check square
                    for (int k = (x / 3) * 3; k < (x / 3) * 3 + 3; k++)
                    {
                        for (int l = (y / 3) * 3; l < (y / 3) * 3 + 3; l++)
                        {
                            if (x != k && y != l)
                            {
                                m_constraints.Add(new Tuple<CSPVariable, CSPVariable>(grid[x, y], grid[k, l]));
                                grid[x, y].AddNeigbhor(grid[k, l]);
                                grid[k, l].AddNeigbhor(grid[x, y]);
                            }
                        }
                    }
                }
            }
        }

        public void SetValue(CSPVariable variable, int value)
        {
            variable.SetValue(value);
            m_remainingVariable.Remove(variable);
            return;
        }

        public void ResetValue(CSPVariable variable) {
            variable.ResetValue();
            m_remainingVariable.Add(variable);
            return;
        }

        public List<CSPVariable> MinimumRemainingValues()
        {
            int mrv = 10;
            List<CSPVariable> values = new List<CSPVariable>();
            foreach (CSPVariable variable in m_remainingVariable) {
                if (variable.DomainSize < mrv)
                {
                    mrv = variable.DomainSize;
                    values.Clear();
                    values.Add(variable);
                }
                else if (variable.DomainSize == mrv) values.Add(variable);
            }
            return values;
        }

        public List<CSPVariable> DegreeHeuristic(List<CSPVariable> mrvValues)
        {
            int maxConstraints = 0;
            List<CSPVariable> degreeValues = new List<CSPVariable>();
            foreach (var variable in mrvValues)
            {
                if (variable.RemainingConstraint > maxConstraints)
                {
                    maxConstraints = variable.RemainingConstraint;
                    degreeValues.Clear();
                    degreeValues.Add(variable);
                }
                else if (variable.RemainingConstraint == maxConstraints)
                {
                    degreeValues.Add(variable);
                }
            }
            return degreeValues;
        }

        public int LeastConstrainingValue(List<int> currentDomain, int[] neighbourDomain)
        {
            int minConstraint = int.MaxValue;
            int bestValue = -1;
            foreach (int value in currentDomain)
            {
                if (minConstraint > neighbourDomain[value])
                {
                    minConstraint = neighbourDomain[value];
                    bestValue = value;
                }
            }
            return bestValue;
        }

        public void ACThree() {
            //Queue is as effecient as list can't optimize this 
            Queue<Tuple<CSPVariable, CSPVariable>> arcs = new Queue<Tuple<CSPVariable, CSPVariable>>(m_constraints);

            while (arcs.Count != 0) {
                Tuple<CSPVariable, CSPVariable> arc = arcs.Dequeue();
                if (arc.Item1.HasSetValue() || arc.Item2.HasSetValue()) break;
                if (RemoveInconsistentValue(arc)) {
                    foreach (CSPVariable neighbor in arc.Item1.Neighbors) {
                        if(!neighbor.HasSetValue()) arcs.Enqueue(new Tuple<CSPVariable, CSPVariable>(neighbor, arc.Item1));
                    }
                }
            }
        }

        bool RemoveInconsistentValue(Tuple<CSPVariable, CSPVariable> arc) {
            bool removed = false;
            List<int> domainsNode1 = new List<int>(arc.Item1.NodeDomain);
            foreach (int x in domainsNode1) {
                //Contains seems actually faster than equal 
                if (arc.Item2.NodeDomain.Contains(x) && arc.Item2.DomainSize == 1) {
                    arc.Item1.RemoveValueFromDomain(x);
                    removed = true;
                }
            }
            return removed;
        }

    }
}
