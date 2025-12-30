using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    private Rigidbody rb;

    //인스펙터를 깔끔하게 하기위해 header
    [Header("Wheels")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Car Sys")]
    public float motorForce = 1500f; //엔진 마력
    public float brakeForce = 4000f; //브레이크

    //https://forums.unrealengine.com/t/max-steering-angle/1718078 - 언리얼 엔진이지만 차량 제작시 최대 조향각이 필요함을 이걸 보고 참고했습니다.
    /*public float maxSteerAngle = 12f; //차량의 바퀴가 좌우로 꺾일 수 있는 최대 각도
    
        하지만 이런 고정적인 최대 각도를 지정할 경우, 유저가 가속할 수록 회전이 급격하게 진행됩니다.
        현실에서도 속도가 높을때 핸들을 약간만 틀어도 급격하게 회전하듯이 물리엔진의 영향으로 격렬하게 회전하게 되는거 같습니다.
        이를 해결하기 위해 제미나이에게 조언을 구했고 '속도 감응형 스티어링'이란 해결 방안을 제시해줬습니다.
        간단하게 차량의 속도가 빠를수록 최대 조향각도를 줄이는 현실의 파워핸들 내 장착되어있는 MDPS와 똑같은 기능을 구현하면 된다는 것입니다.
    */
    [Header("Steering Sensitivity")]
    public float lowSpeedSteerAngle = 12f; //저속 최대 조향 각도
    public float highSpeedSteerAngle = 5f; //고속 최대 조향 각도
    public float speedThreshold = 50f; //고속 판단 기준

    [Header("Controls")]
    public Joystick moveJoystick;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle; //매 프레임의 조향 각도를 저장할 변수가 필요함.

    [Header("HighLight Effect")]
    public GameObject highLightObject; //Npc_HighLight
    public float minSpawnTime = 20f;
    public float maxSpawnTime = 40f;
    public float effectDuration = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        /*https://docs.unity3d.com/6000.2/Documentation/ScriptReference/Rigidbody-centerOfMass.html
          https://m.blog.naver.com/droneaje/222156709585 - 두 글을 참고 및 제미나이의 조언으로 
          centerOfMass(무게중심)을 설정해야 현실과 비슷한 차량 퍼포먼스를 보여줄 수 있다 판단하여 사용했습니다.
          하지만 관계식에 대한 공부는 더해야할 것같습니다.
        */

        if (highLightObject != null)
        {
            highLightObject.SetActive(false);
            StartCoroutine(RandomHighLightSpawner());
        }
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = moveJoystick.Horizontal;
        verticalInput = moveJoystick.Vertical;
    }

    void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
    }

    private void HandleMotor()
    {
        // 전륜구동
        float motorInput = Mathf.Max(0, verticalInput);
        frontLeftWheel.motorTorque = verticalInput * motorForce;
        frontRightWheel.motorTorque = verticalInput * motorForce;

        float brakeInput = verticalInput < 0 ? brakeForce : 0f;
        ApplyBraking(brakeInput);
    }

    private void ApplyBraking(float force)
    {
        frontLeftWheel.brakeTorque = force;
        frontRightWheel.brakeTorque = force;
        rearLeftWheel.brakeTorque = force;
        rearRightWheel.brakeTorque = force;
    }

    private void HandleSteering()
    {
        float currentSpeed = rb.linearVelocity.magnitude * 3.6f; //현재 차량 속도를 km/h 표준식으로 계산
        float maxAngle = Mathf.Lerp(lowSpeedSteerAngle, highSpeedSteerAngle, currentSpeed / speedThreshold);
        //차량 속도에 따라 최대 조향각도를 Lerp(부드럽게) 변경

        float currentSteerAngle = maxAngle * horizontalInput * -1;
        frontLeftWheel.steerAngle = currentSteerAngle;
        frontRightWheel.steerAngle = currentSteerAngle;
    }

    IEnumerator RandomHighLightSpawner()
    {
        while (true)
        {
            // 1. 최소 최대 사이의 랜덤 시간 동안 대기
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            //Debug.Log($"다음 상향등까지 {waitTime}초 대기..");
            yield return new WaitForSeconds(waitTime);

            // 2. 상향등 오브젝트 활성화 (-> NpcHighLightController.OnEnable 활성화)
            highLightObject.SetActive(true);

            // 3. 정해진 지속 시간만큼 진행
            yield return new WaitForSeconds(effectDuration);

            // 4. 상향등 오브젝트 비활성화 (->OnDisable 활성화)
            highLightObject.SetActive(false);
        }
    }
}
