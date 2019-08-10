using UnityEngine;

namespace Player
{
    public class PlayerFreeLook : MonoBehaviour
    {
        [Tooltip("Size of Circle around player where mouse cant move cursor (a deadzone)")]
        [Range(0f, 1f)]
        public float aimDeadzone = 0f;
        [Tooltip("Max distance cam target can move from player horizontally")]
        public float maxDistanceFromPlayerX = 3.5f;
        [Tooltip("Max distance cam target can move from player vertically")]
        public float maxDistanceFromPlayerY = 4f;
        [Tooltip("Transform of the player so the cam target can follow")]
        public Rigidbody2D playerRigidbody;

        private Camera mainCam;

        void Awake()
        {
            transform.position = playerRigidbody.position;
        }

        void Start()
        {
            mainCam = Camera.main;
        }

        void Update()
        {
            Move();
        }

        /// <summary>
        /// Move the cam target to follow the player and be offset
        /// based on the mouse's position on the screen
        /// </summary>
        private void Move()
        {
            //The mouse's position in screen space where (0,0) is bottom left, and (1,1) is top right
            Vector2 mousePercent = mainCam.ScreenToViewportPoint(Input.mousePosition);

            //Shift that position so that (0,0) is actually in the center of the screen
            Vector2 shiftedPercent = (mousePercent - new Vector2(.5f, .5f));

            //Apply the deadzone by subtracting its size from the mouse's current offset
            Vector2 scaledShiftedPercent = shiftedPercent - (shiftedPercent.normalized * aimDeadzone);

            //Shift it back to origin (0,0) is at bottom left corner 
            //but now with the deadzone taken into account
            Vector2 scaledPercent = (scaledShiftedPercent + new Vector2(.5f, .5f));

            //The distance of mouse from center of screen (for enforcing deadzone)
            float unitsFromCenter = (mousePercent - new Vector2(.5f, .5f)).magnitude;

            //Enforce the deadzone
            Vector3 moveVec = Vector3.zero;
            if (unitsFromCenter < aimDeadzone)
            {
                moveVec = Vector3.zero;
            }
            //Else, move cam target based on scaledPercent
            else
            {
                float moveX = Mathf.Lerp(-maxDistanceFromPlayerX, maxDistanceFromPlayerX, scaledPercent.x);
                float moveZ = Mathf.Lerp(-maxDistanceFromPlayerY, maxDistanceFromPlayerY, scaledPercent.y);

                moveVec = new Vector3(moveX, moveZ, 0f);
            }

            transform.position = playerRigidbody.position + moveVec.ToVector2();
        }
    }
}
