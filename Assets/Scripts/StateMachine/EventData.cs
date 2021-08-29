

using System.Collections.Generic;

public class EventData : Dictionary<string, object>
{
    public HSM Root {get{
        return HSM.GetRoot(this);
    }}   
    public StateMachineMemory Memory {get{
        return HSM.GetRoot(this).memory;
    }}    
    public T GetVar<T>(string name){
        return HSM.GetVar<T>(name, this);
    }

    public EventData() : base()
    {
    }

    public EventData(Dictionary<string, object> from) : base()
    {
        foreach(var keyValue in from){
            this.Add(keyValue.Key, keyValue.Value);
        }
    }
}