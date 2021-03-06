using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using Scripts.Hacking;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class DeviceUI : MonoBehaviour
{
    public Device device;
    public HackUI actionsToolbar;
    public HackUI actionsContainer;
    public GameObject deviceCircle;
    public TextMeshProUGUI title;
    public GameObject AccessDeniedDisplay;
    public TextMeshProUGUI AccessDeniedText;
    public SpriteRenderer selectionCircle;
    public float selectionRadius;

    internal bool disableLine;
    internal bool minimized;

    internal bool selected;
    private bool canBeSelected;

    private LineFactory lineFactory;
    private Line line;
    private Transform mousePos;

    void Start()
    {
        UpdateVisible();

        if (!disableLine)
        {
            lineFactory = GameObject.FindGameObjectWithTag("LineFactory").GetComponent<LineFactory>();
            line = lineFactory.GetLine(Vector2.zero, Vector2.zero, 0.02f, Color.green);
        }
        else
        {
            deviceCircle.SetActive(false);
        }

        mousePos = GameObject.FindGameObjectWithTag("MousePos").transform;
    }

    void OnDestroy()
    {
        if (line != null)
        {
            line.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if ((mousePos.position - transform.position).magnitude < selectionRadius && !selected)
        {
            selectionCircle.enabled = true;
            if (Input.GetMouseButtonDown(0))
            {
                ToggleSelected();
            }
        }
        else
        {
            selectionCircle.enabled = false;
        }

        if (line != null)
        {
            if (selected)
            {
                line.gameObject.SetActive(true);
                line.start = transform.position;
                line.end = device.actionDisplayContainer.position +
                new Vector3(device.actionDisplayContainer.rect.size.x / 2, -device.actionDisplayContainer.rect.size.y / 2);
            }
            else
            {
                line.gameObject.SetActive(false);
            }
        }

        UpdateAccessLevelUI();
    }

    public void ToggleSelected()
    {
        selected = !selected;
        UpdateVisible();
    }

    private void UpdateVisible()
    {
        if (selected)
        {
            actionsToolbar.Show();
            actionsContainer.Show();
        }
        else
        {
            actionsToolbar.Hide();
            actionsContainer.Hide();
        }
    }

    internal void UpdateAccessLevelUI()
    {
        if (device.playerCanAccess)
        {
            AccessDeniedDisplay.SetActive(false);
            title.SetText(device.deviceName);
        }
        else if (selected)
        {
            AccessDeniedDisplay.SetActive(true);
            title.SetText(device.deviceName + " (Secured)");
            AccessDeniedText.SetText($"Level {device.accessLevel}");
        }
    }

}
