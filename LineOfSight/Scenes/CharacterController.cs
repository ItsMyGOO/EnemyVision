using UnityEngine;

namespace LineOfSight
{
    public class CharacterController : MonoBehaviour
    {
        Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            Transform cameraTrans = Camera.main.transform;

            Vector3 forward = cameraTrans.rotation.eulerAngles.x == 90 ? cameraTrans.up : cameraTrans.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = cameraTrans.right;

            Vector3 velocity = Input.GetAxis("Vertical") * forward + Input.GetAxis("Horizontal") * right;
            velocity *= 5;
            rb.velocity = velocity;
        }
    }
}