using Hsm;
using Scripts.States;

class InvestigatingObjectSuperState : SuperState
{

    public InvestigatingObjectSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var start = AddState(root.GetComponent<IntriguedState>(), "start");
        var interacting = new InteractSuperState(sm, root, "interacting");
        start.AddUpdateHandler(interacting.sub, EventRepo.Timeout(1));
    }
}