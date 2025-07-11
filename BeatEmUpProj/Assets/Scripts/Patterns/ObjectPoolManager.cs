using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manager
{
    public enum PoolType
    {
        ParticleSystem,
        GameObject,
        None
    }
    public class ObjectPoolManager : MonoBehaviour
    {
        public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

        public static PoolType PoolingType;

        private static GameObject _particleSystemEmpty;
        private static GameObject _gameObjectsEmpty;

        public static GameObject _objectPoolEmptyHolder;

        public Action OnRestart;

        private void Awake()
        {
            SetupEmpties();
        }

        public void ResetAllPool() {
            OnRestart?.Invoke();
        }

        private void SetupEmpties()
        {
            _objectPoolEmptyHolder = new GameObject(Constants.POOLED_OBJECTS_NAME);

            _particleSystemEmpty = new GameObject(Constants.PARTICLE_OBJECTS_NAME);
            _particleSystemEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);

            _gameObjectsEmpty = new GameObject(Constants.GAME_OBJECTS_NAME);
            _gameObjectsEmpty.transform.SetParent(_objectPoolEmptyHolder.transform);
        }
        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.GameObject)
        {
            PooledObjectInfo pool = ObjectPools.Find(obj => obj.LookupString == objectToSpawn.name);
            if (pool == null)
            {
                pool = new PooledObjectInfo(objectToSpawn.name);
                ObjectPools.Add(pool);
            }

            //Check inactive objects in the pool
            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();
            if (spawnableObj == null)
            {
                
                //if there is none, then instantiate
                spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
                GameObject parentObject = SetParentObject(poolType);
                if (parentObject != null)
                {
                    spawnableObj.transform.SetParent(parentObject.transform);
                }
            }
            else
            {
                spawnableObj.transform.position = spawnPosition;
                spawnableObj.transform.rotation = spawnRotation;
                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);

            }
            return spawnableObj;
        }

        public static GameObject SpawnObject(GameObject objectToSpawn, Transform parent)
        {
            PooledObjectInfo pool = ObjectPools.Find(obj => obj.LookupString == objectToSpawn.name);
            if (pool == null)
            {
                pool = new PooledObjectInfo(objectToSpawn.name);
                ObjectPools.Add(pool);
            }

            //Check inactive objects in the pool
            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();
            if (spawnableObj == null)
            {

                //if there is none, then instantiate
                spawnableObj = Instantiate(objectToSpawn,parent);
               
                if (parent != null)
                {
                    spawnableObj.transform.SetParent(parent.transform);
                }
            }
            else
            {
                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);
            }
            return spawnableObj;
        }

		public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Transform parent)
		{
			PooledObjectInfo pool = ObjectPools.Find(obj => obj.LookupString == objectToSpawn.name);
			if (pool == null)
			{
				pool = new PooledObjectInfo(objectToSpawn.name);
				ObjectPools.Add(pool);
			}

			//Check inactive objects in the pool
			GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();
			if (spawnableObj == null)
			{

				//if there is none, then instantiate
				spawnableObj = Instantiate(objectToSpawn, parent);
                spawnableObj.transform.position = spawnPosition;

				if (parent != null)
				{
					spawnableObj.transform.SetParent(parent.transform);
				}
			}
			else
			{
				spawnableObj.transform.position = spawnPosition;
				pool.InactiveObjects.Remove(spawnableObj);
				spawnableObj.SetActive(true);
			}
			return spawnableObj;
		}
		public static void ReturnObjectToPool(GameObject obj)
        {
            string goName = obj.name.Replace(Constants.CLONE_SUFIX, string.Empty); //remove (Clone) from name
            PooledObjectInfo pool = ObjectPools.Find(obj => obj.LookupString == goName);

            if (pool == null)
            {
                Debug.LogWarning("Trying to return a object to the pool that does not belong to any pool " + obj.name);
            }
            else
            {
                obj.SetActive(false);
                pool.InactiveObjects.Add(obj);
            }
        }

        private static GameObject SetParentObject(PoolType poolType) {
            switch (poolType)
            {
                case PoolType.None:
                    return null;
                case PoolType.GameObject:
                    return _gameObjectsEmpty;
                case PoolType.ParticleSystem: 
                    return _particleSystemEmpty;
                default: 
                    return null;
            }
        }
    }

    public class PooledObjectInfo
    {
        public string LookupString;
        public List<GameObject> InactiveObjects = new List<GameObject>();
        public PooledObjectInfo(string lookupString)
        {
            LookupString = lookupString;
        }
    }
}
