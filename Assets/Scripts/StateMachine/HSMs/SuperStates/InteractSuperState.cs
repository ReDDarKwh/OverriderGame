using Hsm;
using Scripts.States;

class InteractSuperState : SuperState
{
    public InteractSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var start = AddState(root.GetComponent<InstantState>(), "start");
        var go = new GotoSuperState(sm, root, "goto");

        start.AddInstantHandler(go.sub, null, (data) => {});
    }
}