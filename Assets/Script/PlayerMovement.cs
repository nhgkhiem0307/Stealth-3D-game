using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 5.5f;
    public float crouchSpeed = 1.5f;

    [Header("Mouse")]
    public float mouseSensitivity = 150f;

    [Header("Jump & Gravity")]
    public float gravity = -9.81f;
    //public float jumpHeight = 1.2f;

    [Header("Crouch")]
    public float standingHeight = 2.6f;
    public float crouchingHeight = 1.2f;
    public float crouchCameraY = 1.2f;
    public float standCameraY = 2.6f;

    [Header("Stand Check")]
    public float headCheckDistance = 1.5f;
    public LayerMask obstacleMask;

    [Header("Interaction")]
    public Transform holdPoint;
    public float throwForce = 12f;

    public GameObject heldObject;

    public FootstepSound footstep;

    CharacterController controller;
    Transform cameraTransform;

    float yVelocity;
    float xRotation = 0f;
    bool isCrouching;
    float noiseTimer = 0f;
    float noiseInterval = 0.5f; //am thanh phat tieng

    Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Look();
        Move();
        HandleInteraction();
        footstep.isWalking = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        footstep.isRunning = footstep.isWalking && Input.GetKey(KeyCode.LeftShift);
    }

    void Move()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && yVelocity < 0)
            yVelocity = -2f;

        float speed = walkSpeed;

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
            speed = sprintSpeed;
        else if (isCrouching)
            speed = crouchSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            animator.speed = 1.8f;   // chạy animation nhanh
        else
            animator.speed = 1f;     // đi bộ bình thường

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float moveAmount = new Vector2(x, z).magnitude;
        animator.SetFloat("Speed", moveAmount);

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Jump
        //if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        //{
         //   yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        //}

        // Gravity
        yVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * yVelocity * Time.deltaTime);

        HandleCrouch();

        bool isMoving = Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f;

        if (isMoving && controller.isGrounded)
        {
            noiseTimer -= Time.deltaTime;

            if (noiseTimer <= 0f)
            {
                float radius = 1f; // đi bộ nhỏ

                if (Input.GetKey(KeyCode.LeftShift))
                    radius = 4f;
                else if (isCrouching)
                    radius = 0f; //hmm balance sau

                if (radius > 0f)
                    NoiseSystem.instance.MakeNoise(transform.position, radius);//tranh ton cpu

                noiseTimer = noiseInterval;
            }
        }
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            controller.height = crouchingHeight;
            controller.center = new Vector3(0, crouchingHeight / 2f, 0);
            cameraTransform.localPosition = new Vector3(0, crouchCameraY, 0.5f);
        }
        else
        {
            if (CanStand())
            {
                isCrouching = false;
                controller.height = standingHeight;
                controller.center = new Vector3(0, standingHeight / 2f, 0);
                cameraTransform.localPosition = new Vector3(0, standCameraY, 0.5f);
            }
        }
    }
    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
            TryPickUp();

        if (Input.GetKeyDown(KeyCode.Q))
            Drop();

        if (Input.GetMouseButtonDown(0))
            Throw();
    }
    void TryPickUp()
    {
        if (heldObject != null) return; // đã cầm đồ rồi thì không nhặt nữa

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 30f))
        {
            Bottle bottle = hit.collider.GetComponent<Bottle>();
            KeyItem key = hit.collider.GetComponent<KeyItem>();

            if (bottle != null)
            {
                heldObject = bottle.gameObject;
                bottle.PickUp(holdPoint);
            }
            else if (key != null)
            {
                heldObject = key.gameObject;
                key.PickUp(holdPoint);
            }
        }
    }
    void Drop()
    {
        if (heldObject == null) return;

        Bottle bottle = heldObject.GetComponent<Bottle>();
        KeyItem key = heldObject.GetComponent<KeyItem>();

        if (bottle != null)
            bottle.Drop();
        else if (key != null)
            key.Drop();

        heldObject = null;
    }
    void Throw()
    {
        if (heldObject == null) return;

        Bottle bottle = heldObject.GetComponent<Bottle>();

        if (bottle != null)
        {
            bottle.Throw(cameraTransform.forward);
        }

        heldObject = null;
    }

    bool CanStand()
    {
        Vector3 origin = transform.position + Vector3.up * crouchingHeight;
        return !Physics.Raycast(origin, Vector3.up, headCheckDistance, obstacleMask);
    }
}
