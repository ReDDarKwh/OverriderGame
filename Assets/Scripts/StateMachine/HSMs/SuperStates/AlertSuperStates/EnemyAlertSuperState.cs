using Hsm;
using Scripts.States;

class EnemyAlertSuperState : SuperState
{
    public EnemyAlertSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var alert = AddState(root.GetComponent<EmptyState>(), "alert");
    }
}