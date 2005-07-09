using System;
using System.Collections;

namespace Commons.Collections {

    /// <summary>
    ///Using The KeyedList
    ///Using the KeyedList is just like using a Hashtable, except that when you enumerate through the list, the entries are in the same order as when they were added to the list.  Believe me, I needed this capability!
    ///
    ///using System;
    ///using System.Collections;
    ///
    ///namespace KeyedListTest {
    ///    class ConsoleApp {
    ///	[STAThread]
    ///	static void Main(string[] args) {
    ///	    KeyedList k=new KeyedList();
    ///	    k.Add("One", 1);
    ///	    k.Add("Two", 2);
    ///	    k.Add("Three", 3);
    ///	    k.Add("Four", 4);
    ///	    k.Add("Five", 5);
    ///	    k.Add("Six", 6);
    ///	    k.Add("Seven", 7);
    ///	    k.Add("Eight", 8);
    ///	    k.Add("Nine", 9);
    ///	    k.Add("Ten", 10);
    ///
    ///	    foreach(DictionaryEntry entry in k) {
    ///		Console.WriteLine(entry.Key+": "+entry.Value.ToString());
    ///	    }
    ///	}
    ///    }
    ///}
    /// </summary>
    [Serializable]
    public class KeyedList : ICollection, IDictionary, IEnumerable, IOrderedDictionary {
	private Hashtable objectTable = new Hashtable ();
	private ArrayList objectList = new ArrayList ();

	public void Add (object key, object value) {
	    objectTable.Add (key, value);
	    objectList.Add (new DictionaryEntry (key, value));
	}

	public void Clear () {
	    objectTable.Clear ();
	    objectList.Clear ();
	}

	public bool Contains (object key) {
	    return objectTable.Contains (key);
	}

	public void CopyTo (Array array, int idx) {
	    objectTable.CopyTo (array, idx);
	}

	public void Insert (int idx, object key, object value) {
	    if (idx > Count)
		throw new ArgumentOutOfRangeException ("index");

	    objectTable.Add (key, value);
	    objectList.Insert (idx, new DictionaryEntry (key, value));
	}

	public void Remove (object key) {
	    objectTable.Remove (key);
	    objectList.RemoveAt (IndexOf (key));
	}

	public void RemoveAt (int idx) {
	    if (idx >= Count)
		throw new ArgumentOutOfRangeException ("index");

	    objectTable.Remove ( ((DictionaryEntry)objectList[idx]).Key );
	    objectList.RemoveAt (idx);
	}

	IDictionaryEnumerator IDictionary.GetEnumerator () {
	    return new KeyedListEnumerator (objectList);
	}

	IEnumerator IEnumerable.GetEnumerator () {
	    return new KeyedListEnumerator (objectList);
	}

	public int Count {
	    get { return objectList.Count; }
	}

	public bool IsFixedSize {
	    get { return false; }
	}

	public bool IsReadOnly {
	    get { return false; }
	}

	public bool IsSynchronized {
	    get { return false; }
	}

	public object this[int idx] {
	    get { return ((DictionaryEntry) objectList[idx]).Value; }
	    set {
		if (idx < 0 || idx >= Count)
		    throw new ArgumentOutOfRangeException ("index");

		object key = ((DictionaryEntry) objectList[idx]).Key;
		objectList[idx] = new DictionaryEntry (key, value);
		objectTable[key] = value;
	    }
	}

	public object this[object key] {
	    get { return objectTable[key]; }
	    set {
		if (objectTable.Contains (key)) {
		    objectTable[key] = value;
		    objectTable[IndexOf (key)] = new DictionaryEntry (key, value);
		    return;
		}
		Add (key, value);
	    }
	}

	public ICollection Keys {
	    get { 
		ArrayList retList = new ArrayList ();
		for (int i = 0; i < objectList.Count; i++) {
		    retList.Add ( ((DictionaryEntry)objectList[i]).Key );
		}
		return retList;
	    }
	}

	public ICollection Values {
	    get {
		ArrayList retList = new ArrayList ();
		for (int i = 0; i < objectList.Count; i++) {
		    retList.Add ( ((DictionaryEntry)objectList[i]).Value );
		}
		return retList;
	    }
	}
 
	public object SyncRoot {
	    get { return this; }
	} 

	private int IndexOf (object key) {
	    for (int i = 0; i < objectList.Count; i++) {
		if (((DictionaryEntry) objectList[i]).Key.Equals (key)) {
		    return i;
		}
	    }
	    return -1;
	}
    }
}
