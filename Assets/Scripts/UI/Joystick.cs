using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FreeFire.UI
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Joystick Settings")]
        public float joystickRange = 50f;
        public float deadZone = 0.1f;
        public bool snapToCenter = true;
        public float snapSpeed = 10f;

        [Header("Visual Settings")]
        public Image background;
        public Image handle;
        public Color normalColor = Color.white;
        public Color pressedColor = Color.gray;

        [Header("Events")]
        public System.Action<Vector2> OnJoystickMoved;
        public System.Action OnJoystickPressed;
        public System.Action OnJoystickReleased;

        // Private variables
        private Vector2 centerPosition;
        private Vector2 currentPosition;
        private bool isPressed = false;
        private RectTransform rectTransform;
        private RectTransform handleRect;

        // Public properties
        public Vector2 Direction { get; private set; }
        public float Magnitude { get; private set; }
        public bool IsPressed => isPressed;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            handleRect = handle.GetComponent<RectTransform>();
            
            // Store center position
            centerPosition = rectTransform.anchoredPosition;
            currentPosition = centerPosition;
            
            // Set initial handle position
            if (handleRect != null)
            {
                handleRect.anchoredPosition = Vector2.zero;
            }
        }

        void Update()
        {
            if (isPressed && snapToCenter)
            {
                // Smooth return to center when released
                currentPosition = Vector2.Lerp(currentPosition, centerPosition, snapSpeed * Time.deltaTime);
                UpdateHandlePosition();
                UpdateDirection();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            OnJoystickPressed?.Invoke();
            
            // Change visual state
            if (background != null)
            {
                background.color = pressedColor;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            OnJoystickReleased?.Invoke();
            
            // Reset to center
            if (snapToCenter)
            {
                currentPosition = centerPosition;
            }
            
            // Reset direction
            Direction = Vector2.zero;
            Magnitude = 0f;
            
            // Reset visual state
            if (background != null)
            {
                background.color = normalColor;
            }
            
            // Reset handle position
            if (handleRect != null)
            {
                handleRect.anchoredPosition = Vector2.zero;
            }
            
            OnJoystickMoved?.Invoke(Direction);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isPressed) return;

            // Convert screen position to local position
            Vector2 localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPosition
            );

            // Calculate direction from center
            Vector2 direction = localPosition.normalized;
            float distance = Vector2.Distance(Vector2.zero, localPosition);

            // Clamp to joystick range
            if (distance > joystickRange)
            {
                localPosition = direction * joystickRange;
                distance = joystickRange;
            }

            currentPosition = localPosition;
            UpdateHandlePosition();
            UpdateDirection();
        }

        void UpdateHandlePosition()
        {
            if (handleRect != null)
            {
                handleRect.anchoredPosition = currentPosition;
            }
        }

        void UpdateDirection()
        {
            // Calculate direction and magnitude
            Direction = currentPosition.normalized;
            Magnitude = currentPosition.magnitude / joystickRange;

            // Apply dead zone
            if (Magnitude < deadZone)
            {
                Direction = Vector2.zero;
                Magnitude = 0f;
            }

            // Notify listeners
            OnJoystickMoved?.Invoke(Direction);
        }

        // Public methods for external control
        public void SetJoystickRange(float range)
        {
            joystickRange = range;
        }

        public void SetDeadZone(float deadZoneValue)
        {
            deadZone = Mathf.Clamp01(deadZoneValue);
        }

        public void SetVisualColors(Color normal, Color pressed)
        {
            normalColor = normal;
            pressedColor = pressed;
            
            if (background != null && !isPressed)
            {
                background.color = normalColor;
            }
        }

        public void ResetJoystick()
        {
            isPressed = false;
            currentPosition = centerPosition;
            Direction = Vector2.zero;
            Magnitude = 0f;
            
            if (handleRect != null)
            {
                handleRect.anchoredPosition = Vector2.zero;
            }
            
            if (background != null)
            {
                background.color = normalColor;
            }
        }

        // Utility methods
        public Vector2 GetDirection()
        {
            return Direction;
        }

        public float GetMagnitude()
        {
            return Magnitude;
        }

        public bool IsInDeadZone()
        {
            return Magnitude < deadZone;
        }

        public Vector2 GetRawDirection()
        {
            return currentPosition.normalized;
        }

        public float GetRawMagnitude()
        {
            return currentPosition.magnitude / joystickRange;
        }
    }
}
