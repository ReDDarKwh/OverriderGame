namespace Scripts.Actions
{
    [System.Serializable]
    public struct Preconnection
    {
        public Device fromDevice;
        public Device toDevice;
        public Action fromAction;
        public Action toAction;
        public string fromNodeName;
        public string toNodeName;
    }
}