using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [HideInInspector] public Transform playerTransform;

    public Vector3 offset = new Vector3(0, 0, 50); // Przesunięcie względem gracza (np. 50 jednostek przed)

    void Update()
    {
        
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerGameObject.transform;

        if (playerTransform != null)
        {
            // Ustaw pozycję Spawnera w oparciu o pozycję gracza i jego kierunek do przodu
            // offset.z steruje odległością przed graczem
            // offset.y steruje wysokością nad graczem
            transform.position = playerTransform.position + playerTransform.forward * offset.z + playerTransform.up * offset.y;

            // Opcjonalnie: aby spawner był zawsze skierowany w tym samym kierunku co gracz
            // (może być przydatne, jeśli spawnForwardOffset jest używane)
            transform.rotation = playerTransform.rotation;
        }
    }
}