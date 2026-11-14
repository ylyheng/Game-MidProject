using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] private Color newColor = Color.red;

    [SerializeField] private Renderer rend;

    void Start()
    {
        Debug.Log("player is awake!");
        rend = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

    }

    void OnMouseDown()
    {
        Debug.Log("clicked");
        rend.material.color = newColor;
    }

}
