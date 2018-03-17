using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {

    // This works when objects dont need to override or need to be change within the pool.
    //Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

    // This was created instead so that instead of gameobjects being stored, 
    // there are "ObjectInstances" (Class) being stored. EI (Gameobjects name , transform , poolObject component)
    Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();
    // An easy way to access the following methods using the Singleton Pattern
    static PoolManager _instance;

    // Accessor
    public static PoolManager instance
    {
        get
        {
            // if instance is null than what we want to do is find active PoolManager Script
            // in the currently active scene.
            if (_instance == null)
            {
                _instance = FindObjectOfType<PoolManager>();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Creates the pool
    /// </summary>
    /// <param name="prefab">The prefab that we are loading into the pool</param>
    /// <param name="poolSize">Size of prefab spawn</param>
    public void CreatePool(GameObject prefab, int poolSize)
    {
        // Get a unique ID for every gameObject 
        int poolKey = prefab.GetInstanceID();

        // Creates a Gameobject that encapsulates all the new ObjectInstances
        GameObject poolHolder = new GameObject(prefab.name + " Pool");
        poolHolder.transform.parent = transform;

        // If the Unique ID of the Gameobject is not in the dictionary
        // Then Add Unique ID to the Dictionary
        if (!poolDictionary.ContainsKey(poolKey))
        {
            // =======Obsolete==== See Line ( 7 - 8)
            //poolDictionary.Add(poolKey, new Queue<GameObject>());

            poolDictionary.Add(poolKey, new Queue<ObjectInstance>());
            // for the pool size given the Gameobject will instantiate and record the new Gamobjects
            // into the Dictionary file
            for (int i = 0; i < poolSize; i++)
            {
                // ====Obsolete==== See Line (7 - 8)
                //GameObject newObject = Instantiate(prefab) as GameObject;

                //instead of passing a gamobject, we pass an ObjectInstance
                // and will pass through a Gamobject
                ObjectInstance newObject = new ObjectInstance(Instantiate(prefab) as GameObject);
                // Moved to the ObjectInstance 
                //newObject.SetActive(false);
                poolDictionary[poolKey].Enqueue(newObject);
                // ObjectInstance to set parent
                newObject.SetParent(poolHolder.transform);
            }
        }
    }
    /// <summary>
    /// Used if Objects are perfered to Hide and Reuse. This will save memory and add to performance
    /// </summary>
    public void ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();
        // If the Unique Id of the GameObject is true than 
        // Remove off the Queue of the dictionary variable and 
        if (poolDictionary.ContainsKey(poolKey))
        {
            // objectToReuse = the uniqueId in the poolDictionary
            // Think of Dequeing like Walmart Line
            // First Come First Go - The person in front of the line has gotten serviced, so lets moved
            // the person in the front to the back of the line to rejoin the line and wait to get 
            // serviced. In this case were getting the Id of the Gameobject and sending it to the back of the line.
            // GameObject objectToReuse = poolDictionary[poolKey].Dequeue(); // Obsolete - See Line (7 - 8);
            ObjectInstance objectToReuse = poolDictionary[poolKey].Dequeue();
            poolDictionary[poolKey].Enqueue(objectToReuse);

            // Once ReuseObject is called itll tell the "objectToReuse" (ObjectInstance) to reuse 
            objectToReuse.Reuse(position, rotation);

            /*
            objectToReuse.SetActive(true);
            objectToReuse.transform.position = position;
            objectToReuse.transform.rotation = rotation;
            */
        }
    }

    /// <summary>
    /// Used to store some values of the gameobject that instanced to the pool
    /// </summary>
    public class ObjectInstance
    {
        GameObject gameObject;
        Transform transform;

        // Since not all object in the pool are required to have the "PoolObject.cs" class attached to
        // a bool will be made to declare if object has and if object doesnt
        bool hasPoolObjectComponent;
        PoolObject poolObjectScript;

        // Constructor
        // Any Gamobject that passes through this class will go through this constructor.
        public ObjectInstance(GameObject objectInstance)
        {

            gameObject = objectInstance;
            transform = gameObject.transform;
            gameObject.SetActive(false);
            // if the Object given has the PoolObject component in it then se the 
            // bool that says the object has the poolobject to true and
            // grab declare PoolObjectScript = to the gamobject given component "PoolObject"
            if (gameObject.GetComponent<PoolObject>())
            {
                hasPoolObjectComponent = true;
                poolObjectScript = gameObject.GetComponent<PoolObject>();
            }
        }

        /// <summary>
        /// Use this to reuse ObjectIntance(s) in the Pool
        /// </summary>
        /// <param name="position">Position of the Reused ObjectInstance</param>
        /// <param name="rotation">Rotation of the Reused ObjectInstance</param>
        public void Reuse(Vector3 position, Quaternion rotation)
        {
            gameObject.SetActive(true);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;

            // if Object has the PoolObject component than Notify the "PoolObject.cs"
            if (hasPoolObjectComponent)
            {
                poolObjectScript.OnObjectReuse();
            }
        }

        /// <summary>
        /// Sets Parent of ObjectInstance
        /// </summary>
        /// <param name="parent"> desired parent </param>
        public void SetParent(Transform parent)
        {
            transform.parent = parent;
        }
    }
}
