
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Hacking
{
    public class DataGate : AbstractGate
    {
        public DataGateType dataGateType;
        public string name;
        public string dataType;
        public bool saveData;
        private object data;

        public override bool CanConnect(AbstractGate gate)
        {
            return gate is DataGate && ((DataGate)gate).dataType == dataType && base.CanConnect(gate);
        }

        public override bool CanBeConnectedTo(AbstractGate gate)
        {
            return gate is DataGate && ((DataGate)gate).dataType == dataType && base.CanBeConnectedTo(gate);
        }

        public IEnumerable<T> GetData<T>()
        {
            return parents.Where(x => ((DataGate)x).data != null)
            .SelectMany(x => (IEnumerable<T>)((DataGate)x).data);
        }

        public T GetSingleData<T>()
        {
            var val = parents.Where(x => ((DataGate)x).data != null).Select(x => ((DataGate)x).data).FirstOrDefault();
            return val == null ? default(T) : (T)Convert.ChangeType(val, typeof(T));
        }

        public T GetCurrentSingleData<T>()
        {
            var val = new HashSet<AbstractGate>() { this }.Where(x => ((DataGate)x).data != null).Select(x => ((DataGate)x).data).FirstOrDefault();
            return val == null ? default(T) : (T)Convert.ChangeType(val, typeof(T));
        }

        public object GetCurrentData()
        {
            return data;
        }

        internal void SetData(object data)
        {
            this.data = data;
            UpdateValue();
        }

        public override void UpdateValue()
        {
            BroadcastValue(true);
        }

        public enum DataGateType
        {
            Input,
            Output,
            Filter
        }
    }
}
