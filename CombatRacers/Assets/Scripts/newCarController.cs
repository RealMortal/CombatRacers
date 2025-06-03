using System.Drawing;
using System.IO.IsolatedStorage;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
// https://www.youtube.com/watch?v=sWshRRDxdSU
public class NewCarController : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationPoint;
    [SerializeField] private GameObject[] frontTireParent = new GameObject[2];
    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;
    [SerializeField] private float dampingFactor;
    [SerializeField] private TrailRenderer[] skidMarks = new TrailRenderer[2];
    [SerializeField] private ParticleSystem[] skidSmokes = new ParticleSystem[2];
    [SerializeField] private AudioSource engineSound, skidSound;

    private PlayerInput playerInput;
    private Vector2 moveVector;

    private float moveInput;
    private float steerInput;
    private bool hasPowerUp = false;
    private bool isDrifting = false;
    private Vector3 currentCarVelocity = Vector3.zero;
    private float carVelocityRatio = 0;
    private int[] wheelsIsGrounded = new int[4];
    private bool isGrounded = false;

    [Header("Car Settings")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float turnSpeed = 15f;
    [SerializeField] private float customPower = 0.2f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private float dragCoefficient = 1f;

    [SerializeField] private float rearGrip = 1.0f;
    [SerializeField] private float rearGripWhenDrifting = 0.6f;

    [Header("Visuals")]
    [SerializeField] private float maxSteeringAngle = 30f;
    [SerializeField] private float minSideSkidVelocity = 10f;
    [SerializeField] private float skidDelay = 0.2f;



    [Header("Audio")]
    [SerializeField][Range(0, 1)] private float minPitch = 1f;
    [SerializeField][Range(1, 5)] private float maxPitch = 5f;

    [SerializeField] private CinemachineCamera playerCamera;


    public void SetPlayerCamera(CinemachineCamera playerCamera)
    {
        this.playerCamera = playerCamera;
    }
    public CinemachineCamera GetPlayerCamera() => playerCamera;
    public bool isAbleToOneShot = false;

    #endregion






    #region Power-Up Management

    public bool HasActivePowerUp()
    {
        return hasPowerUp;
    }

    public void SetHasActivePowerUp(bool hasPowerUp)
    {
        this.hasPowerUp = hasPowerUp;
    }

    public void SetAcceleration(float newAcceleration)
    {
        this.acceleration = newAcceleration;
    }

    public float GetAcceleration()
    {
        return acceleration;
    }

    #endregion

    #region Unity Lifecycle Methods

    private void Awake()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        engineSound = sources[0];
        skidSound = sources[1];
    }

    private void Start()
    {

       playerInput = GetComponent<PlayerInput>();   
        playerInput.actions["Drive"].performed += ctx => moveVector = ctx.ReadValue<Vector2>();
        playerInput.actions["Drive"].canceled += ctx => moveVector = Vector2.zero;

        playerInput.actions["HandBrake"].started += ctx => isDrifting = true;
        playerInput.actions["HandBrake"].canceled += ctx => isDrifting = false;

        carRB = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
        TireVisuals();
        Vfx();
        SidewaysDrag();
        YawStabilizer();
        EngineSound();
    }

    #endregion

    #region Input Handling

    private void GetInput()
    {
        moveInput = moveVector.y;
        steerInput = moveVector.x;
    }

    #endregion

    #region Movement

    private void Movement()
    {
        if (isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
        }
    }

    private void Acceleration()
    {
        float effectiveMassFactor = Mathf.Pow(carRB.mass, customPower);
        float actualAcceleration = isDrifting ? acceleration * 0.7f : acceleration;

        Vector3 force = (transform.forward * actualAcceleration * moveInput) * (1f / effectiveMassFactor);
        carRB.AddForceAtPosition(force * 20, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Deceleration()
    {
        float speedFactor = Mathf.Clamp01(carVelocityRatio);
        carRB.AddForceAtPosition(deceleration * moveInput * -transform.forward * speedFactor, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Turn()
    {
        if (!isGrounded) return;

        float turnAmount = turnSpeed * steerInput * turningCurve.Evaluate(Mathf.Abs(carVelocityRatio));
        float directionMultiplier = (carVelocityRatio < 0) ? -1f : 1f;

        if (isDrifting)
            turnAmount *= 1.2f;

        carRB.AddTorque(turnAmount * directionMultiplier * transform.up, ForceMode.Acceleration);
    }


    #endregion

    #region Visuals

    public void TireVisuals()
    {
        float steeringAngle = maxSteeringAngle * steerInput;

        for (int i = 0; i < frontTireParent.Length; i++)
        {
            frontTireParent[i].transform.localEulerAngles = new Vector3(frontTireParent[i].transform.localEulerAngles.x, steeringAngle, frontTireParent[i].transform.localEulerAngles.z);
        }
    }


    private bool skidActive = false;
    private float skidTimer = 0f;

    private void Vfx()
    {
        bool shouldEmit = isGrounded && Mathf.Abs(currentCarVelocity.x) > minSideSkidVelocity;

        if (shouldEmit)
            skidTimer += Time.fixedDeltaTime;
        else
            skidTimer = 0f;

        bool allowVFX = skidTimer > skidDelay;

        if (allowVFX != skidActive)
        {
            ToggleSkidMarks(allowVFX);
            ToggleSkidSmokes(allowVFX);
            ToggleSkidSound(allowVFX);
            skidActive = allowVFX;
        }
    }

    private void ToggleSkidMarks(bool toggle)
    {
        foreach(var skidMark in skidMarks)
        {
            foreach (var mark in skidMarks)
            {
                if (mark.emitting != toggle)
                    mark.emitting = toggle;
            }
        }
    }

    private void ToggleSkidSmokes(bool toggle)
    {
        foreach (var smoke in skidSmokes)
        {
            if (toggle)
            {
                if (!smoke.isPlaying)
                    smoke.Play();
            }
            else
            {
                if (smoke.isPlaying)
                    smoke.Stop();
            }
        }
    }
    #endregion

    #region Physics & Suspension

    private void GroundCheck()
    {
        int tempGroundedWheels = 0;

        for (int i = 0; i < wheelsIsGrounded.Length; i++)
        {
            tempGroundedWheels += wheelsIsGrounded[i];
        }

        isGrounded = tempGroundedWheels > 1;
    }

    private void SidewaysDrag()
    {
        if (!isGrounded) return;

        float rearSlipFactor = isDrifting ? rearGripWhenDrifting : rearGrip;
        float sidewaysSpeed = currentCarVelocity.x;

        float dragForce = -sidewaysSpeed * dragCoefficient * rearSlipFactor;
        carRB.AddForce(transform.right * dragForce, ForceMode.Acceleration);
    }


    private void YawStabilizer()
    {
        if (!isGrounded || !isDrifting) return;

        float angularY = carRB.angularVelocity.y;
        float correction = -angularY * 0.5f;
        carRB.AddTorque(Vector3.up * correction, ForceMode.Acceleration);
    }




    private void CalculateCarVelocity()
    {
        currentCarVelocity = transform.InverseTransformDirection(carRB.linearVelocity);
        carVelocityRatio = Mathf.Clamp(currentCarVelocity.z / maxSpeed, -1f, 1f);
    }

    private void Suspension()
    {
        for (int i = 0; i < rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;

            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, maxLength + wheelRadius, drivable))
            {
                wheelsIsGrounded[i] = 1;

                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;
                float springVelocity = Vector3.Dot(carRB.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);

                float suspensionForce = CalculateSuspensionForce(springCompression, springVelocity);
                ApplySuspensionForce(suspensionForce, rayPoints[i]);

                Debug.DrawLine(rayPoints[i].position, hit.point, UnityEngine.Color.red);
            }
            else
            {
                wheelsIsGrounded[i] = 0;        
                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position + (wheelRadius + maxLength) * -rayPoints[i].up, UnityEngine.Color.green);
            }
        }
    }

    private void ApplySuspensionForce(float suspensionForce, Transform rayPoint)
    {
        carRB.AddForceAtPosition(suspensionForce * rayPoint.up, rayPoint.position);

        Vector3 lateralVelocity = Vector3.ProjectOnPlane(carRB.GetPointVelocity(rayPoint.position), transform.forward);
        Vector3 lateralDampingForce = -lateralVelocity * dampingFactor;
        carRB.AddForceAtPosition(lateralDampingForce, rayPoint.position);
    }

    private float CalculateSuspensionForce(float springCompression, float springVelocity)
    {
        float springForce = springStiffness * springCompression;
        float dampForce = damperStiffness * springVelocity;
        return springForce - dampForce;
    }

    #endregion


    #region Audio

    private void EngineSound()
    {
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(carVelocityRatio));
    }

    private void ToggleSkidSound(bool toggle)
    {
        if (toggle && !skidSound.isPlaying)
            skidSound.Play();
        else if (!toggle && skidSound.isPlaying)
            skidSound.Stop();
    }
    #endregion
}
