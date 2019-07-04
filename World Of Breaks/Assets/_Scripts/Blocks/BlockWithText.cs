using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockWithText : MonoBehaviour {

	public bool active;

	public int coordsX;

	public int coordsY;

	protected SpriteRenderer spriteRenderer;

	protected TextMesh textMesh;

	public Gradient g;

	public ParticleSystem destroyParticle;

	public bool canLooseBeforeDown;

	protected Color curColor;

    public bool isDead;

    public bool isLoadingBlock;

	protected virtual void Start ()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		textMesh = GetComponentInChildren <TextMesh> ();
		UpdateText ();
		curColor = g.Evaluate ((BlocksController.Instance.blockMap [coordsY] [coordsX].blockLife % 30) / 30f);
		spriteRenderer.color = curColor;
    }

    protected void ChangeSpriteColor(SpriteRenderer spriteRenderer)
    {

    }

	public virtual void Hit (Ball ball)
	{
		Hit ();
	}

    protected float t = 0;
    protected float timerSpeed = 6;

    void Update()
    {

        if (t > 0)
        {
            t -= Time.deltaTime * timerSpeed;

            TimerStart();

        }
        else if (t <= -0.01f)
        {
            t = 0;

            TimerEnd();
        }

    }

    protected virtual void TimerStart()
    {
        spriteRenderer.color = Color.Lerp(Color.black, curColor, 1f - t);
    }
    protected virtual void TimerEnd()
    {
        spriteRenderer.color = curColor;
    }

    public virtual void Hit ()
	{
        BlocksController.Instance.blockMap[coordsY][coordsX].blockLife = (BlocksController.Instance.blockMap[coordsY][coordsX].blockLife - 1 < 0)? 0 : BlocksController.Instance.blockMap[coordsY][coordsX].blockLife - 1;

		if (BlocksController.Instance.blockMap [coordsY] [coordsX].blockLife <= 0) {
			Die ();
			return;
		}

        t = 1;

		curColor = g.Evaluate ((BlocksController.Instance.blockMap [coordsY] [coordsX].blockLife % 30) / 30f);

		spriteRenderer.color = curColor;

		UpdateText ();
	}

	public virtual void UpdateText ()
	{
		textMesh.text = BlocksController.Instance.blockMap [coordsY] [coordsX].blockLife.ToString ();
		//spriteRenderer.color = g.Evaluate (BlocksController.Instance.blockMap [coordsY] [coordsX].blockLife / 100);
	}

	public void Die ()
	{

        if (isDead)
            return;
        isDead = true;

        OnDead();

        BlocksController.Instance.blockMap[coordsY][coordsX].blockIndex = -1;
    }

    protected virtual void OnDead()
    {
        BlocksController.Instance.blockMap[coordsY][coordsX].blockLife = 0;
        BlocksController.Instance.CalculateBlockLife();
        Destroy(Instantiate<GameObject>(destroyParticle.gameObject, transform.position + (Vector3.up - Vector3.left) * .5f, Quaternion.identity), 2);
        Destroy(gameObject);

    }

}
