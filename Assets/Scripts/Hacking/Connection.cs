using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;
using Vectrosity;
using System.Linq;
using Network = Scripts.Hacking.Network;

public class Connection : MonoBehaviour
{
    [System.Serializable]
    public struct electricHumSetting
    {
        public SoundPreset electricHumSound;
        public bool changePitch;
        internal AudioSource audioSource;
    }
    
    public NoodlePath noodlePath;
    public float zoom = 1;
    public float divMultiplier;
    public float lineWidth = 1;
    public int lineDepth;
    public float lineMaxWeldDistance;
    public Color removeColor;
    public Material lineMaterialOn;
    public Material lineMaterialOff;
    public electricHumSetting[] electricHumSounds;
    public SoundPreset connectedSound;
    public SoundPreset connectedStartSound;
    public SoundPreset deconnectedSound;
    public Vector2 electricHumPitchMinMax;
    public Vector2 electricHumDistanceMinMax;
    public bool soundOn;
    public ParticleSystem sparksEffect;
    public ParticleSystem connectionEffect;
    public Animator animator;
    public Color lineColor;
    public float selfConnectRadiusY;
    public float selfConnectRadiusX;
    public int selfConnectPointCount;
    public float wavyLineModifier;
    [Range(0.0f, 2.0f)]
    public float wavyLineFrequency;
    public float wavyLineAmplitude;
    public float wavyLineOffsetY;
    public float waveStrength;
    public float selectedWaveStrength;
    public float lineSelectionStrength;

    internal VectorLine line;
    internal Node end;
    internal Node start;
    internal bool selectedForDelete;
    internal bool reversed;
    internal bool isSelectedForce;
    internal bool isSelectedForDeleteForce;
    
    private Transform lastStartPos;
    private Transform lastEndPos;
    private IEnumerable<electricHumSetting> electricHum;
    private bool hasAnimationPlayed;
    private Camera cam;
    private bool isConnected;

    // Start is called before the first frame update
    void Start()
    {
        //lineColor = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);
        line = new VectorLine("ConnectionLine", new List<Vector3>(), lineWidth, lineDepth);
        line.joins = Joins.Weld;
        line.lineType = LineType.Continuous;
        line.maxWeldDistance = lineMaxWeldDistance;
        if (soundOn)
        {
            connectedStartSound.Play(transform.position);
            electricHum = electricHumSounds.Select(x =>
                new electricHumSetting
                {
                    audioSource = x.electricHumSound.Play(end.transform.position),
                    electricHumSound = x.electricHumSound,
                    changePitch = x.changePitch
                }
            ).ToList();
        }

        cam = Camera.main;
    }

    public void Connected()
    {
        if (soundOn)
        {
            SoundManager.Instance.FadeOut(electricHum?.Select(x => x.audioSource).ToList());
            electricHum = null;
            connectedSound.Play(lastEndPos.position);
            connectionEffect.Play();
            animator.SetTrigger("Connect");
        }

        sparksEffect.Stop();
        isConnected = true;
    }

    public void PlayDisconnectionSound()
    {
        SoundManager.Instance.FadeOut(electricHum?.Select(x => x.audioSource));
        electricHum = null;
        if(deconnectedSound){
            deconnectedSound.Play(lastEndPos.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var startPos = GetPos(start);
        var endPos = GetPos(end);
        var isSelected = IsSelected();

        if (electricHum != null)
        {
            foreach (var sounds in electricHum)
            {
                sounds.audioSource.transform.position = endPos.position;
                if (sounds.changePitch)
                {
                    sounds.audioSource.pitch = sounds.electricHumSound.pitch + Mathf.Lerp(
                        electricHumPitchMinMax.x, electricHumPitchMinMax.y,
                        Mathf.InverseLerp(
                            electricHumDistanceMinMax.x,
                            electricHumDistanceMinMax.y,
                            (startPos.position - endPos.position).magnitude
                        )
                    );
                }
            }
        }

        transform.position = reversed ? startPos.position : endPos.position;

        SetLineOn(GetRootNode().gate.currentValue);

        UpdateSelection(isSelected);

        var baseLineWidth = (isSelected ? lineWidth * lineSelectionStrength : lineWidth);

        if (startPos != endPos)
        {
            DrawNoodle(noodlePath, new List<Vector3>
            {
                startPos.position,
                endPos.position
            });
            UpdateLineColor();
        }
        else if (start.deviceUI?.device.playerCanAccess ?? true)
        {
            var p = new List<Vector3>();
            for (var i = 0; i < selfConnectPointCount; i++)
            {
                p.Add(Vector3.zero);
            }

            line.points3 = p;
            line.MakeEllipse(startPos.position + new Vector3(0, selfConnectRadiusY, 0), selfConnectRadiusX, selfConnectRadiusY);
            UpdateLineColor();
        }
        else
        {
            line.points3.Clear();
        }

        if (GetRootNode().gate.currentValue)
        {
            if (!hasAnimationPlayed)
            {
                animator.SetTrigger("On");
                wavyLineAmplitude = 0;
                hasAnimationPlayed = true;
            }

            if (line.points3.Any())
            {
                line.SetWidths(line.points3.Skip(1).Select((x, i) =>
                {
                    var baseWidth = line.GetWidth(i);
                    var wave = Mathf.Max(0, Mathf.Sin(Time.unscaledTime * wavyLineModifier + i * wavyLineFrequency) * wavyLineAmplitude - wavyLineOffsetY);
                    return Screen.height / ((baseLineWidth - wave / ((isSelected ? selectedWaveStrength : waveStrength)))) / cam.orthographicSize;
                }).ToList());
            }

        }
        else
        {
            line.SetWidth(Screen.height / baseLineWidth / cam.orthographicSize);
            hasAnimationPlayed = false;
        }

        line.Draw3D();

        lastStartPos = startPos;
        lastEndPos = endPos;
    }

    private void UpdateLineColor()
    {
        if (start.rightClickDown || end.rightClickDown || selectedForDelete || isSelectedForDeleteForce)
        {
            line.color = removeColor;
        }
        else
        {
            line.color = (start.accessible) ? lineColor : GetAccessDeniedColor();
        }
    }

    private Color GetAccessDeniedColor()
    {
        var c = Network.Instance.accessLevels[start.deviceUI.device.accessLevel];
        return new Color(c.r, c.g, c.b, lineColor.a);
    }

    private Node GetRootNode(){
        return start.gate == null ? end: start;
    }

    private void UpdateSelection(bool isSelected)
    {
        if (isSelected && (start.deviceUI?.device.playerCanAccess ?? true))
        {
            if (selectedForDelete && Input.GetMouseButtonUp(1))
            {
                PlayDisconnectionSound();
                start.Disconnect(end);
            }

            if (Input.GetMouseButton(1))
            {
                selectedForDelete = true;
            }
            
        } else {
            selectedForDelete = false;
        }
    }

    private bool IsSelected(){
        return ((Network.Instance.isConnectionSelectionEnabled && !Network.Instance.isNodeDragStarted && line.Selected(Input.mousePosition)) || isSelectedForce) && isConnected;
    }

    void OnDestroy()
    {
        if (electricHum != null)
        {
            SoundManager.Instance.FadeOut(electricHum.Select(x => x.audioSource));
        }
        VectorLine.Destroy(ref line);
    }

    private Transform GetPos(Node node)
    {
        if (node == null)
        {
            Debug.Log("");
        }

        return node.deviceUI == null || node.deviceUI.selected ? node.transform : node.deviceUI.transform;
    }

    private Vector2 CalculateBezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t, uu = u * u;
        float uuu = uu * u, ttt = tt * t;
        return new Vector2(
            (uuu * p0.x) + (3 * uu * t * p1.x) + (3 * u * tt * p2.x) + (ttt * p3.x),
            (uuu * p0.y) + (3 * uu * t * p1.y) + (3 * u * tt * p2.y) + (ttt * p3.y)
        );
    }

    internal void SetLineOn(bool currentValue)
    {
        if (line != null)
        {
            line.material = currentValue? lineMaterialOn : lineMaterialOff;
        }
    }

    /// <summary> Draw a bezier from output to input in grid coordinates </summary>
    public void DrawNoodle(NoodlePath path, List<Vector3> gridPoints)
    {

        int length = gridPoints.Count;
        switch (path)
        {
            case NoodlePath.Curvy:
                Vector2 outputTangent = Vector2.right;
                for (int i = 0; i < length - 1; i++)
                {
                    Vector2 inputTangent;
                    // Cached most variables that repeat themselves here to avoid so many indexer calls :p
                    Vector2 point_a = gridPoints[i];
                    Vector2 point_b = gridPoints[i + 1];
                    float dist_ab = Vector2.Distance(point_a, point_b);
                    if (i == 0) outputTangent = zoom * dist_ab * 0.01f * Vector2.right;
                    if (i < length - 2)
                    {
                        Vector2 point_c = gridPoints[i + 2];
                        Vector2 ab = (point_b - point_a).normalized;
                        Vector2 cb = (point_b - point_c).normalized;
                        Vector2 ac = (point_c - point_a).normalized;
                        Vector2 p = (ab + cb) * 0.5f;
                        float tangentLength = (dist_ab + Vector2.Distance(point_b, point_c)) * 0.005f * zoom;
                        float side = ((ac.x * (point_b.y - point_a.y)) - (ac.y * (point_b.x - point_a.x)));

                        p = tangentLength * Mathf.Sign(side) * new Vector2(-p.y, p.x);
                        inputTangent = p;
                    }
                    else
                    {
                        inputTangent = zoom * dist_ab * 0.01f * Vector2.left;
                    }

                    // Calculates the tangents for the bezier's curves.
                    float zoomCoef = 50 / zoom;
                    Vector2 tangent_a = point_a + outputTangent * zoomCoef;
                    Vector2 tangent_b = point_b + inputTangent * zoomCoef;
                    // Hover effect.
                    int division = Mathf.RoundToInt(divMultiplier * dist_ab) + 3;
                    // Coloring and bezier drawing.
                    Vector2 bezierPrevious = point_a;

                    var points = new Vector3[division];
                    for (int j = 1; j <= division; ++j)
                    {
                        Vector2 bezierNext = CalculateBezierPoint(point_a, tangent_a, tangent_b, point_b, j / (float)division);
                        points[j - 1] = bezierPrevious;
                        bezierPrevious = bezierNext;
                    }
                    points[division - 1] = bezierPrevious;
                    line.points3 = points.ToList();
                    outputTangent = -inputTangent;
                }
                break;
        }
    }

    public enum NoodlePath
    {
        Angled,
        Curvy
    }
}
