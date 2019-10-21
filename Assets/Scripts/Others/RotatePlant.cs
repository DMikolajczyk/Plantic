using UnityEngine;

public class RotatePlant : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        transform.Rotate(0, 0, 350);
        transform.localScale = Vector3.zero;
        gameObject.transform.Rotate(new Vector3(0, 0, -135));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.localScale.x < 1)
        {
            transform.localScale += new Vector3(0.02F, 0.02F, 0);
        }
        if (transform.localScale.z < 1)
        {
            transform.localScale += new Vector3(0, 0, 0.015F);
        }
        gameObject.transform.Rotate(new Vector3(0, 0, 2));
    }
}
