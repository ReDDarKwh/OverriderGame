using Hsm;
using Scripts.States;

class EnemyAlertSuperState : SuperState
{
    public EnemyAlertSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var chasing = AddState(root.GetComponent<ChasingState>(), "chasing");
        var go = new GotoSuperState(sm, root, "goto");
        var attacking = AddState(root.GetComponent<AttackingState>(), "attacking");

        chasing.AddHandler("done", go.sub);
        go.sub.AddHandler("isAtPosition", attacking);

        attacking.AddUpdateHandler(go.sub, EventRepo.TargetOutOfTargetList);
        attacking.AddHandler("targetOutOfAttackRange", go.sub);

    }
}