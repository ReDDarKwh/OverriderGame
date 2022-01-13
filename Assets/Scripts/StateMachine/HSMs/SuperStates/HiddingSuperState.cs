using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

class HiddingSuperState : SuperState
{
    public HiddingSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var creature = root.GetComponent<Creature>();
        var hiddingGoto = new GotoSuperState(sm, root, "goingToHideSpot").sub;
        var hidding = AddState(root.GetComponent<EmptyState>(), "hiding");

        hiddingGoto.AddHandler("isAtPosition", hidding, (EventData data) => {
            creature.headDir = -creature.headDir;
        });
    }
}