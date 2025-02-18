using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public enum ProjectileType { emp, bullet }
    public ProjectileType type;
    public float stopScale = 2f; // Maximum scale before disabling
    public float scaleDuration = 1.5f; // Time to grow

    private EnemyData eData;

    float waitDiableTime;

    public void Setup(Vector3 shootDir, float bulletForce, float destroyTime, EnemyData enemyData)
    {
        eData = enemyData;
        waitDiableTime = eData.projectileShelfLife;

        if (type == ProjectileType.bullet)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(shootDir * bulletForce, ForceMode.Impulse);
            StartCoroutine(DisableWithTime());
        }
        else if (type == ProjectileType.emp)
        {
            transform.eulerAngles = Vector3.zero;
            transform.localScale = Vector3.zero; // Start from 0 scale
            StartCoroutine(GrowAndDisable());
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IDamagable damagable = Player_v2.Instance.GetComponent<IDamagable>();
            damagable?.TakeDamage(eData.damage);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player"))
        {
            IDamagable damagable = Player_v2.Instance.GetComponent<IDamagable>();
            damagable?.TakeDamage(eData.damage);
            gameObject.SetActive(false);
        }
    }
    
    

    private IEnumerator GrowAndDisable()
    {
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = new Vector3(stopScale, stopScale, stopScale);
        float elapsedTime = 0f;

        while (elapsedTime < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        gameObject.SetActive(false);
    }

    private IEnumerator DisableWithTime()
    {
        yield return new WaitForSeconds(waitDiableTime);
        gameObject.SetActive(false);
    }
}
