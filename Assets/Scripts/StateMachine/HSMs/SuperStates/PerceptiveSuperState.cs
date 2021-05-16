using Hsm;
using Scripts.States;

class PerceptiveSuperState : SuperState
{

    public PerceptiveSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var idle = AddState(root.GetComponent<EmptyState>(), "idle");
        var unsure = AddState(root.GetComponent<UnsureState>(), "unsure");
        var curious = AddState(root.GetComponent<CuriousState>(), "curious");



        idle.AddHandler("update", unsure, EventRepo.targetInTargetList);
        unsure.AddHandler("update", curious, EventRepo.targetOutOfTargetList);

    }
}