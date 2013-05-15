using UnityEngine;
using System.Collections;

public class Forcefield : MonoBehaviour {

    public GameObject forceField;
	public AudioClip wall_sound;
	
    public float decayTime = 2.0f;
	
	private GameObject ball;
	private Material[] mats;
    private MeshFilter mesh;
	
	// Use this for initialization
	void Start ()
    {
        mats = forceField.gameObject.renderer.materials;
        mesh = forceField.gameObject.GetComponent<MeshFilter>();    
		ball = GameObject.FindGameObjectWithTag("ball");
	}

    void UpdateMask(Vector3 hitPoint)
    {
        foreach (Material m in mats)
        {
            Vector4 vTemp = mesh.transform.InverseTransformPoint(hitPoint);
            vTemp.w = 1.0f;
     
            m.SetVector("_Pos_a", vTemp);
        }    
    }

    void FadeMask()
    {
        for (int u = 0; u < mats.Length; u++)
        {
            Vector4 oldPos = mats[u].GetVector("_Pos_a");

            if (oldPos.w > 0.005)
            {
                Vector4 NewPos = oldPos;
                NewPos.w = 0.0f;

                Vector4 vTemp = Vector4.Lerp(oldPos, NewPos, Time.deltaTime * decayTime);
                mats[u].SetVector("_Pos_a", vTemp);
            }       
        }
    }

    public void OnHit(Vector3 hitPoint)
    {
        UpdateMask(hitPoint);      
    }
	
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.name == "Ball") {
			ContactPoint contact = collision.contacts[0];
			UpdateMask(contact.point);
			AudioSource.PlayClipAtPoint(wall_sound, contact.point, 0.05f);
		}
	}

    void OnMouseHit()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            MeshCollider col = gameObject.GetComponentInChildren<MeshCollider>();
         
            if (col.Raycast(ray, out hit, 100.0f))            
                UpdateMask(hit.point);
        }    
    }

	// Update is called once per frame
	void Update ()
    {
        //OnMouseHit();
        FadeMask();
	}
}
