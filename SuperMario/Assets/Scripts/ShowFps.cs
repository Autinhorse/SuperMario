//@script ExecuteInEditMode

using UnityEngine;
using System.Collections;

public enum PositionTypes
{
    TopLeft = 0,
    TopMiddle,
    TopRight,
    MiddleLeft,
    MiddleRight,
    BottomLeft,
    BottomMiddle,
    BottomRight
}

public class ShowFps : MonoBehaviour
{
    #region Fields

    public PositionTypes position = PositionTypes.TopLeft;
    public Color color = Color.black;

    private float updateInterval = 1.0f;
    private float lastInterval; // Last interval end time
    private int frames = 0; // Frames over current interval

    private Rect rect;
    private GUIStyle style;

    private float fps;
    private float frameTime;

    #endregion

    #region Functions

    // Use this for initialization
    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        
        style = new GUIStyle();
        style.normal.textColor = color;
        style.fontSize = 20;

        float w = 240, h = 20, off = 20;

        switch (position)
        {
            case PositionTypes.TopLeft:
                rect = new Rect(off, off, w, h);
                break;
            case PositionTypes.TopMiddle:
                rect = new Rect(Screen.width/2 - w/2, off, w, h);
                break;
            case PositionTypes.TopRight:
                rect = new Rect(Screen.width - w/2 - off, off, w, h);
                break;
            case PositionTypes.MiddleLeft:
                rect = new Rect(off, Screen.height/2 - h/2, w, h);
                break;
            case PositionTypes.MiddleRight:
                rect = new Rect(Screen.width - w/2 - off, Screen.height / 2 - h / 2, w, h);
                break;
            case PositionTypes.BottomLeft:
                rect = new Rect(off, Screen.height - off - h, w, h);
                break;
            case PositionTypes.BottomMiddle:
                rect = new Rect(Screen.width / 2 - w/2, Screen.height - off - h, w, h);
                break;
            case PositionTypes.BottomRight:
                rect = new Rect(Screen.width - w/2 - off, Screen.height - off - h, w, h);
                break;
        }
    }

    void OnGUI()
    {
        GUI.Label(rect, frameTime.ToString("f1") + "ms " + fps.ToString("f2") + "FPS", style);
    }

    // Update is called once per frame
    void Update()
    {
        frames++;
        float timeNow = Time.realtimeSinceStartup;

        if (timeNow > lastInterval + updateInterval)
        {
            fps = frames / (timeNow - lastInterval);
            frameTime = 1000.0f / Mathf.Max(fps, 0.00001f);

            frames = 0;
            lastInterval = timeNow;
        }
    }

    #endregion
}

