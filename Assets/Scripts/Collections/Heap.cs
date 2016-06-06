using System;
using System.Collections;
using System.Collections.Generic;

/**
 * A basic generic Heap.  Objects can be added with Push, and removed with Pop.
 * The top value can be checked with Peek(),
 * The heap invariant (ordering) is maintained via abstract _Compare.
 */
abstract public class Heap<T> 
where T : IComparable<T> {
  private List<T> _items;

  public int Count {
    get { return _items.Count; }
  }

  public Heap() {
    _items = new List<T>();
  }

  public Heap(int capacity) {
    _items = new List<T>(capacity);
  }

  public T Peek() {
    if (_items.Count == 0) {
      throw new InvalidOperationException();
    }

    return _items[0];
  }

  public void Push(T item) {
    _items.Add(item);
    _SiftDown(0, _items.Count - 1);
  }

  public T Pop() {
    if (_items.Count == 0) {
      throw new InvalidOperationException();
    }

    T lastelt = _items[_items.Count - 1];
    _items.RemoveAt(_items.Count - 1);

    T returnitem;

    if (_items.Count > 0) {
      returnitem = _items[0];
      _items[0] = lastelt;
      _SiftUp(0);
    } else {
      returnitem = lastelt;
    }

    return returnitem;
  }

  abstract protected int _Compare(T a, T b);

  private void _SiftUp(int pos) {
    int endpos = _items.Count;
    int startpos = pos;
    T newitem = _items[pos];

    int childpos = (2 * pos) + 1;
    while (childpos < endpos) {
      int rightpos = childpos + 1;
      if ((rightpos < endpos) &&
          !(_Compare(_items[childpos], _items[rightpos]) < 0)) {
        childpos = rightpos;
      }

      _items[pos] = _items[childpos];
      pos = childpos;
      childpos = (2 * pos) + 1;
    }

    _items[pos] = newitem;
    _SiftDown(startpos, pos);
  }

  private void _SiftDown(int startpos, int pos) {
    T newitem = _items[pos];
    while (pos > startpos) {
      int parentpos = (pos - 1) >> 1;
      T parent = _items[parentpos];
      if (_Compare(newitem, parent) < 0) {
        _items[pos] = parent;
        pos = parentpos;
        continue;
      }
      break;
    }
    _items[pos] = newitem;
  }
}

/**
 * A Heap that maintains the smallest value for Peek() and first Pop()
 */
public class MinHeap<T> : Heap<T>
where T : IComparable<T> {
  override protected int _Compare(T a, T b) {
    return a.CompareTo(b);
  }
}

/**
 * A Heap that maintains the greatest value for Peek() and first Pop()
 */
public class MaxHeap<T> : Heap<T>
where T : IComparable<T> {
  override protected int _Compare(T a, T b) {
    return -a.CompareTo(b);
  }
}
