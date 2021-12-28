using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using Scripts.Hacking;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using Network = Scripts.Hacking.Network;
using UnityEngine.UI;

public class DeviceUI : MonoBehaviour
{
    public Device device;
    public HackUI actionsToolbar;
    public HackUI actionsContainer;
    public GameObject deviceCircle;
    public TextMeshProUGUI title;
    public GameObject AccessDeniedDisplay;
    public TextMeshProUGUI AccessDeniedText;
    public Image selectionCircle;
    public float selectionRadius;
    public int baseSortingOrder = 60;
    public Canvas actionDisplayContainerCanvas;
    public Transform actionWindow;
    public Canvas actionDisplayCanvas;
    public Canvas accessDeniedCanvas;
    public Button closeButton;
    public Button anchorButton;
    public bool disableLine;
    public bool disableClose;
    public bool disableAnchor;
    public bool UIDestroyable;

    internal bool minimized;
    internal bool selected;
    internal bool isAnchored;

    private bool canBeSelected;
    private LineFactory lineFactory;
    private Line line;
    private Transform mousePos;
    private Vector3 diff;
    private bool isMoving;
    private Vector3 pos;
    private bool isHovered;

    void Start()
    {

        if (!device.initiated)
        {
            device.Init();
        }

        mousePos = GameObject.FindGameObjectWithTag("MousePos").transform;
        selected = disableClose;
        UpdateVisible();

        pos = actionWindow.transform.position;

        if (device.parentDevice != null)
        {
            actionWindow.gameObject.SetActive(false);
        }

        if (!disableLine)
        {
            lineFactory = GameObject.FindGameObjectWithTag("LineFactory").GetComponent<LineFactory>();
            line = lineFactory.GetLine(Vector2.zero, Vector2.zero, 0.02f, Color.green);
        }
        else
        {
            deviceCircle.SetActive(false);
        }

        if (disableClose && closeButton)
        {
            closeButton.gameObject.SetActive(false);
        }

        if (disableAnchor && anchorButton)
        {
            anchorButton.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (line != null)
        {
            line.gameObject.SetActive(false);
        }
    }

    void OnDisable(){
        if (line != null)
        {
            line.gameObject.SetActive(false);
        }
    }

    public void OnHover()
    {
        isHovered = true;
    }

    public void OnHoverExit()
    {
        isHovered = false;
    }

    void Update()
    {
        if (isHovered && !disableClose)
        {
            selectionCircle.enabled = true;
            if (Input.GetMouseButtonDown(1))
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

            // Move actionWindow above others;
            SetIsMoving(true);
            SetIsMoving(false);
        }
        else
        {
            actionsToolbar.Hide();
            actionsContainer.Hide();
        }
    }

    void LateUpdate()
    {
        if (isMoving)
        {
            pos = mousePos.transform.position + diff;
            actionWindow.position = pos;
        }
        else
        {
            if (isAnchored)
            {
                actionWindow.position = pos;
            }
        }
    }

    public void ToggleIsAnchored()
    {
        isAnchored = !isAnchored;

        if (isAnchored)
        {
            pos = actionWindow.transform.position;
        }
    }

    public void SetIsMoving(bool isMoving)
    {
        this.isMoving = isMoving;
        if (isMoving)
        {
            diff = actionWindow.position - mousePos.transform.position;

            if (Network.Instance.lastDeviceMoved != this)
            {
                Network.Instance.baseDeviceSortingOrder += 4;

                actionDisplayCanvas.sortingOrder = baseSortingOrder + Network.Instance.baseDeviceSortingOrder;
                actionDisplayContainerCanvas.sortingOrder = baseSortingOrder + Network.Instance.baseDeviceSortingOrder;
                accessDeniedCanvas.sortingOrder = baseSortingOrder + 2 + Network.Instance.baseDeviceSortingOrder;

                foreach (var action in device.nodesPerAction)
                {
                    foreach (var node in action.Value.Values)
                    {
                        node.nodeCanvas.overrideSorting = true;
                        node.nodeCanvas.sortingOrder = baseSortingOrder + 1 + Network.Instance.baseDeviceSortingOrder;
                    }
                }

                Network.Instance.lastDeviceMoved = this;
            }
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
