using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [System.Serializable]
    public class CameraConfig
    {
        public Camera camera;
        public float speedMultiplier = 1f;
        [HideInInspector] public Vector3 originalPosition;
    }

    public CameraConfig[] cameraConfigs;
    public float dragSensitivity = 0.5f;
    public float inertiaDuration = 0.5f;
    public AnimationCurve inertiaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Vector2 velocity;
    private bool isDragging;
    private float inertiaTimer;
    private Vector2 lastDelta;

    void Start()
    {
        foreach (var config in cameraConfigs)
        {
            config.originalPosition = config.camera.transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 记录当前速度
        lastDelta = eventData.delta;
        velocity = lastDelta * dragSensitivity;
        
        MoveCameras(lastDelta);
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        inertiaTimer = inertiaDuration;
    }

    void Update()
    {
        HandleInertia();
    }

    void MoveCameras(Vector2 delta)
    {
        foreach (var config in cameraConfigs)
        {
            // 计算各镜头的独立移动量
            Vector3 movement = new Vector3(
                delta.x * config.speedMultiplier,
                0,
                0) * Time.deltaTime;

            config.camera.transform.Translate(-movement);
        }
    }

    void HandleInertia()
    {
        if (!isDragging && inertiaTimer > 0)
        {
            // 计算惯性衰减曲线
            float curveValue = inertiaCurve.Evaluate(1 - (inertiaTimer / inertiaDuration));
            
            // 应用惯性移动
            MoveCameras(velocity * curveValue);
            
            // 递减计时器
            inertiaTimer -= Time.deltaTime;
            
            // 自然减速
            velocity *= Mathf.Clamp01(1 - Time.deltaTime * 2);
        }
    }

    public void ResetAllPositions()
    {
        foreach (var config in cameraConfigs)
        {
            config.camera.transform.position = config.originalPosition;
        }
    }
}