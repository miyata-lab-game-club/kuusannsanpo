using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WindManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speed;

    // Player��Rigidbody
    private Rigidbody playerRigidbody;

    private bool up;
    private bool boost;
    private bool twiceBoost;// ���x��{

    // �����Ă������x
    [SerializeField] private Vector3 gravityDirection;

    [SerializeField] private Transform rightControllerTransform;
    [SerializeField] private GameObject[] directionUIs;
    [SerializeField] private float windCicleTime = 5;
    private float timer;
    private Vector3 currentWind;
    private int currentWindIndex = 0;

    //�@��A�k�A�k���A���A�쓌�A��A�쐼�A���A�k��
    public Vector3[] windDirection = new Vector3[]
    {new Vector3(0,1,0), new Vector3(0, 1, 1),new Vector3(1, 1, 1),new Vector3(1, 1, 0),
     new Vector3(1,1,-1), new Vector3(0, 1, -1),new Vector3(-1, 1, -1),new Vector3(-1, 1, 0),
     new Vector3(-1, 1, 1)
    };

    public Vector3[] windXZDirection = new Vector3[]
{new Vector3(0,0,0), new Vector3(0, 0, 1),new Vector3(1, 0, 1),new Vector3(1, 0, 0),
     new Vector3(1,0,-1), new Vector3(0, 0, -1),new Vector3(-1, 0, -1),new Vector3(-1, 0, 0),
     new Vector3(-1, 0, 1)
};

    // ��UI
    [SerializeField] private TextMeshProUGUI heightText;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI similarityText;

    private void Start()
    {
        heightText.text = this.transform.position.y.ToString();
        playerRigidbody = player.GetComponent<Rigidbody>();
        timer = 0;
        timerText.text = timer.ToString();
        currentWind = currentWindDirection();
    }

    private Vector3 rightControllerTilt;

    private void Update()
    {
        // �E�R���g���[���[�̌X��
        Quaternion rightControllerRotation = rightControllerTransform.rotation;
        // �E�R���g���[���[�̌X�����x�N�g���ɂ���
        rightControllerTilt = (rightControllerRotation * Vector3.forward).normalized;
        // xz���ʂɖ߂�
        rightControllerTilt.y = 0;

        heightText.text = this.transform.position.y.ToString();
        if (OVRInput.GetDown(OVRInput.RawButton.A) && !boost)
        {
            boost = true;
            timer = 0;
            twiceBoost = currentWindFromController();
            SetActiveWindDirection(currentWindIndex);
        }

        Debug.DrawLine(new Vector3(-28, 9, -60), new Vector3(-28, 9, -60) + rightControllerTilt * 5, Color.red);

        timerText.text = timer.ToString();
    }

    private void FixedUpdate()
    {
        if (boost != true)
        {
            if (player.transform.position.y > 5 && !up)
            {
                timer += Time.deltaTime;
                if (timer > windCicleTime)
                {
                    currentWind = currentWindDirection();
                    timer = 0;
                }

                float similarity;
                similarity = Vector3.Dot(rightControllerTilt.normalized, windXZDirection[currentWindIndex].normalized);
                // �ގ��x��0.7��肨�������Ƃ�
                //Debug.Log(similarity);
                similarityText.text = similarity.ToString();
                if (similarity >= 0.8)
                {
                    playerRigidbody.velocity = currentWind * speed;
                }
                // �ގ����Ă��Ȃ����
                else
                {
                    // �����Ă���
                    playerRigidbody.velocity = gravityDirection;
                }
            }
            // 5m����20m�܂ŏ㏸
            if (player.transform.position.y < 5)
            {
                up = true;
            }
            // 20m�܂ŏ㏸������Ƃ܂�
            if (player.transform.position.y > 20 && up)
            {
                up = false;
            }
            // �㏸��
            if (up)
            {
                playerRigidbody.AddForce(windDirection[0], ForceMode.Impulse);
                //playerRigidbody.velocity = windDirection[0];
                SetActiveWindDirection(0);
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer < windCicleTime && twiceBoost)
            {
                currentWind = windDirection[currentWindIndex];
                // �X���Ɉ�ԋ߂������ɕ�������
                playerRigidbody.velocity = currentWind * 3 * speed;
            }
            else if (timer < windCicleTime && !twiceBoost)
            {
                currentWind = windDirection[currentWindIndex];
                // �X���Ɉ�ԋ߂������ɕ�������
                playerRigidbody.velocity = currentWind * speed;
            }
            else
            {
                timer = 0;
                boost = false;
            }
        }
    }

    private bool currentWindFromController()
    {
        float similarity;
        float maxSimilarity = -1;
        int tmpCurrentWindIndex = currentWindIndex;
        // �X���ƈ�ԋ߂�����������o��
        for (int i = 1; i < windDirection.Length; i++)
        {
            rightControllerTilt.y = 0;
            similarity = Vector3.Dot(rightControllerTilt.normalized, windXZDirection[i].normalized);
            /*Debug.Log("�E�R���g���[���[�̌X��" + rightControllerTilt.normalized + "�~" + windXZDirection[i].normalized);
            Debug.Log("�ގ��x" + similarity);*/
            if (similarity > maxSimilarity)
            {
                maxSimilarity = similarity;
                currentWindIndex = i;
            }
        }

        similarityText.text = maxSimilarity.ToString();
        if (tmpCurrentWindIndex == currentWindIndex)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector3 currentWindDirection()
    {
        currentWindIndex = Random.Range(1, 9);
        SetActiveWindDirection(currentWindIndex);
        return windDirection[currentWindIndex].normalized;
    }

    private void SetActiveWindDirection(int currentWindIndex)
    {
        for (int i = 0; i < windDirection.Length; i++)
        {
            directionUIs[i].SetActive(false);
        }
        directionUIs[currentWindIndex].SetActive(true);
    }
}