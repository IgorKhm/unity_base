using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter2D : MonoBehaviour
{
    public Transform muzzle;
    public Projectile projectilePrefab;
    public float fireCooldown = 0.15f;

    private PlayerInput playerInput;
    private InputAction fireAction;

    private float nextFireTime;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        fireAction = playerInput.currentActionMap.FindAction("Fire", false);
    }

    void Update()
    {

        if (muzzle == null || projectilePrefab == null || fireAction == null) return;


        if (Time.time < nextFireTime) return;

        if (fireAction.WasPressedThisFrame())
        {
            nextFireTime = Time.time + fireCooldown;

            Projectile p = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
            AudioManager.I?.PlayShoot();

            // In 2D, "right" is your forward aim direction because we rotate GunPivot.right
            Vector2 dir = muzzle.right;
            p.Launch(dir);
        }
    }
}