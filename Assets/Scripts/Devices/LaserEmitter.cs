using System;
using Scripts.Actions;
using Scripts.Hacking;
using UnityEngine;

public class LaserEmitter : Scripts.Actions.Action
{
    public LayerMask collisionLayers;
    public LayerMask baseCollisionLayers;
    public float laserDepth;
    public LineRenderer line;
    public Transform end;
    public ParticleSystem endParticleSystem;
    public Transform muzzle;
    public string filterDataInputName;

    private DataGate filterDataInput;

    internal override void OnStart()
    {
        filterDataInput = new DataGate(true) { name = filterDataInputName, dataGateType = DataGate.DataGateType.Input };
        dataGates.Add(filterDataInput);

        line.positionCount = 2;
        line.SetPosition(0, muzzle.position);
        line.SetPosition(1, muzzle.position);

        ToggleLaser(outputGate.currentValue);
        outputGate.ValueHasChanged += (object sender, EventArgs e) =>
        {
            ToggleLaser(outputGate.currentValue);
        };
    }

    private void ToggleLaser(bool currentValue)
    {
        if (outputGate.currentValue)
        {
            line.enabled = true;
            end.gameObject.SetActive(true);
            endParticleSystem.gameObject.SetActive(true);
        }
        else
        {
            line.enabled = false;
            end.gameObject.SetActive(false);
            endParticleSystem.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (outputGate.currentValue)
        {

            var mask = filterDataInput.GetSingleData<int>();
            var cMask = mask == 0 ? (int)collisionLayers : mask;

            var hit = Physics2D.Raycast(muzzle.position, Vector3.right, 30, cMask | baseCollisionLayers);
            end.position = hit.point;
            line.SetPosition(1, new Vector3(hit.point.x, hit.point.y, laserDepth));
        }
    }
}
