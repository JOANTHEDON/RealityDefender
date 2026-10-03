using UnityEngine;

public class WeaponController : MonoBehaviour {
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float projectileSpeed = 15f;

    public void Fire() {
        if (projectilePrefab == null || firePoint == null) return;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.linearVelocity = firePoint.forward * projectileSpeed;
        }
    }
}
