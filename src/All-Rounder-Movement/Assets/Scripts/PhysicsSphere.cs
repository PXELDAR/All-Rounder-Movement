using UnityEngine;

namespace PXELDAR
{
    public class PhysicsSphere : MonoBehaviour
    {
        //=======================================================================================

        [SerializeField][Range(0f, 100f)] private float _maxSpeed = 5f;
        [SerializeField][Range(0f, 100f)] private float _maxAcceleration = 10f;
        private Vector3 _velocity;
        private Vector3 _desiredVelocity;
        private Rigidbody _rigidBody;

        //=======================================================================================

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        //=======================================================================================

        private void Update()
        {
            Vector2 playerInput;

            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");
            playerInput = Vector2.ClampMagnitude(playerInput, 1F);

            _desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * _maxSpeed;
        }

        //=======================================================================================

        private void FixedUpdate()
        {
            _velocity = _rigidBody.velocity;

            float maxSpeedChange = _maxAcceleration * Time.deltaTime;
            _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, maxSpeedChange);
            _velocity.z = Mathf.MoveTowards(_velocity.z, _desiredVelocity.z, maxSpeedChange);

            _rigidBody.velocity = _velocity;
        }

        //=======================================================================================
    }
}