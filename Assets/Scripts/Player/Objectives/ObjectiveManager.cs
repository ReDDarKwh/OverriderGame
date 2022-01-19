using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveManager : MonoBehaviour
{
    public GameObject objectiveUIPrefab;
    public Transform objectiveUIContainer;
    public List<Objective> objectives = new List<Objective>();
    private Dictionary<string, ObjectiveUI> objectiveUIPerName = new Dictionary<string, ObjectiveUI>();

    private int currentObjective = -1;

    void Start()
    {
        InitNextObjective();
    }
    void InitNextObjective()
    {
        currentObjective++;

        if (currentObjective <= objectives.Count - 1)
        {
            InitObjective(objectives[currentObjective]);
        }
    }

    void InitObjective(Objective objective)
    {
        AddObjectiveUI(objective.name);

        EventHandler handler = null;

        switch (objective.objectiveType)
        {
            case ObjectiveType.Kill:

                handler = (object sender, EventArgs e) =>
                    {
                        CompleteObjective(objective);
                        objective.target.hasDied -= handler;
                    };
                objective.target.hasDied += handler;
                break;
            case ObjectiveType.GoToPosition:

                UnityAction gotoPosHandler = null;   
                gotoPosHandler = () =>
                    {
                        CompleteObjective(objective);
                        objective.triggerZone.playerHasEntered.RemoveListener(gotoPosHandler);
                    };
                objective.triggerZone.playerHasEntered.AddListener(gotoPosHandler);
                break;
            
             case ObjectiveType.Interact:
                UnityAction<Creature> interactHandler = null;       
                interactHandler = (Creature creature) =>
                    {
                        if(creature == null){
                            CompleteObjective(objective);
                            objective.interactable.onUsedWithCreature.RemoveListener(interactHandler);
                        }
                    };
                objective.interactable.onUsedWithCreature.AddListener(interactHandler);
                break;
        }
    }

    private void CompleteObjective(Objective objective)
    {
        CompleteObjectiveUI(objective.name);
        objective.onComplete.Invoke();
        InitNextObjective();
    }

    public void AddObjectiveUI(string name)
    {
        var inst = Instantiate(objectiveUIPrefab, objectiveUIContainer);
        var objective = inst.GetComponent<ObjectiveUI>();
        objective.objectiveName = name;
        objectiveUIPerName.Add(name, objective);
    }

    public void CompleteObjectiveUI(string name)
    {
        objectiveUIPerName[name].ObjectiveDone();
    }
}
