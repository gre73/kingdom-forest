using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    public int damageToPlayer = 10;
    public float moveSpeed = 2f;
    
    [Header("Стрибки")]
    public float jumpForce = 6f;
    public LayerMask groundLayer;

    [Header("Інтерфейс")]
    public Slider healthBar;

    [Header("Налаштування Атаки")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public float stoppingDistance = 0.1f; 
    public LayerMask playerLayer; 
    public float attackCooldown = 1.5f; 
    private float nextAttackTime = 0f;

    private Transform player;
    private Animator animator; 
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    
    private bool isDead = false; 
    private bool isHurt = false;
    private float defaultXPos; 

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (attackPoint != null)
            defaultXPos = Mathf.Abs(attackPoint.localPosition.x);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (isDead || player == null || isHurt) return;
        
        if (spriteRenderer != null && attackPoint != null)
        {
            if (spriteRenderer.flipX == true) 
                attackPoint.localPosition = new Vector3(-defaultXPos, attackPoint.localPosition.y, attackPoint.localPosition.z);
            else 
                attackPoint.localPosition = new Vector3(defaultXPos, attackPoint.localPosition.y, attackPoint.localPosition.z);
        }
        
        float distance = Mathf.Abs(transform.position.x - player.position.x);
        
        float heightDifference = Mathf.Abs(transform.position.y - player.position.y);

        if (distance > stoppingDistance || heightDifference > 1.5f) 
        {
            Vector2 target = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            animator.SetBool("isMoving", true); 
    
            CheckAndJump();
        }
        else
        {
            animator.SetBool("isMoving", false); 
    
            if (Time.time >= nextAttackTime)
            {
                EnemyAttack();
                nextAttackTime = Time.time + attackCooldown; 
            }
        }

        if (player.position.x > transform.position.x)
            spriteRenderer.flipX = false;
        else if (player.position.x < transform.position.x)
            spriteRenderer.flipX = true;
    }

    void CheckAndJump()
    {
        Vector2 forwardDir = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        
        Vector2 rayStart = transform.position + Vector3.up * 0.2f;
        RaycastHit2D wallHit = Physics2D.Raycast(rayStart, forwardDir, 0.6f, groundLayer);
        
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, 0.8f, groundLayer);
        
        if (wallHit.collider != null && groundHit.collider != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void EnemyAttack()
    {
        animator.SetTrigger("Attack"); 
        
        StartCoroutine(DealDamageAfterDelay(0.6f));
    }
    
    System.Collections.IEnumerator DealDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (isDead) yield break;
        
        if (attackPoint != null)
        {
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
            foreach (Collider2D p in hitPlayers)
            {
                PlayerHealth ph = p.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(damageToPlayer);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        
        animator.SetBool("isMoving", false); 
        animator.SetTrigger("Hurt"); 

        if (healthBar != null)
            healthBar.value = currentHealth;
    
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HurtStun());
        }
    }
    
    System.Collections.IEnumerator HurtStun()
    {
        isHurt = true;
        
        if (rb != null) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        
        yield return new WaitForSeconds(0.4f); 
    
        isHurt = false;
    }

    void Die()
    {
        isDead = true; 
        
        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(1);
        
        animator.SetTrigger("Death");
        
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        Destroy(gameObject, 2f); 
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        
        Gizmos.color = Color.yellow;
        Vector2 rayStart = transform.position + Vector3.up * 0.2f;
        Vector2 forwardDir = (GetComponent<SpriteRenderer>() != null && GetComponent<SpriteRenderer>().flipX) ? Vector2.left : Vector2.right;
        Gizmos.DrawLine(rayStart, rayStart + (forwardDir * 0.6f));
    }
}