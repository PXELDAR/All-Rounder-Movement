using UnityEngine;

namespace PXELDAR
{
    public class PhysicsSphere : MonoBehaviour
    {
        //=======================================================================================

        [SerializeField][Range(0f, 100f)] private float _maxSpeed = 5f;
        [SerializeField, Range(0f, 10f)] private float _jumpHeight = 2f;
        [SerializeField, Range(0f, 100f)] private float _maxAcceleration = 10f;
        [SerializeField, Range(0f, 100f)] private float _maxAirAcceleration = 10f;
        [SerializeField, Range(0f, 90f)] private float _maxGroundAngle = 25f;
        [SerializeField, Range(0, 5)] private int _maxAirJumps = 0;

        private int _jumpPhase;
        private int _groundContactCount;
        private float _minGroundDotProduct;
        private bool _desiredJump;
        private bool _onGround => _groundContactCount > 0;
        private Vector3 _velocity;
        private Vector3 _desiredVelocity;
        private Vector3 _contactNormal;
        private Rigidbody _rigidBody;


        //=======================================================================================

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            OnValidate();
        }

        //=======================================================================================

        void OnValidate()
        {
            _minGroundDotProduct = Mathf.Cos(_maxGroundAngle * Mathf.Deg2Rad);
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
            AdjustVelocity();

            if (_desiredJump)
            {
                _desiredJump = false;
                Jump();
            }

            _rigidBody.velocity = _velocity;
            ClearState();
        }

        //=======================================================================================

        private void UpdateState()
        {
            _velocity = _rigidBody.velocity;

            if (_onGround)
            {
                _jumpPhase = 0;
                if (_groundContactCount > 1)
                {
                    _contactNormal.Normalize();
                }
            }
            else
            {
                _contactNormal = Vector3.up;
            }
        }

        //=======================================================================================

        private void Jump()
        {
            if (_onGround || _jumpPhase < _maxAirJumps)
            {
                _jumpPhase++;
                float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * _jumpHeight);
                float alignedSpeed = Vector3.Dot(_velocity, _contactNormal);

                if (alignedSpeed > 0f)
                {
                    jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
                }

                _velocity += _contactNormal * jumpSpeed;
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
                if (normal.y >= _minGroundDotProduct)
                {
                    _groundContactCount++;
                    _contactNormal += normal;
                }
                else
                {
                    _contactNormal = Vector3.up;
                }
            }
        }

        //=======================================================================================

        void AdjustVelocity()
        {
            Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
            Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

            float currentX = Vector3.Dot(_velocity, xAxis);
            float currentZ = Vector3.Dot(_velocity, zAxis);

            float acceleration = _onGround ? _maxAcceleration : _maxAirAcceleration;
            float maxSpeedChange = acceleration * Time.deltaTime;

            float newX =
                Mathf.MoveTowards(currentX, _desiredVelocity.x, maxSpeedChange);
            float newZ =
                Mathf.MoveTowards(currentZ, _desiredVelocity.z, maxSpeedChange);

            _velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
        }

        //=======================================================================================

        private Vector3 ProjectOnContactPlane(Vector3 vector)
        {
            return vector - _contactNormal * Vector3.Dot(vector, _contactNormal);
        }

        //=======================================================================================

        private void ClearState()
        {
            _groundContactCount = 0;
            _contactNormal = Vector3.zero;
        }

        //=======================================================================================

    }
}