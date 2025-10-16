using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Algorithm
{
    // Abstract base class for sorting strategies. Provides common functionality like Swap
    // and implements the ISortingStrategy<T> interface. Subclasses must override the Sort method.
    public abstract class SortingBase<T> : ISortingStrategy<T> where T : IComparable<T>
    {
        // Abstract method to be implemented by concrete sorting strategies.
        public abstract void Sort(T[] array);

        // Protected helper method to swap two elements in the array.
        protected void Swap(T[] array, int i, int j)
        {
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    // Interface for sorting algorithms/strategies.
    // Ensures all strategies have a consistent Sort method for arrays of comparable elements.
    public interface ISortingStrategy<T> where T : IComparable<T>
    {
        void Sort(T[] array);
    }

}
