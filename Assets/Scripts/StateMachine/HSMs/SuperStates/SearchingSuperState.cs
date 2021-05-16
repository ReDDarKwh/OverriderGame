using Hsm;
using Scripts.States;

class SearchingSuperState : SuperState
{

    public SearchingSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {

    }
}