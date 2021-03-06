namespace Scripts.Hacking
{
    public class NodeHackUI : HackUI
    {
        public Node node;

        public override void Hide()
        {
            node.Hide();
        }

        public override void Show()
        {
            node.Show();
        }
    }
}
