using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : PoolObject {

    static int id = 1;

    void Start()
    {
        GameObject child = null;
        child = transform.GetChild(0).gameObject;

        var  childTextMesh = child.GetComponent<TextMesh>();
        childTextMesh.text = id.ToString();
        id++;
    }

    void Update()
    {
        transform.localScale += Vector3.one * Time.deltaTime * 3;
        transform.Translate(Vector3.forward * Time.deltaTime * 25f);
    }

    // Since now the Object now has an extension of "PoolObject" PoolObject will give this void

    public override void OnObjectReuse()
    {
        transform.localScale = Vector3.one;
    }
}
