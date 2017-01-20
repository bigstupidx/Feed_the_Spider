﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class gHintClass : MonoBehaviour {


	public static int counter = 0;
	public static action[] actions;
	public static float time;
	public static string hintState = "";
    public static bool isDream = false;

    public static GameObject hint;
	public static bool flagTransform = false;
	public static  Vector3 hintStartPos = new Vector3 (-4, 0, 0);
	public static  Vector3 hintEndPos = new Vector3 (-4, 0, 0);

	//----------------- fixedUpdate ------------------
	public static int fixedFrameCount = 0; 
	public static int fixedFrameCountLast = 0; 
	//private int frame = 0; 

	// Use this for initialization, 
	void Start () {
		hint = GameObject.Find("hint folder");

		if (ctrProgressClass.progress.Count == 0) ctrProgressClass.getProgress();
		transform.GetChild(0).GetComponent<UILabel>().text = ctrProgressClass.progress["hints"].ToString();
		Time.maximumDeltaTime = 0.9F;

	    if (hintState == "enable bonus picture")
	    {
	        //для тел, которые работают по-умолчанию с 30fps (поему-то не работает на iPad)
	        //Application.targetFrameRate = 60;

	        Time.maximumDeltaTime = 0.02F;

	        Time.timeScale = 0;
	        GameObject.Find("bonuses pictures").transform.GetChild(0).gameObject.SetActive(true);
	        GameObject.Find("bonuses pictures").transform.GetChild(1).gameObject.SetActive(true);
	        StartCoroutine(coroutineBonusPictureEnable());

	    }
	    else
	    {
	        hintState = "";
            isDream = false;

        }



        fixedFrameCount = 0;
		fixedFrameCountLast = 0;
		flagTransform = false;
		hintStartPos = new Vector3 (-4, 0, 0);






	}
	
	// Update is called once per frame
	void Update() {

		if (flagTransform) {
			//перемещение подсказки
			float t = (Time.unscaledTime - time) * 2;

			float offsetX = 0.25F * hint.transform.localScale.x;



			if (counter > 0) {
				if (actions [counter].id == actions [counter - 1].id) t = 1;	
			}
			hint.transform.position = сalculateBezierPoint(t, hintStartPos, new Vector2(hintStartPos.x, hintStartPos.y + 0.2F),  new Vector2(hintEndPos.x - offsetX, hintEndPos.y + 0.6F),   new Vector2(hintEndPos.x - offsetX, hintEndPos.y + 0.4F));
			//заканчиваем перемещение подсказки
			if (t >= 1) {
				//hint.transform.position = new Vector2 (actions [counter].id.x - offsetX, actions [counter].id.y + 0.4F);
				hint.transform.position = new Vector2 (hintEndPos.x - offsetX, hintEndPos.y + 0.4F);
				hintState = "pause";
				flagTransform = false;

				hint.transform.GetChild (0).GetChild (2).gameObject.SetActive (true);
				hint.transform.GetChild(0).GetComponent<Animator>().Play("hint show");
				hint.transform.GetChild (0).GetComponent<Animator> ().speed = 1;
				//fixedFrameCountLast = fixedFrameCount;
				fixedFrameCountLast = gBerryClass.fixedCounter;

                Debug.Log("isDream: " + isDream);
                if (isDream)
			    {
                    RaycastHit2D hit = Physics2D.Raycast(actions[counter].id, -Vector2.up);
                    Debug.Log(hit.collider.name);
			        if (hit.collider != null)
			        {
			            Debug.Log(hit.collider.name);
			            var nameObj = hit.collider.name;

                        if (nameObj == "destroyer" || nameObj == "groot" || nameObj == "sluggish physics") hit.collider.gameObject.SendMessage("OnPress", true);
                        else hit.collider.gameObject.SendMessage("OnClick");  
                    }
                }
			}



		}

	}



	void OnPress(bool flag) {
		if (!flag) {

			if (ctrProgressClass.progress ["hints"] > 0) {
				GetComponent<AudioSource> ().Play ();
				ctrProgressClass.progress ["hints"]--;
				transform.GetChild (0).GetComponent<UILabel> ().text = ctrProgressClass.progress [name].ToString ();
				ctrProgressClass.saveProgress ();
				Time.timeScale = 1;
				hintState = "enable bonus picture";
				counter = 0;
				staticClass.scenePrev = SceneManager.GetActiveScene ().name;
				SceneManager.LoadScene (SceneManager.GetActiveScene().name);
				SendMessage(SceneManager.GetActiveScene().name + "_" + initLevelMenuClass.levelDemands); 
			}
		}
	}



	public static Vector3 checkHint(GameObject obj, bool flag = false) {
		if (hintState == "pause") { 

			if (actions[counter].id == obj.transform.position) {
				Time.timeScale = 1;

				if (flag) return actions[counter].mouse;
				//hint.transform.position = new Vector3(-4, 0, 0);

				if (obj.name == "destroyer" || obj.name == "groot") {
					obj.SendMessage("OnDrag");
					obj.SendMessage("OnPress", false);
				}
				if (obj.name == "sluggish") {
					obj.transform.GetChild(1).SendMessage("dragSluggish");
					obj.transform.GetChild(1).SendMessage("OnPress", false);
				}


				counter ++;
				time = Time.unscaledTime;

				hintState = "start";
				//hint.transform.GetChild (0).GetComponent<Animator> ().Rebind ();
				//hint.transform.GetChild (0).GetComponent<Animator> ().Stop();
				hint.transform.GetChild (0).GetComponent<Animator> ().speed = 0;
				//hint point делаем прозрачным
				hint.transform.GetChild (0).GetChild (2).gameObject.SetActive (false);
				//hint.transform.GetChild (0).GetChild (2).transform.position = new Vector3 (0, 0, -1000);
				if (counter <= actions.Length - 1)
				if (actions [counter].frame == 0)
					GameObject.Find ("root/berry").SendMessage ("FixedUpdate");
					
				Time.timeScale = 1;
			
			} //else {
				//return new Vector3 (-100, -100, -100);
				//gHintClass.hintState = "";
				//hint.transform.position = new Vector3(-4, 0, 0);
			//} 
		//}	else if (hintState == "start" && !flag) {
			//return new Vector3 (-100, -100, -100);
			//Time.timeScale = 1;

			//hintState = "";
			//hint.transform.position = new Vector3(-4, 0, 0);
		}


		return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
	}



	IEnumerator coroutineBonusPictureEnable() {

		yield return StartCoroutine(staticClass.waitForRealTime(0.5F));

		GameObject.Find("bonuses pictures").transform.GetChild(1).gameObject.GetComponent<Animator>().Play("menu exit");
		GameObject.Find("bonuses pictures").transform.GetChild(0).gameObject.SetActive(false);
		yield return StartCoroutine(staticClass.waitForRealTime(0.3F));

		GameObject.Find("bonuses pictures").transform.GetChild(1).gameObject.SetActive(false);
		hintState = "start";
		time = Time.unscaledTime;
		Time.timeScale = 1;
	}

	Vector3 сalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {

		float u = 1 - t;
		float tt = t*t;
		float uu = u*u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vector3 p = uuu * p0;    //first term
		p += 3 * uu * t * p1;    //second term
		p += 3 * u * tt * p2;    //third term
		p += ttt * p3;           //fourth term

		return p;
	}
	public struct action {
		public Vector3 id;
		public Vector3 mouse;
		public float frame;
	}


	void level1_0 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.009765625F, -0.5292969F, 0F); //web
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.009765625F, -0.5292969F, 0F); //web
		actions[1].frame = 66;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level2_0 () {
		actions = new action[4];

		actions[0].id = new Vector3(0.5390625F, 1.205078F, 0F); //web
		actions[0].frame = 63;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.5390625F, 1.205078F, 0F); //web
		actions[1].frame = 138;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.4179688F, 0.3320313F, 0F); //web
		actions[2].frame = 41;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.4179688F, 0.3320313F, 0F); //web
		actions[3].frame = 69;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}

	void level3_0 () {
		actions = new action[15];

		actions[0].id = new Vector3(0.4765625F, -0.2382813F, 0F); //web
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.8046875F, 0.4863281F, 0F); //web
		actions[1].frame = 34;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.4785156F, -0.2207031F, 0F); //web
		actions[2].frame = 33;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.4941406F, 1.257813F, 0F); //web
		actions[3].frame = 47;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.453125F, 1.271484F, 0F); //web
		actions[4].frame = 62;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.4765625F, -0.2382813F, 0F); //web
		actions[5].frame = 37;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.8046875F, 0.4863281F, 0F); //web
		actions[6].frame = 85;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.4785156F, -0.2207031F, 0F); //web
		actions[7].frame = 54;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.4941406F, 1.257813F, 0F); //web
		actions[8].frame = 48;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(-0.453125F, 1.271484F, 0F); //web
		actions[9].frame = 47;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(0.8046875F, 0.4863281F, 0F); //web
		actions[10].frame = 43;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
		actions[11].id = new Vector3(0.4765625F, -0.2382813F, 0F); //web
		actions[11].frame = 42;
		actions[11].mouse = new Vector3(0F, 0F, 0F);
		actions[12].id = new Vector3(-0.4785156F, -0.2207031F, 0F); //web
		actions[12].frame = 40;
		actions[12].mouse = new Vector3(0F, 0F, 0F);
		actions[13].id = new Vector3(0.4941406F, 1.257813F, 0F); //web
		actions[13].frame = 54;
		actions[13].mouse = new Vector3(0F, 0F, 0F);
		actions[14].id = new Vector3(-0.453125F, 1.271484F, 0F); //web
		actions[14].frame = 78;
		actions[14].mouse = new Vector3(0F, 0F, 0F);
	}

	void level4_0 () {
		actions = new action[4];

		actions[0].id = new Vector3(0.5390625F, 1.205078F, 0F); //web
		actions[0].frame = 40;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.4179688F, 0.3320313F, 0F); //web
		actions[1].frame = 113;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.5390625F, 1.205078F, 0F); //web
		actions[2].frame = 66;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.4179688F, 0.3320313F, 0F); //web
		actions[3].frame = 120;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}

	void level5_0 () {
		actions = new action[13];

		actions[0].id = new Vector3(0.3359375F, -0.5917969F, 0F); //web
		actions[0].frame = 90;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.1425781F, -1.013672F, 0F); //sluggish
		actions[1].frame = 102;
		actions[1].mouse = new Vector3(0.1614584F, -1.385417F, 0F);
		actions[2].id = new Vector3(0.3359375F, -0.5917969F, 0F); //web
		actions[2].frame = 76;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.3046875F, 0.4179688F, 0F); //web
		actions[3].frame = 59;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.2949219F, 0.3359375F, 0F); //sluggish
		actions[4].frame = 96;
		actions[4].mouse = new Vector3(0.796875F, 0.5625F, 0F);
		actions[5].id = new Vector3(-0.3046875F, 0.4179688F, 0F); //web
		actions[5].frame = 155;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.2949219F, 0.3359375F, 0F); //sluggish
		actions[6].frame = 86;
		actions[6].mouse = new Vector3(0.2447917F, -0.1666666F, 0F);
		actions[7].id = new Vector3(0.3359375F, -0.5917969F, 0F); //web
		actions[7].frame = 101;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(-0.1425781F, -1.013672F, 0F); //sluggish
		actions[8].frame = 94;
		actions[8].mouse = new Vector3(0.4791667F, -1.34375F, 0F);
		actions[9].id = new Vector3(0.3359375F, -0.5917969F, 0F); //web
		actions[9].frame = 50;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(-0.3046875F, 0.4179688F, 0F); //web
		actions[10].frame = 39;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
		actions[11].id = new Vector3(0.2949219F, 0.3359375F, 0F); //sluggish
		actions[11].frame = 41;
		actions[11].mouse = new Vector3(-0.3385417F, -0.1302084F, 0F);
		actions[12].id = new Vector3(-0.3046875F, 0.4179688F, 0F); //web
		actions[12].frame = 19;
		actions[12].mouse = new Vector3(0F, 0F, 0F);
	}
	void level5_1 () {
		actions = new action[5];

		actions[0].id = new Vector3(0.3359375F, -0.5917969F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.1425781F, -1.013672F, 0F); //sluggish
		actions[1].frame = 40;
		actions[1].mouse = new Vector3(-1.442708F, -0.2552084F, 0F);
		actions[2].id = new Vector3(0.3359375F, -0.5917969F, 0F); //web
		actions[2].frame = 60;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.3046875F, 0.4179688F, 0F); //web
		actions[3].frame = 40;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.2949219F, 0.3359375F, 0F); //sluggish
		actions[4].frame = 40;
		actions[4].mouse = new Vector3(0.9791667F, 0.8958333F, 0F);
	}

	void level6_0 () {
		actions = new action[5];

		actions[0].id = new Vector3(0.6328125F, 0.5449219F, 0F); //web
		actions[0].frame = 65;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.6328125F, 0.5449219F, 0F); //web
		actions[1].frame = 115;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.6835938F, 0.1015625F, 0F); //sluggish
		actions[2].frame = 75;
		actions[2].mouse = new Vector3(0.9322917F, -0.296875F, 0F);
		actions[3].id = new Vector3(-0.3671875F, -0.05273438F, 0F); //web
		actions[3].frame = 46;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.3671875F, -0.05273438F, 0F); //web
		actions[4].frame = 59;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
	}
	void level6_1 () {
		actions = new action[8];

		actions[0].id = new Vector3(0.6328125F, 0.5449219F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.6328125F, 0.5449219F, 0F); //web
		actions[1].frame = 106;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.6835938F, 0.1015625F, 0F); //sluggish
		actions[2].frame = 40;
		actions[2].mouse = new Vector3(1.015625F, -0.390625F, 0F);
		actions[3].id = new Vector3(-0.3671875F, -0.05273438F, 0F); //web
		actions[3].frame = 46;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.6328125F, 0.5449219F, 0F); //web
		actions[4].frame = 110;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.3671875F, -0.05273438F, 0F); //web
		actions[5].frame = 60;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.6328125F, 0.5449219F, 0F); //web
		actions[6].frame = 40;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.6835938F, 0.1015625F, 0F); //sluggish
		actions[7].frame = 60;
		actions[7].mouse = new Vector3(0.5624999F, 0.9322917F, 0F);
	}

	void level7_0 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.5761719F, -0.5097656F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.5761719F, -0.5097656F, 0F); //web
		actions[1].frame = 57;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}
	void level7_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.5761719F, -0.5097656F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.5761719F, -0.5097656F, 0F); //web
		actions[1].frame = 57;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level8_0 () {
		actions = new action[2];

		actions[0].id = new Vector3(0.03515625F, 1.210938F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.7597656F, 1.224609F, 0F); //web
		actions[1].frame = 74;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}
	void level8_1 () {
		actions = new action[4];

		actions[0].id = new Vector3(-0.7597656F, 1.224609F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.03515625F, 1.210938F, 0F); //web
		actions[1].frame = 45;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.03515625F, 1.210938F, 0F); //web
		actions[2].frame = 49;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.03515625F, 1.210938F, 0F); //web
		actions[3].frame = 119;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}

	void level9_0 () {
		actions = new action[7];

		actions[0].id = new Vector3(-0.015625F, 0.4609375F, 0F); //web
		actions[0].frame = 26;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.01953125F, -0.001953125F, 0F); //sluggish
		actions[1].frame = 70;
		actions[1].mouse = new Vector3(0.2447917F, -0.4895834F, 0F);
		actions[2].id = new Vector3(-0.015625F, 0.4609375F, 0F); //web
		actions[2].frame = 95;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.01953125F, -0.001953125F, 0F); //sluggish
		actions[3].frame = 60;
		actions[3].mouse = new Vector3(0.08333328F, -0.5416666F, 0F);
		actions[4].id = new Vector3(-0.015625F, 0.4609375F, 0F); //web
		actions[4].frame = 89;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.015625F, 0.4609375F, 0F); //web
		actions[5].frame = 134;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.01953125F, -0.001953125F, 0F); //sluggish
		actions[6].frame = 61;
		actions[6].mouse = new Vector3(-0.1302083F, -0.5729166F, 0F);
	}
	void level9_1 () {
		actions = new action[3];

		actions[0].id = new Vector3(-0.015625F, 0.4609375F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.015625F, 0.4609375F, 0F); //web
		actions[1].frame = 40;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.01953125F, -0.001953125F, 0F); //sluggish
		actions[2].frame = 60;
		actions[2].mouse = new Vector3(0.0520833F, -0.4270834F, 0F);
	}

	void level10_0 () {
		actions = new action[15];

		actions[0].id = new Vector3(0.7949219F, 1.248047F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.6386719F, 0.7207031F, 0F); //web
		actions[1].frame = 40;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.7949219F, 1.248047F, 0F); //web
		actions[2].frame = 68;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.6386719F, 0.7207031F, 0F); //web
		actions[3].frame = 84;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.9140625F, -0.1113281F, 0F); //web
		actions[4].frame = 64;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.04296875F, -0.4785156F, 0F); //web
		actions[5].frame = 47;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.9140625F, -0.1113281F, 0F); //web
		actions[6].frame = 36;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.8808594F, -0.765625F, 0F); //sluggish
		actions[7].frame = 80;
		actions[7].mouse = new Vector3(1.1875F, -0.9427084F, 0F);
		actions[8].id = new Vector3(-0.9140625F, -0.1113281F, 0F); //web
		actions[8].frame = 80;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(-0.9140625F, -0.1113281F, 0F); //web
		actions[9].frame = 40;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(0.04296875F, -0.4785156F, 0F); //web
		actions[10].frame = 80;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
		actions[11].id = new Vector3(0.8808594F, -0.765625F, 0F); //sluggish
		actions[11].frame = 60;
		actions[11].mouse = new Vector3(0.8958334F, -1.208333F, 0F);
		actions[12].id = new Vector3(0.7949219F, 1.248047F, 0F); //web
		actions[12].frame = 45;
		actions[12].mouse = new Vector3(0F, 0F, 0F);
		actions[13].id = new Vector3(0.04296875F, -0.4785156F, 0F); //web
		actions[13].frame = 52;
		actions[13].mouse = new Vector3(0F, 0F, 0F);
		actions[14].id = new Vector3(0.7949219F, 1.248047F, 0F); //web
		actions[14].frame = 61;
		actions[14].mouse = new Vector3(0F, 0F, 0F);
	}
	void level10_1 () {
		actions = new action[13];

		actions[0].id = new Vector3(0.7949219F, 1.248047F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.6386719F, 0.7207031F, 0F); //web
		actions[1].frame = 39;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.7949219F, 1.248047F, 0F); //web
		actions[2].frame = 57;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.7949219F, 1.248047F, 0F); //web
		actions[3].frame = 23;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.6386719F, 0.7207031F, 0F); //web
		actions[4].frame = 57;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.7949219F, 1.248047F, 0F); //web
		actions[5].frame = 40;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.8808594F, -0.765625F, 0F); //sluggish
		actions[6].frame = 117;
		actions[6].mouse = new Vector3(1.265625F, -0.9739584F, 0F);
		actions[7].id = new Vector3(0.04296875F, -0.4785156F, 0F); //web
		actions[7].frame = 32;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.04296875F, -0.4785156F, 0F); //web
		actions[8].frame = 109;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(0.8808594F, -0.765625F, 0F); //sluggish
		actions[9].frame = 79;
		actions[9].mouse = new Vector3(0.9010417F, -1.322917F, 0F);
		actions[10].id = new Vector3(0.7949219F, 1.248047F, 0F); //web
		actions[10].frame = 40;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
		actions[11].id = new Vector3(0.04296875F, -0.4785156F, 0F); //web
		actions[11].frame = 54;
		actions[11].mouse = new Vector3(0F, 0F, 0F);
		actions[12].id = new Vector3(0.7949219F, 1.248047F, 0F); //web
		actions[12].frame = 65;
		actions[12].mouse = new Vector3(0F, 0F, 0F);
	}

	void level11_0 () {
		actions = new action[8];

		actions[0].id = new Vector3(-0.671875F, 0.7792969F, 0F); //sluggish
		actions[0].frame = 150;
		actions[0].mouse = new Vector3(-1.046875F, 0.02604175F, 0F);
		actions[1].id = new Vector3(0.5449219F, 1.027344F, 0F); //web
		actions[1].frame = 53;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.5449219F, 1.027344F, 0F); //web
		actions[2].frame = 40;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.009765625F, -0.2363281F, 0F); //web
		actions[3].frame = 60;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.671875F, 0.7792969F, 0F); //sluggish
		actions[4].frame = 60;
		actions[4].mouse = new Vector3(-0.6302083F, 1.541667F, 0F);
		actions[5].id = new Vector3(0.2382813F, -0.7617188F, 0F); //sluggish
		actions[5].frame = 80;
		actions[5].mouse = new Vector3(-0.9270834F, -1.03125F, 0F);
		actions[6].id = new Vector3(-0.009765625F, -0.2363281F, 0F); //web
		actions[6].frame = 80;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.2382813F, -0.7617188F, 0F); //sluggish
		actions[7].frame = 50;
		actions[7].mouse = new Vector3(0.8489583F, -0.578125F, 0F);
	}
	void level11_1 () {
		actions = new action[4];

		actions[0].id = new Vector3(-0.009765625F, -0.2363281F, 0F); //web
		actions[0].frame = 100;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.2382813F, -0.7617188F, 0F); //sluggish
		actions[1].frame = 135;
		actions[1].mouse = new Vector3(-0.6197917F, -1.046875F, 0F);
		actions[2].id = new Vector3(-0.009765625F, -0.2363281F, 0F); //web
		actions[2].frame = 83;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.2382813F, -0.7617188F, 0F); //sluggish
		actions[3].frame = 73;
		actions[3].mouse = new Vector3(0.7239584F, -0.6927084F, 0F);
	}

	void level12_0 () {
		actions = new action[7];

		actions[0].id = new Vector3(-0.2675781F, 1.214844F, 0F); //web
		actions[0].frame = 70;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.875F, -0.3398438F, 0F); //web
		actions[1].frame = 45;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.5214844F, 0.296875F, 0F); //web
		actions[2].frame = 50;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.875F, -0.3398438F, 0F); //web
		actions[3].frame = 58;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.5214844F, 0.296875F, 0F); //web
		actions[4].frame = 56;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.5214844F, 0.296875F, 0F); //web
		actions[5].frame = 18;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.5214844F, 0.296875F, 0F); //web
		actions[6].frame = 83;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
	}
	void level12_1 () {
		actions = new action[3];

		actions[0].id = new Vector3(-0.2675781F, 1.214844F, 0F); //web
		actions[0].frame = 101;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.5214844F, 0.296875F, 0F); //web
		actions[1].frame = 26;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.5214844F, 0.296875F, 0F); //web
		actions[2].frame = 62;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		// Второй вариант

		/*actions[0].id = new Vector3(-0.2675781F, 1.214844F, 0F); //web
		actions[0].frame = 42;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.2675781F, 1.214844F, 0F); //web
		actions[1].frame = 19;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.2675781F, 1.214844F, 0F); //web
		actions[2].frame = 170;
		actions[2].mouse = new Vector3(0F, 0F, 0F);*/
	}

	void level13_0 () {
		actions = new action[12];

		actions[0].id = new Vector3(-0.9648438F, 0.4140625F, 0F); //sluggish
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(-1.203125F, -0.2760416F, 0F);
		actions[1].id = new Vector3(0.6425781F, 0.6640625F, 0F); //web
		actions[1].frame = 87;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.6425781F, 0.6640625F, 0F); //web
		actions[2].frame = 197;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.1992188F, -0.9609375F, 0F); //web
		actions[3].frame = 35;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.1992188F, -0.9609375F, 0F); //web
		actions[4].frame = 80;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.7207031F, -0.9960938F, 0F); //sluggish (1)
		actions[5].frame = 76;
		actions[5].mouse = new Vector3(-0.625F, -1.515625F, 0F);
		actions[6].id = new Vector3(-0.9648438F, 0.4140625F, 0F); //sluggish
		actions[6].frame = 94;
		actions[6].mouse = new Vector3(-1.020833F, -0.0625F, 0F);
		actions[7].id = new Vector3(0.6425781F, 0.6640625F, 0F); //web
		actions[7].frame = 106;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.6425781F, 0.6640625F, 0F); //web
		actions[8].frame = 63;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(0.1992188F, -0.9609375F, 0F); //web
		actions[9].frame = 38;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(-0.7207031F, -0.9960938F, 0F); //sluggish (1)
		actions[10].frame = 118;
		actions[10].mouse = new Vector3(-1.411458F, -0.609375F, 0F);
		actions[11].id = new Vector3(0.1992188F, -0.9609375F, 0F); //web
		actions[11].frame = 94;
		actions[11].mouse = new Vector3(0F, 0F, 0F);
	}
	void level13_1 () {
		actions = new action[3];

		actions[0].id = new Vector3(-0.9648438F, 0.4140625F, 0F); //sluggish
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(-0.9895834F, 0.005208254F, 0F);
		actions[1].id = new Vector3(0.6425781F, 0.6640625F, 0F); //web
		actions[1].frame = 108;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.6425781F, 0.6640625F, 0F); //web
		actions[2].frame = 90;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
	}

	void level14_0 () {
		actions = new action[8];

		actions[0].id = new Vector3(-0.3320313F, 0.6816406F, 0F); //web
		actions[0].frame = 20;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.3320313F, 0.6816406F, 0F); //web
		actions[1].frame = 74;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.03125F, -0.9648438F, 0F); //sluggish
		actions[2].frame = 138;
		actions[2].mouse = new Vector3(0.02687575F, -1.444569F, 0F);
		actions[3].id = new Vector3(0.03125F, -0.9648438F, 0F); //sluggish
		actions[3].frame = 157;
		actions[3].mouse = new Vector3(0.34F, -1.43561F, 0F);
		actions[4].id = new Vector3(-0.3320313F, 0.6816406F, 0F); //web
		actions[4].frame = 37;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.7675781F, 0.65625F, 0F); //web
		actions[5].frame = 73;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.3320313F, 0.6816406F, 0F); //web
		actions[6].frame = 35;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.7675781F, 0.65625F, 0F); //web
		actions[7].frame = 88;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
	}
	void level14_1 () {
		actions = new action[6];

		actions[0].id = new Vector3(-0.3320313F, 0.6816406F, 0F); //web
		actions[0].frame = 20;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.7675781F, 0.65625F, 0F); //web
		actions[1].frame = 110;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.7675781F, 0.65625F, 0F); //web
		actions[2].frame = 32;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.7675781F, 0.65625F, 0F); //web
		actions[3].frame = 108;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.3320313F, 0.6816406F, 0F); //web
		actions[4].frame = 43;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.7675781F, 0.65625F, 0F); //web
		actions[5].frame = 90;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}

	void level15_0 () {
		actions = new action[2];

		actions[0].id = new Vector3(0.6972656F, 0.3496094F, 0F); //destroyer
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0.25F, 0.05729175F, 0F);
		actions[1].id = new Vector3(-0.6914063F, 1.396484F, 0F); //destroyer
		actions[1].frame = 60;
		actions[1].mouse = new Vector3(-0.1145833F, 0.9895833F, 0F);
	}
	void level15_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.6914063F, 1.396484F, 0F); //destroyer
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0.01041666F, 0.921875F, 0F);
		actions[1].id = new Vector3(0.6972656F, 0.3496094F, 0F); //destroyer
		actions[1].frame = 102;
		actions[1].mouse = new Vector3(0.5572916F, 0.1875F, 0F);
	}

	void level16_0 () {
		actions = new action[3];

		actions[0].id = new Vector3(0.5703125F, -0.296875F, 0F); //cloud
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.5703125F, -0.296875F, 0F); //cloud
		actions[1].frame = 50;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.390625F, 0.6953125F, 0F); //cloud
		actions[2].frame = 50;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
	}
	void level16_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(0.5703125F, -0.296875F, 0F); //cloud
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.390625F, 0.6953125F, 0F); //cloud
		actions[1].frame = 60;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level17_0 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.5996094F, 0.04492188F, 0F); //cloud
		actions[0].frame = 66;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.5996094F, 0.04492188F, 0F); //cloud
		actions[1].frame = 112;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}
	void level17_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.5996094F, 0.04492188F, 0F); //cloud
		actions[0].frame = 66;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.5996094F, 0.04492188F, 0F); //cloud
		actions[1].frame = 112;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level18_0 () {
		actions = new action[8];

		actions[0].id = new Vector3(0.8105469F, 0.6269531F, 0F); //destroyer
		actions[0].frame = 40;
		actions[0].mouse = new Vector3(0.3906251F, 0.2083333F, 0F);
		actions[1].id = new Vector3(0.3125F, 0.9492188F, 0F); //cloud
		actions[1].frame = 60;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.8808594F, -0.4707031F, 0F); //cloud
		actions[2].frame = 90;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.828125F, -0.578125F, 0F); //web
		actions[3].frame = 150;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.828125F, -0.578125F, 0F); //web
		actions[4].frame = 24;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.3359375F, -1.183594F, 0F); //cloud
		actions[5].frame = 25;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.828125F, -0.578125F, 0F); //web
		actions[6].frame = 85;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.828125F, -0.578125F, 0F); //web
		actions[7].frame = 47;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
	}
	void level18_1 () {
		actions = new action[7];

		actions[0].id = new Vector3(0.8105469F, 0.6269531F, 0F); //destroyer
		actions[0].frame = 40;
		actions[0].mouse = new Vector3(0.3802084F, 0.1770833F, 0F);
		actions[1].id = new Vector3(0.3125F, 0.9492188F, 0F); //cloud
		actions[1].frame = 43;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.828125F, -0.578125F, 0F); //web
		actions[2].frame = 35;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.828125F, -0.578125F, 0F); //web
		actions[3].frame = 29;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.3359375F, -1.183594F, 0F); //cloud
		actions[4].frame = 36;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.828125F, -0.578125F, 0F); //web
		actions[5].frame = 121;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.828125F, -0.578125F, 0F); //web
		actions[6].frame = 32;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
	}

	void level19_0 () {
		actions = new action[4];

		actions[0].id = new Vector3(0.1464844F, -0.1699219F, 0F); //cloud
		actions[0].frame = 36;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.07382812F, 0.2734375F, 0F); //cloud
		actions[1].frame = 122;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.1464844F, -0.1699219F, 0F); //cloud
		actions[2].frame = 100;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.1621094F, -0.8197266F, 0F); //cloud
		actions[3].frame = 133;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}
	void level19_1 () {
		actions = new action[4];

		actions[0].id = new Vector3(0.1464844F, -0.1699219F, 0F); //cloud
		actions[0].frame = 36;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.07382812F, 0.2734375F, 0F); //cloud
		actions[1].frame = 122;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.1464844F, -0.1699219F, 0F); //cloud
		actions[2].frame = 100;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.1621094F, -0.8197266F, 0F); //cloud
		actions[3].frame = 133;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}

	void level20_0 () {
		actions = new action[3];

		actions[0].id = new Vector3(-0.71875F, 1.40625F, 0F); //web
		actions[0].frame = 501;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.71875F, 1.40625F, 0F); //web
		actions[1].frame = 85;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.7539063F, 0.5F, 0F); //destroyer
		actions[2].frame = 214;
		actions[2].mouse = new Vector3(0.7447917F, -0.21875F, 0F);
	}
	void level20_1 () {
		actions = new action[3];

		actions[0].id = new Vector3(0.7539063F, 0.5F, 0F); //destroyer
		actions[0].frame = 100;
		actions[0].mouse = new Vector3(-0.2239583F, 0.4114583F, 0F);
		actions[1].id = new Vector3(-0.71875F, 1.40625F, 0F); //web
		actions[1].frame = 400;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.71875F, 1.40625F, 0F); //web
		actions[2].frame = 50;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
	}

	void level21_0 () {
		actions = new action[6];

		actions[0].id = new Vector3(-0.5527344F, 0.9414063F, 0F); //web
		actions[0].frame = 91;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.8300781F, 1.310547F, 0F); //web
		actions[1].frame = 134;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.5527344F, 0.9414063F, 0F); //web
		actions[2].frame = 96;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.8300781F, 1.310547F, 0F); //web
		actions[3].frame = 102;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.1367188F, 1.376953F, 0F); //destroyer
		actions[4].frame = 200;
		actions[4].mouse = new Vector3(-0.03645831F, 0.4166667F, 0F);
		actions[5].id = new Vector3(-0.5527344F, 0.9414063F, 0F); //web
		actions[5].frame = 95;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}
	void level21_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(0.1367188F, 1.376953F, 0F); //destroyer
		actions[0].frame = 71;
		actions[0].mouse = new Vector3(-0.08333336F, 0.3072917F, 0F);
		actions[1].id = new Vector3(-0.5527344F, 0.9414063F, 0F); //web
		actions[1].frame = 91;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level22_0 () {
		actions = new action[4];

		actions[0].id = new Vector3(-0.5957031F, -0.8769531F, 0F); //destroyer
		actions[0].frame = 100;
		actions[0].mouse = new Vector3(-0.8437499F, -0.1458334F, 0F);
		actions[1].id = new Vector3(0.4003906F, 0.07421875F, 0F); //destroyer
		actions[1].frame = 178;
		actions[1].mouse = new Vector3(0.3854167F, 0.9739583F, 0F);
		actions[2].id = new Vector3(-0.5957031F, -0.8769531F, 0F); //destroyer
		actions[2].frame = -734;
		actions[2].mouse = new Vector3(-0.8541666F, -0.05208337F, 0F);
		actions[3].id = new Vector3(0.4003906F, 0.07421875F, 0F); //destroyer
		actions[3].frame = 214;
		actions[3].mouse = new Vector3(0.3958334F, 1.015625F, 0F);
	}
	void level22_1 () {
		actions = new action[1];

		actions[0].id = new Vector3(-0.5957031F, -0.8769531F, 0F); //destroyer
		actions[0].frame = 140;
		actions[0].mouse = new Vector3(-0.5833334F, 0.203125F, 0F);
	}

	void level23_0 () {
		actions = new action[8];

		actions[0].id = new Vector3(0.6171875F, 0.6347656F, 0F); //sluggish
		actions[0].frame = 80;
		actions[0].mouse = new Vector3(0.8593749F, 1.276042F, 0F);
		actions[1].id = new Vector3(0.2695313F, -0.328125F, 0F); //cloud
		actions[1].frame = 100;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.7929688F, 0.046875F, 0F); //destroyer
		actions[2].frame = 60;
		actions[2].mouse = new Vector3(-0.3541667F, 0.04166675F, 0F);
		actions[3].id = new Vector3(0.5429688F, -0.6796875F, 0F); //web
		actions[3].frame = 60;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.7539063F, -1.025391F, 0F); //destroyer
		actions[4].frame = 60;
		actions[4].mouse = new Vector3(-0.296875F, -0.3802084F, 0F);
		actions[5].id = new Vector3(0.6171875F, 0.6347656F, 0F); //sluggish
		actions[5].frame = 60;
		actions[5].mouse = new Vector3(-0.6041667F, 0.6354167F, 0F);
		actions[6].id = new Vector3(0.5429688F, -0.6796875F, 0F); //web
		actions[6].frame = 80;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.6171875F, 0.6347656F, 0F); //sluggish
		actions[7].frame = 60;
		actions[7].mouse = new Vector3(1.234375F, 0.8020833F, 0F);
	}
	void level23_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.7929688F, 0.046875F, 0F); //destroyer
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(-0.4583334F, 0.04166675F, 0F);
		actions[1].id = new Vector3(0.6171875F, 0.6347656F, 0F); //sluggish
		actions[1].frame = 60;
		actions[1].mouse = new Vector3(0.6927084F, 0.09375F, 0F);
	}

	void level24_0 () {
		actions = new action[9];

		actions[0].id = new Vector3(-0.6972656F, -1.361328F, 0F); //destroyer
		actions[0].frame = 60;
		actions[0].mouse = new Vector3(-0.04687497F, 1.625F, 0F);
		actions[1].id = new Vector3(0.6191406F, 1.228516F, 0F); //web
		actions[1].frame = 94;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.5742188F, -0.6289063F, 0F); //web
		actions[2].frame = 65;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.7792969F, -0.02148438F, 0F); //web
		actions[3].frame = 100;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.5742188F, -0.6289063F, 0F); //web
		actions[4].frame = 50;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.7792969F, -0.02148438F, 0F); //web
		actions[5].frame = 50;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.6191406F, 1.228516F, 0F); //web
		actions[6].frame = 60;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.5742188F, -0.6289063F, 0F); //web
		actions[7].frame = 60;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.5742188F, -0.6289063F, 0F); //web
		actions[8].frame = 84;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
	}
	void level24_1 () {
		actions = new action[5];

		actions[0].id = new Vector3(-0.6972656F, -1.361328F, 0F); //destroyer
		actions[0].frame = 60;
		actions[0].mouse = new Vector3(-0.1927084F, 0.828125F, 0F);
		actions[1].id = new Vector3(0.6191406F, 1.228516F, 0F); //web
		actions[1].frame = 60;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.6191406F, 1.228516F, 0F); //web
		actions[2].frame = 27;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.5742188F, -0.6289063F, 0F); //web
		actions[3].frame = 63;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.5742188F, -0.6289063F, 0F); //web
		actions[4].frame = 71;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
	}

	void level25_0 () {
		actions = new action[7];

		actions[0].id = new Vector3(0.1113281F, 0.2324219F, 0F); //destroyer
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(-0.78125F, 0.8125F, 0F);
		actions[1].id = new Vector3(0.3515625F, 0.7539063F, 0F); //web
		actions[1].frame = 80;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.3515625F, 0.7539063F, 0F); //web
		actions[2].frame = 40;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.6347656F, -0.3730469F, 0F); //web
		actions[3].frame = 40;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.859375F, 0.625F, -0.015625F); //sluggish (1)
		actions[4].frame = 50;
		actions[4].mouse = new Vector3(-0.2760417F, 0.546875F, 0F);
		actions[5].id = new Vector3(0.6347656F, -0.3730469F, 0F); //web
		actions[5].frame = 80;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.859375F, 0.625F, -0.015625F); //sluggish (1)
		actions[6].frame = 80;
		actions[6].mouse = new Vector3(1.041667F, 1.088542F, 0F);
	}
	void level25_1 () {
		actions = new action[3];

		actions[0].id = new Vector3(0.3515625F, 0.7539063F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.3515625F, 0.7539063F, 0F); //web
		actions[1].frame = 70;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.859375F, 0.625F, -0.015625F); //sluggish (1)
		actions[2].frame = 90;
		actions[2].mouse = new Vector3(1.104167F, 1.223958F, 0F);
	}

	void level26_0 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.4882813F, 1.269531F, 0F); //web
		actions[0].frame = 36;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.4882813F, 1.269531F, 0F); //web
		actions[1].frame = 139;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}
	void level26_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.4882813F, 1.269531F, 0F); //web
		actions[0].frame = 49;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.4882813F, 1.269531F, 0F); //web
		actions[1].frame = 26;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level27_0 () {
		actions = new action[6];

		actions[0].id = new Vector3(0.1445313F, 1.496094F, 0F); //web
		actions[0].frame = 67;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.796875F, 0.859375F, 0F); //destroyer
		actions[1].frame = 142;
		actions[1].mouse = new Vector3(0.6041666F, 0.7552083F, 0F);
		actions[2].id = new Vector3(0.06835938F, 0.2070313F, 0F); //web
		actions[2].frame = 45;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.1445313F, 1.496094F, 0F); //web
		actions[3].frame = 68;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.1445313F, 1.496094F, 0F); //web
		actions[4].frame = 102;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.06835938F, 0.2070313F, 0F); //web
		actions[5].frame = 157;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}
	void level27_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(0.1445313F, 1.496094F, 0F); //web
		actions[0].frame = 65;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.796875F, 0.859375F, 0F); //destroyer
		actions[1].frame = 94;
		actions[1].mouse = new Vector3(0.5416666F, 0.734375F, 0F);
	}

	void level28_0 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.84375F, 0.04559524F, -0.01953125F); //yeti body
		actions[0].frame = 168;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.84375F, 0.04559524F, -0.01953125F); //yeti body
		actions[1].frame = 0;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}
	void level28_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.84375F, 0.04559524F, -0.01953125F); //yeti body
		actions[0].frame = 178;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.84375F, 0.04559524F, -0.01953125F); //yeti body
		actions[1].frame = 0;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level29_0 () {
		actions = new action[6];

		actions[0].id = new Vector3(0.8789063F, -0.9465923F, -0.01953125F); //yeti body
		actions[0].frame = 75;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.8789063F, -0.9465923F, -0.01953125F); //yeti body
		actions[1].frame = 0;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.65625F, -0.1679688F, 0F); //sluggish
		actions[2].frame = 150;
		actions[2].mouse = new Vector3(-0.8072916F, -0.65625F, 0F);
		actions[3].id = new Vector3(0.8789063F, -0.9465923F, -0.01953125F); //yeti body
		actions[3].frame = 39;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.8789063F, -0.9465923F, -0.01953125F); //yeti body
		actions[4].frame = 0;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.65625F, -0.1679688F, 0F); //sluggish
		actions[5].frame = 60;
		actions[5].mouse = new Vector3(-1.067708F, 0.4427083F, 0F);
	}
	void level29_1 () {
		actions = new action[3];

		actions[0].id = new Vector3(0.8789063F, -0.9465923F, -0.01953125F); //yeti body
		actions[0].frame = 64;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.8789063F, -0.9465923F, -0.01953125F); //yeti body
		actions[1].frame = 0;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.65625F, -0.1679688F, 0F); //sluggish
		actions[2].frame = 180;
		actions[2].mouse = new Vector3(-1.067708F, 0.40625F, 0F);
	}

	void level30_0 () {
		actions = new action[10];

		actions[0].id = new Vector3(0.4882813F, 1.113281F, 0F); //web
		actions[0].frame = 40;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.4882813F, 1.113281F, 0F); //web
		actions[1].frame = 145;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.6855469F, -0.5618266F, -0.01953125F); //yeti body
		actions[2].frame = 33;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.6855469F, -0.5618266F, -0.01953125F); //yeti body
		actions[3].frame = 0;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.04492188F, 0.4296875F, 0F); //web
		actions[4].frame = 27;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.4882813F, 1.113281F, 0F); //web
		actions[5].frame = 75;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.4882813F, 1.113281F, 0F); //web
		actions[6].frame = 64;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.04492188F, 0.4296875F, 0F); //web
		actions[7].frame = 82;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.6855469F, -0.5618266F, -0.01953125F); //yeti body
		actions[8].frame = 85;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(0.6855469F, -0.5618266F, -0.01953125F); //yeti body
		actions[9].frame = 0;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
	}
	void level30_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(0.4882813F, 1.113281F, 0F); //web
		actions[0].frame = 28;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.4882813F, 1.113281F, 0F); //web
		actions[1].frame = 125;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level31_0 () {
		actions = new action[7];

		actions[0].id = new Vector3(0.4414063F, 1.056641F, 0F); //web
		actions[0].frame = 38;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.5449219F, 1.283203F, 0F); //web
		actions[1].frame = 204;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.5449219F, 1.283203F, 0F); //web
		actions[2].frame = 56;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.4414063F, 1.056641F, 0F); //web
		actions[3].frame = 92;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}
	void level31_1 () {
		actions = new action[4];
		actions[0].id = new Vector3(-0.07382812F, 0.2734375F, 0F); //cloud
		actions[0].frame = 19;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.07382812F, 0.2734375F, 0F); //cloud
		actions[1].frame = 89;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.1621094F, -0.8197266F, 0F); //cloud
		actions[2].frame = 79;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.1464844F, -0.1699219F, 0F); //cloud
		actions[3].frame = 93;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}

	void level32_0 () {
		actions = new action[4];

		actions[0].id = new Vector3(-0.8007813F, 1.097656F, 0F); //destroyer
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(-0.5781251F, 0.546875F, 0F);
		actions[1].id = new Vector3(-0.5644531F, 0.2838765F, -0.01953125F); //yeti body
		actions[1].frame = 68;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.5644531F, 0.2838765F, -0.01953125F); //yeti body
		actions[2].frame = 0;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.7382813F, -1.232422F, 0F); //destroyer
		actions[3].frame = 60;
		actions[3].mouse = new Vector3(0F, -0.1510416F, 0F);
		actions[4].id = new Vector3(-0.5644531F, 0.2838765F, -0.01953125F); //yeti body
		actions[4].frame = 76;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.5644531F, 0.2838765F, -0.01953125F); //yeti body
		actions[5].frame = 0;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}
	void level32_1 () {
		actions = new action[4];

		actions[0].id = new Vector3(-0.8007813F, 1.097656F, 0F); //destroyer
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(-0.59375F, 0.78125F, 0F);
		actions[1].id = new Vector3(-0.5644531F, 0.2838765F, -0.01953125F); //yeti body
		actions[1].frame = 54;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.5644531F, 0.2838765F, -0.01953125F); //yeti body
		actions[2].frame = 0;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.7382813F, -1.232422F, 0F); //destroyer
		actions[3].frame = 150;
		actions[3].mouse = new Vector3(0.78125F, 0.9895833F, 0F);
	}

	void level33_0 () {
		actions = new action[6];

		actions[0].id = new Vector3(-0.001953125F, -0.2363281F, 0F); //cloud
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.01367188F, -1.027344F, -0.015625F); //sluggish
		actions[1].frame = 80;
		actions[1].mouse = new Vector3(-0.02083332F, -1.390625F, 0F);
		actions[2].id = new Vector3(0.8671875F, 0.703125F, 0F); //web
		actions[2].frame = 34;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.3867188F, 1.019531F, 0F); //cloud
		actions[3].frame = 197;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.001953125F, -0.2363281F, 0F); //cloud
		actions[4].frame = 34;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.8671875F, 0.703125F, 0F); //web
		actions[5].frame = 115;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}
	void level33_1 () {
		actions = new action[4];

		actions[0].id = new Vector3(-0.001953125F, -0.2363281F, 0F); //cloud
		actions[0].frame = 40;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.3867188F, 1.019531F, 0F); //cloud
		actions[1].frame = 40;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.01367188F, -1.027344F, -0.015625F); //sluggish
		actions[2].frame = 50;
		actions[2].mouse = new Vector3(0.08854161F, -1.494792F, 0F);
		actions[3].id = new Vector3(-0.001953125F, -0.2363281F, 0F); //cloud
		actions[3].frame = 25;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}

	void level34_0 () {
		actions = new action[18];

		actions[0].id = new Vector3(-0.8183594F, 1.4375F, 0F); //web
		actions[0].frame = 37;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.8183594F, 1.4375F, 0F); //web
		actions[1].frame = 85;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.8183594F, 1.4375F, 0F); //web
		actions[2].frame = 46;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.6816406F, 0.8691406F, 0F); //web
		actions[3].frame = 57;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.8183594F, 1.4375F, 0F); //web
		actions[4].frame = 63;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //web
		actions[5].frame = 29;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.6816406F, 0.8691406F, 0F); //web
		actions[6].frame = 70;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //web
		actions[7].frame = 89;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.6816406F, 0.8691406F, 0F); //web
		actions[8].frame = 34;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(0.40625F, -0.3417969F, 0F); //web
		actions[9].frame = 206;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(-0.3007813F, -0.984375F, 0F); //web
		actions[10].frame = 59;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
		actions[11].id = new Vector3(0.6816406F, 0.8691406F, 0F); //web
		actions[11].frame = 55;
		actions[11].mouse = new Vector3(0F, 0F, 0F);
		actions[12].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //web
		actions[12].frame = 61;
		actions[12].mouse = new Vector3(0F, 0F, 0F);
		actions[13].id = new Vector3(0.40625F, -0.3417969F, 0F); //web
		actions[13].frame = 81;
		actions[13].mouse = new Vector3(0F, 0F, 0F);
		actions[14].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //web
		actions[14].frame = 65;
		actions[14].mouse = new Vector3(0F, 0F, 0F);
		actions[15].id = new Vector3(0.40625F, -0.3417969F, 0F); //web
		actions[15].frame = 65;
		actions[15].mouse = new Vector3(0F, 0F, 0F);
		actions[16].id = new Vector3(-0.3007813F, -0.984375F, 0F); //web
		actions[16].frame = 102;
		actions[16].mouse = new Vector3(0F, 0F, 0F);
		actions[17].id = new Vector3(0.40625F, -0.3417969F, 0F); //web
		actions[17].frame = 55;
		actions[17].mouse = new Vector3(0F, 0F, 0F);
	}
	void level34_1 () {
		actions = new action[11];

		actions[0].id = new Vector3(-0.8183594F, 1.4375F, 0F); //web
		actions[0].frame = 32;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.8183594F, 1.4375F, 0F); //web
		actions[1].frame = 30;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.8183594F, 1.4375F, 0F); //web
		actions[2].frame = 79;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.6816406F, 0.8691406F, 0F); //web
		actions[3].frame = 45;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.8183594F, 1.4375F, 0F); //web
		actions[4].frame = 50;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //web
		actions[5].frame = 30;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.6816406F, 0.8691406F, 0F); //web
		actions[6].frame = 52;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //web
		actions[7].frame = 52;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.6816406F, 0.8691406F, 0F); //web
		actions[8].frame = 39;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(0.40625F, -0.3417969F, 0F); //web
		actions[9].frame = 67;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(0.6816406F, 0.8691406F, 0F); //web
		actions[10].frame = 95;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
	}

	void level35_0 () {
		actions = new action[4];

		actions[0].id = new Vector3(-0.1542969F, 1.492188F, 0F); //web
		actions[0].frame = 85;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.2011719F, -0.3496094F, 0F); //sluggish
		actions[1].frame = 67;
		actions[1].mouse = new Vector3(0.4739583F, -0.890625F, 0F);
		actions[2].id = new Vector3(-0.1542969F, 1.492188F, 0F); //web
		actions[2].frame = 41;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.1542969F, 1.492188F, 0F); //web
		actions[3].frame = 72;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}
	void level35_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(0.2011719F, -0.3496094F, 0F); //sluggish
		actions[0].frame = 116;
		actions[0].mouse = new Vector3(0.2083334F, -0.703125F, 0F);
		actions[1].id = new Vector3(-0.1542969F, 1.492188F, 0F); //web
		actions[1].frame = 138;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level36_0 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.07226563F, 1.654297F, 0F); //web
		actions[0].frame = 66;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.07226563F, 1.654297F, 0F); //web
		actions[1].frame = 122;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}
	void level36_1 () {
		actions = new action[4];

		actions[0].id = new Vector3(-0.07226563F, 1.654297F, 0F); //web
		actions[0].frame = 40;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.07226563F, 1.654297F, 0F); //web
		actions[1].frame = 103;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.07226563F, 1.654297F, 0F); //web
		actions[2].frame = 37;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.07226563F, 1.654297F, 0F); //web
		actions[3].frame = 135;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}

	void level37_0 () {
		actions = new action[4];

		actions[0].id = new Vector3(-0.515625F, 1.191406F, 0F); //web
		actions[0].frame = 92;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.7089844F, 0.3984375F, 0F); //sluggish
		actions[1].frame = 100;
		actions[1].mouse = new Vector3(0.7822014F, -0.01405156F, 0F);
		actions[2].id = new Vector3(-0.515625F, 1.191406F, 0F); //web
		actions[2].frame = 36;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.515625F, 1.191406F, 0F); //web
		actions[3].frame = 136;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}
	void level37_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.515625F, 1.191406F, 0F); //web
		actions[0].frame = 104;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.7089844F, 0.3984375F, 0F); //sluggish
		actions[1].frame = 100;
		actions[1].mouse = new Vector3(1.124122F, 1.100703F, 0F);
	}

	void level38_0 () {
		actions = new action[6];

		actions[0].id = new Vector3(-0.578125F, 0.2148438F, 0F); //groot
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0.6093749F, -0.2083334F, 0F);
		actions[1].id = new Vector3(-0.08398438F, 1.224609F, 0F); //web
		actions[1].frame = 30;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.08398438F, 1.224609F, 0F); //web
		actions[2].frame = 66;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.578125F, 0.2148438F, 0F); //groot
		actions[3].frame = 50;
		actions[3].mouse = new Vector3(-0.5520833F, 0.15625F, 0F);
		actions[4].id = new Vector3(-0.578125F, 0.2148438F, 0F); //groot
		actions[4].frame = 50;
		actions[4].mouse = new Vector3(-0.4739583F, -0.9635416F, 0F);
		actions[5].id = new Vector3(-0.08398438F, 1.224609F, 0F); //web
		actions[5].frame = 80;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}
	void level38_1 () {
		actions = new action[5];

		actions[0].id = new Vector3(-0.578125F, 0.2148438F, 0F); //groot
		actions[0].frame = 60;
		actions[0].mouse = new Vector3(0.5052083F, -0.2135416F, 0F);
		actions[1].id = new Vector3(-0.08398438F, 1.224609F, 0F); //web
		actions[1].frame = 30;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.08398438F, 1.224609F, 0F); //web
		actions[2].frame = 68;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.08398438F, 1.224609F, 0F); //web
		actions[3].frame = 38;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.578125F, 0.2148438F, 0F); //groot
		actions[4].frame = 23;
		actions[4].mouse = new Vector3(-0.5052083F, 0.21875F, 0F);
	}

	void level39_0 () {
		actions = new action[6];

		actions[0].id = new Vector3(-0.578125F, 0.2148438F, 0F); //groot
		actions[0].frame = 166;
		actions[0].mouse = new Vector3(0.515625F, -0.171875F, 0F);
		actions[1].id = new Vector3(-0.08398438F, 1.224609F, 0F); //web
		actions[1].frame = 108;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.08398438F, 1.224609F, 0F); //web
		actions[2].frame = 63;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.578125F, 0.2148438F, 0F); //groot
		actions[3].frame = 119;
		actions[3].mouse = new Vector3(-0.4322916F, 0.1510417F, 0F);
		actions[4].id = new Vector3(-0.578125F, 0.2148438F, 0F); //groot
		actions[4].frame = 95;
		actions[4].mouse = new Vector3(-0.40625F, -0.90625F, 0F);
		actions[5].id = new Vector3(-0.08398438F, 1.224609F, 0F); //web
		actions[5].frame = 105;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}
	void level39_1 () {
		actions = new action[7];

		actions[0].id = new Vector3(-0.78125F, 1.367188F, 0F); //web
		actions[0].frame = 37;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.78125F, 1.367188F, 0F); //web
		actions[1].frame = 53;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.78125F, 1.367188F, 0F); //web
		actions[2].frame = 18;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.78125F, 1.367188F, 0F); //web
		actions[3].frame = 70;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.78125F, 1.367188F, 0F); //web
		actions[4].frame = 18;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.390625F, -0.390625F, 0F); //web
		actions[5].frame = 35;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.390625F, -0.390625F, 0F); //web
		actions[6].frame = 35;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
	}

	void level40_0 () {
		actions = new action[6];

		actions[0].id = new Vector3(0.3476563F, 1.378906F, 0F); //web
		actions[0].frame = 24;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.3691406F, 0.2695313F, 0F); //web
		actions[1].frame = 33;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.3691406F, 0.2695313F, 0F); //web
		actions[2].frame = 31;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.3476563F, 1.378906F, 0F); //web
		actions[3].frame = 46;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.3691406F, 0.2695313F, 0F); //web
		actions[4].frame = 98;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.3691406F, 0.2695313F, 0F); //web
		actions[5].frame = 90;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}
	void level40_1 () {
		actions = new action[4];

		actions[0].id = new Vector3(0.3476563F, 1.378906F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.3476563F, 1.378906F, 0F); //web
		actions[1].frame = 30;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.3691406F, 0.2695313F, 0F); //web
		actions[2].frame = 121;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.3691406F, 0.2695313F, 0F); //web
		actions[3].frame = 91;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}

	void level41_0 () {
		actions = new action[10];

		actions[0].id = new Vector3(0.3398438F, 0.5253906F, 0F); //groot
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(-0.859375F, -0.171875F, 0F);
		actions[1].id = new Vector3(-0.640625F, 1.199219F, 0F); //web
		actions[1].frame = 30;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.640625F, 1.199219F, 0F); //web
		actions[2].frame = 11;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.640625F, 1.199219F, 0F); //web
		actions[3].frame = 70;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.640625F, 1.199219F, 0F); //web
		actions[4].frame = 28;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.3691406F, -0.6777344F, 0F); //groot
		actions[5].frame = 60;
		actions[5].mouse = new Vector3(0.4270834F, 0.4635417F, 0F);
		actions[6].id = new Vector3(0.3398438F, 0.5253906F, 0F); //groot
		actions[6].frame = 30;
		actions[6].mouse = new Vector3(0.2395833F, 0.4635417F, 0F);
		actions[7].id = new Vector3(0.9160156F, -0.3535156F, 0F); //web
		actions[7].frame = 70;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.9160156F, -0.3535156F, 0F); //web
		actions[8].frame = 30;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(-0.3691406F, -0.6777344F, 0F); //groot
		actions[9].frame = 40;
		actions[9].mouse = new Vector3(-0.28125F, -0.5833334F, 0F);
	}
	void level41_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(-0.640625F, 1.199219F, 0F); //web
		actions[0].frame = 33;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.640625F, 1.199219F, 0F); //web
		actions[1].frame = 10;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level42_0 () {
		actions = new action[7];

		actions[0].id = new Vector3(0.6328125F, -0.1992188F, 0F); //sluggish
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(1.130208F, 0.05729175F, 0F);
		actions[1].id = new Vector3(-0.7050781F, -0.2265625F, 0F); //sluggish
		actions[1].frame = 70;
		actions[1].mouse = new Vector3(-0.765625F, -0.6197916F, 0F);
		actions[2].id = new Vector3(-0.5917969F, 1.486328F, 0F); //web
		actions[2].frame = 38;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.5917969F, 1.486328F, 0F); //web
		actions[3].frame = 80;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.6113281F, 0.5117188F, 0F); //groot
		actions[4].frame = 40;
		actions[4].mouse = new Vector3(-0.78125F, 0.765625F, 0F);
		actions[5].id = new Vector3(-0.7050781F, -0.2265625F, 0F); //sluggish
		actions[5].frame = 40;
		actions[5].mouse = new Vector3(-1.052083F, -0.7447916F, 0F);
		actions[6].id = new Vector3(0.6328125F, -0.1992188F, 0F); //sluggish
		actions[6].frame = 80;
		actions[6].mouse = new Vector3(0.6614584F, 0.5260417F, 0F);
	}
	void level42_1 () {
		actions = new action[3];

		actions[0].id = new Vector3(0.6328125F, -0.1992188F, 0F); //sluggish
		actions[0].frame = 80;
		actions[0].mouse = new Vector3(0.8645833F, -0.6145834F, 0F);
		actions[1].id = new Vector3(-0.7050781F, -0.2265625F, 0F); //sluggish
		actions[1].frame = 100;
		actions[1].mouse = new Vector3(-1.0625F, -0.06770837F, 0F);
		actions[2].id = new Vector3(0.6328125F, -0.1992188F, 0F); //sluggish
		actions[2].frame = 80;
		actions[2].mouse = new Vector3(0.6406251F, 0.5572917F, 0F);
	}

	void level43_0 () {
		actions = new action[9];

		actions[0].id = new Vector3(0.2167969F, 0.5351563F, 0F); //groot
		actions[0].frame = 40;
		actions[0].mouse = new Vector3(-0.6875001F, 0.4166667F, 0F);
		actions[1].id = new Vector3(-0.6386719F, 0.2402344F, 0F); //groot
		actions[1].frame = 40;
		actions[1].mouse = new Vector3(0.5729166F, -0.4895834F, 0F);
		actions[2].id = new Vector3(0.6660156F, -0.6699219F, 0F); //groot
		actions[2].frame = 40;
		actions[2].mouse = new Vector3(0.9739584F, 0.140625F, 0F);
		actions[3].id = new Vector3(-0.0625F, 1.488281F, 0F); //web
		actions[3].frame = 40;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.0625F, 1.488281F, 0F); //web
		actions[4].frame = 50;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.2167969F, 0.5351563F, 0F); //groot
		actions[5].frame = 87;
		actions[5].mouse = new Vector3(0.1562501F, 0.5625F, 0F);
		actions[6].id = new Vector3(0.6660156F, -0.6699219F, 0F); //groot
		actions[6].frame = 151;
		actions[6].mouse = new Vector3(0.7395834F, -0.5520834F, 0F);
		actions[7].id = new Vector3(0.6660156F, -0.6699219F, 0F); //groot
		actions[7].frame = 79;
		actions[7].mouse = new Vector3(-0.2239583F, -0.9166666F, 0F);
		actions[8].id = new Vector3(-0.6386719F, 0.2402344F, 0F); //groot
		actions[8].frame = 36;
		actions[8].mouse = new Vector3(-0.5781251F, 0.1666667F, 0F);
	}
	void level43_1 () {
		actions = new action[3];

		actions[0].id = new Vector3(0.6660156F, -0.6699219F, 0F); //groot
		actions[0].frame = 60;
		actions[0].mouse = new Vector3(-0.1510417F, -0.9010416F, 0F);
		actions[1].id = new Vector3(-0.0625F, 1.488281F, 0F); //web
		actions[1].frame = 50;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.0625F, 1.488281F, 0F); //web
		actions[2].frame = 103;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
	}

	void level44_1 () {
		actions = new action[10];

		actions[0].id = new Vector3(0.5488281F, 1.257813F, 0F); //groot
		actions[0].frame = 78;
		actions[0].mouse = new Vector3(-0.9010417F, 1.0625F, 0F);
		actions[1].id = new Vector3(-0.6210938F, 0.4980469F, 0F); //groot
		actions[1].frame = 109;
		actions[1].mouse = new Vector3(0.9947917F, 0.8020833F, 0F);
		actions[2].id = new Vector3(0.5488281F, 1.257813F, 0F); //groot
		actions[2].frame = 60;
		actions[2].mouse = new Vector3(0.4635417F, 1.270833F, 0F);
		actions[3].id = new Vector3(0.2929688F, 0.2792969F, 0F); //groot
		actions[3].frame = 100;
		actions[3].mouse = new Vector3(-0.9427084F, -0.171875F, 0F);
		actions[4].id = new Vector3(-0.6210938F, 0.4980469F, 0F); //groot
		actions[4].frame = 100;
		actions[4].mouse = new Vector3(-0.484375F, 0.6302083F, 0F);
		actions[5].id = new Vector3(-0.6210938F, 0.4980469F, 0F); //groot
		actions[5].frame = 60;
		actions[5].mouse = new Vector3(-0.4322916F, -0.71875F, 0F);
		actions[6].id = new Vector3(0.2929688F, 0.2792969F, 0F); //groot
		actions[6].frame = 40;
		actions[6].mouse = new Vector3(0.3020833F, 0.2604167F, 0F);
		actions[7].id = new Vector3(0.2929688F, 0.2792969F, 0F); //groot
		actions[7].frame = 40;
		actions[7].mouse = new Vector3(0.3072916F, -0.8072916F, 0F);
		actions[8].id = new Vector3(-0.6210938F, 0.4980469F, 0F); //groot
		actions[8].frame = 40;
		actions[8].mouse = new Vector3(-0.640625F, 0.4791667F, 0F);
		actions[9].id = new Vector3(0.2929688F, 0.2792969F, 0F); //groot
		actions[9].frame = 86;
		actions[9].mouse = new Vector3(0.2760417F, 0.1979167F, 0F);
	}
	void level44_0 () {
		actions = new action[4];

		actions[0].id = new Vector3(0.2929688F, 0.2792969F, 0F); //groot
		actions[0].frame = 73;
		actions[0].mouse = new Vector3(0.2447917F, -0.8072916F, 0F);
		actions[1].id = new Vector3(-0.6210938F, 0.4980469F, 0F); //groot
		actions[1].frame = 54;
		actions[1].mouse = new Vector3(0.2864583F, 0.1458333F, 0F);
		actions[2].id = new Vector3(-0.6210938F, 0.4980469F, 0F); //groot
		actions[2].frame = 100;
		actions[2].mouse = new Vector3(-0.5572916F, 0.5364583F, 0F);
		actions[3].id = new Vector3(0.2929688F, 0.2792969F, 0F); //groot
		actions[3].frame = 109;
		actions[3].mouse = new Vector3(0.3020833F, 0.203125F, 0F);
	}

	void level45_0 () {
		actions = new action[7];

		actions[0].id = new Vector3(0.5917969F, 0.3496094F, 0F); //groot
		actions[0].frame = 40;
		actions[0].mouse = new Vector3(-0.9114583F, 0.59375F, 0F);
		actions[1].id = new Vector3(0.05664063F, -0.04492188F, 0F); //sluggish
		actions[1].frame = 40;
		actions[1].mouse = new Vector3(0.1927084F, -0.515625F, 0F);
		actions[2].id = new Vector3(0.5917969F, 0.3496094F, 0F); //groot
		actions[2].frame = 80;
		actions[2].mouse = new Vector3(0.4947917F, 0.3489583F, 0F);
		actions[3].id = new Vector3(-0.640625F, -0.6367188F, 0F); //sluggish
		actions[3].frame = 30;
		actions[3].mouse = new Vector3(-0.4166667F, -1.645833F, 0F);
		actions[4].id = new Vector3(0.5917969F, 0.3496094F, 0F); //groot
		actions[4].frame = 70;
		actions[4].mouse = new Vector3(0.9375001F, -0.7291666F, 0F);
		actions[5].id = new Vector3(0.05664063F, -0.04492188F, 0F); //sluggish
		actions[5].frame = 40;
		actions[5].mouse = new Vector3(0F, -0.5885416F, 0F);
		actions[6].id = new Vector3(0.5917969F, 0.3496094F, 0F); //groot
		actions[6].frame = 150;
		actions[6].mouse = new Vector3(0.6354167F, 0.3125F, 0F);
	}
	void level45_1 () {
		actions = new action[3];

		actions[0].id = new Vector3(0.05664063F, -0.04492188F, 0F); //sluggish
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(0.09895828F, -0.3854166F, 0F);
		actions[1].id = new Vector3(-0.640625F, -0.6367188F, 0F); //sluggish
		actions[1].frame = 100;
		actions[1].mouse = new Vector3(-0.5104167F, -1.307292F, 0F);
		actions[2].id = new Vector3(0.05664063F, -0.04492188F, 0F); //sluggish
		actions[2].frame = 100;
		actions[2].mouse = new Vector3(0.01562499F, -0.578125F, 0F);
	}

	void level46_0 () {
		actions = new action[5];

		actions[0].id = new Vector3(-0.6875F, 1.173828F, 0F); //web
		actions[0].frame = 126;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.4265625F, 0.9521484F, 0F); //groot
		actions[1].frame = 100;
		actions[1].mouse = new Vector3(0.3697916F, -0.2395834F, 0F);
		actions[2].id = new Vector3(-0.6875F, 1.173828F, 0F); //web
		actions[2].frame = 200;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.3867188F, 0.5292969F, 0F); //destroyer
		actions[3].frame = 80;
		actions[3].mouse = new Vector3(-0.06249996F, 0.015625F, 0F);
		actions[4].id = new Vector3(0.4265625F, 0.9521484F, 0F); //groot
		actions[4].frame = 18;
		actions[4].mouse = new Vector3(0.2239583F, -0.4427084F, 0F);
	}
	void level46_1 () {
		actions = new action[3];

		actions[0].id = new Vector3(-0.3867188F, 0.5292969F, 0F); //destroyer
		actions[0].frame = 66;
		actions[0].mouse = new Vector3(-0.1875F, 1.192708F, 0F);
		actions[1].id = new Vector3(-0.6875F, 1.173828F, 0F); //web
		actions[1].frame = 116;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.6875F, 1.173828F, 0F); //web
		actions[2].frame = 112;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
	}

	void level47_0 () {
		actions = new action[5];

		actions[0].id = new Vector3(0.7304688F, 1.267578F, 0F); //web
		actions[0].frame = 141;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.7304688F, 1.267578F, 0F); //web
		actions[1].frame = 68;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.7714844F, 1.316406F, 0F); //web
		actions[2].frame = 120;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.7304688F, 1.267578F, 0F); //web
		actions[3].frame = 48;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.7304688F, 1.267578F, 0F); //web
		actions[4].frame = 81;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
	}
	void level47_1 () {
		actions = new action[2];

		actions[0].id = new Vector3(0.3613281F, -0.8769531F, 0F); //cloud
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.7714844F, 1.316406F, 0F); //web
		actions[1].frame = 96;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level48_0 () {
		actions = new action[9];

		actions[0].id = new Vector3(-0.9511719F, 1.386719F, 0F); //web
		actions[0].frame = 65;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.0234375F, -0.2265625F, 0F); //web
		actions[1].frame = 52;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.9511719F, 1.386719F, 0F); //web
		actions[2].frame = 57;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.0234375F, -0.2265625F, 0F); //web
		actions[3].frame = 45;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.9355469F, -0.2851563F, 0F); //groot
		actions[4].frame = 90;
		actions[4].mouse = new Vector3(0.06249996F, -1.234375F, 0F);
		actions[5].id = new Vector3(0.1699219F, 1.480469F, 0F); //web
		actions[5].frame = 90;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.9511719F, 1.386719F, 0F); //web
		actions[6].frame = 90;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.0234375F, -0.2265625F, 0F); //web
		actions[7].frame = 32;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(-0.0234375F, -0.2265625F, 0F); //web
		actions[8].frame = 98;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
	}
	void level48_1 () {
		actions = new action[5];

		actions[0].id = new Vector3(0.9355469F, -0.2851563F, 0F); //groot
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0.08333328F, -1.208333F, 0F);
		actions[1].id = new Vector3(0.1699219F, 1.480469F, 0F); //web
		actions[1].frame = 48;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.9511719F, 1.386719F, 0F); //web
		actions[2].frame = 78;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.0234375F, -0.2265625F, 0F); //web
		actions[3].frame = 37;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.0234375F, -0.2265625F, 0F); //web
		actions[4].frame = 68;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
	}

	void level49_0 () {
		actions = new action[9];

		actions[0].id = new Vector3(0.5253906F, 1.308594F, 0F); //web
		actions[0].frame = 74;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.5898438F, 1.085938F, 0F); //web
		actions[1].frame = 119;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.5253906F, 1.308594F, 0F); //web
		actions[2].frame = 47;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.5898438F, 1.085938F, 0F); //web
		actions[3].frame = 94;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.5214844F, -1.248047F, 0F); //sluggish
		actions[4].frame = 170;
		actions[4].mouse = new Vector3(-0.5260416F, -1.729167F, 0F);
		actions[5].id = new Vector3(-0.5898438F, 1.085938F, 0F); //web
		actions[5].frame = 41;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.5253906F, 1.308594F, 0F); //web
		actions[6].frame = 88;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.5898438F, 1.085938F, 0F); //web
		actions[7].frame = 45;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.5253906F, 1.308594F, 0F); //web
		actions[8].frame = 74;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
	}
	void level49_1 () {
		actions = new action[3];

		actions[0].id = new Vector3(-0.5898438F, 1.085938F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.5898438F, 1.085938F, 0F); //web
		actions[1].frame = 86;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.5214844F, -1.248047F, 0F); //sluggish
		actions[2].frame = 185;
		actions[2].mouse = new Vector3(-1.463542F, -1.078125F, 0F);
	}

	void level50_1 () {
		actions = new action[7];

		actions[0].id = new Vector3(0.8535156F, 0.7285156F, 0F); //groot
		actions[0].frame = 100;
		actions[0].mouse = new Vector3(-0.7760416F, 1.03125F, 0F);
		actions[1].id = new Vector3(0.9589844F, -0.2109375F, 0F); //groot
		actions[1].frame = 150;
		actions[1].mouse = new Vector3(0.2604167F, 1.057292F, 0F);
		actions[2].id = new Vector3(0.8535156F, 0.7285156F, 0F); //groot
		actions[2].frame = 50;
		actions[2].mouse = new Vector3(0.8177083F, 0.703125F, 0F);
		actions[3].id = new Vector3(0.8535156F, 0.7285156F, 0F); //groot
		actions[3].frame = 50;
		actions[3].mouse = new Vector3(-0.1614583F, -0.171875F, 0F);
		actions[4].id = new Vector3(0.9589844F, -0.2109375F, 0F); //groot
		actions[4].frame = 70;
		actions[4].mouse = new Vector3(0.9322917F, -0.1197916F, 0F);
		actions[5].id = new Vector3(0.9589844F, -0.2109375F, 0F); //groot
		actions[5].frame = 100;
		actions[5].mouse = new Vector3(0.3020833F, -1.151042F, 0F);
		actions[6].id = new Vector3(0.8535156F, 0.7285156F, 0F); //groot
		actions[6].frame = 39;
		actions[6].mouse = new Vector3(0.7708333F, 0.734375F, 0F);
	}
	void level50_0 () {
		actions = new action[2];

		actions[0].id = new Vector3(0.9589844F, -0.2109375F, 0F); //groot
		actions[0].frame = 93;
		actions[0].mouse = new Vector3(-0.4166667F, -0.3072916F, 0F);
		actions[1].id = new Vector3(0.9589844F, -0.2109375F, 0F); //groot
		actions[1].frame = 345;
		actions[1].mouse = new Vector3(0.8645833F, -0.2239584F, 0F);
	}

	void level51_1 () {
		actions = new action[7];

		actions[0].id = new Vector3(0.1386719F, -0.8046875F, 0F); //web
		actions[0].frame = 59;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.1386719F, -0.8046875F, 0F); //web
		actions[1].frame = 48;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.6464844F, -1.001953F, 0F); //sluggish (2)
		actions[2].frame = 101;
		actions[2].mouse = new Vector3(0.5781249F, -1.380208F, 0F);
		actions[3].id = new Vector3(0.8984375F, 0.5605469F, 0F); //sluggish (1)
		actions[3].frame = 134;
		actions[3].mouse = new Vector3(1.140625F, 0.6927083F, 0F);
		actions[4].id = new Vector3(-0.9472656F, -0.08007813F, 0F); //sluggish
		actions[4].frame = 139;
		actions[4].mouse = new Vector3(-1.135417F, 0.3385417F, 0F);
		actions[5].id = new Vector3(0.6464844F, -1.001953F, 0F); //sluggish (2)
		actions[5].frame = 124;
		actions[5].mouse = new Vector3(0.5833333F, -1.333333F, 0F);
		actions[6].id = new Vector3(0.8984375F, 0.5605469F, 0F); //sluggish (1)
		actions[6].frame = 97;
		actions[6].mouse = new Vector3(1.130208F, 0.1666667F, 0F);
	}
	void level51_0 () {
		actions = new action[5];

		actions[0].id = new Vector3(0.1386719F, -0.8046875F, 0F); //web
		actions[0].frame = 77;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.1386719F, -0.8046875F, 0F); //web
		actions[1].frame = 46;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.6464844F, -1.001953F, 0F); //sluggish (2)
		actions[2].frame = 113;
		actions[2].mouse = new Vector3(0.5833333F, -1.401042F, 0F);
		actions[3].id = new Vector3(0.8984375F, 0.5605469F, 0F); //sluggish (1)
		actions[3].frame = 146;
		actions[3].mouse = new Vector3(1.166667F, 0.65625F, 0F);
		actions[4].id = new Vector3(-0.9472656F, -0.08007813F, 0F); //sluggish
		actions[4].frame = 145;
		actions[4].mouse = new Vector3(-0.9218751F, -0.5989584F, 0F);
	}

	void level52_0 () {
		actions = new action[13];

		actions[0].id = new Vector3(-0.3632813F, 0.7773438F, 0F); //web
		actions[0].frame = 196;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.3632813F, 0.7773438F, 0F); //web
		actions[1].frame = 34;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.7285156F, 1.052734F, 0F); //web
		actions[2].frame = 45;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.7285156F, 1.052734F, 0F); //web
		actions[3].frame = 28;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.7285156F, 1.052734F, 0F); //web
		actions[4].frame = 45;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.3632813F, 0.7773438F, 0F); //web
		actions[5].frame = 32;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.0546875F, 0.6386719F, 0F); //sluggish
		actions[6].frame = 128;
		actions[6].mouse = new Vector3(0.09895828F, 0.1979167F, 0F);
		actions[7].id = new Vector3(0.7285156F, 1.052734F, 0F); //web
		actions[7].frame = 82;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.0546875F, 0.6386719F, 0F); //sluggish
		actions[8].frame = 110;
		actions[8].mouse = new Vector3(-0.6979167F, 1.348958F, 0F);
		actions[9].id = new Vector3(0.7285156F, 1.052734F, 0F); //web
		actions[9].frame = 120;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(0.0546875F, 0.6386719F, 0F); //sluggish
		actions[10].frame = 90;
		actions[10].mouse = new Vector3(0.02083332F, 0.15625F, 0F);
		actions[11].id = new Vector3(0.7285156F, 1.052734F, 0F); //web
		actions[11].frame = 93;
		actions[11].mouse = new Vector3(0F, 0F, 0F);
		actions[12].id = new Vector3(0.7285156F, 1.052734F, 0F); //web
		actions[12].frame = 107;
		actions[12].mouse = new Vector3(0F, 0F, 0F);
	}
	void level52_1 () {
		actions = new action[7];

		actions[0].id = new Vector3(0.7285156F, 1.052734F, 0F); //web
		actions[0].frame = 107;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.3632813F, 0.7773438F, 0F); //web
		actions[1].frame = 92;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.3632813F, 0.7773438F, 0F); //web
		actions[2].frame = 27;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.7285156F, 1.052734F, 0F); //web
		actions[3].frame = 61;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.3632813F, 0.7773438F, 0F); //web
		actions[4].frame = 42;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.0546875F, 0.6386719F, 0F); //sluggish
		actions[5].frame = 39;
		actions[5].mouse = new Vector3(0.09895828F, 0.1354167F, 0F);
		actions[6].id = new Vector3(0.7285156F, 1.052734F, 0F); //web
		actions[6].frame = 130;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
	}

	void level53_0 () {
		actions = new action[8];

		actions[0].id = new Vector3(0.7363281F, -0.7441406F, -0.015625F); //sluggish
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(0.96875F, -0.9635416F, 0F);
		actions[1].id = new Vector3(-0.7695313F, 0.0234375F, 0F); //sluggish
		actions[1].frame = 83;
		actions[1].mouse = new Vector3(-0.59375F, 0.53125F, 0F);
		actions[2].id = new Vector3(0.7363281F, -0.7441406F, -0.015625F); //sluggish
		actions[2].frame = 84;
		actions[2].mouse = new Vector3(1.130208F, -1.015625F, 0F);
		actions[3].id = new Vector3(-0.7695313F, 0.0234375F, 0F); //sluggish
		actions[3].frame = 79;
		actions[3].mouse = new Vector3(-1.067708F, -0.484375F, 0F);
		actions[4].id = new Vector3(0.4277344F, 0.6074219F, 0F); //web
		actions[4].frame = 53;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.4277344F, 0.6074219F, 0F); //web
		actions[5].frame = 34;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.4101563F, 0.5820313F, 0F); //web
		actions[6].frame = 50;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.7695313F, 0.0234375F, 0F); //sluggish
		actions[7].frame = 42;
		actions[7].mouse = new Vector3(-2.151042F, 0.8020833F, 0F);
	}
	void level53_1 () {
		actions = new action[8];

		actions[0].id = new Vector3(0.7363281F, -0.7441406F, -0.015625F); //sluggish
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(0.96875F, -0.9635416F, 0F);
		actions[1].id = new Vector3(-0.7695313F, 0.0234375F, 0F); //sluggish
		actions[1].frame = 83;
		actions[1].mouse = new Vector3(-0.59375F, 0.53125F, 0F);
		actions[2].id = new Vector3(0.7363281F, -0.7441406F, -0.015625F); //sluggish
		actions[2].frame = 84;
		actions[2].mouse = new Vector3(1.130208F, -1.015625F, 0F);
		actions[3].id = new Vector3(-0.7695313F, 0.0234375F, 0F); //sluggish
		actions[3].frame = 79;
		actions[3].mouse = new Vector3(-1.067708F, -0.484375F, 0F);
		actions[4].id = new Vector3(0.4277344F, 0.6074219F, 0F); //web
		actions[4].frame = 53;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.4277344F, 0.6074219F, 0F); //web
		actions[5].frame = 34;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.4101563F, 0.5820313F, 0F); //web
		actions[6].frame = 50;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.7695313F, 0.0234375F, 0F); //sluggish
		actions[7].frame = 42;
		actions[7].mouse = new Vector3(-2.151042F, 0.8020833F, 0F);
	}

	void level54_1 () {
		actions = new action[7];

		actions[0].id = new Vector3(0.1699219F, 1.304688F, 0F); //web
		actions[0].frame = 46;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.1699219F, 1.304688F, 0F); //web
		actions[1].frame = 22;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.1699219F, 1.304688F, 0F); //web
		actions[2].frame = 64;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.05273438F, -0.0625F, 0F); //web
		actions[3].frame = 50;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.1699219F, 1.304688F, 0F); //web
		actions[4].frame = 75;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.1699219F, 1.304688F, 0F); //web
		actions[5].frame = 46;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.05273438F, -0.0625F, 0F); //web
		actions[6].frame = 67;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
	}
	void level54_0 () {
		actions = new action[1];

		actions[0].id = new Vector3(0.1699219F, 1.304688F, 0F); //web
		actions[0].frame = 129;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
	}

	void level55_0 () {
		actions = new action[12];

		actions[0].id = new Vector3(0.8945313F, 0.2792969F, 0F); //groot
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(-0.3125F, 0.04166675F, 0F);
		actions[1].id = new Vector3(0.4023438F, 1.435547F, 0F); //web
		actions[1].frame = 43;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.4023438F, 1.435547F, 0F); //web
		actions[2].frame = 88;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.8945313F, 0.2792969F, 0F); //groot
		actions[3].frame = 110;
		actions[3].mouse = new Vector3(0.8229166F, 0.2864583F, 0F);
		actions[4].id = new Vector3(-0.78125F, 0.71875F, 0F); //web
		actions[4].frame = 91;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.78125F, 0.71875F, 0F); //web
		actions[5].frame = 63;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.6855469F, -0.8046875F, 0F); //groot
		actions[6].frame = 50;
		actions[6].mouse = new Vector3(-0.06770837F, -0.6666666F, 0F);
		actions[7].id = new Vector3(0.8945313F, 0.2792969F, 0F); //groot
		actions[7].frame = 50;
		actions[7].mouse = new Vector3(0.8958334F, -0.5260416F, 0F);
		actions[8].id = new Vector3(-1.003906F, -0.5488281F, -0.015625F); //sluggish
		actions[8].frame = 80;
		actions[8].mouse = new Vector3(-1.140625F, -1.192708F, 0F);
		actions[9].id = new Vector3(0.4023438F, 1.435547F, 0F); //web
		actions[9].frame = 65;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(0.4023438F, 1.435547F, 0F); //web
		actions[10].frame = 143;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
		actions[11].id = new Vector3(0.6855469F, -0.8046875F, 0F); //groot
		actions[11].frame = 104;
		actions[11].mouse = new Vector3(0.5833333F, -0.7291666F, 0F);
	}
	void level55_1 () {
		actions = new action[5];

		actions[0].id = new Vector3(0.6855469F, -0.8046875F, 0F); //groot
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(-0.2239583F, 0.1145833F, 0F);
		actions[1].id = new Vector3(0.8945313F, 0.2792969F, 0F); //groot
		actions[1].frame = 50;
		actions[1].mouse = new Vector3(0.8489583F, -0.578125F, 0F);
		actions[2].id = new Vector3(0.4023438F, 1.435547F, 0F); //web
		actions[2].frame = 50;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.4023438F, 1.435547F, 0F); //web
		actions[3].frame = 114;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.6855469F, -0.8046875F, 0F); //groot
		actions[4].frame = 100;
		actions[4].mouse = new Vector3(0.6666667F, -0.7552084F, 0F);
	}

	void level56_0 () {
		actions = new action[7];

		actions[0].id = new Vector3(-0.734375F, 1.509766F, 0F); //web
		actions[0].frame = 34;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.734375F, 1.509766F, 0F); //web
		actions[1].frame = 44;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.3886719F, 1.007813F, 0F); //web
		actions[2].frame = 49;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.734375F, 1.509766F, 0F); //web
		actions[3].frame = 120;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.5351563F, -0.6464844F, 0F); //web
		actions[4].frame = 78;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.5957031F, 0.1894531F, -0.015625F); //sluggish
		actions[5].frame = 53;
		actions[5].mouse = new Vector3(-0.625F, 0.2395833F, 0F);
		actions[6].id = new Vector3(0.5351563F, -0.6464844F, 0F); //web
		actions[6].frame = 25;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
	}
	void level56_1 () {
		actions = new action[8];

		actions[0].id = new Vector3(0.1933594F, 1.638672F, 0F); //web
		actions[0].frame = 32;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.1933594F, 1.638672F, 0F); //web
		actions[1].frame = 33;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.1933594F, 1.638672F, 0F); //web
		actions[2].frame = 62;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.796875F, 0.859375F, 0F); //destroyer
		actions[3].frame = 68;
		actions[3].mouse = new Vector3(0.5761124F, 0.7540984F, 0F);
		actions[4].id = new Vector3(0.06835938F, 0.2070313F, 0F); //web
		actions[4].frame = 34;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.1933594F, 1.638672F, 0F); //web
		actions[5].frame = 72;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.1933594F, 1.638672F, 0F); //web
		actions[6].frame = 23;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.06835938F, 0.2070313F, 0F); //web
		actions[7].frame = 60;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
	}

	void level57_0 () {
		actions = new action[12];

		actions[0].id = new Vector3(0.7558594F, 1.027344F, 0F); //web
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.7558594F, 1.027344F, 0F); //web
		actions[1].frame = 111;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.6367188F, -0.1230469F, -0.015625F); //sluggish
		actions[2].frame = 80;
		actions[2].mouse = new Vector3(0.9895834F, -0.4739584F, 0F);
		actions[3].id = new Vector3(-0.3808594F, 1.521484F, 0F); //web
		actions[3].frame = 34;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.7558594F, 1.027344F, 0F); //web
		actions[4].frame = 77;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.3808594F, 1.521484F, 0F); //web
		actions[5].frame = 49;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.7558594F, 1.027344F, 0F); //web
		actions[6].frame = 105;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.7558594F, 1.027344F, 0F); //web
		actions[7].frame = 35;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.6367188F, -0.1230469F, -0.015625F); //sluggish
		actions[8].frame = 50;
		actions[8].mouse = new Vector3(-0.01041666F, -0.390625F, 0F);
		actions[9].id = new Vector3(0.7558594F, 1.027344F, 0F); //web
		actions[9].frame = 240;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(-0.5976563F, -0.3105469F, 0F); //groot
		actions[10].frame = 40;
		actions[10].mouse = new Vector3(-0.6458333F, -1.328125F, 0F);
		actions[11].id = new Vector3(0.6367188F, -0.1230469F, -0.015625F); //sluggish
		actions[11].frame = 40;
		actions[11].mouse = new Vector3(1.239583F, 0.09895825F, 0F);
	}
	void level57_1 () {
		actions = new action[4];

		actions[0].id = new Vector3(0.4824219F, 1.244141F, 0F); //web
		actions[0].frame = 5;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.6601563F, 0.6230469F, 0F); //web
		actions[1].frame = 32;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.4824219F, 1.244141F, 0F); //web
		actions[2].frame = 36;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.4824219F, 1.244141F, 0F); //web
		actions[3].frame = 30;
		actions[3].mouse = new Vector3(0F, 0F, 0F);

	}

	void level58_0 () {
		actions = new action[8];

		actions[0].id = new Vector3(-0.2441406F, 1.519531F, 0F); //web
		actions[0].frame = 40;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.7441406F, -0.5664063F, -0.015625F); //sluggish
		actions[1].frame = 50;
		actions[1].mouse = new Vector3(-0.9166666F, -0.984375F, 0F);
		actions[2].id = new Vector3(-0.2441406F, 1.519531F, 0F); //web
		actions[2].frame = 47;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.2441406F, 1.519531F, 0F); //web
		actions[3].frame = 132;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.5136719F, 0.1894531F, 0F); //web
		actions[4].frame = 50;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.7675781F, 0.4101563F, 0F); //web
		actions[5].frame = 80;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.5136719F, 0.1894531F, 0F); //web
		actions[6].frame = 40;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.7675781F, 0.4101563F, 0F); //web
		actions[7].frame = 70;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
	}
	void level58_1 () {
		actions = new action[4];

		actions[0].id = new Vector3(0.05260581F, -0.380742F, 0F); //sluggish
		actions[0].frame = 91;
		actions[0].mouse = new Vector3(-0.6276346F, 0.6182671F, 0F);
		actions[1].id = new Vector3(-0.3300781F, 1.527344F, 0F); //web
		actions[1].frame = 190;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.05260581F, -0.380742F, 0F); //sluggish
		actions[2].frame = 50;
		actions[2].mouse = new Vector3(0.165F, -1.124122F, 0F);
		actions[3].id = new Vector3(0.05260581F, -0.380742F, 0F); //sluggish
		actions[3].frame = 200;
		actions[3].mouse = new Vector3(-0.02341918F, -0.9320843F, 0F);
	}

	void level59_0 () {
		actions = new action[9];

		actions[0].id = new Vector3(-0.6347656F, -0.4335938F, 0F); //groot
		actions[0].frame = 40;
		actions[0].mouse = new Vector3(0.5624999F, 0.02083325F, 0F);
		actions[1].id = new Vector3(-0.8066406F, 1.478516F, 0F); //web
		actions[1].frame = 40;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.8457031F, 1.126953F, 0F); //web
		actions[2].frame = 61;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.8066406F, 1.478516F, 0F); //web
		actions[3].frame = 137;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.8066406F, 1.478516F, 0F); //web
		actions[4].frame = 37;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.8457031F, 1.126953F, 0F); //web
		actions[5].frame = 120;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.6347656F, -0.4335938F, 0F); //groot
		actions[6].frame = 150;
		actions[6].mouse = new Vector3(-0.5F, -0.3541666F, 0F);
		actions[7].id = new Vector3(-0.6347656F, -0.4335938F, 0F); //groot
		actions[7].frame = 61;
		actions[7].mouse = new Vector3(0.5052083F, -1.005208F, 0F);
		actions[8].id = new Vector3(-0.6347656F, -0.4335938F, 0F); //groot
		actions[8].frame = 205;
		actions[8].mouse = new Vector3(-0.53125F, -0.4270834F, 0F);
	}
	void level59_1 () {
		actions = new action[11];
		actions[0].id = new Vector3(0.1875F, 1.6875F, 0F); //web
		actions[0].frame = 80;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.1875F, 1.6875F, 0F); //web
		actions[1].frame = 11;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.1875F, 1.6875F, 0F); //web
		actions[2].frame = 14;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.1875F, 1.6875F, 0F); //web
		actions[3].frame = 61;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.59375F, 0.3066406F, 0F); //web
		actions[4].frame = 36;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.59375F, 0.3066406F, 0F); //web
		actions[5].frame = 30;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.59375F, 0.3066406F, 0F); //web
		actions[6].frame = 10;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.59375F, 0.3066406F, 0F); //web
		actions[7].frame = 90;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.2988281F, -0.9082031F, 0F); //web
		actions[8].frame = 23;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(0.7285156F, 0.2695313F, 0F); //web
		actions[9].frame = 63;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(0.7285156F, 0.2695313F, 0F); //web
		actions[10].frame = 181;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
	}

	void level60_0 () {
		actions = new action[20];
		actions[0].id = new Vector3(-0.8183594F, 1.488281F, 0F); //web
		actions[0].frame = 33;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.8183594F, 1.488281F, 0F); //web
		actions[1].frame = 50;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.8183594F, 1.488281F, 0F); //web
		actions[2].frame = 28;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.6777344F, 0.9550781F, 0F); //web
		actions[3].frame = 44;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.8183594F, 1.488281F, 0F); //web
		actions[4].frame = 111;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //web
		actions[5].frame = 62;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //web
		actions[6].frame = 35;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.6777344F, 0.9550781F, 0F); //web
		actions[7].frame = 42;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.6777344F, 0.9550781F, 0F); //web
		actions[8].frame = 37;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(-0.3007813F, -0.984375F, 0F); //web
		actions[9].frame = 65;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(-0.3007813F, -0.984375F, 0F); //web
		actions[10].frame = 44;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
		actions[11].id = new Vector3(0.6777344F, 0.9550781F, 0F); //web
		actions[11].frame = 60;
		actions[11].mouse = new Vector3(0F, 0F, 0F);
		actions[12].id = new Vector3(-0.3007813F, -0.984375F, 0F); //web
		actions[12].frame = 70;
		actions[12].mouse = new Vector3(0F, 0F, 0F);
		actions[13].id = new Vector3(0.40625F, -0.3417969F, 0F); //web
		actions[13].frame = 126;
		actions[13].mouse = new Vector3(0F, 0F, 0F);
		actions[14].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //web
		actions[14].frame = 98;
		actions[14].mouse = new Vector3(0F, 0F, 0F);
		actions[15].id = new Vector3(0.40625F, -0.3417969F, 0F); //web
		actions[15].frame = 102;
		actions[15].mouse = new Vector3(0F, 0F, 0F);
		actions[16].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //web
		actions[16].frame = 110;
		actions[16].mouse = new Vector3(0F, 0F, 0F);
		actions[17].id = new Vector3(0.40625F, -0.3417969F, 0F); //web
		actions[17].frame = 127;
		actions[17].mouse = new Vector3(0F, 0F, 0F);
		actions[18].id = new Vector3(-0.3007813F, -0.984375F, 0F); //web
		actions[18].frame = 38;
		actions[18].mouse = new Vector3(0F, 0F, 0F);
		actions[19].id = new Vector3(0.40625F, -0.3417969F, 0F); //web
		actions[19].frame = 16;
		actions[19].mouse = new Vector3(0F, 0F, 0F);
	}
	void level60_1 () {
		actions = new action[17];
		actions[0].id = new Vector3(-0.8183594F, 1.488281F, 0F); //web
		actions[0].frame = 15;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.8183594F, 1.488281F, 0F); //web
		actions[1].frame = 54;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.8183594F, 1.488281F, 0F); //web
		actions[2].frame = 21;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.6777344F, 0.9550781F, 0F); //web
		actions[3].frame = 25;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.8183594F, 1.488281F, 0F); //web
		actions[4].frame = 20;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.6777344F, 0.9550781F, 0F); //web
		actions[5].frame = 15;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.6777344F, 0.9550781F, 0F); //web
		actions[6].frame = 30;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.3007813F, -0.984375F, 0F); //web
		actions[7].frame = 20;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(-0.3007813F, -0.984375F, 0F); //web
		actions[8].frame = 35;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(0.6777344F, 0.9550781F, 0F); //web
		actions[9].frame = 60;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(-0.3007813F, -0.984375F, 0F); //web
		actions[10].frame = 60;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
		actions[11].id = new Vector3(0.40625F, -0.3417969F, 0F); //web
		actions[11].frame = 50;
		actions[11].mouse = new Vector3(0F, 0F, 0F);
		actions[12].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //web
		actions[12].frame = 30;
		actions[12].mouse = new Vector3(0F, 0F, 0F);
		actions[13].id = new Vector3(0.40625F, -0.3417969F, 0F); //web
		actions[13].frame = 30;
		actions[13].mouse = new Vector3 (0F, 0F, 0F);
		actions[14].id = new Vector3(0.40625F, -0.3417969F, 0F); //web
		actions[14].frame = 30;
		actions[14].mouse = new Vector3 (0F, 0F, 0F);
		actions[15].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //web
		actions[15].frame = 30;
		actions[15].mouse = new Vector3(0F, 0F, 0F);
		actions[16].id = new Vector3(-0.3007813F, -0.984375F, 0F); //web
		actions[16].frame = 20;
		actions[16].mouse = new Vector3(0F, 0F, 0F);
	}

	void level61_0 () {
		actions = new action[14];

		actions[0].id = new Vector3(-0.9785156F, -0.1660156F, 0F); //web
		actions[0].frame = 63;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.03320313F, -0.05664063F, 0F); //web
		actions[1].frame = 97;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.9785156F, -0.1660156F, 0F); //web
		actions[2].frame = 48;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.9785156F, -0.1660156F, 0F); //web
		actions[3].frame = 32;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.9785156F, -0.1660156F, 0F); //web
		actions[4].frame = 42;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.7011719F, -0.2910156F, -0.015625F); //sluggish
		actions[5].frame = 47;
		actions[5].mouse = new Vector3(-1.098958F, -0.7916666F, 0F);
		actions[6].id = new Vector3(-0.03320313F, -0.05664063F, 0F); //web
		actions[6].frame = 63;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.6132813F, 0.3632813F, -0.015625F); //sluggish (1)
		actions[7].frame = 80;
		actions[7].mouse = new Vector3(0.7864583F, -0.234375F, 0F);
		actions[8].id = new Vector3(-0.1640625F, 1.648438F, 0F); //web
		actions[8].frame = 209;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(-0.1640625F, 1.648438F, 0F); //web
		actions[9].frame = 74;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(-0.03320313F, -0.05664063F, 0F); //web
		actions[10].frame = 38;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
		actions[11].id = new Vector3(-0.03320313F, -0.05664063F, 0F); //web
		actions[11].frame = 106;
		actions[11].mouse = new Vector3(0F, 0F, 0F);
		actions[12].id = new Vector3(-0.01757813F, -1.805967F, -0.01953125F); //yeti body
		actions[12].frame = 18;
		actions[12].mouse = new Vector3(0F, 0F, 0F);
		actions[13].id = new Vector3(-0.01757813F, -1.805967F, -0.01953125F); //yeti body
		actions[13].frame = 0;
		actions[13].mouse = new Vector3(0F, 0F, 0F);
	}
	void level61_1 () {
		actions = new action[2];
		actions[0].id = new Vector3(0.4882813F, 1.162109F, 0F); //web
		actions[0].frame = 25;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.4882813F, 1.162109F, 0F); //web
		actions[1].frame = 144;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level62_0 () {
		actions = new action[7];

		actions[0].id = new Vector3(0.7519531F, 0.7070313F, 0F); //destroyer
		actions[0].frame = 40;
		actions[0].mouse = new Vector3(0.296875F, 0.6041667F, 0F);
		actions[1].id = new Vector3(0.2285156F, 0.1464844F, 0F); //web
		actions[1].frame = 263;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.2285156F, 0.1464844F, 0F); //web
		actions[2].frame = 108;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.2285156F, 0.1464844F, 0F); //web
		actions[3].frame = 26;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.7617188F, -0.1953125F, 0F); //web
		actions[4].frame = 77;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.2285156F, 0.1464844F, 0F); //web
		actions[5].frame = 76;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.7617188F, -0.1953125F, 0F); //web
		actions[6].frame = 20;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
	}
	void level62_1 () {
		actions = new action[4];
		actions[0].id = new Vector3(-0.8300781F, 0.59375F, 0F); //web
		actions[0].frame = 18;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.08398438F, 1.445313F, 0F); //web
		actions[1].frame = 43;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.05859375F, 0.7695313F, 0F); //web
		actions[2].frame = 52;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.7324219F, 0.734375F, 0F); //web
		actions[3].frame = 214;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}

	void level63_0 () {
		actions = new action[5];

		actions[0].id = new Vector3(0.78125F, -0.2402344F, 0F); //destroyer
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(-0.765625F, 0.8020833F, 0F);
		actions[1].id = new Vector3(-0.6464844F, -0.625F, 0F); //destroyer
		actions[1].frame = 315;
		actions[1].mouse = new Vector3(0.2395833F, 0.4479167F, 0F);
		actions[2].id = new Vector3(-0.1640625F, 0.1914063F, 0F); //cloud
		actions[2].frame = 106;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.4375F, -0.4570313F, 0F); //web
		actions[3].frame = 90;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.4375F, -0.4570313F, 0F); //web
		actions[4].frame = 112;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
	}
	void level63_1 () {
		actions = new action[2];
		actions[0].id = new Vector3(-0.8007813F, 1.097656F, 0F); //destroyer
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(-0.5199063F, 0.7822013F, 0F);
		actions[1].id = new Vector3(-0.7382813F, -1.232422F, 0F); //destroyer
		actions[1].frame = 250;
		actions[1].mouse = new Vector3(0.117096F, 0.004683733F, 0F);
	}

	void level64_0 () {
		actions = new action[6];

		actions[0].id = new Vector3(0.6171875F, 0.8359375F, 0F); //destroyer
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0.04166664F, 1.286458F, 0F);
		actions[1].id = new Vector3(-0.640625F, 0.09179688F, 0F); //sluggish
		actions[1].frame = 200;
		actions[1].mouse = new Vector3(-0.6458333F, -0.390625F, 0F);
		actions[2].id = new Vector3(-0.0546875F, 0.0390625F, 0F); //web
		actions[2].frame = 73;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.0546875F, 0.0390625F, 0F); //web
		actions[3].frame = 30;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.8945313F, 0.8964844F, 0F); //destroyer (1)
		actions[4].frame = 50;
		actions[4].mouse = new Vector3(-0.2239583F, -0.3958334F, 0F);
		actions[5].id = new Vector3(-0.640625F, 0.09179688F, 0F); //sluggish
		actions[5].frame = 60;
		actions[5].mouse = new Vector3(-0.9218751F, 0.671875F, 0F);
	}
	void level64_1 () {
		actions = new action[7];
		actions[0].id = new Vector3(0.2773438F, 1.501953F, 0F); //web
		actions[0].frame = 89;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.2773438F, 1.501953F, 0F); //web
		actions[1].frame = 41;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.2773438F, 1.501953F, 0F); //web
		actions[2].frame = 58;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.2773438F, 1.501953F, 0F); //web
		actions[3].frame = 21;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.2773438F, 1.501953F, 0F); //web
		actions[4].frame = 109;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.2773438F, 1.501953F, 0F); //web
		actions[5].frame = 42;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.2773438F, 1.501953F, 0F); //web
		actions[6].frame = 106;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
	}

	void level65_0 () {
		actions = new action[1];

		actions[0].id = new Vector3(0.6894531F, 1.390625F, 0F); //destroyer
		actions[0].frame = 50;
		actions[0].mouse = new Vector3(0.03124998F, 0.4270833F, 0F);
	}
	void level65_1 () {
		actions = new action[9];
		actions [0].id = new Vector3 (-0.5703125F, 0.5175781F, 0F); //web
		actions [0].frame = 30;
		actions [0].mouse = new Vector3 (0F, 0F, 0F);
		actions [1].id = new Vector3 (-0.5703125F, 0.5175781F, 0F); //web
		actions [1].frame = 12;
		actions [1].mouse = new Vector3 (0F, 0F, 0F);
		actions [2].id = new Vector3 (-0.6933594F, 1.050781F, 0F); //web
		actions [2].frame = 23;
		actions [2].mouse = new Vector3 (0F, 0F, 0F);
		actions [3].id = new Vector3 (0.703125F, -0.07226563F, 0F); //web
		actions [3].frame = 33;
		actions [3].mouse = new Vector3 (0F, 0F, 0F);
		actions [4].id = new Vector3 (0.703125F, -0.07226563F, 0F); //web
		actions [4].frame = 27;
		actions [4].mouse = new Vector3 (0F, 0F, 0F);
		actions [5].id = new Vector3 (0.703125F, -0.07226563F, 0F); //web
		actions [5].frame = 26;
		actions [5].mouse = new Vector3 (0F, 0F, 0F);
		actions [6].id = new Vector3 (-0.5703125F, 0.5175781F, 0F); //web
		actions [6].frame = 42;
		actions [6].mouse = new Vector3 (0F, 0F, 0F);
		actions [7].id = new Vector3 (-0.5703125F, 0.5175781F, 0F); //web
		actions [7].frame = 22;
		actions [7].mouse = new Vector3 (0F, 0F, 0F);
		actions [8].id = new Vector3 (-0.5703125F, 0.5175781F, 0F); //web
		actions [8].frame = 9;
		actions [8].mouse = new Vector3 (0F, 0F, 0F);
	}

	void level66_0 () {
		actions = new action[10];

		actions[0].id = new Vector3(-0.1269531F, 1.787109F, 0F); //destroyer
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0.2864583F, 1.307292F, 0F);
		actions[1].id = new Vector3(-0.1816406F, 1.212891F, 0F); //web
		actions[1].frame = 39;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.1816406F, 1.212891F, 0F); //web
		actions[2].frame = 43;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.1816406F, 1.212891F, 0F); //web
		actions[3].frame = 39;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.1816406F, 1.212891F, 0F); //web
		actions[4].frame = 50;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.1816406F, 1.212891F, 0F); //web
		actions[5].frame = 35;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.4179688F, -0.6582031F, 0F); //destroyer
		actions[6].frame = 50;
		actions[6].mouse = new Vector3(-0.1458333F, -0.04166663F, 0F);
		actions[7].id = new Vector3(-0.1816406F, 1.212891F, 0F); //web
		actions[7].frame = 185;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(-0.8964844F, -0.7617188F, 0F); //web
		actions[8].frame = 37;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(-0.8964844F, -0.7617188F, 0F); //web
		actions[9].frame = 117;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
	}
	void level66_1 () {
		actions = new action[4];
		actions[0].id = new Vector3(-0.4160156F, 1.046875F, 0F); //web
		actions[0].frame = 79;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.4160156F, 1.046875F, 0F); //web
		actions[1].frame = 7;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.6328125F, 0.7421875F, 0F); //web
		actions[2].frame = 49;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.6328125F, 0.7421875F, 0F); //web
		actions[3].frame = 100;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}

	void level67_0 () {
		actions = new action[3];

		actions[0].id = new Vector3(-0.7910156F, -0.1816406F, 0F); //destroyer
		actions[0].frame = 40;
		actions[0].mouse = new Vector3(-0.1822916F, 1.348958F, 0F);
		actions[1].id = new Vector3(0.03515625F, -1.115234F, 0F); //destroyer
		actions[1].frame = 178;
		actions[1].mouse = new Vector3(0.546875F, 0.5833333F, 0F);
		actions[2].id = new Vector3(0.01953125F, -1.371094F, 0F); //destroyer
		actions[2].frame = 406;
		actions[2].mouse = new Vector3(0.6510417F, -0.5989584F, 0F);
/*
		actions[0].id = new Vector3(0.03515625F, -1.115234F, 0F); //destroyer
		actions[0].frame = 165;
		actions[0].mouse = new Vector3(0.8437499F, -0.4583334F, 0F);
		actions[1].id = new Vector3(0.01953125F, -1.371094F, 0F); //destroyer
		actions[1].frame = 165;
		actions[1].mouse = new Vector3(-0.3645833F, 1.484375F, 0F);
		actions[2].id = new Vector3(-0.7910156F, -0.1816406F, 0F); //destroyer
		actions[2].frame = 153;
		actions[2].mouse = new Vector3(0.9062501F, 0.8020833F, 0F);*/
	}

	void level67_1 () {
		actions = new action[7];
		actions[0].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //sluggish
		actions[0].frame = 48;
		actions[0].mouse = new Vector3(-0.6F, -0.6229508F, 0F);
		actions[1].id = new Vector3(-0.06054688F, -0.9290141F, -0.01953125F); //yeti body
		actions[1].frame = 49;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.06054688F, -0.9290141F, -0.01953125F); //yeti body
		actions[2].frame = 0;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.06054688F, -0.9290141F, -0.01953125F); //yeti body
		actions[3].frame = 82;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.06054688F, -0.9290141F, -0.01953125F); //yeti body
		actions[4].frame = 0;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.5566406F, 0.5917969F, 0F); //cloud
		actions[5].frame = 40;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.4101563F, 0.2773438F, 0F); //sluggish
		actions[6].frame = 87;
		actions[6].mouse = new Vector3(-1.124122F, -0.2014052F, 0F);
	}

	void level68_0 () {
		actions = new action[4];
		
		actions[0].id = new Vector3(-0.515625F, 1.191406F, 0F); //web
		actions[0].frame = 92;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.7089844F, 0.3984375F, 0F); //sluggish
		actions[1].frame = 100;
		actions[1].mouse = new Vector3(0.7822014F, -0.01405156F, 0F);
		actions[2].id = new Vector3(-0.515625F, 1.191406F, 0F); //web
		actions[2].frame = 36;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.515625F, 1.191406F, 0F); //web
		actions[3].frame = 136;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}
	void level68_1 () {
		actions = new action[8];
		actions[0].id = new Vector3(0.00390625F, 0.8945313F, 0F); //web
		actions[0].frame = 21;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.7363281F, -0.921875F, 0F); //destroyer
		actions[1].frame = 91;
		actions[1].mouse = new Vector3(-0.14F, 0.6199999F, 0F);
		actions[2].id = new Vector3(0.00390625F, 0.8945313F, 0F); //web
		actions[2].frame = 38;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.00390625F, 0.8945313F, 0F); //web
		actions[3].frame = 121;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.1972656F, 1.388672F, 0F); //destroyer
		actions[4].frame = 111;
		actions[4].mouse = new Vector3(0.03999996F, 0.9000001F, 0F);
		actions[5].id = new Vector3(0.00390625F, 0.8945313F, 0F); //web
		actions[5].frame = 82;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.00390625F, 0.8945313F, 0F); //web
		actions[6].frame = 31;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.00390625F, 0.8945313F, 0F); //web
		actions[7].frame = 101;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
	}

	void level69_0 () {
		actions = new action[11];

		actions[0].id = new Vector3(-0.9570313F, 0.9765625F, 0F); //destroyer (1)
		actions[0].frame = 125;
		actions[0].mouse = new Vector3(0.234375F, -0.005208373F, 0F);
		actions[1].id = new Vector3(0.8691406F, 0.9980469F, 0F); //destroyer
		actions[1].frame = 147;
		actions[1].mouse = new Vector3(-0.171875F, 1.338542F, 0F);
		actions[2].id = new Vector3(-0.3007813F, 0.0546875F, 0F); //web
		actions[2].frame = 127;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.3007813F, 0.0546875F, 0F); //web
		actions[3].frame = 94;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.8398438F, 0.4101563F, 0F); //sluggish (1)
		actions[4].frame = 85;
		actions[4].mouse = new Vector3(-0.8645834F, -0.015625F, 0F);
		actions[5].id = new Vector3(-0.3007813F, 0.0546875F, 0F); //web
		actions[5].frame = 33;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.3007813F, 0.0546875F, 0F); //web
		actions[6].frame = 44;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.8398438F, 0.4101563F, 0F); //sluggish (1)
		actions[7].frame = 98;
		actions[7].mouse = new Vector3(-1.213542F, -0.1666666F, 0F);
		actions[8].id = new Vector3(0.4375F, 0.6777344F, 0F); //web
		actions[8].frame = 59;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(0.4375F, 0.6777344F, 0F); //web
		actions[9].frame = 99;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(0.4375F, 0.6777344F, 0F); //web
		actions[10].frame = 46;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
	}
	void level69_1 () {
		actions = new action[8];
		actions[0].id = new Vector3(-0.375F, 0.9902344F, 0F); //cloud
		actions[0].frame = 161;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.828125F, 1.174502F, -0.01953125F); //yeti body
		actions[1].frame = 24;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.828125F, 1.174502F, -0.01953125F); //yeti body
		actions[2].frame = 0;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.828125F, 1.174502F, -0.01953125F); //yeti body
		actions[3].frame = 76;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.828125F, 1.174502F, -0.01953125F); //yeti body
		actions[4].frame = 0;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.828125F, 1.174502F, -0.01953125F); //yeti body
		actions[5].frame = 60;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.828125F, 1.174502F, -0.01953125F); //yeti body
		actions[6].frame = 0;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.3867188F, -0.7460938F, 0F); //cloud
		actions[7].frame = 10;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
	}

	void level70_0 () {
		actions = new action[17];

		actions[0].id = new Vector3(0.7402344F, 1.099609F, 0F); //destroyer
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(-0.5104167F, 1.25F, 0F);
		actions[1].id = new Vector3(0.01953125F, 0.1875F, 0F); //groot
		actions[1].frame = 40;
		actions[1].mouse = new Vector3(0.9010417F, -0.6875F, 0F);
		actions[2].id = new Vector3(0.8458844F, -0.07405625F, 0F); //web
		actions[2].frame = 158;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.8458844F, -0.07405625F, 0F); //web
		actions[3].frame = 38;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.4863281F, -0.09960938F, 0F); //web
		actions[4].frame = 33;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.8458844F, -0.07405625F, 0F); //web
		actions[5].frame = 40;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.4863281F, -0.09960938F, 0F); //web
		actions[6].frame = 40;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.4767438F, -1.242025F, 0F); //web
		actions[7].frame = 120;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.8458844F, -0.07405625F, 0F); //web
		actions[8].frame = 40;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(-0.4863281F, -0.09960938F, 0F); //web
		actions[9].frame = 85;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(0.01953125F, 0.1875F, 0F); //groot
		actions[10].frame = 40;
		actions[10].mouse = new Vector3(0.1249999F, 0.2083333F, 0F);
		actions[11].id = new Vector3(0.01953125F, 0.1875F, 0F); //groot
		actions[11].frame = 40;
		actions[11].mouse = new Vector3(-0.1927084F, -1.010417F, 0F);
		actions[12].id = new Vector3(0.4767438F, -1.242025F, 0F); //web
		actions[12].frame = 40;
		actions[12].mouse = new Vector3(0F, 0F, 0F);
		actions[13].id = new Vector3(0.8458844F, -0.07405625F, 0F); //web
		actions[13].frame = 80;
		actions[13].mouse = new Vector3(0F, 0F, 0F);
		actions[14].id = new Vector3(0.01953125F, 0.1875F, 0F); //groot
		actions[14].frame = 132;
		actions[14].mouse = new Vector3(0.01562499F, 0.109375F, 0F);
		actions[15].id = new Vector3(-0.4863281F, -0.09960938F, 0F); //web
		actions[15].frame = 75;
		actions[15].mouse = new Vector3(0F, 0F, 0F);
		actions[16].id = new Vector3(0.8458844F, -0.07405625F, 0F); //web
		actions[16].frame = 11;
		actions[16].mouse = new Vector3(0F, 0F, 0F);
	}
	void level70_1 () {
		actions = new action[9];
		actions[0].id = new Vector3(0.4414063F, 1.056641F, 0F); //web
		actions[0].frame = 6;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.5449219F, 1.283203F, 0F); //web
		actions[1].frame = 19;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.4414063F, 1.056641F, 0F); //web
		actions[2].frame = 95;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.4414063F, 1.056641F, 0F); //web
		actions[3].frame = 20;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.5449219F, 1.283203F, 0F); //web
		actions[4].frame = 30;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.4414063F, 1.056641F, 0F); //web
		actions[5].frame = 20;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.5449219F, 1.283203F, 0F); //web
		actions[6].frame = 71;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.5449219F, 1.283203F, 0F); //web
		actions[7].frame = 19;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(-0.5449219F, 1.283203F, 0F); //web
		actions[8].frame = 145;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
	}

	void level71_0 () {
		actions = new action[17];

		actions[0].id = new Vector3(0.90625F, 0.8945313F, 0F); //destroyer
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(-0.8541666F, 1.28125F, 0F);
		actions[1].id = new Vector3(0.3359375F, -1.68292F, -0.01953125F); //yeti body
		actions[1].frame = 105;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.3359375F, -1.68292F, -0.01953125F); //yeti body
		actions[2].frame = 0;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.6699219F, 1.070313F, 0F); //web
		actions[3].frame = 34;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.6699219F, 1.070313F, 0F); //web
		actions[4].frame = 118;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.4160156F, 0.7929688F, 0F); //web
		actions[5].frame = 42;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.4160156F, 0.7929688F, 0F); //web
		actions[6].frame = 120;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.4160156F, 0.7929688F, 0F); //web
		actions[7].frame = 26;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(-0.4160156F, 0.7929688F, 0F); //web
		actions[8].frame = 70;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(0.3359375F, -1.68292F, -0.01953125F); //yeti body
		actions[9].frame = 88;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(0.3359375F, -1.68292F, -0.01953125F); //yeti body
		actions[10].frame = 0;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
		actions[11].id = new Vector3(0.7421875F, -0.8691406F, -0.015625F); //sluggish
		actions[11].frame = 60;
		actions[11].mouse = new Vector3(0.71875F, -1.229167F, 0F);
		actions[12].id = new Vector3(0.6699219F, 1.070313F, 0F); //web
		actions[12].frame = 44;
		actions[12].mouse = new Vector3(0F, 0F, 0F);
		actions[13].id = new Vector3(0.6699219F, 1.070313F, 0F); //web
		actions[13].frame = 159;
		actions[13].mouse = new Vector3(0F, 0F, 0F);
		actions[14].id = new Vector3(0.7421875F, -0.8691406F, -0.015625F); //sluggish
		actions[14].frame = 60;
		actions[14].mouse = new Vector3(1.177083F, -1.067708F, 0F);
		actions[15].id = new Vector3(0.3359375F, -1.68292F, -0.01953125F); //yeti body
		actions[15].frame = 30;
		actions[15].mouse = new Vector3(0F, 0F, 0F);
		actions[16].id = new Vector3(0.3359375F, -1.68292F, -0.01953125F); //yeti body
		actions[16].frame = 0;
		actions[16].mouse = new Vector3(0F, 0F, 0F);
	}
	void level71_1 () {
		actions = new action[6];
		actions[0].id = new Vector3(0.3203125F, -0.2773438F, 0F); //sluggish
		actions[0].frame = 66;
		actions[0].mouse = new Vector3(0.6885245F, -0.6229508F, 0F);
		actions[1].id = new Vector3(-0.2988281F, 0.6523438F, 0F); //sluggish
		actions[1].frame = 91;
		actions[1].mouse = new Vector3(-0.6323185F, 0.1358314F, 0F);
		actions[2].id = new Vector3(-0.2988281F, 0.6523438F, 0F); //sluggish
		actions[2].frame = 100;
		actions[2].mouse = new Vector3(-1.039813F, 0.1124122F, 0F);
		actions[3].id = new Vector3(-0.2988281F, 0.6523438F, 0F); //sluggish
		actions[3].frame = 150;
		actions[3].mouse = new Vector3(0.4262296F, 1.077283F, 0F);
		actions[4].id = new Vector3(0.3203125F, -0.2773438F, 0F); //sluggish
		actions[4].frame = 150;
		actions[4].mouse = new Vector3(0.3044497F, -0.7587821F, 0F);
		actions[5].id = new Vector3(0.3203125F, -0.2773438F, 0F); //sluggish
		actions[5].frame = 120;
		actions[5].mouse = new Vector3(-0.1498829F, -0.0374707F, 0F);
	}

	void level72_0 () {
		actions = new action[7];

		actions[0].id = new Vector3(-0.6386719F, -1.103516F, 0F); //destroyer (2)
		actions[0].frame = 152;
		actions[0].mouse = new Vector3(0.28125F, -0.7135416F, 0F);
		actions[1].id = new Vector3(-0.9492188F, 0.3164063F, 0F); //destroyer (1)
		actions[1].frame = 126;
		actions[1].mouse = new Vector3(0.7135417F, -0.3697916F, 0F);
		actions[2].id = new Vector3(-0.8671875F, 0.671875F, 0F); //destroyer
		actions[2].frame = 171;
		actions[2].mouse = new Vector3(0.7604167F, 1.208333F, 0F);
		actions[3].id = new Vector3(0.4863281F, -1.591123F, -0.01953125F); //yeti body
		actions[3].frame = 88;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.4863281F, -1.591123F, -0.01953125F); //yeti body
		actions[4].frame = 0;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.4863281F, -1.591123F, -0.01953125F); //yeti body
		actions[5].frame = 187;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.4863281F, -1.591123F, -0.01953125F); //yeti body
		actions[6].frame = 0;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
	}
	void level72_1 () {
		actions = new action[6];
		actions[0].id = new Vector3(0.6640625F, 0.5761719F, 0F); //web
		actions[0].frame = 12;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.6640625F, 0.5761719F, 0F); //web
		actions[1].frame = 18;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.7421875F, 0.7597656F, 0F); //web
		actions[2].frame = 98;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.7421875F, 0.7597656F, 0F); //web
		actions[3].frame = 126;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.6640625F, 0.5761719F, 0F); //web
		actions[4].frame = 51;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.7421875F, 0.7597656F, 0F); //web
		actions[5].frame = 12;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}

	void level73_0 () {
		actions = new action[20];

		actions[0].id = new Vector3(-0.07617188F, 0.1914063F, 0F); //groot
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0.2916667F, -1.104167F, 0F);
		actions[1].id = new Vector3(0.8203125F, -1.054688F, -0.015625F); //sluggish
		actions[1].frame = 40;
		actions[1].mouse = new Vector3(0.8854166F, -1.489583F, 0F);
		actions[2].id = new Vector3(-0.07617188F, 0.1914063F, 0F); //groot
		actions[2].frame = 90;
		actions[2].mouse = new Vector3(-0.01562499F, 0.1041667F, 0F);
		actions[3].id = new Vector3(0.8203125F, -1.054688F, -0.015625F); //sluggish
		actions[3].frame = 140;
		actions[3].mouse = new Vector3(0.765625F, -1.640625F, 0F);
		actions[4].id = new Vector3(0.1328125F, 1.179688F, 0F); //web
		actions[4].frame = 43;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.1328125F, 1.179688F, 0F); //web
		actions[5].frame = 43;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.8710938F, 0.9101563F, 0F); //web
		actions[6].frame = 33;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.1328125F, 1.179688F, 0F); //web
		actions[7].frame = 139;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(-0.8710938F, 0.9101563F, 0F); //web
		actions[8].frame = 43;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(-0.8710938F, 0.9101563F, 0F); //web
		actions[9].frame = 29;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(0.1328125F, 1.179688F, 0F); //web
		actions[10].frame = 37;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
		actions[11].id = new Vector3(-1.025391F, -0.4941406F, 0F); //groot
		actions[11].frame = 80;
		actions[11].mouse = new Vector3(-0.2447917F, -1.447917F, 0F);
		actions[12].id = new Vector3(-0.8710938F, 0.9101563F, 0F); //web
		actions[12].frame = 175;
		actions[12].mouse = new Vector3(0F, 0F, 0F);
		actions[13].id = new Vector3(0.8203125F, -1.054688F, -0.015625F); //sluggish
		actions[13].frame = 100;
		actions[13].mouse = new Vector3(0.7916666F, -1.598958F, 0F);
		actions[14].id = new Vector3(0.1328125F, 1.179688F, 0F); //web
		actions[14].frame = 48;
		actions[14].mouse = new Vector3(0F, 0F, 0F);
		actions[15].id = new Vector3(0.1328125F, 1.179688F, 0F); //web
		actions[15].frame = 49;
		actions[15].mouse = new Vector3(0F, 0F, 0F);
		actions[16].id = new Vector3(-0.8710938F, 0.9101563F, 0F); //web
		actions[16].frame = 40;
		actions[16].mouse = new Vector3(0F, 0F, 0F);
		actions[17].id = new Vector3(-1.025391F, -0.4941406F, 0F); //groot
		actions[17].frame = 31;
		actions[17].mouse = new Vector3(-0.9531251F, -0.4947916F, 0F);
		actions[18].id = new Vector3(-1.025391F, -0.4941406F, 0F); //groot
		actions[18].frame = 30;
		actions[18].mouse = new Vector3(-0.359375F, -0.6354166F, 0F);
		actions[19].id = new Vector3(-0.8710938F, 0.9101563F, 0F); //web
		actions[19].frame = 74;
		actions[19].mouse = new Vector3(0F, 0F, 0F);
	}
	void level73_1 () {
		actions = new action[14];
		actions[0].id = new Vector3(0.9160156F, 0.6894531F, 0F); //web
		actions[0].frame = 15;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.9160156F, 0.6894531F, 0F); //web
		actions[1].frame = 32;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.9160156F, 0.6894531F, 0F); //web
		actions[2].frame = 22;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.9160156F, 0.6894531F, 0F); //web
		actions[3].frame = 19;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.9160156F, 0.6894531F, 0F); //web
		actions[4].frame = 28;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.8925781F, 0.6503906F, 0F); //web
		actions[5].frame = 137;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.8925781F, 0.6503906F, 0F); //web
		actions[6].frame = 29;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.9160156F, 0.6894531F, 0F); //web
		actions[7].frame = 141;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(-0.8925781F, 0.6503906F, 0F); //web
		actions[8].frame = 33;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(0.9160156F, 0.6894531F, 0F); //web
		actions[9].frame = 43;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(0.9160156F, 0.6894531F, 0F); //web
		actions[10].frame = 48;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
		actions[11].id = new Vector3(0.9160156F, 0.6894531F, 0F); //web
		actions[11].frame = 53;
		actions[11].mouse = new Vector3(0F, 0F, 0F);
		actions[12].id = new Vector3(-0.8925781F, 0.6503906F, 0F); //web
		actions[12].frame = 45;
		actions[12].mouse = new Vector3(0F, 0F, 0F);
		actions[13].id = new Vector3(-0.8925781F, 0.6503906F, 0F); //web
		actions[13].frame = 95;
		actions[13].mouse = new Vector3(0F, 0F, 0F);
	}

	void level74_0 () {
		actions = new action[11];

		actions[0].id = new Vector3(0.40625F, 0.2695313F, 0F); //web
		actions[0].frame = 45;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.40625F, 0.2695313F, 0F); //web
		actions[1].frame = 50;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.7441406F, 0.8613281F, -0.015625F); //sluggish
		actions[2].frame = 30;
		actions[2].mouse = new Vector3(1.015625F, 0.7395833F, 0F);
		actions[3].id = new Vector3(-0.5507813F, 0.3398438F, 0F); //web
		actions[3].frame = 58;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.5507813F, 0.3398438F, 0F); //web
		actions[4].frame = 50;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.40625F, 0.2695313F, 0F); //web
		actions[5].frame = 42;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.7421875F, -0.6699219F, 0F); //web
		actions[6].frame = 78;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.5507813F, 0.3398438F, 0F); //web
		actions[7].frame = 40;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.40625F, 0.2695313F, 0F); //web
		actions[8].frame = 40;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(-0.7421875F, -0.6699219F, 0F); //web
		actions[9].frame = 40;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(-0.5507813F, 0.3398438F, 0F); //web
		actions[10].frame = 40;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
	}
	void level74_1 () {
		actions = new action[3];
		actions[0].id = new Vector3(0.2382813F, 1.503906F, 0F); //web
		actions[0].frame = 15;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.2382813F, 1.503906F, 0F); //web
		actions[1].frame = 30;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.2382813F, 1.503906F, 0F); //web
		actions[2].frame = 25;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
	}

	void level75_0 () {
		actions = new action[6];

		actions[0].id = new Vector3(-0.1699219F, 0.08789063F, 0F); //destroyer
		actions[0].frame = 40;
		actions[0].mouse = new Vector3(0.0520833F, 0.5364583F, 0F);
		actions[1].id = new Vector3(0.2148438F, 1.083984F, 0F); //groot
		actions[1].frame = 40;
		actions[1].mouse = new Vector3(0.8593749F, -0.2135416F, 0F);
		actions[2].id = new Vector3(-0.9023438F, 1.507813F, 0F); //web
		actions[2].frame = 40;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.9023438F, 1.507813F, 0F); //web
		actions[3].frame = 144;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.4355469F, 0.2597656F, 0F); //web
		actions[4].frame = 85;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.4355469F, 0.2597656F, 0F); //web
		actions[5].frame = 80;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}
	void level75_1 () {
		actions = new action[9];
		actions[0].id = new Vector3(0.3886719F, 0.5605469F, 0F); //cloud
		actions[0].frame = 9;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.8847656F, 1.332705F, -0.01953125F); //yeti body
		actions[1].frame = 54;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.8847656F, 1.332705F, -0.01953125F); //yeti body
		actions[2].frame = 0;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.7246094F, 0.3632813F, 0F); //sluggish
		actions[3].frame = 250;
		actions[3].mouse = new Vector3(0.8337237F, -0.09836066F, 0F);
		actions[4].id = new Vector3(0.8847656F, 1.332705F, -0.01953125F); //yeti body
		actions[4].frame = 38;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.8847656F, 1.332705F, -0.01953125F); //yeti body
		actions[5].frame = 0;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.09960938F, -0.6269531F, 0F); //cloud
		actions[6].frame = 112;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.7246094F, 0.3632813F, 0F); //sluggish
		actions[7].frame = 100;
		actions[7].mouse = new Vector3(1.105386F, 1.4F, 0F);
		actions[8].id = new Vector3(-0.6933594F, -0.9042969F, 0F); //sluggish
		actions[8].frame = 167;
		actions[8].mouse = new Vector3(-0.7025761F, -1.320843F, 0F);
	}
	// desert -----------------------------------------------------------------------------------------------------------
	void level76_0 () {
		actions = new action[10];

		actions[0].id = new Vector3(0.4824219F, 1.107422F, 0F); //web
		actions[0].frame = 103;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.4824219F, 1.107422F, 0F); //web
		actions[1].frame = 32;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.4824219F, 1.107422F, 0F); //web
		actions[2].frame = 99;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.4824219F, 1.107422F, 0F); //web
		actions[3].frame = 37;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.4824219F, 1.107422F, 0F); //web
		actions[4].frame = 90;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.7890625F, -0.0703125F, 0F); //cloud
		actions[5].frame = 61;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.6601563F, 0.6230469F, 0F); //web
		actions[6].frame = 70;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.4824219F, 1.107422F, 0F); //web
		actions[7].frame = 128;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.4824219F, 1.107422F, 0F); //web
		actions[8].frame = 126;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(-0.6601563F, 0.6230469F, 0F); //web
		actions[9].frame = 19;
		actions[9].mouse = new Vector3(0F, 0F, 0F);

	}
	void level76_1 () {
		actions = new action[2];
		actions[0].id = new Vector3(0.3183594F, 0.1875F, 0F); //groot
		actions[0].frame = 63;
		actions[0].mouse = new Vector3(-0.9184358F, 0.3597765F, 0F);
		actions[1].id = new Vector3(0.3183594F, 0.1875F, 0F); //groot
		actions[1].frame = 305;
		actions[1].mouse = new Vector3(0.2167597F, 0.1765363F, 0F);
	}

	void level77_0 () {
		actions = new action[4];

		actions[0].id = new Vector3(0.3203125F, -0.2773438F, 0F); //sluggish
		actions[0].frame = 88;
		actions[0].mouse = new Vector3(0.7291667F, -0.5989584F, 0F);
		actions[1].id = new Vector3(-0.2988281F, 0.6523438F, 0F); //sluggish
		actions[1].frame = 94;
		actions[1].mouse = new Vector3(-0.8020833F, 0.3802083F, 0F);
		actions[2].id = new Vector3(-0.2988281F, 0.6523438F, 0F); //sluggish
		actions[2].frame = 180;
		actions[2].mouse = new Vector3(-0.0520833F, 0.8229167F, 0F);
		actions[3].id = new Vector3(0.3203125F, -0.2773438F, 0F); //sluggish
		actions[3].frame = 185;
		actions[3].mouse = new Vector3(-0.2083334F, 0.109375F, 0F);
	}
	void level77_1 () {
		actions = new action[6];
		actions[0].id = new Vector3(-0.578125F, 0.2148438F, 0F); //groot
		actions[0].frame = 20;
		actions[0].mouse = new Vector3(0.5620609F, -0.2201405F, 0F);
		actions[1].id = new Vector3(-0.08398438F, 1.224609F, 0F); //web
		actions[1].frame = 32;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.08398438F, 1.224609F, 0F); //web
		actions[2].frame = 71;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.578125F, 0.2148438F, 0F); //groot
		actions[3].frame = 33;
		actions[3].mouse = new Vector3(-0.4824356F, 0.1779859F, 0F);
		actions[4].id = new Vector3(-0.578125F, 0.2148438F, 0F); //groot
		actions[4].frame = 35;
		actions[4].mouse = new Vector3(-0.4309133F, -1.100703F, 0F);
		actions[5].id = new Vector3(-0.08398438F, 1.224609F, 0F); //web
		actions[5].frame = 107;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}

	void level78_0 () {
		actions = new action[10];

		actions[0].id = new Vector3(0.9179688F, -0.6992188F, 0F); //destroyer
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(-0.3177084F, 0.25F, 0F);
		actions[1].id = new Vector3(0.1855469F, 0.2109375F, 0F); //web
		actions[1].frame = 80;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.1855469F, 0.2109375F, 0F); //web
		actions[2].frame = 80;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.6875F, 1.384766F, 0F); //web
		actions[3].frame = 178;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.6875F, 1.384766F, 0F); //web
		actions[4].frame = 227;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.125F, -0.8222656F, 0F); //web
		actions[5].frame = 129;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.125F, -0.8222656F, 0F); //web
		actions[6].frame = 50;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.8183594F, -0.7871094F, -0.015625F); //sluggish
		actions[7].frame = 40;
		actions[7].mouse = new Vector3(-0.9479167F, -1.270833F, 0F);
		actions[8].id = new Vector3(0.1855469F, 0.2109375F, 0F); //web
		actions[8].frame = 43;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(0.1855469F, 0.2109375F, 0F); //web
		actions[9].frame = 18;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
	}
	void level78_1 () {
		actions = new action[3];
		actions[0].id = new Vector3(-0.75F, 0.1328125F, 0F); //web
		actions[0].frame = 5;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.75F, 0.1328125F, 0F); //web
		actions[1].frame = 20;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.3554688F, 1.423828F, 0F); //web
		actions[2].frame = 44;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
	}

	void level79_0 () {
		actions = new action[6];

		actions[0].id = new Vector3(0.8378906F, -0.6464844F, 0F); //destroyer
		actions[0].frame = 60;
		actions[0].mouse = new Vector3(-0.203125F, 0.1666667F, 0F);
		actions[1].id = new Vector3(-0.90625F, -0.5996094F, -0.015625F); //sluggish
		actions[1].frame = 60;
		actions[1].mouse = new Vector3(-0.96875F, -1.010417F, 0F);
		actions[2].id = new Vector3(0.7363281F, 1.376953F, 0F); //web
		actions[2].frame = 57;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.7363281F, 1.376953F, 0F); //web
		actions[3].frame = 149;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.06640625F, 0.90625F, 0F); //web
		actions[4].frame = 81;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.06640625F, 0.90625F, 0F); //web
		actions[5].frame = 155;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}
	void level79_1 () {
		actions = new action[4];
		actions[0].id = new Vector3(0.3476563F, 1.378906F, 0F); //web
		actions[0].frame = 8;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.3476563F, 1.378906F, 0F); //web
		actions[1].frame = 29;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.3691406F, 0.2695313F, 0F); //web
		actions[2].frame = 27;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.3691406F, 0.2695313F, 0F); //web
		actions[3].frame = 92;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}

	void level80_0 () {
		actions = new action[7];

		actions[0].id = new Vector3(-0.3300781F, 1.007813F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.3300781F, 1.007813F, 0F); //web
		actions[1].frame = 218;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.3300781F, 1.007813F, 0F); //web
		actions[2].frame = 72;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.3300781F, 1.007813F, 0F); //web
		actions[3].frame = 40;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.9433594F, 0.8398438F, 0F); //web
		actions[4].frame = 39;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.9433594F, 0.8398438F, 0F); //web
		actions[5].frame = 60;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.2304688F, -0.6269531F, 0F); //web
		actions[6].frame = 33;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
	}
	void level80_1 () {
		actions = new action[2];
		actions[0].id = new Vector3(-0.640625F, 1.199219F, 0F); //web
		actions[0].frame = 7;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.640625F, 1.199219F, 0F); //web
		actions[1].frame = 10;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level81_0 () {
		actions = new action[4];

		actions[0].id = new Vector3(-0.7421875F, 0.7597656F, 0F); //web
		actions[0].frame = 170;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.7421875F, 0.7597656F, 0F); //web
		actions[1].frame = 125;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.6640625F, 0.5488281F, 0F); //web
		actions[2].frame = 56;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.7421875F, 0.7597656F, 0F); //web
		actions[3].frame = 128;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}
	void level81_1 () {
		actions = new action[3];
		actions[0].id = new Vector3(-0.8515625F, -1.060547F, 0F); //groot
		actions[0].frame = 29;
		actions[0].mouse = new Vector3(-0.9601874F, -0.4964871F, 0F);
		actions[1].id = new Vector3(0.2695313F, 1.332031F, 0F); //web
		actions[1].frame = 77;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.9394531F, -0.4960938F, 0F); //web
		actions[2].frame = 61;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
	}

	void level82_0 () {
		actions = new action[4];

		actions[0].id = new Vector3(-0.8300781F, 0.59375F, 0F); //web
		actions[0].frame = 35;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.08398438F, 1.445313F, 0F); //web
		actions[1].frame = 116;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.05859375F, 0.7695313F, 0F); //web
		actions[2].frame = 169;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.7324219F, 0.734375F, 0F); //web
		actions[3].frame = 188;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}
	void level82_1 () {
		actions = new action[7];
		actions[0].id = new Vector3(0.6113281F, 0.5117188F, 0F); //groot
		actions[0].frame = 59;
		actions[0].mouse = new Vector3(-0.7306792F, 0.796253F, 0F);
		actions[1].id = new Vector3(0.6328125F, -0.1992188F, 0F); //sluggish
		actions[1].frame = 73;
		actions[1].mouse = new Vector3(1.124122F, -1.09F, 0F);
		actions[2].id = new Vector3(-0.7324219F, -0.3535156F, 0F); //sluggish
		actions[2].frame = 133;
		actions[2].mouse = new Vector3(-1.124122F, -1.05F, 0F);
		actions[3].id = new Vector3(0.6113281F, 0.5117188F, 0F); //groot
		actions[3].frame = 74;
		actions[3].mouse = new Vector3(0.468384F, 0.5011709F, 0F);
		actions[4].id = new Vector3(0.6113281F, 0.5117188F, 0F); //groot
		actions[4].frame = 62;
		actions[4].mouse = new Vector3(-0.3372366F, -0.8384075F, 0F);
		actions[5].id = new Vector3(0.6328125F, -0.1992188F, 0F); //sluggish
		actions[5].frame = 169;
		actions[5].mouse = new Vector3(1.124122F, 0.12F, 0F);
		actions[6].id = new Vector3(0.6328125F, -0.1992188F, 0F); //sluggish
		actions[6].frame = 100;
		actions[6].mouse = new Vector3(0.69F, 0.5011709F, 0F);
	}

	void level83_0 () {
		actions = new action[9];

		actions[0].id = new Vector3(0.3886719F, 0.5605469F, 0F); //cloud
		actions[0].frame = 44;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.7246094F, 0.3632813F, 0F); //sluggish
		actions[1].frame = 195;
		actions[1].mouse = new Vector3(0.9947917F, -0.08333337F, 0F);
		actions[2].id = new Vector3(0.8847656F, 1.332705F, -0.01953125F); //yeti body
		actions[2].frame = 39;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.8847656F, 1.332705F, -0.01953125F); //yeti body
		actions[3].frame = 0;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.07226563F, -0.5332031F, 0F); //cloud
		actions[4].frame = 154;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.7246094F, 0.3632813F, 0F); //sluggish
		actions[5].frame = 128;
		actions[5].mouse = new Vector3(0.9062501F, 0.796875F, 0F);
		actions[6].id = new Vector3(-0.7597656F, -0.8496094F, 0F); //sluggish
		actions[6].frame = 185;
		actions[6].mouse = new Vector3(-0.7447917F, -1.338542F, 0F);
		actions[7].id = new Vector3(-0.7597656F, -0.8496094F, 0F); //sluggish
		actions[7].frame = 213;
		actions[7].mouse = new Vector3(-1.083333F, -1.296875F, 0F);
		actions[8].id = new Vector3(0.7246094F, 0.3632813F, 0F); //sluggish
		actions[8].frame = 138;
		actions[8].mouse = new Vector3(1.104167F, 0.3385417F, 0F);
	}
	void level83_1 () {
		actions = new action[9];
		actions[0].id = new Vector3(-0.1816406F, 0.8378906F, 0F); //web
		actions[0].frame = 23;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.1816406F, 0.8378906F, 0F); //web
		actions[1].frame = 14;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.1816406F, 0.8378906F, 0F); //web
		actions[2].frame = 19;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.1816406F, 0.8378906F, 0F); //web
		actions[3].frame = 19;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.1816406F, 0.8378906F, 0F); //web
		actions[4].frame = 21;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.1816406F, 0.8378906F, 0F); //web
		actions[5].frame = 142;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.1816406F, 0.8378906F, 0F); //web
		actions[6].frame = 21;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.2402344F, -1.679688F, 0F); //groot
		actions[7].frame = 114;
		actions[7].mouse = new Vector3(-0.5620609F, -0.6510539F, 0F);
		actions[8].id = new Vector3(-0.1816406F, 0.8378906F, 0F); //web
		actions[8].frame = 128;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
	}

	void level84_0 () {
		actions = new action[8];
		
		actions[0].id = new Vector3(0.2382813F, 1.503906F, 0F); //web
		actions[0].frame = 66;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.2382813F, 1.503906F, 0F); //web
		actions[1].frame = 26;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.25F, 1.503906F, 0F); //web
		actions[2].frame = 59;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.7265625F, 1.503906F, 0F); //web
		actions[3].frame = 25;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.2382813F, 1.503906F, 0F); //web
		actions[4].frame = 39;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.25F, 1.503906F, 0F); //web
		actions[5].frame = 36;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.7265625F, 1.503906F, 0F); //web
		actions[6].frame = 75;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.7382813F, 1.503906F, 0F); //web
		actions[7].frame = 115;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
	}
	void level84_1 () {
		actions = new action[6];
		actions[0].id = new Vector3(0.7304688F, 1.267578F, 0F); //web
		actions[0].frame = 9;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.7714844F, 1.316406F, 0F); //web
		actions[1].frame = 29;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.7714844F, 1.316406F, 0F); //web
		actions[2].frame = 9;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.7304688F, 1.267578F, 0F); //web
		actions[3].frame = 68;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.7714844F, 1.316406F, 0F); //web
		actions[4].frame = 5;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.3613281F, -0.8769531F, 0F); //cloud
		actions[5].frame = 34;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}

	void level85_0 () {
		actions = new action[5];

		actions[0].id = new Vector3(0.8691406F, 0.7597656F, 0F); //destroyer
		actions[0].frame = 187;
		actions[0].mouse = new Vector3(0.5572916F, 0.4739583F, 0F);
		actions[1].id = new Vector3(-0.25F, 1.042969F, 0F); //web
		actions[1].frame = 34;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.25F, 1.042969F, 0F); //web
		actions[2].frame = 111;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.25F, 1.042969F, 0F); //web
		actions[3].frame = 20;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.25F, 1.042969F, 0F); //web
		actions[4].frame = 46;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
	}
	void level85_1 () {
		actions = new action[2];
		actions[0].id = new Vector3(-0.0625F, 1.488281F, 0F); //web
		actions[0].frame = 26;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.0625F, 1.488281F, 0F); //web
		actions[1].frame = 100;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
	}

	void level86_0 () {
		actions = new action[7];

		actions[0].id = new Vector3(0.6796875F, 0.3007813F, -0.015625F); //sluggish
		actions[0].frame = 166;
		actions[0].mouse = new Vector3(1.041667F, -0.03645837F, 0F);
		actions[1].id = new Vector3(-0.4003906F, 1.189453F, 0F); //web
		actions[1].frame = 33;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.4003906F, 1.189453F, 0F); //web
		actions[2].frame = 157;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.5273438F, -1.134766F, -0.015625F); //sluggish (1)
		actions[3].frame = 156;
		actions[3].mouse = new Vector3(0.46875F, -1.635417F, 0F);
		actions[4].id = new Vector3(0.6796875F, 0.3007813F, -0.015625F); //sluggish
		actions[4].frame = 116;
		actions[4].mouse = new Vector3(0.5989583F, -0.234375F, 0F);
		actions[5].id = new Vector3(-0.4003906F, 1.189453F, 0F); //web
		actions[5].frame = 30;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.4003906F, 1.189453F, 0F); //web
		actions[6].frame = 144;
		actions[6].mouse = new Vector3(0F, 0F, 0F);

	}
	void level86_1 () {
		actions = new action[2];
		actions[0].id = new Vector3(0.8769531F, 1.091797F, 0F); //groot
		actions[0].frame = 70;
		actions[0].mouse = new Vector3(0.071F, -0.3606558F, 0F);
		actions[1].id = new Vector3(0.8769531F, 1.091797F, 0F); //groot
		actions[1].frame = 75;
		actions[1].mouse = new Vector3(0.8430914F, 1.063232F, 0F);
	}

	void level87_0 () {
		actions = new action[5];

		actions[0].id = new Vector3(0.3320313F, 1.152344F, 0F); //web
		actions[0].frame = 84;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.3320313F, 1.152344F, 0F); //web
		actions[1].frame = 51;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.3320313F, 1.152344F, 0F); //web
		actions[2].frame = 22;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.6972656F, 0.05078125F, 0F); //groot
		actions[3].frame = 40;
		actions[3].mouse = new Vector3(0.04166664F, -1.28125F, 0F);
		actions[4].id = new Vector3(0.3320313F, 1.152344F, 0F); //web
		actions[4].frame = 55;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
	}
	void level87_1 () {
		actions = new action[7];
		actions[0].id = new Vector3(0.3320313F, 1.152344F, 0F); //web
		actions[0].frame = 39;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.6972656F, 0.05078125F, 0F); //groot
		actions[1].frame = 57;
		actions[1].mouse = new Vector3(-0.5667447F, -0.1639345F, 0F);
		actions[2].id = new Vector3(0.3320313F, 1.152344F, 0F); //web
		actions[2].frame = 51;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.3320313F, 1.152344F, 0F); //web
		actions[3].frame = 171;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.6972656F, 0.05078125F, 0F); //groot
		actions[4].frame = 34;
		actions[4].mouse = new Vector3(0.529274F, 0.02810311F, 0F);
		actions[5].id = new Vector3(0.6972656F, 0.05078125F, 0F); //groot
		actions[5].frame = 56;
		actions[5].mouse = new Vector3(-0.1264637F, -1.236534F, 0F);
		actions[6].id = new Vector3(0.3320313F, 1.152344F, 0F); //web
		actions[6].frame = 65;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
	}

	void level88_0 () {
		actions = new action[9];
		
		actions[0].id = new Vector3(-0.8847656F, -0.6028423F, -0.01953125F); //yeti body
		actions[0].frame = 67;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.8847656F, -0.6028423F, -0.01953125F); //yeti body
		actions[1].frame = 0;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.1816406F, 0.8378906F, 0F); //web
		actions[2].frame = 58;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.1816406F, 0.8378906F, 0F); //web
		actions[3].frame = 119;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.1816406F, 0.8378906F, 0F); //web
		actions[4].frame = 29;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.2402344F, -1.679688F, 0F); //groot
		actions[5].frame = 129;
		actions[5].mouse = new Vector3(-0.6302083F, -0.6302084F, 0F);
		actions[6].id = new Vector3(-0.8847656F, -0.6028423F, -0.01953125F); //yeti body
		actions[6].frame = 62;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.8847656F, -0.6028423F, -0.01953125F); //yeti body
		actions[7].frame = 0;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(-0.1816406F, 0.8378906F, 0F); //web
		actions[8].frame = 120;
		actions[8].mouse = new Vector3(0F, 0F, 0F);

	}
	void level88_1 () {
		actions = new action[9];
		actions[0].id = new Vector3(0.05664063F, -0.04492188F, 0F); //sluggish
		actions[0].frame = 57;
		actions[0].mouse = new Vector3(0.05152227F, -0.4309133F, 0F);
		actions[1].id = new Vector3(0.6015625F, 0.2792969F, 0F); //groot
		actions[1].frame = 41;
		actions[1].mouse = new Vector3(-0.4777518F, 0.1779859F, 0F);
		actions[2].id = new Vector3(0.6015625F, 0.2792969F, 0F); //groot
		actions[2].frame = 108;
		actions[2].mouse = new Vector3(0.468384F, 0.2950819F, 0F);
		actions[3].id = new Vector3(-0.734375F, -0.009765625F, 0F); //sluggish
		actions[3].frame = 55;
		actions[3].mouse = new Vector3(-0.7306792F, -0.4215456F, 0F);
		actions[4].id = new Vector3(0.6015625F, 0.2792969F, 0F); //groot
		actions[4].frame = 43;
		actions[4].mouse = new Vector3(-0.6978923F, 0.58548F, 0F);
		actions[5].id = new Vector3(0.6015625F, 0.2792969F, 0F); //groot
		actions[5].frame = 79;
		actions[5].mouse = new Vector3(0.468384F, 0.3606558F, 0F);
		actions[6].id = new Vector3(0.05664063F, -0.04492188F, 0F); //sluggish
		actions[6].frame = 97;
		actions[6].mouse = new Vector3(-0.1030445F, -1.484777F, 0F);
		actions[7].id = new Vector3(0.6015625F, 0.2792969F, 0F); //groot
		actions[7].frame = 63;
		actions[7].mouse = new Vector3(0.9086651F, -0.7166276F, 0F);
		actions[8].id = new Vector3(0.6015625F, 0.2792969F, 0F); //groot
		actions[8].frame = 105;
		actions[8].mouse = new Vector3(0.6276346F, 0.2482436F, 0F);
	}

	void level89_0 () {
		actions = new action[5];

		actions[0].id = new Vector3(-0.7832031F, 0.1738281F, 0F); //groot
		actions[0].frame = 60;
		actions[0].mouse = new Vector3(0.515625F, 1.234375F, 0F);
		actions[1].id = new Vector3(-0.06054688F, 1.632813F, 0F); //web
		actions[1].frame = 133;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.7832031F, 0.1738281F, 0F); //groot
		actions[2].frame = 84;
		actions[2].mouse = new Vector3(-0.71875F, 0.265625F, 0F);
		actions[3].id = new Vector3(-0.7832031F, 0.1738281F, 0F); //groot
		actions[3].frame = 80;
		actions[3].mouse = new Vector3(-0.484375F, -1.125F, 0F);
		actions[4].id = new Vector3(0.7226563F, 1.121094F, 0F); //web
		actions[4].frame = 60;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
	}
	void level89_1 () {
		actions = new action[9];
		actions[0].id = new Vector3(-0.5195313F, -1.330078F, 0F); //sluggish
		actions[0].frame = 86;
		actions[0].mouse = new Vector3(-0.3793911F, -1.761124F, 0F);
		actions[1].id = new Vector3(-0.6230469F, 0.8886719F, 0F); //web
		actions[1].frame = 37;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.046875F, 1.361328F, 0F); //web
		actions[2].frame = 70;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.6230469F, 0.8886719F, 0F); //web
		actions[3].frame = 50;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.046875F, 1.361328F, 0F); //web
		actions[4].frame = 40;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.6230469F, 0.8886719F, 0F); //web
		actions[5].frame = 40;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.6230469F, 0.8886719F, 0F); //web
		actions[6].frame = 47;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.6230469F, 0.8886719F, 0F); //web
		actions[7].frame = 15;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(-0.046875F, 1.361328F, 0F); //web
		actions[8].frame = 57;
		actions[8].mouse = new Vector3(0F, 0F, 0F);

	}

	void level90_0 () {
		actions = new action[8];
		
		actions[0].id = new Vector3(1.005859F, 0.09765625F, 0F); //groot
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0.07812496F, 0.8229167F, 0F);
		actions[1].id = new Vector3(0.7988281F, -1.173828F, 0F); //sluggish
		actions[1].frame = 50;
		actions[1].mouse = new Vector3(0.8229166F, -1.604167F, 0F);
		actions[2].id = new Vector3(0.7441406F, 1.361328F, 0F); //web
		actions[2].frame = 44;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(1.005859F, 0.09765625F, 0F); //groot
		actions[3].frame = 50;/*103*/
		actions[3].mouse = new Vector3(0.9166667F, 0.1458333F, 0F);
		actions[4].id = new Vector3(1.005859F, 0.09765625F, 0F); //groot
		actions[4].frame = 60;/*85*/
		actions[4].mouse = new Vector3(-0.4114583F, -0.1458334F, 0F);
		actions[5].id = new Vector3(0.7441406F, 1.361328F, 0F); //web
		actions[5].frame = 70;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(1.005859F, 0.09765625F, 0F); //groot
		actions[6].frame = 210;
		actions[6].mouse = new Vector3(0.8749999F, 0.09375F, 0F);
		actions[7].id = new Vector3(0.7988281F, -1.173828F, 0F); //sluggish
		actions[7].frame = 80;
		actions[7].mouse = new Vector3(1.34375F, -1.151042F, 0F);

	}
	void level90_1 () {
		actions = new action[2];
		actions[0].id = new Vector3(-0.6171875F, 0.5058594F, 0F); //destroyer
		actions[0].frame = 64;
		actions[0].mouse = new Vector3(-0.08430912F, -0.09367681F, 0F);
		actions[1].id = new Vector3(-0.3964844F, -0.8320313F, 0F); //destroyer
		actions[1].frame = 75;
		actions[1].mouse = new Vector3(0.4028103F, 0.5620608F, 0F);
	}

	void level91_0 () {
		actions = new action[6];
		
		actions[0].id = new Vector3(-0.5019531F, 0.2265625F, 0F); //groot
		actions[0].frame = 90;
		actions[0].mouse = new Vector3(-0.265625F, -0.6927084F, 0F);
		actions[1].id = new Vector3(-0.3261719F, -0.8554688F, 0F); //groot
		actions[1].frame = 77;
		actions[1].mouse = new Vector3(0.2552083F, -1.713542F, 0F);
		actions[2].id = new Vector3(0.4375F, 1.171875F, 0F); //web
		actions[2].frame = 90;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.4980469F, 1.486328F, 0F); //web
		actions[3].frame = 49;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.4980469F, 1.486328F, 0F); //web
		actions[4].frame = 30;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.4980469F, 1.486328F, 0F); //web
		actions[5].frame = 85;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}
	void level91_1 () {
		actions = new action[12];
		actions[0].id = new Vector3(1.03125F, -0.2148438F, -1.953125E-05F); //groot
		actions[0].frame = 20;
		actions[0].mouse = new Vector3(-0.08899295F, 0.1779859F, 0F);
		actions[1].id = new Vector3(-0.4980469F, 1.486328F, 0F); //web
		actions[1].frame = 31;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.3261719F, -0.8554688F, 0F); //groot
		actions[2].frame = 85;
		actions[2].mouse = new Vector3(0.913349F, -0.17F, 0F);
		actions[3].id = new Vector3(-0.4980469F, 1.486328F, 0F); //web
		actions[3].frame = 34;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.4980469F, 1.486328F, 0F); //web
		actions[4].frame = 68;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.4980469F, 1.486328F, 0F); //web
		actions[5].frame = 28;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.4980469F, 1.486328F, 0F); //web
		actions[6].frame = 49;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(1.03125F, -0.2148438F, -1.953125E-05F); //groot
		actions[7].frame = 120;
		actions[7].mouse = new Vector3(0.9274005F, -0.2248243F, 0F);
		actions[8].id = new Vector3(1.03125F, -0.2148438F, -1.953125E-05F); //groot
		actions[8].frame = 55;
		actions[8].mouse = new Vector3(-0.2014052F, -1.067916F, 0F);
		actions[9].id = new Vector3(-0.3261719F, -0.8554688F, 0F); //groot
		actions[9].frame = 155;
		actions[9].mouse = new Vector3(-0.2810304F, -0.824356F, 0F);
		actions[10].id = new Vector3(-0.3261719F, -0.8554688F, 0F); //groot
		actions[10].frame = 50;
		actions[10].mouse = new Vector3(0.3091335F, -1.733021F, 0F);
		actions[11].id = new Vector3(1.03125F, -0.2148438F, -1.953125E-05F); //groot
		actions[11].frame = 20;
		actions[11].mouse = new Vector3(0.852459F, -0.3185011F, 0F);
	}

	void level92_0 () {
		actions = new action[9];
	
		actions[0].id = new Vector3(0.6816406F, 1.050781F, 0F); //groot
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(-0.03645831F, -0.421875F, 0F);
		actions[1].id = new Vector3(0.25F, 1.591797F, 0F); //web
		actions[1].frame = 40;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.6816406F, 1.050781F, 0F); //groot
		actions[2].frame = 90;
		actions[2].mouse = new Vector3(0.6718751F, 0.953125F, 0F);
		actions[3].id = new Vector3(0.6816406F, 1.050781F, 0F); //groot
		actions[3].frame = 40;
		actions[3].mouse = new Vector3(-0.1302083F, 0.21875F, 0F);
		actions[4].id = new Vector3(-0.9726563F, 0.1523438F, 0F); //sluggish
		actions[4].frame = 40;
		actions[4].mouse = new Vector3(-1.114583F, -0.484375F, 0F);
		actions[5].id = new Vector3(0.6816406F, 1.050781F, 0F); //groot
		actions[5].frame = 150;
		actions[5].mouse = new Vector3(0.6249999F, 0.984375F, 0F);
		actions[6].id = new Vector3(0.6816406F, 1.050781F, 0F); //groot
		actions[6].frame = 40;
		actions[6].mouse = new Vector3(-0.04166664F, -0.390625F, 0F);
		actions[7].id = new Vector3(-0.9726563F, 0.1523438F, 0F); //sluggish
		actions[7].frame = 40;
		actions[7].mouse = new Vector3(-1.260417F, 0.2552083F, 0F);
		actions[8].id = new Vector3(0.6816406F, 1.050781F, 0F); //groot
		actions[8].frame = 90;
		actions[8].mouse = new Vector3(0.6197916F, 0.9739583F, 0F);
	}
	void level92_1 () {
		actions = new action[11];
		actions[0].id = new Vector3(1.005859F, 0.06445313F, 0F); //groot
		actions[0].frame = 36;
		actions[0].mouse = new Vector3(0.1967213F, 0.8149884F, 0F);
		actions[1].id = new Vector3(0.7988281F, -1.173828F, 0F); //sluggish
		actions[1].frame = 47;
		actions[1].mouse = new Vector3(0.8384075F, -1.686183F, 0F);
		actions[2].id = new Vector3(0.7441406F, 1.361328F, 0F); //web
		actions[2].frame = 45;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(1.005859F, 0.06445313F, 0F); //groot
		actions[3].frame = 41;
		actions[3].mouse = new Vector3(0.913349F, 0.1077282F, 0F);
		actions[4].id = new Vector3(1.005859F, 0.06445313F, 0F); //groot
		actions[4].frame = 52;
		actions[4].mouse = new Vector3(-0.4355972F, -0.09836066F, 0F);
		actions[5].id = new Vector3(0.7441406F, 1.361328F, 0F); //web
		actions[5].frame = 34;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.7441406F, 1.361328F, 0F); //web
		actions[6].frame = 260;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(1.005859F, 0.06445313F, 0F); //groot
		actions[7].frame = 46;
		actions[7].mouse = new Vector3(0.8899297F, 0.09367681F, 0F);
		actions[8].id = new Vector3(0.7441406F, 1.361328F, 0F); //web
		actions[8].frame = 106;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(1.005859F, 0.06445313F, 0F); //groot
		actions[9].frame = 64;
		actions[9].mouse = new Vector3(-0.1217799F, -0.67F, 0F);
		actions[10].id = new Vector3(0.7988281F, -1.173828F, 0F); //sluggish
		actions[10].frame = 82;
		actions[10].mouse = new Vector3(1.124122F, -1.615925F, 0F);
	}

	void level93_0 () {
		actions = new action[2];
		
		actions[0].id = new Vector3(-0.7207031F, -0.5488281F, 0F); //destroyer
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0.06249996F, 1.208333F, 0F);
		actions[1].id = new Vector3(-0.4042969F, -1.707031F, 0F); //destroyer
		actions[1].frame = 400;
		actions[1].mouse = new Vector3(0.1614584F, -0.421875F, 0F);
	}
	void level93_1 () {
		actions = new action[6];
		actions[0].id = new Vector3(0.5488281F, 1.257813F, 0F); //groot
		actions[0].frame = 36;
		actions[0].mouse = new Vector3(-0.6838408F, 0.9367681F, 0F);
		actions[1].id = new Vector3(0.2929688F, 0.2792969F, 0F); //groot
		actions[1].frame = 76;
		actions[1].mouse = new Vector3(-0.7400468F, -0.1077284F, 0F);
		actions[2].id = new Vector3(0.5488281F, 1.257813F, 0F); //groot
		actions[2].frame = 109;
		actions[2].mouse = new Vector3(0.4028103F, 1.29274F, 0F);
		actions[3].id = new Vector3(0.2929688F, 0.2792969F, 0F); //groot
		actions[3].frame = 106;
		actions[3].mouse = new Vector3(0.1733021F, 0.1920376F, 0F);
		actions[4].id = new Vector3(0.2929688F, 0.2792969F, 0F); //groot
		actions[4].frame = 36;
		actions[4].mouse = new Vector3(0.2248244F, -0.8009368F, 0F);
		actions[5].id = new Vector3(0.2929688F, 0.2792969F, 0F); //groot
		actions[5].frame = 33;
		actions[5].mouse = new Vector3(0.2622951F, 0.1779859F, 0F);
	}

	void level94_0 () {
		actions = new action[11];
		
		actions[0].id = new Vector3(-0.5195313F, -1.330078F, 0F); //sluggish
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(-0.3958333F, -1.776042F, 0F);
		actions[1].id = new Vector3(0.7695313F, -0.7519531F, 0F); //sluggish
		actions[1].frame = 90;
		actions[1].mouse = new Vector3(0.6249999F, -1.25F, 0F);
		actions[2].id = new Vector3(-0.6230469F, 0.8886719F, 0F); //web
		actions[2].frame = 62;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.046875F, 1.361328F, 0F); //web
		actions[3].frame = 65;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.046875F, 1.361328F, 0F); //web
		actions[4].frame = 20;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.6230469F, 0.8886719F, 0F); //web
		actions[5].frame = 40;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.6230469F, 0.8886719F, 0F); //web
		actions[6].frame = 216;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.046875F, 1.361328F, 0F); //web
		actions[7].frame = 60;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(-0.046875F, 1.361328F, 0F); //web
		actions[8].frame = 79;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(-0.6230469F, 0.8886719F, 0F); //web
		actions[9].frame = 34;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(-0.046875F, 1.361328F, 0F); //web
		actions[10].frame = 0;
		actions[10].mouse = new Vector3(0F, 0F, 0F);

	}
	void level94_1 () {
		actions = new action[3];
		actions[0].id = new Vector3(-0.06054688F, 1.632813F, 0F); //web
		actions[0].frame = 14;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.7226563F, 1.121094F, 0F); //web
		actions[1].frame = 37;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.8066406F, 0.4003906F, 0F); //groot
		actions[2].frame = 47;
		actions[2].mouse = new Vector3(-0.5339579F, -0.6276346F, 0F);
	}

	void level95_0 () {
		actions = new action[4];

		actions[0].id = new Vector3(-0.8515625F, -1.060547F, 0F); //groot
		actions[0].frame = 60;
		actions[0].mouse = new Vector3(-1.098958F, -0.0625F, 0F);
		actions[1].id = new Vector3(0.2695313F, 1.332031F, 0F); //web
		actions[1].frame = 66;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.7871094F, -0.4960938F, 0F); //web
		actions[2].frame = 125;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.7871094F, -0.4960938F, 0F); //web
		actions[3].frame = 15;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
	}
	void level95_1 () {
		actions = new action[11];
		actions[0].id = new Vector3(-0.5898438F, 1.085938F, 0F); //web
		actions[0].frame = 7;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(-0.5898438F, 1.085938F, 0F); //web
		actions[1].frame = 11;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.5898438F, 1.085938F, 0F); //web
		actions[2].frame = 27;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.5214844F, -1.248047F, 0F); //sluggish
		actions[3].frame = 114;
		actions[3].mouse = new Vector3(-0.5245901F, -1.761124F, 0F);
		actions[4].id = new Vector3(-0.5898438F, 1.085938F, 0F); //web
		actions[4].frame = 36;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.6992188F, 1.326172F, 0F); //web
		actions[5].frame = 117;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.6992188F, 1.326172F, 0F); //web
		actions[6].frame = 114;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(-0.5898438F, 1.085938F, 0F); //web
		actions[7].frame = 93;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.6992188F, 1.326172F, 0F); //web
		actions[8].frame = 12;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(0.6992188F, 1.326172F, 0F); //web
		actions[9].frame = 27;
		actions[9].mouse = new Vector3(0F, 0F, 0F);
		actions[10].id = new Vector3(0.6992188F, 1.326172F, 0F); //web
		actions[10].frame = 46;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
	}

	void level96_0 () {
		actions = new action[6];

		actions[0].id = new Vector3(-0.953125F, 0.4667969F, 0F); //destroyer (1)
		actions[0].frame = 188;
		actions[0].mouse = new Vector3(0.02604165F, 0.02604175F, 0F);
		actions[1].id = new Vector3(0.2773438F, 0.4785156F, 0F); //web
		actions[1].frame = 55;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.2773438F, 0.4785156F, 0F); //web
		actions[2].frame = 26;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.2773438F, 0.4785156F, 0F); //web
		actions[3].frame = 132;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.9140625F, 0.4921875F, 0F); //destroyer
		actions[4].frame = 290;
		actions[4].mouse = new Vector3(0.3177083F, -0.078125F, 0F);
		actions[5].id = new Vector3(0.2773438F, 0.4785156F, 0F); //web
		actions[5].frame = 129;
		actions[5].mouse = new Vector3(0F, 0F, 0F);

		// hz kakoi lychshe
		/*actions[0].id = new Vector3(-0.953125F, 0.4667969F, 0F); //destroyer (1)
		actions[0].frame = 101;
		actions[0].mouse = new Vector3(0.1614584F, -0.046875F, 0F);
		actions[1].id = new Vector3(0.2773438F, 0.4785156F, 0F); //web
		actions[1].frame = 33;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.2773438F, 0.4785156F, 0F); //web
		actions[2].frame = 23;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.2773438F, 0.4785156F, 0F); //web
		actions[3].frame = 128;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.9140625F, 0.4921875F, 0F); //destroyer
		actions[4].frame = 304;
		actions[4].mouse = new Vector3(0.5364583F, 0.109375F, 0F);
		actions[5].id = new Vector3(0.2773438F, 0.4785156F, 0F); //web
		actions[5].frame = 117;
		actions[5].mouse = new Vector3(0F, 0F, 0F);*/

	}
	void level96_1 () {
		actions = new action[7];
		actions[0].id = new Vector3(0.8535156F, 0.7285156F, 0F); //groot
		actions[0].frame = 36;
		actions[0].mouse = new Vector3(-0.7213115F, 1.007026F, 0F);
		actions[1].id = new Vector3(0.9589844F, -0.2109375F, 0F); //groot
		actions[1].frame = 83;
		actions[1].mouse = new Vector3(0.2997658F, 1.133489F, 0F);
		actions[2].id = new Vector3(0.8535156F, 0.7285156F, 0F); //groot
		actions[2].frame = 40;
		actions[2].mouse = new Vector3(0.7822014F, 0.7166276F, 0F);
		actions[3].id = new Vector3(0.8535156F, 0.7285156F, 0F); //groot
		actions[3].frame = 42;
		actions[3].mouse = new Vector3(-0.2763466F, -0.2014052F, 0F);
		actions[4].id = new Vector3(0.9589844F, -0.2109375F, 0F); //groot
		actions[4].frame = 38;
		actions[4].mouse = new Vector3(0.9555035F, -0.1264637F, 0F);
		actions[5].id = new Vector3(0.9589844F, -0.2109375F, 0F); //groot
		actions[5].frame = 63;
		actions[5].mouse = new Vector3(0.4449649F, -1.203747F, 0F);
		actions[6].id = new Vector3(0.8535156F, 0.7285156F, 0F); //groot
		actions[6].frame = 42;
		actions[6].mouse = new Vector3(0.70726F, 0.6885245F, 0F);
	}

	void level97_0 () {
		actions = new action[11];

		actions[0].id = new Vector3(0.3417969F, 0.5332031F, 0F); //groot
		actions[0].frame = 80;
		actions[0].mouse = new Vector3(-0.375F, -0.4583334F, 0F);
		actions[1].id = new Vector3(0.6953125F, 0.5449219F, 0F); //web
		actions[1].frame = 143;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.6953125F, 0.5449219F, 0F); //web
		actions[2].frame = 40;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.1171875F, -0.7128906F, 0F); //web
		actions[3].frame = 70;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.3417969F, 0.5332031F, 0F); //groot
		actions[4].frame = 40;
		actions[4].mouse = new Vector3(0.3385416F, 0.4791667F, 0F);
		actions[5].id = new Vector3(0.6953125F, 0.5449219F, 0F); //web
		actions[5].frame = 40;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(0.3417969F, 0.5332031F, 0F); //groot
		actions[6].frame = 60;
		actions[6].mouse = new Vector3(0.3958334F, -0.5572916F, 0F);
		actions[7].id = new Vector3(-0.6152344F, 0.1503906F, 0F); //web
		actions[7].frame = 45;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.1171875F, -0.7128906F, 0F); //web
		actions[8].frame = 49;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(0.3417969F, 0.5332031F, 0F); //groot
		actions[9].frame = 60;
		actions[9].mouse = new Vector3(0.3072916F, 0.46875F, 0F);
		actions[10].id = new Vector3(0.6953125F, 0.5449219F, 0F); //web
		actions[10].frame = 30;
		actions[10].mouse = new Vector3(0F, 0F, 0F);
	}
	void level97_1 () {
		actions = new action[3];
		actions[0].id = new Vector3(-0.4042969F, -1.707031F, 0F); //destroyer
		actions[0].frame = 51;
		actions[0].mouse = new Vector3(0.02810302F, -0.8103044F, 0F);
		actions[1].id = new Vector3(-0.7207031F, -0.5488281F, 0F); //destroyer
		actions[1].frame = 95;
		actions[1].mouse = new Vector3(-0.3419204F, 0.323185F, 0F);
		actions[2].id = new Vector3(-0.7792969F, 1.533203F, 0F); //destroyer
		actions[2].frame = 88;
		actions[2].mouse = new Vector3(-0.5901639F, 1.17096F, 0F);
	}

	void level98_0 () {
		actions = new action[7];
		
		actions[0].id = new Vector3(0.3027344F, -0.1523438F, 0F); //web
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0F, 0F, 0F);
		actions[1].id = new Vector3(0.1367188F, 1.136719F, 0F); //groot
		actions[1].frame = 40;
		actions[1].mouse = new Vector3(0.1197916F, 0.03125F, 0F);
		actions[2].id = new Vector3(0.7871094F, -0.5800781F, -0.015625F); //sluggish
		actions[2].frame = 40;
		actions[2].mouse = new Vector3(0.1093749F, -1.114583F, 0F);
		actions[3].id = new Vector3(-0.5332031F, 1.236328F, 0F); //web
		actions[3].frame = 34;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.1367188F, 1.136719F, 0F); //groot
		actions[4].frame = 40;
		actions[4].mouse = new Vector3(0.1510417F, 1.088542F, 0F);
		actions[5].id = new Vector3(0.3027344F, -0.1523438F, 0F); //web
		actions[5].frame = 40;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.5332031F, 1.236328F, 0F); //web
		actions[6].frame = 202;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
	}
	void level98_1 () {
		actions = new action[13];
		actions[0].id = new Vector3(-0.6113281F, 0.09765625F, 0F); //groot
		actions[0].frame = 62;
		actions[0].mouse = new Vector3(0.4964872F, -0.4215456F, 0F);
		actions[1].id = new Vector3(-0.7363281F, 1.257813F, 0F); //web
		actions[1].frame = 32;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(0.8203125F, 1.404297F, 0F); //web
		actions[2].frame = 26;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.7363281F, 1.257813F, 0F); //web
		actions[3].frame = 38;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.7363281F, 1.257813F, 0F); //web
		actions[4].frame = 84;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.7363281F, 1.257813F, 0F); //web
		actions[5].frame = 26;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
		actions[6].id = new Vector3(-0.7363281F, 1.257813F, 0F); //web
		actions[6].frame = 38;
		actions[6].mouse = new Vector3(0F, 0F, 0F);
		actions[7].id = new Vector3(0.8203125F, 1.404297F, 0F); //web
		actions[7].frame = 73;
		actions[7].mouse = new Vector3(0F, 0F, 0F);
		actions[8].id = new Vector3(0.8203125F, 1.404297F, 0F); //web
		actions[8].frame = 70;
		actions[8].mouse = new Vector3(0F, 0F, 0F);
		actions[9].id = new Vector3(-0.6113281F, 0.09765625F, 0F); //groot
		actions[9].frame = 33;
		actions[9].mouse = new Vector3(-0.5339579F, 0.0843091F, 0F);
		actions[10].id = new Vector3(-0.6113281F, 0.09765625F, 0F); //groot
		actions[10].frame = 63;
		actions[10].mouse = new Vector3(0.323185F, -1.250586F, 0F);
		actions[11].id = new Vector3(-0.7890625F, -0.8652344F, 0F); //web
		actions[11].frame = 50;
		actions[11].mouse = new Vector3(0F, 0F, 0F);
		actions[12].id = new Vector3(-0.6113281F, 0.09765625F, 0F); //groot
		actions[12].frame = 74;
		actions[12].mouse = new Vector3(-0.5480094F, 0.05152225F, 0F);

	}

	void level99_0 () {
		actions = new action[6];
		
		actions[0].id = new Vector3(0.6445313F, -0.3847656F, 0F); //groot
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0.6562501F, -1.0625F, 0F);
		actions[1].id = new Vector3(0.8652344F, 0.6152344F, -0.015625F); //sluggish
		actions[1].frame = 40;
		actions[1].mouse = new Vector3(1F, -0.02083337F, 0F);
		actions[2].id = new Vector3(-0.2402344F, 0.08203125F, 0F); //web
		actions[2].frame = 123;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(-0.2402344F, 0.08203125F, 0F); //web
		actions[3].frame = 38;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(0.7148438F, -1.265625F, 0F); //web
		actions[4].frame = 108;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(0.7148438F, -1.265625F, 0F); //web
		actions[5].frame = 16;
		actions[5].mouse = new Vector3(0F, 0F, 0F);

	}
	void level99_1 () {
		actions = new action[6];
		actions[0].id = new Vector3(0.9355469F, -0.2851563F, 0F); //groot
		actions[0].frame = 19;
		actions[0].mouse = new Vector3(0.5667448F, -0.6651053F, 0F);
		actions[1].id = new Vector3(-0.0234375F, -0.2265625F, 0F); //web
		actions[1].frame = 35;
		actions[1].mouse = new Vector3(0F, 0F, 0F);
		actions[2].id = new Vector3(-0.0234375F, -0.2265625F, 0F); //web
		actions[2].frame = 11;
		actions[2].mouse = new Vector3(0F, 0F, 0F);
		actions[3].id = new Vector3(0.1699219F, 1.480469F, 0F); //web
		actions[3].frame = 36;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.0234375F, -0.2265625F, 0F); //web
		actions[4].frame = 29;
		actions[4].mouse = new Vector3(0F, 0F, 0F);
		actions[5].id = new Vector3(-0.0234375F, -0.2265625F, 0F); //web
		actions[5].frame = 117;
		actions[5].mouse = new Vector3(0F, 0F, 0F);
	}

	void level100_0 () {
		actions = new action[11];
		
		actions[0].id = new Vector3(0.5722656F, -0.21875F, 0F); //groot
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(-0.65625F, 0.6875F, 0F);
		actions[1].id = new Vector3(-0.6679688F, 0.1835938F, 0F); //groot
		actions[1].frame = 50;
		actions[1].mouse = new Vector3(0.6249999F, 0.2708333F, 0F);
		actions[2].id = new Vector3(0.7246094F, 0.5429688F, 0F); //groot
		actions[2].frame = 50;
		actions[2].mouse = new Vector3(-0.7604167F, -0.1197916F, 0F);
		actions[3].id = new Vector3(-0.2890625F, 1.154297F, 0F); //cloud
		actions[3].frame = 60;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.6679688F, 0.1835938F, 0F); //groot
		actions[4].frame = 135;
		actions[4].mouse = new Vector3(-0.5416666F, 0.2135417F, 0F);
		actions[5].id = new Vector3(-0.6679688F, 0.1835938F, 0F); //groot
		actions[5].frame = 60;
		actions[5].mouse = new Vector3(0.6249999F, 0.05208325F, 0F);
		actions[6].id = new Vector3(0.7246094F, 0.5429688F, 0F); //groot
		actions[6].frame = 60;
		actions[6].mouse = new Vector3(0.7395834F, 0.5052083F, 0F);
		actions[7].id = new Vector3(0.5722656F, -0.21875F, 0F); //groot
		actions[7].frame = 50;
		actions[7].mouse = new Vector3(0.5364583F, -0.171875F, 0F);
		actions[8].id = new Vector3(0.5722656F, -0.21875F, 0F); //groot
		actions[8].frame = 60;
		actions[8].mouse = new Vector3(-0.7708333F, -0.2083334F, 0F);
		actions[9].id = new Vector3(-0.6679688F, 0.1835938F, 0F); //groot
		actions[9].frame = 60;
		actions[9].mouse = new Vector3(-0.5781251F, 0.2135417F, 0F);
		actions[10].id = new Vector3(0.5722656F, -0.21875F, 0F); //groot
		actions[10].frame = 100;
		actions[10].mouse = new Vector3(0.5104167F, -0.203125F, 0F);
	}
	void level100_1 () {
		actions = new action[12];
		actions[0].id = new Vector3(-0.6679688F, 0.1835938F, 0F); //groot
		actions[0].frame = 30;
		actions[0].mouse = new Vector3(0.6978922F, 0.2810304F, 0F);
		actions[1].id = new Vector3(0.7246094F, 0.5429688F, 0F); //groot
		actions[1].frame = 30;
		actions[1].mouse = new Vector3(-0.7400468F, -0.08899295F, 0F);
		actions[2].id = new Vector3(0.5722656F, -0.21875F, 0F); //groot
		actions[2].frame = 30;
		actions[2].mouse = new Vector3(-0.5386417F, 0.5620608F, 0F);
		actions[3].id = new Vector3(-0.2890625F, 1.154297F, 0F); //cloud
		actions[3].frame = 30;
		actions[3].mouse = new Vector3(0F, 0F, 0F);
		actions[4].id = new Vector3(-0.6679688F, 0.1835938F, 0F); //groot
		actions[4].frame = 100;
		actions[4].mouse = new Vector3(-0.557377F, 0.1826699F, 0F);
		actions[5].id = new Vector3(-0.6679688F, 0.1835938F, 0F); //groot
		actions[5].frame = 50;
		actions[5].mouse = new Vector3(0.6042155F, 0.04683852F, 0F);
		actions[6].id = new Vector3(0.7246094F, 0.5429688F, 0F); //groot
		actions[6].frame = 30;
		actions[6].mouse = new Vector3(0.6135831F, 0.4824357F, 0F);
		actions[7].id = new Vector3(0.5722656F, -0.21875F, 0F); //groot
		actions[7].frame = 50;
		actions[7].mouse = new Vector3(0.4871194F, -0.1873536F, 0F);
		actions[8].id = new Vector3(0.5722656F, -0.21875F, 0F); //groot
		actions[8].frame = 30;
		actions[8].mouse = new Vector3(-0.8946136F, -0.1779859F, 0F);
		actions[9].id = new Vector3(-0.6679688F, 0.1835938F, 0F); //groot
		actions[9].frame = 30;
		actions[9].mouse = new Vector3(-0.618267F, 0.1358314F, 0F);
		actions[10].id = new Vector3(-0.6679688F, 0.1835938F, 0F); //groot
		actions[10].frame = 30;
		actions[10].mouse = new Vector3(-0.5F, -0.8899298F, 0F);
		actions[11].id = new Vector3(0.5722656F, -0.21875F, 0F); //groot
		actions[11].frame = 20;
		actions[11].mouse = new Vector3(0.4496487F, -0.2435597F, 0F);
	}
}

