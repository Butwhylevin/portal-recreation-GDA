using Unity.VisualScripting;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public Quaternion TargetRotation { private set; get; }
    
    [SerializeField] private CharacterController cc;

    private void Awake()
    {
        TargetRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        // Make sure the player stays upright
        var targetEuler = TargetRotation.eulerAngles;
        targetEuler.y = transform.rotation.eulerAngles.y;
        
        TargetRotation = Quaternion.Euler(targetEuler);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, 
            Time.deltaTime * 15.0f);
    }
}
