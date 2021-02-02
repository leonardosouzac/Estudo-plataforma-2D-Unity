using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //componentes
    private Rigidbody2D rig;
    public Animator anim;

    //configuração do ataque
    public Transform point;
    public float radius;

    //velocidade do movimento e força do pulo
    public float speed;
    public float jumpForce;


    //booleanos
    private bool isJumping;
    private bool doubleJump;
    private bool isAttacking;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //chama o metódo de pulo. (muitos bugs no fixedUpdate, deixei no update mesmo)
        Jump();
        //chama o método de atk
        Attack();
    }

    private void FixedUpdate()
    {
        //chama o método de movimentação
        Move();

    }
    void Move()
    {

        //SE NÃO PRESSIONAR NADA, RETORNA ZERO. SE PRESSIONAR DIREITA, RETORNA 1. ESQUERDA, -1.
        float movement = Input.GetAxis("Horizontal");

        rig.velocity = new Vector2(movement * speed, rig.velocity.y);

        if (movement == 0 && !isJumping && !isAttacking)
        {
            anim.SetInteger("transition", 0);
        }

        if (movement > 0)
        {
            if (!isJumping && !isAttacking)
            {
                anim.SetInteger("transition", 1);
                //vira o sprite para a direita caso o player esteja para a esquerda (180 graus).
                transform.eulerAngles = new Vector3(0, 0, 0);
            }

        }
        if (movement < 0)
        {
            if (!isJumping && !isAttacking)
            {
                //vira o sprite do player 180 graus quando X < 0, ou seja, quando o player anda para a esquerda.
                transform.eulerAngles = new Vector3(0, 180, 0);
                anim.SetInteger("transition", 1);
            }

        }
    }

    void Jump()
    {
        //SE isJumping É FALSO, O PLAYER PODE PULAR NOVAMENTE. APÓS O PULO, isJumping É ALTERADO PARA TRUE e o doubleJump também.
        if (Input.GetButtonDown("Jump"))
        {
            if (!isJumping)
            {
                rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isJumping = true;
                doubleJump = true;
                anim.SetInteger("transition", 2);

            }
            //após o primeiro pulo, doubleJump se torna true e será verificado na linha a baixo. sendo true, permite um segundo pulo e seta isJumping e doubleJump para false;
            else if (doubleJump)
            {
                rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                doubleJump = false;
                anim.SetInteger("transition", 2);
            }

        }
    }
    //método para o ataque do player
    void Attack()
    {
        
        //se clicar com o esquerdo do mouse
        if (Input.GetButtonDown("Fire1"))
        {
            isAttacking = true;
            anim.SetInteger("transition", 3);
            //é criado uma esfera com um collider2d na posição da variável point e com o raio passado na variável radius
            Collider2D hit = Physics2D.OverlapCircle(point.position, radius);
            //caso acerto algo, ou seja, colisão do hit não seja nula, é feito um debug.Log para mostrar qual objeto foi atingido
            if (hit != null)
            {
                Debug.Log(hit.name);
            }
            StartCoroutine (OnAttack());
        }
        
    }

    //método para controlar a animação do ataque por tempo.
    IEnumerator OnAttack()
    {
        yield return new WaitForSeconds(0.33f);
        isAttacking = false;
    }

    //esse método cria um gizmo para visualizar o raio do collider do Point
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(point.position, radius);
    }

    private void OnCollisionEnter2D(Collision2D colisor)
    {
        //QUANDO O COLISOR DO PLAYER COLIDE COM O COLISOR DO TILEMAP, isJumping É ALTERADO PARA FALSO E PERMITE QUE O PLAYER DÊ UM NOVO PULO.
        if (colisor.gameObject.layer == 8)
        {
            isJumping = false;
        }
    }
}
    
