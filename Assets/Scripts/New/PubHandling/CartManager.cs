using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CartManager : MonoBehaviour
{
    [System.Serializable]
    public class StationType
    {
        public string name;
        public bool isFoodStation; // true for food plate station
        public Transform[] wallSpawnPoints;
        public Transform tablePoint; // only used for wine stations
        public GameObject[] itemPrefabs;
        [HideInInspector] public List<GameObject> currentWallItems = new List<GameObject>();
    }

    public List<StationType> stations;
    public Transform[] userTablePoints;
    public float speed = 1f;

    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform backLeftWheel;
    public Transform backRightWheel;
    public float wheelRotationSpeed = 300f;

    void Start()
    {
        foreach (var station in stations)
        {
            foreach (var spawnPoint in station.wallSpawnPoints)
            {
                GameObject prefab = station.itemPrefabs[Random.Range(0, station.itemPrefabs.Length)];
                GameObject obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
                station.currentWallItems.Add(obj);
            }
        }

        StartCoroutine(CartRoutine());
    }

    IEnumerator CartRoutine()
    {
        while (true)
        {
            foreach (var station in stations)
            {
                GameObject item = GetAvailableItem(station);
                if (item == null) continue;

                if (station.isFoodStation)
                {
                    // Cart directly picks food from wall
                    yield return MoveTo(station.wallSpawnPoints[0].position);
                    station.currentWallItems.Remove(item);
                    yield return MoveItem(item, transform.position + Vector3.up);
                }
                else
                {
                    // Wine: wall → vending table → cart
                    yield return MoveTo(station.wallSpawnPoints[0].position);
                    station.currentWallItems.Remove(item);
                    yield return MoveItem(item, station.tablePoint.position + Vector3.up);
                    yield return MoveTo(station.tablePoint.position);
                    yield return new WaitForSeconds(1.5f);

                    // Check if item was grabbed
                    if (item == null || (item.GetComponent<GrabbableItem>()?.isGrabbedByPlayer ?? false))
                        continue;

                    yield return MoveItem(item, transform.position + Vector3.up);
                }

                // Deliver to user table
                Transform userTable = userTablePoints[Random.Range(0, userTablePoints.Length)];
                yield return MoveTo(userTable.position);
                yield return MoveItem(item, userTable.position + Vector3.up);
                yield return new WaitForSeconds(1.5f);

                // Refill wall
                StartCoroutine(RestockWall(station, item.transform.position, 4f));
            }
        }
    }

    GameObject GetAvailableItem(StationType station)
    {
        foreach (var item in station.currentWallItems)
        {
            if (item != null && (!item.GetComponent<GrabbableItem>()?.isGrabbedByPlayer ?? true))
                return item;
        }
        return null;
    }

    IEnumerator MoveTo(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            RotateWheels(speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator MoveItem(GameObject item, Vector3 target)
    {
        if (item == null) yield break;

        while (Vector3.Distance(item.transform.position, target) > 0.05f)
        {
            if (item.GetComponent<GrabbableItem>()?.isGrabbedByPlayer == true)
                yield break;

            item.transform.position = Vector3.MoveTowards(item.transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator RestockWall(StationType station, Vector3 spawnPos, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject prefab = station.itemPrefabs[Random.Range(0, station.itemPrefabs.Length)];
        GameObject newObj = Instantiate(prefab, spawnPos, Quaternion.identity);
        station.currentWallItems.Add(newObj);
    }

    void RotateWheels(float moveAmount)
    {
        float rotation = wheelRotationSpeed * moveAmount;
        frontLeftWheel?.Rotate(Vector3.right, rotation);
        frontRightWheel?.Rotate(Vector3.right, rotation);
        backLeftWheel?.Rotate(Vector3.right, rotation);
        backRightWheel?.Rotate(Vector3.right, rotation);
    }
}