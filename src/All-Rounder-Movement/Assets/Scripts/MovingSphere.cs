using UnityEngine;

namespace PXELDAR
{
    public class MovingSphere : MonoBehaviour
    {
        //=======================================================================================

        [SerializeField][Range(0f, 100f)] private float _maxSpeed = 5f;
        [SerializeField][Range(0f, 100f)] private float _maxAcceleration = 10f;
        [SerializeField, Range(0f, 1f)] private float _bounciness = 0.5f;
        [SerializeField] private Rect _allowedArea = new Rect(-4.5f, -4.5f, 9f, 9f);
        private Vector3 _velocity;

        //=======================================================================================

        private void Update()
        {
            Vector2 playerInput;

            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");
            playerInput = Vector2.ClampMagnitude(playerInput, 1F);

            Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * _maxSpeed;
            float maxSpeedChange = _maxAcceleration * Time.deltaTime;

            _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, maxSpeedChange);
            _velocity.z = Mathf.MoveTowards(_velocity.z, desiredVelocity.z, maxSpeedChange);

            Vector3 displacement = _velocity * Time.deltaTime;
            Vector3 newPosition = transform.localPosition + displacement;

            if (newPosition.x < _allowedArea.xMin)
            {
                newPosition.x = _allowedArea.xMin;
                _velocity.x = -_velocity.x * _bounciness;
            }
            else if (newPosition.x > _allowedArea.xMax)
            {
                newPosition.x = _allowedArea.xMax;
                _velocity.x = -_velocity.x * _bounciness;
            }
            if (newPosition.z < _allowedArea.yMin)
            {
                newPosition.z = _allowedArea.yMin;
                _velocity.z = -_velocity.z * _bounciness;
            }
            else if (newPosition.z > _allowedArea.yMax)
            {
                newPosition.z = _allowedArea.yMax;
                _velocity.z = -_velocity.z * _bounciness;
            }

            transform.localPosition = newPosition;
        }

        //=======================================================================================
    }
}