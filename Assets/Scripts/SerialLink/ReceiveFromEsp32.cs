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
        Debug.Log(message);
        try
        {
            string[] parts = message.Split(',');
            if (parts.Length == 4)
            {
                float[] data = new float[3];
                for (int i = 0; i < 3; i++)
                {
                    data[i] = float.Parse(parts[i]);
                }
                char buttonStatus = parts[3].Trim()[0];
                outputQueue.Enqueue(new Tuple<float[], char>(data, buttonStatus));
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error while processing received data: {e.Message}");
        }
    }

    private void Update()
    {
        if (outputQueue.Count != 0)
        {
            var dataTuple = (Tuple<float[], char>)outputQueue.Dequeue();
            float[] rotate = dataTuple.Item1;
            rotateObject.transform.eulerAngles = new Vector3(rotate[0], rotate[1], rotate[2]);
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
