using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    [SerializeField]Vector3 rotationSpeed = new Vector3(0f, 0f, 100f);
    [SerializeField] float stiffness = 200f, damping = 15f;
    SpringFloat rotationSpring;
    float rotationZ;
    void Start()
    {
        rotationSpring = new SpringFloat(0f);
    }
    void Update()
    {
        rotationSpring.stiffness = stiffness;
        rotationSpring.damping = damping;
        rotationSpring.Update(rotationZ, Time.deltaTime);
        rotationZ += rotationSpeed.z * Time.deltaTime;
      
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, rotationSpring.value);
        
    }
}
