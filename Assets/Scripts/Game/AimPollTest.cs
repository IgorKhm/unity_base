using UnityEngine;
using UnityEngine.InputSystem;

public class AimPollTest : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current != null)
            Debug.Log(Mouse.current.position.ReadValue());
    }
}