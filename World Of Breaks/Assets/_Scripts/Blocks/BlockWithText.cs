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

	Color origColor;

    public bool isDead;

    public bool isLoadingBlock;

	protected virtual void Start ()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		textMesh = GetComponentInChildren <TextMesh> ();
		UpdateText ();
		origColor = g.Evaluate ((BlocksController.Instance.blockMap [coordsY] [coordsX].blockLife % 30) / 30f);
		spriteRenderer.color = origColor;
	}

	public virtual void Hit (Ball ball)
	{
		Hit ();
	}

	public virtual void Hit ()
	{
        BlocksController.Instance.blockMap[coordsY][coordsX].blockLife = (BlocksController.Instance.blockMap[coordsY][coordsX].blockLife - 1 < 0)? 0 : BlocksController.Instance.blockMap[coordsY][coordsX].blockLife - 1;

		if (BlocksController.Instance.blockMap [coordsY] [coordsX].blockLife <= 0) {
			Die ();
			return;
		}

		origColor = g.Evaluate ((BlocksController.Instance.blockMap [coordsY] [coordsX].blockLife % 30) / 30f);

		spriteRenderer.color = origColor;

		UpdateText ();
	}

	public virtual void UpdateText ()
	{
		textMesh.text = BlocksController.Instance.blockMap [coordsY] [coordsX].blockLife.ToString ();
		Shine ();
		//spriteRenderer.color = g.Evaluate (BlocksController.Instance.blockMap [coordsY] [coordsX].blockLife / 100);
	}

	public void Shine ()
	{
		StopCoroutine ("ShineCoroutine");
		StartCoroutine ("ShineCoroutine");
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

	IEnumerator ShineCoroutine ()
	{
		float t = 1f;

		while (t >= 0) {
			
			t -= Time.deltaTime / .1f;

			spriteRenderer.color = Color.Lerp (Color.black, origColor, 1f - t);

			yield return null;
		}

		spriteRenderer.color = origColor;
	}

}
