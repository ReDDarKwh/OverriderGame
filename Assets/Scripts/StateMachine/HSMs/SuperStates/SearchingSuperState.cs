using Hsm;
using Scripts.States;

class SearchingSuperState : SuperState
{

    public SearchingSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var start = AddState(root.GetComponent<SearchingState>(), "searchStart");
        var go = new GotoSuperState(sm, root, "goto");
        var lookAround = AddState(root.GetComponent<LookAroundState>(), "lookaround");

        start.AddHandler("done", go.sub);
        go.sub.AddHandler("isAtPosition", lookAround);
    }
}