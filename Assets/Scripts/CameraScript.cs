using UnityEngine;

public class CameraScript : MonoBehaviour {

    [SerializeField] Transform _focusPoint = null;

    /// <summary>
    /// Initial offset from the focus point
    /// </summary>
    float initialDistanceFromFocusPoint = 0;
    [SerializeField]
    Transform pivot = null;
    // Use this for initialization
    void Start () {
        initialDistanceFromFocusPoint= (transform.position - _focusPoint.position).magnitude;
	}

    
    

    [SerializeField] Vector2 rotationRangeX = Vector2.zero;
    [SerializeField] Vector2 rotationRangeY = Vector2.zero;
    

    [SerializeField] float rotationSpeed = 4;
    // Update is called once per frame
    void Update () {

        Vector3 mouseRatio = new Vector3(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height, 1);
       

        float rotX = Mathf.Lerp(rotationRangeX.x, rotationRangeX.y, mouseRatio.x);// lerpX);
        float rotY = Mathf.Lerp(rotationRangeY.x, rotationRangeY.y, mouseRatio.y);// lerpY);

        Quaternion rotationX = Quaternion.Euler(-rotY, 0, 0);
        Quaternion rotationY = Quaternion.Euler(0, rotX, 0);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotationX, Time.deltaTime * rotationSpeed);
        pivot.localRotation = Quaternion.Lerp(pivot.localRotation, rotationY, Time.deltaTime * rotationSpeed);

        transform.position = _focusPoint.position + -transform.forward * initialDistanceFromFocusPoint;
        
    }
}
