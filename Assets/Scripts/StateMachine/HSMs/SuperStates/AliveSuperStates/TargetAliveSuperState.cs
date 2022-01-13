using Hsm;

class TargetAliveSuperState : SuperState
{
    public TargetAliveSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var perceptive = new PerceptiveSuperState(sm, root, "perceptive").sub;
        var alert = new TargetAlertSuperState(sm, root, "alert").sub;
        var fleeing = new FleeingSuperState(sm, root, "fleeing").sub;

        perceptive.AddHandler("isAlert", alert);
        perceptive.AddHandler("alarmTriggered", fleeing);
    }
}