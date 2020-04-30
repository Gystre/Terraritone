using System;
using System.Collections;
using System.Collections.Generic;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    //remove the value at the top of the heap and sort it back down
    //returns the removed value
    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;

        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);

        return firstItem;
    }

    //use this if find node with lower fcost and need to update the node
    public void UpdateItem(T item)
    {
        SortUp(item);
        //would call SortDown() but will never need to decrease the priority of a node in pathfinding, but might in certain situations
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1; //get the index of left child with (n * 2) + 1
            int childIndexRight = item.HeapIndex * 2 + 2; //get right child with (n * 2) + 2
            int swapIndex = 0;

            //figure out which of the two children have a higher priority and set the swapIndex to that
            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) //if left has lower priority than right
                    {
                        swapIndex = childIndexRight;
                    }
                }

                //check if parent has higher priority than it's child
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    //item is less than parent and greater than children, successfully sorted
                    return;
                }
            }
            else
            {
                //doesn't have children
                return;
            }
        }
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2; //get the index of the parent node with (n-1)/2
        while (true)
        {
            T parentItem = items[parentIndex];

            //higher priority = 1, same priority = 0, lower priority = -1
            //if item has a higher priority than parent item
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T a, T b)
    {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;

        int tempIndex = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = tempIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
