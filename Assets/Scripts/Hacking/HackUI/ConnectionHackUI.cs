namespace Scripts.Hacking
{
    public class ConnectionHackUI : HackUI
    {
        public Connection connection;
        public override void Hide()
        {
            connection.lineRenderer.enabled = false;
        }

        public override void Show()
        {
            connection.lineRenderer.enabled = true;
        }
    }
}
