using System;
using System.Collections;
using UnityEngine;

public class ReceiveFromEsp32 : MonoBehaviour
{
    public GameObject rotateObject;
    private Queue outputQueue;

    public char buttonState='a';  // buttonState変数

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
            if (parts.Length == 4)  // 4つの要素を期待
            {
                float[] data = new float[3];
                for (int i = 0; i < 3; i++)
                {
                    data[i] = float.Parse(parts[i]);
                }
                buttonState = char.Parse(parts[3].Trim());  // 追加: buttonStateにデータを格納
                outputQueue.Enqueue(data);
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
            float[] rotate = (float[])outputQueue.Dequeue();
            rotateObject.transform.eulerAngles = new Vector3(-rotate[1], 0, -rotate[0]);

            //Debug.Log($"Button State: {buttonState}");  // 追加: buttonStateを表示
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
