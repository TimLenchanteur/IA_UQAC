using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuAppWPF
{
    class SudokuCSP
    {
        int[,] m_grid;
        public int[,] Grid
        {
            get { return m_grid; }
        }
        int m_size;

        List<int>[,] m_domains;
        public List<int>[,] Domains
        {
            get
            {
                return m_domains;
            }
        }

        int[,] m_constraints;

        public SudokuCSP(int[,] grid)
        {
            m_grid = grid;
            m_size = (int)Math.Sqrt(m_grid.Length);
            m_domains = new List<int>[m_size, m_size];
            m_constraints = new int[m_size, m_size];
            GenerateAllDomainsConstraints();
        }

        void GenerateDomainConstraints(int i, int j)
        {
            m_domains[i, j] = new List<int>(Enumerable.Range(1, m_size));
            m_constraints[i, j] = 0;

            //check line
            for (int k = 0; k<m_size; k++)
            {
                if (k != j)
                {
                    if(m_grid[i, k] != -1)
                    {
                        m_domains[i, j].Remove(m_grid[i, k]);
                    }
                    else
                    {
                        m_constraints[i, j]++;
                    }
                }
            }

            //check column
            for(int k = 0; k < m_size; k++)
            {
                if (k != i)
                {
                    if (m_grid[k, j] != -1)
                    {
                        m_domains[i, j].Remove(m_grid[k, j]);
                    }
                    else
                    {
                        m_constraints[i, j]++;
                    }
                }
            }

            //check square
            for(int k = (i/3) * 3; k<(i/3) * 3+3; k++)
            {
                for(int l=(j/3) * 3; l < (j / 3) * 3 + 3; l++)
                {
                    if((i!=k && j!=l) && m_grid[k, l] != -1)
                    {
                        if(m_grid[k, l] != -1)
                        {
                            m_domains[i, j].Remove(m_grid[k, l]);
                        }
                        else
                        {
                            // Wrong: column and lines may have added this constraint already
                            m_constraints[i, j]++;
                        }
                    }
                }
            }
        }

        void GenerateAllDomainsConstraints()
        {
            for (int i = 0; i < m_size; i++)
            {
                for (int j = 0; j < m_size; j++)
                {
                    if (m_grid[i, j] != -1)
                    {
                        m_domains[i, j] = new List<int>();
                        m_constraints[i, j] = 0;
                    }
                    else
                    {
                        GenerateDomainConstraints(i, j);
                    }
                }
            }
        }

        public List<Tuple<int,int>> MinimumRemainingValues()
        {
            int mrv = 10;
            List<Tuple<int, int>> values = new List<Tuple<int, int>>();
            for(int i=0; i<m_size; i++)
            {
                for(int j=0; j<m_size; j++)
                {
                    if(m_grid[i,j] == -1)
                    {
                        if (m_domains[i, j].Count < mrv)
                        {
                            mrv = m_domains[i, j].Count;
                            values.Clear();
                            values.Add(new Tuple<int, int>(i, j));
                        }
                        else if(m_domains[i, j].Count == mrv)
                        {
                            values.Add(new Tuple<int, int>(i, j));
                        }
                    }
                }
            }
            return values;
        }

        public int[] NeighboursDomains(int i, int j)
        {
           int[] domains = new int[m_size+1];

            for (int k = 0; k < domains.Length; k++) {
                domains[k] = 0;
            }

            //line
            for (int k = 0; k < m_size; k++)
            {
                if (k != j && m_grid[k, j] != -1)
                {
                    foreach (int nonConstrainedValue in m_domains[k, j]) {
                        domains[nonConstrainedValue] += 1;   
                    }
                }
            }

            //column
            for (int k = 0; k < m_size; k++)
            {
                if (k != i && m_grid[i, k] != -1)
                {
                    foreach (int nonConstrainedValue in m_domains[i, k]){
                        domains[nonConstrainedValue] += 1;
                    }
                }
            }

            //check square
            for (int k = (i / 3) * 3; k < (i / 3) * 3 + 3; k++)
            {
                for (int l = (j / 3) * 3; l < (j / 3) * 3 + 3; l++)
                {
                    if (i != k && j != l && m_grid[k, l] != -1)
                    {
                        foreach (int nonConstrainedValue in m_domains[k, l]){
                            domains[nonConstrainedValue] += 1;
                        }
                    }
                }
            }

            return domains;
        }

        public int LeastConstrainingValue(List<int> currentDomain,int[] neighbourDomain)
        {
            int minConstraint = int.MaxValue;
            int bestValue = -1;
            foreach (int value in currentDomain)
            {
                if (minConstraint > neighbourDomain[value]) {
                    minConstraint = neighbourDomain[value];
                    bestValue = value;
                }
            }
            return bestValue;
        }

        public List<Tuple<int,int>> DegreeHeuristic(List<Tuple<int, int>> mrvValues)
        {
            int maxConstraints = 0;
            List<Tuple<int, int>> degreeValues = new List<Tuple<int, int>>();
            foreach (var value in mrvValues)
            {
                if(m_constraints[value.Item1, value.Item2] > maxConstraints)
                {
                    maxConstraints = m_constraints[value.Item1, value.Item2];
                    degreeValues.Clear();
                    degreeValues.Add(value);
                }
                else if(m_constraints[value.Item1, value.Item2] == maxConstraints)
                {
                    degreeValues.Add(value);
                }
            }
            return degreeValues;
        }

        public void SetValue(int i, int j, int value)
        {
            m_grid[i, j] = value;
            GenerateAllDomainsConstraints();
        }
    }
}
