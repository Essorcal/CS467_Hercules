using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Animation{
	public GameObject animationObject;
	public float delay = 0; 
}

public class AnimationController : MonoBehaviour {
	public List<Animation> animations = new List<Animation>();
	private List<Animator> playAnimators = new List<Animator>();
	private Animation maxAnimation;
	// Use this for initialization
	void Start () {
		maxAnimation = this.GetMaxLengthAnimation ();

		for (int i = 0; i < animations.Count; i++) {
			if (animations [i].delay > 0) {
				animations [i].animationObject.SetActive (false);
				StartCoroutine (DelayPlay (animations [i], animations [i].delay));
			} else {
				animations [i].animationObject.SetActive (true);
				Animator animator = animations [i].animationObject.GetComponent<Animator> ();
				animator.Play ("appear");
				StartCoroutine (Disappear(animations [i]));
			}
		}
	}

	protected IEnumerator DelayPlay (Animation animation, float times){
		yield return new WaitForSeconds(times);
		animation.animationObject.SetActive (true);
		Animator animator = animation.animationObject.GetComponent<Animator> ();
		animator.Play ("appear");
		StartCoroutine (Disappear(animation));
	}

	protected IEnumerator Disappear(Animation animation){
		Animator animator = animation.animationObject.GetComponent<Animator> ();
		AnimatorStateInfo info =animator.GetCurrentAnimatorStateInfo (0);
		yield return new WaitForSeconds(info.length);
		animation.animationObject.SetActive (false);

		if(maxAnimation == animation){
			DestroyImmediate (transform.gameObject);
		}
	}

	protected Animation GetMaxLengthAnimation(){
		Animation maxAnimation = new Animation ();
		float maxLength = -1;
		for (int i = 0; i < animations.Count; i++) {
			Animator animator = animations[i].animationObject.GetComponent<Animator> ();
			AnimatorStateInfo info =animator.GetCurrentAnimatorStateInfo (0);
			float length = info.length + animations [i].delay;
			if(length >maxLength){
				maxLength = length;
				maxAnimation = animations [i];
			}
		}
		return maxAnimation;
	}

	// Update is called once per frame
	void Update () {
		//			for (int i = 0; i < playAnimators.Count; i++) {
		//				AnimatorStateInfo info = playAnimators [i].GetCurrentAnimatorStateInfo (0);
		//
		//				if(Time.fixedDeltaTime > info.length){
		//					playAnimators [i].gameObject.SetActive (false);
		//				}
		//
		//			}
	}

}