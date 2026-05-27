//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PlayerControl : MonoBehaviour
//{
//    public float speed = 2.5f;
//    public bool facingRight = true;
//    public bool multiDirectionAnims = false;
//    public bool hasDamagefx = false;

//    private Vector2 moveInput;

//    Animator animator;

//    public bool playHitAnim = false;
//    public bool playerHit = false;
//    public int healthLevel = 10;
//    public int maxhealthLevel = 20;
//    public int minhealthLevel = 0;

//    // Weapon switching
//    public GameObject meleeWeapon;
//    public GameObject rangedWeapon;
//    public float meleeRange = 2f;

//    public GameObject playerFinish;
//    public GameObject playerDamage;

//    // Jump Variables
//    public Rigidbody2D rb;
//    public float jumpForce = 7f;
//    public bool isGrounded = true;

//    void Start()
//    {
//        animator = GetComponent<Animator>();
//        animator.SetInteger("playerState", 0);
//    }

//    // Safe legacy calls guarded by try/catch to avoid InvalidOperationException
//    private bool LegacyGetKey(KeyCode key)
//    {
//        try { return Input.GetKey(key); }
//        catch (System.InvalidOperationException) { return false; }
//    }

//    void Update()
//    {
//        // Read joystick first (touch). If no joystick or zero, fall back to keyboard.
//        Vector2 inputVec = joystick.Direction;

//        // Deadzone to prevent tiny joystick noise
//        if (inputVec.magnitude < 0.4f)
//            inputVec = Vector2.zero;

//        // Horizontal deadzone for cleaner flips
//        if (Mathf.Abs(inputVec.x) < 0.25f)
//            inputVec.x = 0;

//        //Debug.Log(inputVec);

//        // Movement
//        if (inputVec.sqrMagnitude > 0.0001f)
//        {
//            // Instant flip when changing horizontal direction
//            if (inputVec.x > 0.1f && !facingRight)
//                Flip();
//            else if (inputVec.x < -0.1f && facingRight)
//                Flip();

          

//            if (!multiDirectionAnims)
//                animator.SetInteger("playerState", 1);
//            else
//            {
//                if (Mathf.Abs(inputVec.x) >= Mathf.Abs(inputVec.y))
//                    animator.SetInteger("playerState", 1);
//                else if (inputVec.y > 0)
//                    animator.SetInteger("playerState", 5);
//                else
//                    animator.SetInteger("playerState", 6);
//            }

//            Vector3 move = new Vector3(inputVec.x, inputVec.y, 0f).normalized;
//            transform.position += move * speed * Time.deltaTime;
//        }
//        else
//        {
//            animator.SetInteger("playerState", 0);
//        }

//        if (playHitAnim)
//        {
//            animator.SetInteger("playerState", 2);
//            playHitAnim = false;
//        }

//        // --- AUTO WEAPON SWITCHING ---
//        GameObject enemy = FindClosestEnemy();

//        if (enemy == null)
//            SwitchToRanged();
//        else
//        {
//            float distance = Vector2.Distance(transform.position, enemy.transform.position);
//            if (distance <= meleeRange) SwitchToMelee(); else SwitchToRanged();
//        }
//    }

//    GameObject FindClosestEnemy()
//    {
//        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
//        GameObject closest = null;
//        float minDist = Mathf.Infinity;

//        foreach (GameObject enemy in enemies)
//        {
//            float dist = Vector2.Distance(transform.position, enemy.transform.position);
//            if (dist < minDist)
//            {
//                minDist = dist;
//                closest = enemy;
//            }
//        }

//        return closest;
//    }

//    void SwitchToMelee()
//    {
//        if (meleeWeapon) meleeWeapon.SetActive(true);
//        if (rangedWeapon) rangedWeapon.SetActive(false);
//    }

//    void SwitchToRanged()
//    {
//        if (meleeWeapon) meleeWeapon.SetActive(false);
//        if (rangedWeapon) rangedWeapon.SetActive(true);
//    }

//    void OnCollisionEnter2D(Collision2D col)
//    {
//        if (col.gameObject.tag == "Enemy")
//        {
//            if (healthLevel > minhealthLevel) healthLevel--;
//            if (healthLevel <= 0)
//            {
//                FindObjectOfType<UI_Controller>().RestartGame();
//            }

//            playHitAnim = true;
//            Debug.Log("hits enemy!");
//            PlayerHit();
//        }

//        if (col.gameObject.tag == "Health")
//        {
//            if (healthLevel < maxhealthLevel) healthLevel++;
//        }

//        if (col.collider.CompareTag("Ground"))
//        {
//            isGrounded = true;
//        }
//    }

//    void GameEnd()
//    {
//        Instantiate(playerFinish, transform.position, transform.rotation);
//        Destroy(GetComponent<Rigidbody2D>());
//        Debug.Log("finished!");
//    }

//    void Flip()
//    {
//        if (Time.timeScale == 1f)
//        {
//            facingRight = !facingRight;
//            Vector3 theScale = transform.localScale;
//            theScale.x *= -1;
//            transform.localScale = theScale;
//        }
//    }

//    void PlayerHit()
//    {
//        if (hasDamagefx)
//        {
//            Instantiate(playerDamage, transform.position, transform.rotation);
//            Debug.Log("player hit");
//        }
//    }

//    public void JumpButtonPressed()
//    {
//        if (isGrounded)
//        {
//            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
//            isGrounded = false;
//        }
//    }
    

//}


