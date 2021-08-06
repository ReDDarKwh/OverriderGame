using Hsm;
using Scripts.States;

class GotoSuperState : SuperState
{

    public GotoSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var go = AddState(root.GetComponent<GotoState>(), "goto");
    }
}