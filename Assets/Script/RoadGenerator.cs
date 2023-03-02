using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public static RoadGenerator instance;
   [SerializeField] private List<GameObject> _prefabs;
   [SerializeField] private float _speed;
   [SerializeField] private int _maxRoadCount;
   [SerializeField] private float _offset;

   private List<Transform> roads = new List<Transform>();
   private float _currentSpeed;

   private void Awake() 
   {
        instance = this;
   }

   private void Start() 
   {
        ResetLevel();        
   }

   public void StartLevel()
   {
        _currentSpeed = _speed;
        SwipeController.instance.enabled = true;
   }

   private void Update()  
   {
        if(_currentSpeed == 0) 
        return;

        foreach (Transform road in roads)
        {
            road.position -= new Vector3(0,0,_currentSpeed * Time.deltaTime);
        }

        if(roads[0].position.z < -10)
        {
            Destroy(roads[0].gameObject);
            roads.RemoveAt(0);
            CreateNextRoad();
        }
   }

   private void CreateNextRoad()
   {
        Vector3 pos = Vector3.zero;
        int random = Random.Range(0, _prefabs.Count);
        if(roads.Count > 0) pos = roads[roads.Count - 1].transform.position + new Vector3(0,0, _offset);
        GameObject g = Instantiate(_prefabs[random], pos, Quaternion.identity);
        g.transform.SetParent(transform);
        roads.Add(g.transform);
   }

   public void ResetLevel()
   {
        _currentSpeed = 0;
        while(roads.Count > 0)
        {
            Destroy(roads[0].gameObject);
            roads.RemoveAt(0);
        }

        for (int i = 0; i < _maxRoadCount; i++)
        {
            CreateNextRoad();
        }
        SwipeController.instance.enabled = false;

   }
}