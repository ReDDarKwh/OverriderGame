using Hsm;

class TargetAliveSuperState : SuperState
{
    public TargetAliveSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var perceptive = new PerceptiveSuperState(sm, root, "perceptive");
        var alert = new TargetAlertSuperState(sm, root, "alert");

        perceptive.sub.AddHandler("isAlert", alert.sub);
    }
}