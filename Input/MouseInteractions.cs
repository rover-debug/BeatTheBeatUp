using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteractions : MonoBehaviour
{


    GameObject SpawnLaser;
    public GameObject TestPrefab;
    GameObject GrabbedObj;

    public RectTransform CrossHair;
    float PunchCooldown = 0.0f;
    float GrabActive = 0.0f;

    Camera MainCamera;

    // Start is called before the first frame update
    void Start()
    {
        //SpawnLaser = Instantiate(TestPrefab);
        // SpawnLaser.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        MainCamera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 CameraForward = Camera.main.transform.forward;
        CameraForward.y = 0;

        bool LeftClick = Input.GetButton("Fire1");
        bool RightClick = Input.GetButton("Fire2");
        bool Space = Input.GetKey(KeyCode.Space);
        bool Grab = Input.GetKey(KeyCode.G);
        bool Block = Input.GetKey(KeyCode.B);

        // Vector3 Target = transform.position + 4.0f*CameraForward;
        // SpawnLaser.transform.position = Target;
        // //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // // Vector3 RayDirection = WorldPosition - transform.position;
        // // RayDirection.Normalize();
        // // //RayDirection.y = 1.0f;
        // // //ray.direction = RayDirection;
        // // Ray ray = new Ray(transform.position, RayDirection);
        
        // Vector3 TargetScreenSpace = MainCamera.WorldToScreenPoint(Target);
        

        // Vector2 TargetScreen = new Vector2(TargetScreenSpace.x, TargetScreenSpace.y);
        // TargetScreen.x -= (Screen.width/2);
        // TargetScreen.y -= (Screen.height/2);

        //////Debug.Log(TargetScreen);
        // CrossHair.anchoredPosition = TargetScreen / CrossHair.GetComponentInParent<Canvas>().scaleFactor;
        
        //CrossHair.position = Vector3.Lerp(CrossHair.position, TargetScreenSpace, 1.0f * Time.deltaTime);
        Ray ray = new Ray(Camera.main.transform.position, CameraForward);
        Debug.DrawRay(ray.origin, ray.direction, Color.green);

        if (GrabActive > 0.0f)
        {
            GrabActive += Time.deltaTime;

            if (Grab && GrabActive > 1.0f)
            {
                GrabbedObj.GetComponent<ragdoll>().release();
                GrabbedObj.GetComponent<Rigidbody>().AddForce((1.2f*GrabbedObj.transform.up - GrabbedObj.transform.forward)* 200f,ForceMode.Impulse);   
                GrabActive = 0.0f;
            }

            if (GrabActive > 2.0f)
            {
                foreach (Behaviour behaviour in GrabbedObj.GetComponents<Behaviour>())
                {
                    behaviour.enabled = true;
                }
                
                GrabbedObj.GetComponent<ragdoll>().release();

                GrabActive = 0.0f;
            }

            
        
        }
        else if (((LeftClick || Space) && PunchCooldown == 0.0f) || RightClick || Grab || Block)
        { 
                RaycastHit hit;
                ////Debug.Log("Raycast");
                if (Physics.Raycast(ray, out hit, 400.0f))
                {
                   ////Debug.Log(hit.collider.gameObject.name);
                    if (hit.collider.gameObject.name.Contains("Enemy") || hit.collider.gameObject.name.Contains("Grunt"))
                    {

                        if (LeftClick || Space)
                        {
                            // foreach (Behaviour behaviour in hit.collider.gameObject.GetComponents<Behaviour>()){
                            //     behaviour.enabled = false;
                            // }

                            // hit.collider.gameObject.GetComponent<Animator>().enabled = false;
                            // hit.collider.gameObject.GetComponent<Rigidbody>().isKinematic = false;

                            // // hit.collider.gameObject.GetComponent<Rigidbody>().AddForce((1.2f*hit.collider.gameObject.transform.up - hit.collider.gameObject.transform.forward)* 200f,ForceMode.Impulse);
                            // // hit.collider.gameObject.transform.root.GetComponent<Rigidbody>().AddForce(-1*transform.forward * 5000f);
                            // // StartCoroutine(stopForce());
                            // hit.collider.gameObject.transform.root.GetComponent<EventSystem>().AttackEvent.Invoke();
                            // hit.collider.gameObject.transform.root.GetComponent<Animator>().SetTrigger("fall");
                            // hit.collider.gameObject.transform.root.GetComponent<Animator>().SetFloat("slowDownFactor", 0f);
                            // hit.collider.gameObject.GetComponent<Rigidbody>().AddForce((1.2f*hit.collider.gameObject.transform.up - hit.collider.gameObject.transform.forward)* 200f,ForceMode.Impulse);
                            if (hit.collider.isTrigger)
                            {
                                GetComponent<Punch>().CollisionEvent(hit.collider);
                            }
                            PunchCooldown += Time.deltaTime;
                        }
                        else if (Block)
                        {
                            BatGrunt bg;
                            if (hit.collider.gameObject.TryGetComponent<BatGrunt>(out bg))
                            {
                                bg.Blocked();
                            }
                        }
                        else
                        {
                            foreach (Behaviour behaviour in hit.collider.gameObject.GetComponents<Behaviour>()){
                                if (!behaviour.name.Contains("Ragdoll"))
                                {
                                    behaviour.enabled = false;
                                }
                            }

                            //hit.collider.transform.GetComponent<Rigidbody>().isKinematic = false;
                            hit.collider.transform.GetComponent<Rigidbody>().useGravity = false;
                            // hit.collider.transform.gameObject.GetComponent<Animator>().enabled = false;
                            hit.collider.transform.gameObject.GetComponent<ragdoll>().enabled = true;
                            hit.collider.transform.gameObject.GetComponent<ragdoll>().grab();
                            

                            // Vector3 Enemypos = hit.collider.transform.position;
                            // Enemypos.y += 3.0f;
                            // hit.collider.transform.position = Enemypos;

                            GrabActive += Time.deltaTime;

                            GrabbedObj = hit.collider.gameObject;
                            //hit.collider.transform.gameObject.GetComponent<PartDots>().grab();

                        }

                       
                       
                    }
                }  

                // SpawnLaser.transform.position = transform.position;
                // SpawnLaser.transform.rotation = Camera.main.transform.rotation;
                // SpawnLaser.SetActive(true);
               
                
        }
        else if (PunchCooldown > 0.0f)
        {
            PunchCooldown += Time.deltaTime;

            if (PunchCooldown > 0.5f)
            {
                PunchCooldown = 0.0f;
            }
        }

    }

    IEnumerator stopForce()
    {
        yield return new WaitForSeconds(0.5f);
        
    }

    // IEnumerator WaitForCooldown(Vector3 CameraForward)
    // {
        
    // }
}
