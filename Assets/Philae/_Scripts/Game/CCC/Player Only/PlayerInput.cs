using Philae.Core;
using UnityEngine;
using UnityEssentials.PropertyAttribute.readOnly;

namespace Philae.CCC
{
    /// <summary>
    /// InputPlayer Description
    /// </summary>
    public class PlayerInput : EntityAction
    {
        [Tooltip("input for moving Camera horizontally"), ReadOnly]
        public Vector2 cameraInput;

        [Tooltip("input for moving Camera horizontally"), ReadOnly]
        public Vector2 mouseInput;

        [Tooltip("input for moving Camera horizontally"), ReadOnly]
        public bool focus;
        [Tooltip("input for moving Camera horizontally"), ReadOnly]
        public bool focusUp;

        [Tooltip("input for dash"), ReadOnly]
        public bool dash;
        [Tooltip("input for dash"), ReadOnly]
        public bool dashUp;

        [Tooltip("input for moving Camera horizontally"), ReadOnly]
        public float trigger;

        [Tooltip("id unique du joueur correspondant à sa manette"), SerializeField]
        protected PlayerController playerController;
        public PlayerController PlayerController { get { return (playerController); } }



        private bool enableScript = true;

        private void OnEnable()
        {
            EventManager.StartListening(GameData.Event.GameOver, GameOver);
        }

        /// <summary>
        /// retourne si le joueur se déplace ou pas
        /// </summary>
        /// <returns></returns>
        public bool NotMovingCamera(float margin = 0)
        {
            if (GetCameraInput().magnitude <= margin)
                return (true);
            return (false);
        }

        /// <summary>
        /// tout les input du jeu, à chaque update
        /// </summary>
        private void GetInput()
        {
            if (enableScript)
            {
                //all axis
                moveInput = new Vector2(Input.GetAxis("Horizontal"),
                    Input.GetAxis("Vertical"));

                //all button
                Jump = Input.GetButton("Jump");
                JumpUp = Input.GetButtonUp("Jump");

                trigger = Input.GetAxis("Trigger Zoom");

                mouseInput = new Vector2(
                Input.GetAxisRaw("Mouse X"),
                Input.GetAxisRaw("Mouse Y"));
            }

            cameraInput = new Vector2(
                Input.GetAxis("Horizontal 2"),
                Input.GetAxis("Vertical 2"));
        }

        /// <summary>
        /// return the input of the camera (mouse or right stick of gamePad ?)
        /// </summary>
        /// <returns></returns>
        public Vector2 GetCameraInput()
        {
            return (cameraInput);
        }

        private void Update()
        {
            GetInput();
        }

        private void GameOver()
        {
            enableScript = false;
            this.enabled = false;
        }

        private void OnDisable()
        {
            EventManager.StartListening(GameData.Event.GameOver, GameOver);
        }
    }
}