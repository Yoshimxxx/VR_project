﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    private bool start = false;
    public float speed = 0;
    public float rotaF;

    public int HP=100;
    public int rocktime=0;//ロックオン維持しているフレーム

    public int moveType = 0;

    public GameObject bakuhatsu;
    public GameObject simplebullet;
    public GameObject missile_prefab;//使うミサイルプレハブ
    GameObject missile;//発射したミサイル
    public MissileSC CloneMissileSC;
    GameObject hassyakou;//バルカン発射口？
    GameObject hassyakou2;//バルカン発射口2？
    public GameObject targetOBJ;//ロックオンしたオブジェクト
    public Sprite Rockon1, Rockon2;//ロックオン用画像

    private int stayFire;
    public int stayTime = 20;

    public GameObject cameraObject;

    private PlayerMove playerMove;

    private int i = 0;
    
    public Camera camera;
    AudioSource PlayerAS;
    public AudioClip search, rockon;

    GameObject PlayerCanvas;//プレイヤー用Canvas
    GameObject HPtext;//プレイヤーCanvasの各種UI
    Text hp;//テキスト系UIの格納変数

    Image Rockon;

    public GameObject Marker;
    GameObject MarkerOBJ;//インスタンスしたマーカーの格納先

    // Use this for initialization
    void Start ()
    {
        cameraObject = GameObject.Find("Main Camera");
        camera = cameraObject.GetComponent<Camera>();

        hassyakou = transform.FindChild("hassyakou").gameObject;
        hassyakou2 = transform.FindChild("hassyakou2").gameObject;

        playerMove = GetComponent<PlayerMove>();

        transform.localPosition = Vector3.zero;

        stayFire = stayTime;
        
        //プレイヤーOBJにASを追加
        PlayerAS = this.gameObject.GetComponent<AudioSource>();

        PlayerCanvas = cameraObject.transform.FindChild("PlayerCanvas").gameObject;
        HPtext = PlayerCanvas.transform.FindChild("HPtext").gameObject;
        Rockon = PlayerCanvas.transform.FindChild("RockonImage").gameObject.GetComponent<Image>();

        hp = HPtext.GetComponent<Text>();
        Rockon.sprite = Rockon1;
        MarkerOBJ = Instantiate(Marker, transform.position, Quaternion.Euler(90, transform.eulerAngles.y, 0))as GameObject;
    }
	
	// Update is called once per frame
	void Update () {
        if (start != true)
        {
            start = GameObject.Find("CreateComet").GetComponent<CreateComet>().startTF;
            hp.text = "準備中";
        }
        else
        {
            playerMove.Move(cameraObject, speed, moveType, rotaF);
            hp.text = HP.ToString();

            if (stayFire < stayTime)
            {
                stayFire++;
            }
            Ray_RockOn();
            simplebullet_fire();
            missile_fire();
            RadarMarker();
        }
	}

    void simplebullet_fire() //バルカン？
    {
        if (Input.GetButton(buttonName:"Fire1") && stayFire == stayTime)
        {
            Instantiate(simplebullet, hassyakou.transform.position, hassyakou.transform.rotation);
            Instantiate(simplebullet, hassyakou2.transform.position, hassyakou2.transform.rotation);
            stayFire = 0;
        }
    }

    void missile_fire() //ミサイル？
    {
        if (Input.GetButton(buttonName: "Fire2") && stayFire == stayTime)
        {
            //ミサイルを発射＆発射したミサイルのSCを代入
            missile=Instantiate(missile_prefab,hassyakou.transform.position,hassyakou.transform.rotation)as GameObject;
            if (targetOBJ != null)
            {
                CloneMissileSC = missile.GetComponent<MissileSC>();
                CloneMissileSC.Target = targetOBJ;
            }
            stayFire = 0;
        }
    }

    void Ray_RockOn()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        
        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.tag == "Enemy")//当たった物体がエネミー
            {
                hit.transform.Rotate(0, 0, 20);
                rocktime++;
                if (rocktime > 100)//一定時間以上視点を合わせるとロックオン
                {
                    targetOBJ = hit.collider.gameObject;
                    
                }
                else if (rocktime == 100)//目標フレーム到達時、ロックオン完了SE再生
                {
                    PlayerAS.Stop();
                    PlayerAS.clip = rockon;
                    PlayerAS.Play();
                    Rockon.sprite = Rockon2;
                }else if (rocktime == 1)//ターゲット補足時サーチ開始SE再生
                {
                    PlayerAS.clip = search;
                    PlayerAS.Play();
                }
                
            }
            else
            {
                if (rocktime != 0)
                {
                    rocktime = 0;
                    targetOBJ = null;
                    PlayerAS.Stop();
                    Rockon.sprite = Rockon1;
                }
            }
                
        }
        
    }

    void RadarMarker()
    {
        MarkerOBJ.transform.position = transform.position;
        MarkerOBJ.transform.rotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "SComet(Clone)")
        {
            start = false;
            GameObject.Find("CreateComet").GetComponent<CreateComet>().startTF = false;
        }else if (collision.gameObject.tag=="E_bullet")
        {
            //被弾時のダメージ処理や敵弾の消滅処理
            HP -= 5;
        }
    }
}
