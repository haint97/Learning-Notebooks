using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Algorithm
{
    public abstract class Sorting<T> where T : ISortingStrategy<T>
    {
        public abstract void Sort(T[] array);

        protected void Swap(T[] array, int i, int j)
        {
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    // Define interface for Sorting Algorithms/Strategies
    public interface ISortingStrategy<T> where T : IComparable<T>
    {
        void Sort(T[] array);
    }
}
