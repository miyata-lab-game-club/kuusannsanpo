using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ReceiveFromEsp32 : MonoBehaviour
{
    private Queue outputQueue;
    private SerialPortManager spManager;//

    private float my_pitch;
    private float my_roll;
    private float my_yaw;
    private char buttonStatus;

    private void Start()
    {        spManager = SerialPortManager.Instance;

        if (spManager == null)
        {
            Debug.LogError("SerialPortManager instance not found!");
            return;
        }
        //outputQueue = Queue.Synchronized(new Queue());
        //spManager = SerialPortManager.Instance;
        Debug.Log("SerialPortManager initialized.");
    }

    private void Update()
    {

        Debug.Log("dd");
        if (spManager.serialPorts[5].IsOpen)
        {
            Debug.Log("dddddd");
            string message = spManager.serialPorts[5].ReadLine();
            ProcessReceivedData(message);
        }
        else
        {
            Debug.LogWarning("Serial port at index 5 is not open.");
        }

    }


    private void ProcessReceivedData(string message)
    {
        try
        {
            Debug.Log($"Received message: {message}");

            string[] data = message.Split(',');

            if (data.Length == 4)
            {
                my_pitch = float.Parse(data[0].Trim());
                my_roll = float.Parse(data[1].Trim());
                my_yaw = float.Parse(data[2].Trim());
                buttonStatus = data[3].Trim()[0];

                Debug.Log($"Parsed - Pitch: {my_pitch}, Roll: {my_roll}, Yaw: {my_yaw}, ButtonStatus: {buttonStatus}");

                outputQueue.Enqueue(message);
            }
            else
            {
                Debug.LogWarning("Unexpected data format received.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error while processing received data: {e.Message}");
        }
    }
}