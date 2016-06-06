using System;
using System.Collections;
using System.Collections.Generic;


/**
 * A HashSet that uses generics.  Because Unity ships with a mono runtime that does
 * not implement .net 3.5, and writing scripts that do casting is super lame.
 * 
 * Uses an old school Hashtable for the internals.
 */
public class HashSet<T> : IEnumerable<T> {
	private Hashtable _Hashtable;

	/**
	 * Constructor
	 */
	public HashSet ()
	{
		_Hashtable = new Hashtable ();
	}

	/**
	 * Get the number of items in the set
	 */
	public int Count {
		get { return _Hashtable.Count; }
	}

	public void Clear() {
		_Hashtable.Clear();
	}

	/**
	 * Add an item to 
	 */
	public void Add(T item) {
		_Hashtable.Add(item, item);
	}

	public void Remove(T item) {
		_Hashtable.Remove (item);
	}

	public bool Contains(T item) {
		return _Hashtable.ContainsKey (item);
	}

	private class HashSetEnumerator : IEnumerator<T>
	{
		private IEnumerator _HashTableEnumerator;
		internal HashSetEnumerator(HashSet<T> set) {
			_HashTableEnumerator = set._Hashtable.GetEnumerator();
		}

		object IEnumerator.Current {
			get { return this.Current; }
		}

		public T Current {
			get { return (T)(((DictionaryEntry)_HashTableEnumerator.Current).Value); }
		}

		public bool MoveNext() {
			return _HashTableEnumerator.MoveNext ();
		}

		public void Reset() {
			_HashTableEnumerator.Reset ();
		}

		public void Dispose() {	}
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return this.GetEnumerator ();
	}

	public IEnumerator<T> GetEnumerator() {
		return new HashSetEnumerator (this);
	}

	override public string ToString() {
		string result = "Set {";
		int i = 0;
		foreach (T item in this) {
			if (i++ != 0) {
				result += ", ";
			}
			result += item.ToString ();
		}
		result += "}";
		return result;
	}
}