using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacuumAgentWPF
{
    // Basé sur https://ericbackhage.net/c/how-to-design-a-priority-queue-in-c/
    class PriorityQueue<T> where T : IComparable<T>
    {
        private readonly List<T> _queueAbstract = new List<T>();

        public int Count {
            get => _queueAbstract.Count;
        }

        public void Enqueue(T item)
        {
            _queueAbstract.Add(item);
            _queueAbstract.Sort();
        }

        public T Dequeue()
        {
            var item = _queueAbstract[0];
            _queueAbstract.RemoveAt(0);
            return item;
        }
    }
}
