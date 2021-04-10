using Hsm;
using Scripts.States;

public class StateAdapter
{
    private State state;

    public StateAdapter(AbstractState state, string name)
    {
        this.state = new State(name);

        this.state.OnEnter((source, target, data) =>
        {
            state.isRunning = true;
            state.StateEnter();
            this.state.owner.HandleEvent("stateEnter");
        });
        this.state.OnExit((source, target, data) =>
        {
            state.isRunning = false;
            state.StateExit();
        });
    }

    public State GetState()
    {
        return state;
    }
}
