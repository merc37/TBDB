using UnityEngine;

public class DontGoThroughThings : MonoBehaviour {
    // Careful when setting this to true - it might cause double
    // events to be fired - but it won't pass through the trigger
    public bool sendTriggerMessage = false;

    [SerializeField]
    private GameObject impactAnim;

    public LayerMask layerMask = -1; //make sure we aren't in this layer 
    public float skinWidth = 0.1f; //probably doesn't need to be changed 

    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector2 previousPosition;
    private Rigidbody2D myRigidbody;
    private Collider2D myCollider;

    //initialize values 
    void Start() {
        layerMask = LayerMask.NameToLayer("Block");
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        previousPosition = myRigidbody.position;
        minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
        partialExtent = minimumExtent * (1.0f - skinWidth);
        sqrMinimumExtent = minimumExtent * minimumExtent;
    }

    void FixedUpdate() {
        //have we moved more than our minimum extent? 
        Vector2 movementThisStep = myRigidbody.position - previousPosition; ;
        float movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if (movementSqrMagnitude > sqrMinimumExtent) {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            RaycastHit2D hitInfo;

            //check for obstructions we might have missed 
            //if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value)) {
            if (hitInfo = Physics2D.Raycast(previousPosition, movementThisStep, movementMagnitude, layerMask.value)) {
                if (!hitInfo.collider || hitInfo.collider == myCollider) return;

                if (hitInfo.collider.isTrigger) {
                    hitInfo.collider.SendMessage("OnTriggerEnter2D", myCollider);
                }

                if (myCollider.isTrigger) {
                    Debug.Log("Shot position:" + transform.position + ", Raycast hit:" + hitInfo.point + ", Object hit:" + hitInfo.collider.name);
                    myCollider.SendMessage("OnTriggerEnter2D", hitInfo.collider);
                    //myCollider.SendMessage("OnImpact", hitInfo.point);
                    // GameObject impact = (GameObject)Instantiate(impactAnim, hitInfo.point+0.5f*hitInfo.normal, Quaternion.LookRotation(transform.forward, -hitInfo.normal));
                }

                if (!hitInfo.collider.isTrigger && !myCollider.isTrigger) {
                    myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
                }
            }
        }

        previousPosition = myRigidbody.position;
    }
}