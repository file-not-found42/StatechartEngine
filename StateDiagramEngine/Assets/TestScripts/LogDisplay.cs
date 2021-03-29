using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Log code from https://answers.unity.com/questions/1020051/print-debuglog-to-screen-c.html
public class LogDisplay : MonoBehaviour
{
    readonly Queue messageQueue = new Queue();
    Text text = null;
    string displayString;
    bool dirty = true;


    void Start()
    {
        text = GetComponent<Text>();
        Application.targetFrameRate = -1;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        messageQueue.Enqueue("\n [" + type + "] : " + logString);
        
        if (type == LogType.Exception)
            messageQueue.Enqueue("\n" + stackTrace);

        var builder = new System.Text.StringBuilder();
        
        foreach (var message in messageQueue)
            builder.Append(message);
        
        displayString = builder.ToString();
        dirty = true;
    }

    void OnGUI()
    {
        if (dirty)
        {
            text.text = "Frametime: " + (Time.deltaTime * 1e3) + " ms" + displayString;
            messageQueue.Clear();
            dirty = false;
        }
    }
}