using UnityEngine;

namespace PXELDAR
{
    public class PhysicsSphere : MonoBehaviour
    {
        //=======================================================================================

        [SerializeField][Range(0f, 100f)] private float _maxSpeed = 5f;
        [SerializeField, Range(0f, 10f)] private float _jumpHeight = 2f;
        [SerializeField, Range(0, 5)] private int _maxAirJumps = 0;
        [SerializeField, Range(0f, 100f)] private float _maxAcceleration = 10f;
        [SerializeField, Range(0f, 100f)] private float _maxAirAcceleration = 10f;

        private int _jumpPhase;
        private Vector3 _velocity;
        private Vector3 _desiredVelocity;
        private Rigidbody _rigidBody;
        private bool _desiredJump;
        private bool _onGround;

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

            _desiredJump |= Input.GetButtonDown("Jump");
        }

        //=======================================================================================

        private void FixedUpdate()
        {
            UpdateState();

            float acceleration = _onGround ? _maxAcceleration : _maxAirAcceleration;
            float maxSpeedChange = acceleration * Time.deltaTime;
            _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, maxSpeedChange);
            _velocity.z = Mathf.MoveTowards(_velocity.z, _desiredVelocity.z, maxSpeedChange);

            if (_desiredJump)
            {
                _desiredJump = false;
                Jump();
            }

            _rigidBody.velocity = _velocity;
            _onGround = false;
        }

        //=======================================================================================

        private void UpdateState()
        {
            _velocity = _rigidBody.velocity;

            if (_onGround)
            {
                _jumpPhase = 0;
            }
        }

        //=======================================================================================

        private void Jump()
        {
            if (_onGround || _jumpPhase < _maxAirJumps)
            {
                _jumpPhase++;
                float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * _jumpHeight);

                if (_velocity.y > 0f)
                {
                    jumpSpeed = Mathf.Max(jumpSpeed - _velocity.y, 0f);
                }

                _velocity.y += jumpSpeed;
            }
        }

        //=======================================================================================

        private void OnCollisionEnter(Collision collision)
        {
            EvaluateCollision(collision);
        }


        //=======================================================================================

        private void OnCollisionStay(Collision collision)
        {
            EvaluateCollision(collision);
        }

        //=======================================================================================

        private void EvaluateCollision(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                _onGround |= normal.y > 0.9f;
            }
        }

        //=======================================================================================

    }
}