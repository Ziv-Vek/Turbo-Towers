using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [SerializeField] private Transform turretHead;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Transform turretPivot;
    [SerializeField] private float pitchSpeed = 15f;
    [SerializeField, Range(0, 90)] private float maxPitchAngle = 60f;
    [SerializeField, Range(-45, 0)] private float minPitchAngle = 0;

    private int inputStyle = 1;

    private void OnEnable()
    {
        PlayerController.onHorizontalTouchDrag += HorizontalTouchDragHandler;
        PlayerController.onVerticalTouchDrag += VerticalTouchDragHandler;

        //InputSwitchHandler.Instance.onInputStyleSelect += inputStyle => this.inputStyle = inputStyle;
    }

    private void OnDisable()
    {
        PlayerController.onHorizontalTouchDrag -= HorizontalTouchDragHandler;
        PlayerController.onVerticalTouchDrag -= VerticalTouchDragHandler;
        
    }

    private void HorizontalTouchDragHandler(int touchDragDirection)
    {
        turretHead.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime * touchDragDirection));
    }
    
    private void VerticalTouchDragHandler(int touchDragDirection)
    {
        float angleX = 360 - turretPivot.eulerAngles.x;

        if (touchDragDirection > 0)
        {
            // Raise turret's pivot:
            if (angleX <= maxPitchAngle || angleX >= 360 - maxPitchAngle)
            {
                turretPivot.Rotate(Vector3.right * -pitchSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Lower turret's pivot:
            if (turretPivot.eulerAngles.x > 180 || turretPivot.eulerAngles.x <= Mathf.Abs(minPitchAngle))
            {
                turretPivot.Rotate(Vector3.right * pitchSpeed * Time.deltaTime);
            }
        }
    }
}
