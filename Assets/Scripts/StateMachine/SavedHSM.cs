
using System.Collections.Generic;

[System.Serializable]
public class SavedHSM {
    public List<string> path;
    public List<SavedMemory> memory;
}

[System.Serializable]
public class SavedMemory {
    public string key;
    public object value;
    public string uniqueId;
    public MemoryType memType;
}

[System.Serializable]
public class SavedVec {
    public float x;
    public float y;
    public float z;
}




