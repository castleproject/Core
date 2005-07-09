using System;

namespace Commons.Collections {

    /// <summary>
    /// Static utility methods for collections
    /// </summary>
    public class CollectionsUtil {

	public static System.Object PutElement(System.Collections.Hashtable hashTable, System.Object key, System.Object newValue) {
	    System.Object element = hashTable[key];
	    hashTable[key] = newValue;
	    return element;
	}

    }
}
