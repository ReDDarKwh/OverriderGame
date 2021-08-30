
using System.Collections;
using System.Collections.Generic;
using Scripts.States;
using UnityEngine;


namespace Scripts.States
{
    public class InteractingState : AbstractState
    {
        public Interactor interactor;
        private Coroutine coroutine;

        public override void StateEnter()
        {
            var key = memory.Get<string>("interactableName");
            var interactable = memory.Get<GameObject>(key == null? "interactionObject" : key).GetComponent<Interactable>();
            coroutine = StartCoroutine(Interact(interactable.InteractionTime, interactable));
        }

        public override void StateExit()
        {
            if(coroutine != null){
                StopCoroutine(coroutine);
            }
        }

        public override void StateUpdate()
        {
        }

        private IEnumerator Interact(float interactionTime, Interactable interactable){

            yield return new WaitForSeconds(interactionTime);
            interactor.InteractWith(interactable);
            root.TriggerEvent("interactionDone");
        }

    }
}