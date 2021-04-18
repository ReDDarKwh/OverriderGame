using Hsm;
using Scripts.States;

class PerceptiveSuperState : SuperState
{

    public PerceptiveSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var dead = AddState(root.GetComponent<EmptyState>(), "perceptive");
    }
}