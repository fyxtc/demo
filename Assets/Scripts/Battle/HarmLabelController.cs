using UnityEngine;
using System.Collections;

public class HarmLabelController : MonoBehaviour {
    public GameObject Character{get; set;}
    public Vector3 diffVec;
    public Vector3 endDiffPixels;
    public float speed;
    private Vector3 endPos;

    // Use this for initialization
    void Start () {
        endPos = transform.position + endDiffPixels;
	}
	
	// Update is called once per frame
	void Update () {
        if(Character){
            BaseController controller = Character.GetComponent<BaseController>();
            if (!controller.IsDead) {
                endPos = new Vector3(Camera.main.WorldToScreenPoint(Character.transform.position).x, endPos.y, endPos.z);
                transform.position = new Vector3(endPos.x, transform.position.y, transform.position.z);
            }
        }
	   // Debug.Log("harm label update " + transform.position + " to " + endPos);
        // transform.Translate(Vector3.up * Time.deltaTime * speed);
        transform.position = Vector3.MoveTowards(transform.position, endPos, Time.deltaTime * speed);
        if(transform.position == endPos){
            Destroy(gameObject);
        }
	}
}
