using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twiPuzzleInteraction : MonoBehaviour {

    ParticleSystem[] ps;
    public int sfxSource;
    public GameObject exitPortal;
    public Spawn bossSpawn;
    public GameObject bossToSpawn;

    private void Awake()
    {
        if(!exitPortal)
            exitPortal = GameObject.Find("Twilight_Portal_Exit");
        if(exitPortal && GameController.control.twilightPortalStatus != 3)
            exitPortal.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if ((GameController.control.twilightPortalStatus == 0) && (gameObject.name == "Location1") && (col.tag == "Player"))
        {            
            //Destroy(this.gameObject);
            GameController.control.twilightPortalStatus = 1;
            GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);

            GetComponentInChildren<ParticleSystem>().startColor = new Color(0, 255, 0);

            ps = GetComponentsInChildren<ParticleSystem>();
            for(int i = 0; i < ps.Length; i++)
            {
                ps[i].startColor = new Color(0, 255, 0);
            }
            AudioManager.Instance.PlaySFX(sfxSource);
            //AudioManager.Instance.EffectsSource.PlayOneShot(activationEffect);
        }
        else if ((GameController.control.twilightPortalStatus == 1) && (gameObject.name == "Location2") && (col.tag == "Player"))
        {
            //Destroy(this.gameObject);
            GameController.control.twilightPortalStatus = 2;
            GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);

            GetComponentInChildren<ParticleSystem>().startColor = new Color(0, 255, 0);

            ps = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i].startColor = new Color(0, 255, 0);
            }
            AudioManager.Instance.PlaySFX(sfxSource);
            //AudioManager.Instance.EffectsSource.PlayOneShot(activationEffect);
        }
        else if ((GameController.control.twilightPortalStatus == 2) && (gameObject.name == "Location3") && (col.tag == "Player"))
        {
            //Destroy(this.gameObject);
            GameController.control.twilightPortalStatus = 3;
            GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);

            GetComponentInChildren<ParticleSystem>().startColor = new Color(0, 255, 0);

            ps = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i].startColor = new Color(0, 255, 0);
            }
            AudioManager.Instance.PlaySFX(sfxSource);
            //AudioManager.Instance.EffectsSource.PlayOneShot(activationEffect);
            exitPortal.SetActive(true);
            Instantiate(bossToSpawn, bossSpawn.transform.position, Quaternion.identity);
        }
    }
}
