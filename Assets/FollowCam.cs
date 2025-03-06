 using UnityEngine;
 using System.Collections;
 public class FollowCam : MonoBehaviour {
    static public GameObject POI; // The static point of interest                // a
    [Header("Set Dynamically")]
    [Header("Set in Inspector")]
    public float easing = 0.05f;
    public Vector2 minXY = Vector2.zero;
    public float              camZ; // The desired Z pos of the camera
    void Awake() {
        camZ = this.transform.position.z;
    }
    void FixedUpdate () {
        Vector3 destination;
        // if there's only one line following an if, it doesn't need braces
        if (POI == null)
        {
            destination = Vector3.zero;

        } 
        else // return if there is no poi                  // b
        {
            destination = POI.transform.position;
            if(POI.tag == "Projectile")
            {
                if(POI.GetComponent<Rigidbody>().IsSleeping() )
                {
                    POI = null;
                    return;
                }
            }
        }
        // Get the position of the poi
       // Vector3 destination = POI.transform.position;
        destination.x = Mathf.Max( minXY.x, destination.x );
        destination.y = Mathf.Max( minXY.y, destination.y );
        destination = Vector3.Lerp(transform.position,destination,easing);
        // Force destination.z to be camZ to keep the camera far enough away
        destination.z = camZ;
        // Set the camera to the destination
        transform.position = destination;
        Camera.main.orthographicSize = destination.y +10;
        
    }
 }
