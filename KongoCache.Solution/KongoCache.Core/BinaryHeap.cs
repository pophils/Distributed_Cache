using System;

namespace KongoCache.Core
{
    public class BinaryHeap<T> where T : IComparable
    {
        T[] heap;
        int size;
        readonly bool _isMaxHeap;

        public BinaryHeap(bool isMaxHeap = true)
        {
            heap = new T[Capacity];
            _isMaxHeap = isMaxHeap;
        }
               
        public BinaryHeap(int capacity = 0, bool isMaxHeap = true)
        {
            Capacity = capacity > 0 ? capacity : 2;
            heap = new T[Capacity];
            _isMaxHeap = isMaxHeap;
        }


        public void Push(T value)
        {
            EnsureCapacity();

            heap[size] = value;

            if (size == 0)
            {
                size += 1;
                return;
            }

            if (_isMaxHeap)
            {
                PushIntoMaxHeap();
            }
            else
            {
                PushIntoMinHeap();
            }
        }

        public T Pop()
        {
            if (size == 0)
            {
                throw new IndexOutOfRangeException();
            }

            T max = Peek();
            heap[0] = heap[size - 1];

            size -= 1;
            if (size != 1)
            {
                if (_isMaxHeap)
                    BubbleMaxUp();
                else
                    BubbleMinUp();
            }

            return max;
        }
               
        public T Peek()
        {
            if (size == 0)
            {
                throw new IndexOutOfRangeException();
            }
            return size > 0 ? heap[0] : default;
        }
               
        public bool IsEmpty()
        {
            return size == 0;
        }

        public int Capacity { get; private set; }

        void PushIntoMaxHeap()
        {
            int currentIndex = size;
            int currentParentIndex = GetParentIndex(size);

            while (currentIndex > 0 && heap[currentIndex].CompareTo(heap[currentParentIndex]) > 0)
            {
                Swap(currentIndex, currentParentIndex);
                currentIndex = currentParentIndex;
                currentParentIndex = GetParentIndex(currentIndex);
            }

            size += 1;
        }

        void PushIntoMinHeap()
        {
            int currentIndex = size;
            int currentParentIndex = GetParentIndex(size);

            while (currentIndex > 0 && heap[currentIndex].CompareTo(heap[currentParentIndex]) < 0)
            {
                Swap(currentIndex, currentParentIndex);
                currentIndex = currentParentIndex;
                currentParentIndex = GetParentIndex(currentIndex);
            }

            size += 1;
        }

        void BubbleMaxUp()
        {
            int parentIndex = 0;
            int leftIndex = GetLeftChildIndex(0);

            while (leftIndex < size)
            {
                int rightIndex = leftIndex + 1;
                int biggerIndex = leftIndex;

                if (rightIndex < size && heap[rightIndex].CompareTo(heap[leftIndex]) > 0)  // right is more
                {
                    biggerIndex = rightIndex;
                }

                if (heap[biggerIndex].CompareTo(heap[parentIndex]) < 0)
                {
                    break;
                }

                Swap(parentIndex, biggerIndex);
                parentIndex = biggerIndex;
                leftIndex = GetLeftChildIndex(parentIndex);
            }
        }

        void BubbleMinUp()
        {
            int parentIndex = 0;
            int leftIndex = GetLeftChildIndex(0);

            while (leftIndex < size)
            {
                int rightIndex = leftIndex + 1;
                int smallerIndex = leftIndex;

                if (rightIndex < size && heap[rightIndex].CompareTo(heap[leftIndex]) < 0)  // right is less
                {
                    smallerIndex = rightIndex;
                }

                if (heap[smallerIndex].CompareTo(heap[parentIndex]) > 0)
                {
                    break;
                }

                Swap(parentIndex, smallerIndex);
                parentIndex = smallerIndex;
                leftIndex = GetLeftChildIndex(parentIndex);
            }
        }

        void EnsureCapacity()
        {
            if (Capacity == 0)
            {
                heap = new T[2];
            }
            else if (Capacity == size)
            {
                Capacity *= 2;
                T[] temp = new T[Capacity];

                Array.Copy(heap, 0, temp, 0, size);
                heap = temp;
            }
        }

        int GetParentIndex(int index) => (index - 1) / 2;
      
        int GetLeftChildIndex(int index) => (index * 2) + 1;

        void Swap(int i, int j)
        {
            T temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;
        }
    }
}
