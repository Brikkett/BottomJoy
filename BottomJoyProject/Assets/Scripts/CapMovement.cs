using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Animations;
using System.IO;

public class CapMovement : MonoBehaviour
{
    bool selected, turned;
    int timer;
    GameObject selectedObject;
    Vector3 mousePosition, prevPos, offset, momentum;
    float _momentumCoef;
    [SerializeField] private float momentumCoef = 0.6f;
    Rigidbody2D rb;
    TextMeshProUGUI message;
    [SerializeField] private TextAsset file;

    void Start()
    {
        message = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        message.text = "";
        _momentumCoef = momentumCoef;
    }

    void FixedUpdate()
    {
        timer++;
        prevPos = mousePosition;
        mousePosition = Input.mousePosition;
        if (selected)
        {
            if (mousePosition != prevPos || timer > 10)
                momentum = mousePosition - prevPos;
            _momentumCoef = momentumCoef;
        }
        else
        {
            transform.Translate(_momentumCoef * momentum);
            if (_momentumCoef > 0)
                _momentumCoef -= 0.005f;
            else
                _momentumCoef = 0f;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            timer = 0;
            Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);
            if (targetObject)
            {
                selected = targetObject.transform.gameObject == this.gameObject;
                offset = transform.position - mousePosition;
            }
        }
        if (selected)
        {
            transform.position = mousePosition + offset;
        }
        if (Input.GetMouseButtonUp(0) && selected)
        {
            if (momentum == Vector3.zero && timer < 10)
                Turn();
            selected = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLLISION");
        if (collision.transform.name == "Top" || collision.transform.name == "Bottom")
            momentum = new Vector3(momentum.x, -momentum.y, 0);
        if (collision.transform.name == "Left" || collision.transform.name == "Right")
            momentum = new Vector3(-momentum.x, momentum.y, 0);
    }

    void Turn()
    {
        if (turned)
            return;
        turned = true;
        selected = false;
        GetComponent<Animator>().Play("cap");
        string[] messages = file.text.Split('\n');
        int rand = Random.RandomRange(0, messages.Length-1);

        message.text = messages[rand];
    }

}
