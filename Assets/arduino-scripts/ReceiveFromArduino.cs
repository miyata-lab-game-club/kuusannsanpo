using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ReceiveFromArduino : MonoBehaviour
{
    private Queue outputQueue;
    private SerialPort serialPort;
    private Thread thread;
    private bool isRunning = false;

    private void Start()
    {
        outputQueue = Queue.Synchronized(new Queue());

        SerialPortManager.Instance.OpenSerialPort();
        serialPort = SerialPortManager.Instance.GetSerialPort();

        isRunning = true;
        thread = new Thread(Read);
        thread.Start();
    }

    private void Read()
    {
        while (isRunning && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string message = serialPort.ReadLine();
                Debug.Log(message);
                if (message != null)
                {
                    outputQueue.Enqueue(message);
                }
            }
            catch (TimeoutException) { }
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
        if (thread != null)
        {
            thread.Join();
            thread = null;
        }
    }
}
