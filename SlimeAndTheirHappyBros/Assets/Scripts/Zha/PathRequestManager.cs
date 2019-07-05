using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour {

    //Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    List<PathRequest> pathRequestList = new List<PathRequest>();

    PathRequest currentPathRequest;

    static PathRequestManager instance;
    PathFinding pathFinding;

    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        pathFinding = GetComponent<PathFinding>();
    }

    public static PathRequest RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> _successCbk)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, _successCbk);
        //instance.pathRequestQueue.Enqueue(newRequest);
        instance.pathRequestList.Add(newRequest);
        instance.TryProcessNext();
        Debug.Log("add new finding path request   " + instance.pathRequestList.IndexOf(newRequest));
        return newRequest;
    }
    public static PathRequest RequestPath(string name, Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> _successCbk)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, _successCbk);
        //instance.pathRequestQueue.Enqueue(newRequest);
        instance.pathRequestList.Add(newRequest);
        Debug.Log(name + " addddddddddd new finding path request   " + instance.pathRequestList.IndexOf(newRequest));
        instance.TryProcessNext();

        return newRequest;
    }

    void TryProcessNext()
    {
        //if (!isProcessingPath && pathRequestQueue.Count > 0)
        //{
        //    currentPathRequest = pathRequestQueue.Dequeue();
        //    isProcessingPath = true;
        //    pathFinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        //}
        if (!isProcessingPath && pathRequestList.Count > 0)
        {
            currentPathRequest = pathRequestList[0];
            pathRequestList.RemoveAt(0);
            isProcessingPath = true;
            pathFinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public static void CancleRequest(string name, PathRequest request)
    {
        Debug.Log(name + " cancle finding path request   " + instance.pathRequestList.IndexOf(request));
        if (instance.pathRequestList.Contains(request)) instance.pathRequestList.Remove(request);
    }
    public static void CancleRequest(PathRequest request)
    {
        Debug.Log("  cancle finding path request   " +  instance.pathRequestList.IndexOf(request));
        if (instance.pathRequestList.Contains(request))instance.pathRequestList.Remove(request);
    }


    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
        //if (success) {
            
        //}
    }


    public static void ClearExtendPenalty() {
        instance.pathFinding.ClearGridExtendPenalty();
    }

    //public void FailProcessingPath() {
    //    isProcessingPath = false;
    //}

    public class PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _successCbk)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _successCbk;
        }

    }
}
