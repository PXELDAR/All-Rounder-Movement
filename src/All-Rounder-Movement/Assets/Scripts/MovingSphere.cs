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
        Vector3 velocity;

        //=======================================================================================

        private void Update()
        {
            Vector2 playerInput;

            playerInput.x = Input.GetAxis("Horizontal");
            playerInput.y = Input.GetAxis("Vertical");
            playerInput = Vector2.ClampMagnitude(playerInput, 1F);

            Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * _maxSpeed;
            float maxSpeedChange = _maxAcceleration * Time.deltaTime;

            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

            Vector3 displacement = velocity * Time.deltaTime;
            Vector3 newPosition = transform.localPosition + displacement;

            // if (!_allowedArea.Contains(new Vector2(newPosition.x, newPosition.z)))
            // {
            //     newPosition.x = Mathf.Clamp(newPosition.x, _allowedArea.xMin, _allowedArea.xMax);
            //     newPosition.z = Mathf.Clamp(newPosition.z, _allowedArea.yMin, _allowedArea.yMax);
            // }

            if (newPosition.x < _allowedArea.xMin)
            {
                newPosition.x = _allowedArea.xMin;
                velocity.x = -velocity.x * _bounciness;
            }
            else if (newPosition.x > _allowedArea.xMax)
            {
                newPosition.x = _allowedArea.xMax;
                velocity.x = -velocity.x * _bounciness;
            }
            if (newPosition.z < _allowedArea.yMin)
            {
                newPosition.z = _allowedArea.yMin;
                velocity.z = -velocity.z * _bounciness;
            }
            else if (newPosition.z > _allowedArea.yMax)
            {
                newPosition.z = _allowedArea.yMax;
                velocity.z = -velocity.z * _bounciness;
            }

            transform.localPosition = newPosition;
        }

        //=======================================================================================
    }
}