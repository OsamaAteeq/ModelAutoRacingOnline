using UnityEngine;

public class Rotate : MonoBehaviour
{
    private Touch touch;

    [Header("Touch Rotation")]
    [SerializeField]
    public bool isTouchRotatable = false;
    [SerializeField]
    private float rotationSpeedModifier = 0.1f;

    [Header("Direction")]
    [SerializeField]
    private bool x_axis = false;
    [SerializeField]
    private bool y_axis = false;
    [SerializeField]
    private bool z_axis = true;

    [Header("Force (if applicable)")]
    [SerializeField]
    private float x_force = 0;
    [SerializeField]
    private float y_force = 0;
    [SerializeField]
    private float z_force = 1;

    private Transform target;
    // Start is called before the first frame update
    void Start()
    {
        target = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTouchRotatable)
        {
            Vector3 rotation = target.rotation.eulerAngles;
            if (x_axis)
            {
                rotation.x += x_force * Time.deltaTime;
                target.rotation = Quaternion.Euler(rotation);
            }
            if (y_axis)
            {
                rotation.y += y_force * Time.deltaTime;
                target.rotation = Quaternion.Euler(rotation);
            }
            if (z_axis)
            {
                rotation.z += z_force * Time.deltaTime;
                target.rotation = Quaternion.Euler(rotation);
            }
        }
        else 
        {
            
            if (Input.touchCount > 0) 
            {
                touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved) 
                {
                    Vector3 rotation = target.rotation.eulerAngles;
                    if (x_axis)
                    {
                        rotation.x -= touch.deltaPosition.y*rotationSpeedModifier * Time.deltaTime;
                        target.rotation = Quaternion.Euler(rotation);
                    }
                    if (y_axis)
                    {
                        rotation.y -= touch.deltaPosition.x * rotationSpeedModifier * Time.deltaTime;
                        target.rotation = Quaternion.Euler(rotation);
                    }
                }
            }
        }
    }
}
