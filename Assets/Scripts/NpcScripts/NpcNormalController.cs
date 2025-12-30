using UnityEngine;

public class NpcNormalController : MonoBehaviour
{
    [Header("Wheels")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Npc Sys")]
    public float motorForce = 1500f;
    public float maxSpeed = 80f;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //무게 중심을 낮추어 안정성 확보
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //조향과 가속만 처리
        HandleMotor();
        HandleSteering();
    }

    private void HandleMotor()
    {
        float currentSpeed = rb.linearVelocity.magnitude * 3.6f;

        if (currentSpeed < maxSpeed)
        {
            frontLeftWheel.motorTorque = motorForce;
            frontRightWheel.motorTorque = motorForce;
            rearLeftWheel.motorTorque = motorForce;
            rearRightWheel.motorTorque = motorForce;
        }
        else
        {
            frontLeftWheel.motorTorque = 0;
            frontRightWheel.motorTorque = 0;
            rearLeftWheel.motorTorque = 0;
            rearRightWheel.motorTorque = 0;  
        }
    }
    
    private void HandleSteering()
    {
        //조향각을 0으로 고정
        frontLeftWheel.steerAngle = 0;
        frontRightWheel.steerAngle = 0;       
    }
}
