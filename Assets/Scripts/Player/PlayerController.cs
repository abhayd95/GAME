/*
 * Copyright (c) 2024 abhay virus. All rights reserved.
 * 
 * This file is part of the Free Fire Clone game.
 * No part of this software may be reproduced, distributed, or transmitted
 * without the prior written permission of the copyright owner.
 */

using UnityEngine;
using UnityEngine.UI;

namespace FreeFire.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float walkSpeed = 5f;
        public float runSpeed = 8f;
        public float jumpHeight = 3f;
        public float gravity = -9.81f;
        public float groundCheckDistance = 0.4f;
        public LayerMask groundMask;

        [Header("Camera Settings")]
        public Transform cameraTarget;
        public float mouseSensitivity = 100f;
        public float maxLookAngle = 80f;

        [Header("Mobile Controls")]
        public Joystick movementJoystick;
        public Button jumpButton;
        public Button crouchButton;
        public Button reloadButton;

        [Header("Animation")]
        public Animator animator;

        // Private variables
        private CharacterController controller;
        private Vector3 velocity;
        private bool isGrounded;
        private bool isCrouching;
        private bool isRunning;
        private float xRotation = 0f;
        private Transform playerBody;

        // Input variables
        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool jumpInput;
        private bool crouchInput;
        private bool reloadInput;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            playerBody = transform;

            // Lock cursor for desktop
            if (!Application.isMobilePlatform)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            // Setup mobile controls
            SetupMobileControls();
        }

        void Update()
        {
            HandleInput();
            HandleMovement();
            HandleLook();
            HandleAnimation();
        }

        void HandleInput()
        {
            if (Application.isMobilePlatform)
            {
                // Mobile input
                moveInput = movementJoystick.Direction;
                jumpInput = jumpButton != null && jumpButton.GetComponent<Button>().interactable;
                crouchInput = crouchButton != null && crouchButton.GetComponent<Button>().interactable;
                reloadInput = reloadButton != null && reloadButton.GetComponent<Button>().interactable;
            }
            else
            {
                // Desktop input
                moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                jumpInput = Input.GetButtonDown("Jump");
                crouchInput = Input.GetKey(KeyCode.LeftControl);
                reloadInput = Input.GetKeyDown(KeyCode.R);
                lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }

            isRunning = Input.GetKey(KeyCode.LeftShift) || (Application.isMobilePlatform && moveInput.magnitude > 0.8f);
        }

        void HandleMovement()
        {
            // Ground check
            isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // Movement
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            float currentSpeed = isRunning ? runSpeed : walkSpeed;
            
            if (isCrouching)
            {
                currentSpeed *= 0.5f;
            }

            controller.Move(move * currentSpeed * Time.deltaTime);

            // Jumping
            if (jumpInput && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Crouching
            if (crouchInput && isGrounded)
            {
                isCrouching = true;
                controller.height = 1f;
            }
            else
            {
                isCrouching = false;
                controller.height = 2f;
            }

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        void HandleLook()
        {
            if (Application.isMobilePlatform)
            {
                // Mobile look will be handled by touch input
                return;
            }

            // Desktop look
            xRotation -= lookInput.y * mouseSensitivity * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

            cameraTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * lookInput.x * mouseSensitivity * Time.deltaTime);
        }

        void HandleAnimation()
        {
            if (animator == null) return;

            animator.SetFloat("Speed", moveInput.magnitude);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsCrouching", isCrouching);
            animator.SetBool("IsRunning", isRunning);
        }

        void SetupMobileControls()
        {
            if (Application.isMobilePlatform)
            {
                // Setup button events
                if (jumpButton != null)
                {
                    jumpButton.onClick.AddListener(() => jumpInput = true);
                }
                if (crouchButton != null)
                {
                    crouchButton.onClick.AddListener(() => crouchInput = !crouchInput);
                }
                if (reloadButton != null)
                {
                    reloadButton.onClick.AddListener(() => reloadInput = true);
                }
            }
        }

        public void SetLookInput(Vector2 input)
        {
            lookInput = input;
        }

        public bool IsGrounded => isGrounded;
        public bool IsCrouching => isCrouching;
        public bool IsRunning => isRunning;
    }
}
