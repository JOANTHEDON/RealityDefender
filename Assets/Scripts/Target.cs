using UnityEngine;

public class Target : MonoBehaviour {
    [SerializeField] private GameObject explosionPrefab;

    private bool destroyed = false;

    private void OnCollisionEnter(Collision collision) {
        if (destroyed)
            return;

        if (collision.gameObject.CompareTag("Projectile")) {
            destroyed = true;

            // Spawn explosion
            if (explosionPrefab != null) {
                Instantiate(
                    explosionPrefab,
                    transform.position,
                    Quaternion.identity
                );
            }

            // Destroy projectile
            Destroy(collision.gameObject);

            // Update score
            if (GameManager.Instance != null) {
                GameManager.Instance.TargetDestroyed();
            }

            // Destroy target
            Destroy(gameObject);
        }
    }
}