using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Terrain terrain;
    private MyPlayerMovement playerMovementScript;
    private readonly float rotationSpeed = 10.0f;
    private readonly float runningSpeed = 9.4f;
    private readonly float walkingSpeed = 3.2f;
    private readonly float positionThreshold = 0.1f; // Adjust this value to control the sensitivity of position change detection

    /// <summary>
    /// A reference to a running coroutine which periodically checks the height of the terrain at the player's position.
    /// </summary>
    private Coroutine terrainSamplingCoroutine;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterPlayerController(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        terrain = ServiceLocator.Instance.Terrain;
        playerMovementScript = GetComponent<MyPlayerMovement>();
        transform.position = new Vector3(transform.position.x, terrain.SampleHeight(transform.position), transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Start the coroutine if it's not already running
        if (terrainSamplingCoroutine == null)
        {
            terrainSamplingCoroutine = StartCoroutine(SampleTerrainHeight());
        }
    }

    public void ZoomIn(bool zoom)
    {
        playerMovementScript.shift = true;
    }

    public void Stop()
    {
        playerMovementScript.run = false;
        playerMovementScript.walk = false;
    }

    public void Move(Vector3 movement, bool running)
    {
        playerMovementScript.walk = !running;
        playerMovementScript.run = running;

        if (movement != Vector3.zero)
        {
            // Calculate the rotation to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            // Apply the rotation (add smoothing if desired)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        float speed = running ? runningSpeed : walkingSpeed;

        // Calculate the target position for smooth movement
        Vector3 targetPosition = transform.position + movement * speed * Time.deltaTime;

        // Interpolate between the current position and the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, 100.0f * Time.deltaTime);

        // Fix us to the ground
        transform.position = new Vector3(transform.position.x, terrain.SampleHeight(transform.position), transform.position.z);
    }

    private IEnumerator SampleTerrainHeight()
    {
        Vector3 previousPosition = transform.position;
        float terrainHeight = terrain.SampleHeight(transform.position);

        while (true)
        {
            // Check if the player's position has changed significantly
            if (Vector3.Distance(transform.position, previousPosition) > positionThreshold)
            {
                // Sample the terrain height at the new position
                terrainHeight = terrain.SampleHeight(transform.position);
                previousPosition = transform.position;
            }

            // Update the player's position with the sampled terrain height
            transform.position = new Vector3(transform.position.x, terrainHeight, transform.position.z);

            // Wait for the next frame
            yield return null;
        }
    }

}
