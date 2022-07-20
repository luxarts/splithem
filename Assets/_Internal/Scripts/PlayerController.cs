using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Objects")]
    public Camera head;
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip hitSound;
    public AudioSource backgroundMusic;
    public Image muteStatus;
    public GameObject projectile;

    [Header("Settings")]
    public float cameraSensitivity = 0.07f;
    public float movementSpeed = 0.2f;
    
    public float forceBack = 1f;
    public float gravity = -9.81f;

    [Header("Debug")]
    public bool showRay = false;

    private CharacterController characterController;
    private Vector2 look;
    private Vector2 move;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController = gameObject.GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        if (GameHandler.Instance.isPaused) return;
        // Player movement
        Vector3 moveDirection = new Vector3(move.x * movementSpeed, 0, move.y * movementSpeed);
        moveDirection.y = gravity * Time.fixedDeltaTime;
        characterController.Move(transform.TransformDirection(moveDirection));
        
        if (showRay)
        {
            Ray r = new Ray(head.transform.position, head.transform.forward);
            RaycastHit hit;
            Physics.Raycast(r, out hit);
            
            Debug.DrawRay(r.origin, r.direction * hit.distance, Color.red);
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (GameHandler.Instance.isPaused) return;

        look = context.ReadValue<Vector2>();
        HandleLook();
    }
    public void Move(InputAction.CallbackContext context)
    {
        if (GameHandler.Instance.isPaused) return;

        move = context.ReadValue<Vector2>();
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (GameHandler.Instance.isPaused) return;

        if (context.performed) {
            Ray r = new Ray(head.transform.position, head.transform.forward);
            RaycastHit hit;

            //if (Physics.Raycast(r, out hit))
            if (Physics.SphereCast(r, 1f, out hit))
            {
                if (hit.collider.CompareTag("face"))
                {
                    GameHandler.Instance.EnemyHit(hit.collider, r);

                    audioSource.PlayOneShot(hitSound);
                    audioSource.PlayOneShot(fireSound);
                } else
                {
                    audioSource.PlayOneShot(fireSound);
                }
                GameHandler.Instance.AddShoot();
            }
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (!GameHandler.Instance.isPaused)
        {
            GameHandler.Instance.ShowPause();
        } else
        {
            GameHandler.Instance.ShowHUD();
        }
    }

    public void Mute(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            backgroundMusic.gameObject.SetActive(!backgroundMusic.gameObject.activeInHierarchy);
            muteStatus.gameObject.SetActive(!muteStatus.gameObject.activeInHierarchy);
        }
    }

    private void HandleLook()
    {
        // Rotate head
        float xRotation = head.transform.localRotation.eulerAngles.x;
        xRotation -= look.y * cameraSensitivity;
        head.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        
        // Rotate player
        transform.Rotate(new Vector3(0, look.x * cameraSensitivity, 0));
    }

    IEnumerator SpawnProjectile(Vector3 origin, Vector3 direction, float distance)
    {
        
        yield return null;
    }
}
