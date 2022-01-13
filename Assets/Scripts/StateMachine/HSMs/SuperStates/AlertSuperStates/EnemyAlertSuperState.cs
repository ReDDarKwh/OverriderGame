using System;
using System.Collections.Generic;
using Hsm;
using Scripts.States;
using UnityEngine;

class EnemyAlertSuperState : SuperState
{
    public EnemyAlertSuperState(StateMachine sm, HSM root, string name) : base(sm, root, name)
    {
    }

    public override void Init(StateMachine sm, HSM root)
    {
        var alerting = AddState(root.GetComponent<AlertingState>(), "alerting");
        var chasing = AddState(root.GetComponent<ChasingState>(), "chasing");
        var go = new GotoSuperState(sm, root, "goto");
        var attacking = AddState(root.GetComponent<AttackingState>(), "attacking");

        alerting.AddEnterHandler(chasing, null);
        chasing.AddHandler("done", go.sub);
        go.sub.AddHandler("isAtPosition", attacking);

        attacking.AddHandler("targetOutOfAttackRange", go.sub, TransitionKind.External, SetUpChase());
    }

    private static Action<EventData> SetUpChase()
    {
        return (EventData data) =>
        {
            var memory = HSM.GetRoot(data).memory;
            HSM.SetUpGoto(memory, null, memory.Get<GameObject>("target", false)?.transform, "chasing", true);
        };
    }
}