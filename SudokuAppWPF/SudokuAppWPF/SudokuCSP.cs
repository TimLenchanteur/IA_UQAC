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

        public SudokuCSP(int[,] grid)
        {
            m_grid = grid;
            m_size = (int)Math.Sqrt(m_grid.Length);
            m_domains = new List<int>[m_size, m_size];
            GenerateDomains();
        }

        void GenerateDomain(int i, int j)
        {
            m_domains[i, j] = new List<int>(Enumerable.Range(1, m_size));

            //check line
            for (int k = 0; k<m_size; k++)
            {
                if (k != j && m_grid[i,k]!=-1)
                {
                    m_domains[i, j].Remove(m_grid[i, k]);
                }
            }

            //check column
            for(int k = 0; k < m_size; k++)
            {
                if (k != i && m_grid[k, j] != -1)
                {
                    m_domains[i, j].Remove(m_grid[k, j]);
                }
            }

            //check square
            for(int k = (i/3) * 3; k<(i/3) * 3+3; k++)
            {
                for(int l=(j/3) * 3; l < (j / 3) * 3 + 3; l++)
                {
                    if((i!=k && j!=l) && m_grid[k, l] != -1)
                    {
                        m_domains[i, j].Remove(m_grid[k, l]);
                    }
                }
            }
        }

        void GenerateDomains()
        {
            for (int i = 0; i < m_size; i++)
            {
                for (int j = 0; j < m_size; j++)
                {
                    if (m_grid[i, j] != -1)
                    {
                        m_domains[i, j] = new List<int>();
                    }
                    else
                    {
                        GenerateDomain(i, j);
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

        public void SetValue(int i, int j, int value)
        {
            m_grid[i, j] = value;
            GenerateDomains();
        }
    }
}
