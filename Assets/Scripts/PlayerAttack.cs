using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 25;
    
    private SpriteRenderer spriteRenderer;
    private float defaultXPos;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (attackPoint != null)
        {
            defaultXPos = Mathf.Abs(attackPoint.localPosition.x);
        }
    }

    void LateUpdate()
    {
        if (spriteRenderer != null && attackPoint != null)
        {
            if (spriteRenderer.flipX == true) 
            {
                attackPoint.localPosition = new Vector3(-defaultXPos, attackPoint.localPosition.y, attackPoint.localPosition.z);
            }
            else
            {
                attackPoint.localPosition = new Vector3(defaultXPos, attackPoint.localPosition.y, attackPoint.localPosition.z);
            }
        }
        
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}