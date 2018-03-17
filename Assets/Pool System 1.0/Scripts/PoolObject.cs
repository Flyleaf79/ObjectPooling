using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour {
    // Used to overid the PoolManager Method ("ObjectReuse")
    public virtual void OnObjectReuse() { }
    //protected variables, are variables that are visible only to the class to which they belong, and any subclasses.
    protected void Destroy()
    {
        gameObject.SetActive(false);
    }
}
