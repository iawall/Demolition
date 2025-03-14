using UnityEngine;
using System.Collections;
//using System.Numerics;

public class Slingshot : MonoBehaviour {
    static private Slingshot S;
    [SerializeField] private LineRenderer rubber;
    [SerializeField] private Transform firstPoint;
    [SerializeField] private Transform secondPoint;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip snapSound;
    
    [Header("Set in Inspector")]
    public GameObject prefabProjectile;
    public float velocityMult = 8f;
    [Header("Set Dynamically")]
    static public Vector3 LAUNCH_POS {
        get {
            if(S == null) return Vector3.zero;
            return S.launchPos;
        }
    }
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    private Rigidbody projectileRigidbody;
    void Awake() {
        S = this;
        Transform launchPointTrans = transform.Find("LaunchPoint");            // a
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive( false );   
        launchPos = launchPointTrans.position;
        
        if(rubber!=null) {
            rubber.positionCount = 3;
        }                                 // b
    }
    void OnMouseEnter() {
        //print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive( true );                                           // b
    }
    void OnMouseExit() {
        //print("Slingshot:OnMouseExit()");
        launchPoint.SetActive( false );                                          // b
     }

    void OnMouseDown() {                                                    // d
        // The player has pressed the mouse button while over Slingshot
        aimingMode = true;
        // Instantiate a Projectile
        projectile = Instantiate( prefabProjectile ) as GameObject;
        // Start it at the launchPoint
        projectile.transform.position = launchPos;
        // Set it to isKinematic for now
        projectile.GetComponent<Rigidbody>().isKinematic = true;
        projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.isKinematic = true;
    }

    void Update() {
        if(!aimingMode) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D =  Camera.main.ScreenToWorldPoint(mousePos2D);
        Vector3 mouseDelta = mousePos3D-launchPos;
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;

         if (mouseDelta.magnitude > maxMagnitude) {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        // Move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;
        UpdateRubberBand();
        if ( Input.GetMouseButtonUp(0) )
        {                                        // e
            // The mouse has been released
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.linearVelocity = -mouseDelta * velocityMult;

           // if (audioSource && snapSound)
           // {
            Debug.Log("Snap sound is playing!");
            audioSource.PlayOneShot(snapSound);

          //  }
            FollowCam.POI = projectile;
            projectile = null;
            MissionDemolition.ShotFired();
            ProjectileLine.S.poi = projectile;
            Invoke("ResetRubberBand",0.1f);
        }
    }

void UpdateRubberBand() {
        if (rubber == null || firstPoint == null || secondPoint == null || projectile == null) return;

        rubber.SetPosition(0, firstPoint.position);  // Left anchor
        rubber.SetPosition(1, projectile.transform.position); // Projectile
        rubber.SetPosition(2, secondPoint.position);  // Right anchor
    }

    void ResetRubberBand() {
        if (rubber == null) return;
        rubber.SetPosition(1, (firstPoint.position + secondPoint.position) / 2); // Reset to middle
    }
}








