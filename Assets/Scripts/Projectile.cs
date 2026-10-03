using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] private float destroyAfterSeconds = 10f;

    private Vector3 startPosition;

    [SerializeField] private float maxDistance = 5f;
    private void Start() {
        Destroy(gameObject, destroyAfterSeconds);
        startPosition = transform.position;
    }
    private void Update() {
        float distanceTravelled = Vector3.Distance(startPosition, transform.forward);

        if (distanceTravelled >= maxDistance) {
            Destroy(gameObject);

        }
    }
}
