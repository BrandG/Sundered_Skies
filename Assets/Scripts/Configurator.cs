using UnityEngine;

public class Configurator : MonoBehaviour
{
    private PlayerController player;

    private FollowCamera followCameraScript;

    void Awake()
    {
        ServiceLocator.Instance.RegisterConfigurator(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        player = ServiceLocator.Instance.PlayerController;
        followCameraScript = ServiceLocator.Instance.FollowCamera.GetComponent<FollowCamera>();

        Random.InitState(System.DateTime.Now.Millisecond);
    }

    // Update is called once per frame
    void Update()
    {
        bool running = Input.GetKey(KeyCode.LeftShift);
        bool forward = Input.GetKey(KeyCode.W);
        bool back = Input.GetKey(KeyCode.S);
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);
        bool zoomIn = Input.GetKey(KeyCode.I);

        Vector3 movement = (
            Vector3.right * ((left ? 1.0f : 0) - (right ? 1.0f : 0)) +
            Vector3.forward * ((back ? 1.0f : 0) - (forward ? 1.0f : 0))
        ).normalized;

        player.Stop();

        if (followCameraScript.mode != FollowCamera.CameraMode.ZOOMING_IN)
        {
            if (movement != Vector3.zero)
            {
                player.Move(movement, running);
            }
            if (zoomIn)
            {
                followCameraScript.SetZoomInMode();
                player.ZoomIn(true);
            }
        }
    }
}
