using Hsm;

class EnemyAliveSuperState : SuperState
{
    public EnemyAliveSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var perceptive = new PerceptiveSuperState(sm, root, "perceptive");
        var enemyAlert = new EnemyAlertSuperState(sm, root, "enemyAlive");

        perceptive.sub.AddHandler("isAlert", perceptive.sub);
        enemyAlert.sub.AddHandler("update", perceptive.sub, EventRepo.targetOutOfTargetList);
    }
}