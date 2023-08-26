using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ReceiveFromEsp32 : MonoBehaviour
{
    private Queue outputQueue;
    private SerialPortManager serialPortManager;
    private bool isRunning = false;

    private void Start()
    {
        outputQueue = Queue.Synchronized(new Queue());
        serialPortManager = GetComponent<SerialPortManager>();
        if (serialPortManager != null)
        {
            serialPortManager.OnDataReceived += HandleDataReceived;
        }
    }

    private void HandleDataReceived(string message)
    {
        if (message != null)
        {
            outputQueue.Enqueue(message);
        }
    }

    private void Update()
    {
        while (outputQueue.Count != 0)
        {
            string message = (string)outputQueue.Dequeue();

            if (message.Trim() == "1")
            {
                Debug.ClearDeveloperConsole(); // Clears the console when the button is pressed
            }
        }
    }

    private void OnDestroy()
    {
        isRunning = false;
        if (serialPortManager != null)
        {
            serialPortManager.OnDataReceived -= HandleDataReceived;
        }
    }
}
