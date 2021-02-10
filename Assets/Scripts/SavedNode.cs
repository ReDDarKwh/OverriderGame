using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SavedNode
{
    public string id;
    public int gateType;
    public float[] pos;
    public string[] connections;
    public object data;
}
