using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private bool start = false;
    public float speed = 0;
    public float rotaF;

    public int rocktime=0;//ロックオン維持しているフレーム

    public int moveType = 0;

    public GameObject simplebullet;
    public GameObject missile;//使うミサイル
    public MissileSC CloneMissileSC;
    GameObject hassyakou;//バルカン発射口？
    GameObject hassyakou2;//バルカン発射口2？
    GameObject targetOBJ;//ロックオンしたオブジェクト

    private int stayFire;
    public int stayTime = 20;

    public GameObject cameraObject;

    private PlayerMove playerMove;

    private int i = 0;

    public GameObject CameraObj;
    public Camera camera;

	// Use this for initialization
	void Start ()
    {
        cameraObject = GameObject.Find("Main Camera");
        hassyakou = transform.FindChild("hassyakou").gameObject;
        hassyakou2 = transform.FindChild("hassyakou2").gameObject;

        playerMove = GetComponent<PlayerMove>();

        transform.localPosition = Vector3.zero;

        stayFire = stayTime;
        camera = CameraObj.GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        if (start != true)
        {
            start = GameObject.Find("CreateComet").GetComponent<CreateComet>().startTF;
        }
        else
        {
            playerMove.Move(cameraObject, speed, moveType, rotaF);

            if (stayFire < stayTime)
            {
                stayFire++;
            }
            Ray_RockOn();
            simplebullet_fire();
            missile_fire();
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
            CloneMissileSC = (MissileSC)Instantiate(missile, hassyakou.transform.position, hassyakou.transform.rotation);
            if (targetOBJ != null)
            {
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
            }
            else
            {
                rocktime = 0;
                targetOBJ = null;
            }
                
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "SComet(Clone)")
        {
            start = false;
            GameObject.Find("CreateComet").GetComponent<CreateComet>().startTF = false;
        }
    }
}
