using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool interact;

        [Header("Combat Inputs")]
        public bool punchLeft;
        public bool punchRight;
        public bool isHoldingPunchMobile; // <--- Nuestra variable móvil

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        [Header("Pruebas en PC")]
        [Tooltip("Marca esto para que Unity finja ser un celular mientras editas")]
        public bool simularMovilEnPC = true;

        [Header("Tutorial State")]
        public bool tutorialActivo = false;

        public bool cursorSiempreVisible = false;

        // --- NUESTRA FUNCIÓN DETECTORA ---
        private bool EsDispositivoMovil()
        {
            bool esMovil = SystemInfo.deviceType == DeviceType.Handheld && !UnityEngine.XR.XRSettings.isDeviceActive;
#if UNITY_EDITOR
            esMovil = simularMovilEnPC;
#endif
            return esMovil;
        }

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            if (tutorialActivo) return;
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (tutorialActivo) return;
            if(cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnInteract(InputValue value)
        {
            InteractInput(value.isPressed);
        }

        public void OnPunchLeft(InputValue value)
        {
            if (EsDispositivoMovil()) return; 
            PunchLeftInput(value.isPressed);
        }

        public void OnPunchRight(InputValue value)
        {
            if (EsDispositivoMovil()) return; 
            PunchRightInput(value.isPressed);
        }
#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        } 

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (tutorialActivo) return; 
            if (cursorSiempreVisible) return;
            SetCursorState(cursorLocked);
        }

        public void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public void InteractInput(bool newInteractState)
        {
            interact = newInteractState;
        }

        // --- LAS FUNCIONES CORREGIDAS (Sin duplicados) ---
        public void PunchLeftInput(bool newPunchState)
        {
            punchLeft = newPunchState;
            isHoldingPunchMobile = newPunchState; 
        }

        public void PunchRightInput(bool newPunchState)
        {
            punchRight = newPunchState;
            isHoldingPunchMobile = newPunchState; 
        }
    }
}