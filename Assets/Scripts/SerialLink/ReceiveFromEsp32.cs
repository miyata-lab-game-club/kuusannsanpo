using System;
using System.Collections;
using UnityEngine;
public class ReceiveFromEsp32 : MonoBehaviour
{
    public GameObject rotateObject; // Received data will rotate this object.
    private Queue outputQueue;

    private void Start()
    {
        outputQueue = Queue.Synchronized(new Queue());
        if (SerialPortManager.Instance != null)
        {
            SerialPortManager.Instance.OnDataReceived += ProcessReceivedData;
        }
        else
        {
            Debug.LogError("SerialPortManager instance not found!");
        }
    }

    private void ProcessReceivedData(string message)
{
    try
    {
        string[] parts = message.Split(',');
        if (parts.Length == 3)  // 3つの要素を期待
        {
            float[] data = new float[3];
            for (int i = 0; i < 3; i++)
            {
                data[i] = float.Parse(parts[i]);
            }
            outputQueue.Enqueue(data);  // Tupleは不要なので、直接配列をEnqueue
        }
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Error while processing received data: {e.Message}");
    }
}

  private void Update()
{
    Debug.Log(outputQueue.Count);
    if (outputQueue.Count != 0)
    {
        float[] rotate = (float[])outputQueue.Dequeue();
        rotateObject.transform.eulerAngles = new Vector3(-rotate[1], 0,-rotate[0]);
    }
}

    private void OnDestroy()
    {
        if (SerialPortManager.Instance != null)
        {
            SerialPortManager.Instance.OnDataReceived -= ProcessReceivedData;
        }
    }
}
