using UnityEngine;

public class AnimateImpact : MonoBehaviour {

    [SerializeField]
    private GameObject impactAnim;

    private Rigidbody2D myRigidbody;
    private Collider2D myCollider;
    private Vector2 previousPosition;

    void Start () {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        previousPosition = myRigidbody.position;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate() {
        previousPosition = myRigidbody.position;
    }

    private float distanceToAxis(Vector3 point, Vector3 axis) {
        Vector3 component = Vector3.Project(point, axis.normalized);
        return (point - component).magnitude;
    }

    private Vector2 Rotate(Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    void OnCollisionEnter2D(Collision2D coll) {
        ContactPoint2D[] points = coll.contacts;
        ContactPoint2D closest = points[0];
        float min = 0;
        foreach (ContactPoint2D point in points) {
            Debug.DrawLine(point.point, transform.position, Color.red);
            float dist = distanceToAxis((Vector3)point.point - transform.position, myRigidbody.velocity);
            if (dist < min) {
                min = dist;
                closest = point;
            }
        }
        Debug.Log("Impact position:" + closest.point);
        GameObject impact = (GameObject)Instantiate(impactAnim, closest.point + 0.5f * closest.normal, Quaternion.LookRotation(transform.forward, -closest.normal));
    }

    void OnTriggerEnter2D(Collider2D coll) {
        Debug.Log("Animate");
        RaycastHit2D hitInfo;
        Vector2 movementThisStep = myRigidbody.position - previousPosition;
        Vector2 leftEdge = Vector3.Project(myCollider.bounds.extents, Rotate(myRigidbody.velocity.normalized, -90)); // relative to center
        float width = leftEdge.magnitude * 2;
        int[] checkSequence = { 50, 0, 100, 25, 75 }; // in percentage of sprite width, as a point to raycast from
        // Should really check all to see the closest, that must be the one it collided with
        // Also note this is based on direction of velocity
        foreach (int x in checkSequence) {
            Vector2 originPoint = previousPosition + leftEdge;
            Vector2 castPoint = originPoint + (Rotate(myRigidbody.velocity.normalized, 90) * (width * x / 100));
            if (hitInfo = Physics2D.Raycast(castPoint, movementThisStep, movementThisStep.magnitude * 2.0f)) {
                Debug.Log("hit");
                if (!hitInfo.collider || hitInfo.collider == myCollider) continue;
                Debug.Log("Impact position:" + hitInfo.point);
                GameObject impact = (GameObject)Instantiate(impactAnim, hitInfo.point + 0.5f * hitInfo.normal, Quaternion.LookRotation(transform.forward, -hitInfo.normal));
                return;
            }
        }
    }
}
