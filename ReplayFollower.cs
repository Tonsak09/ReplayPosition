using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReplayFollower : MonoBehaviour
{

    [Header("Recording")]
    [SerializeField] private Transform target;
    [SerializeField] private float pointUpdateRate; 
    [SerializeField] private List<Vector3> points;

    [Header("Movement")]
    [SerializeField] private float replaySpeed = 1.0f;

    [Header("Initialization")]
    [SerializeField] float pauseOnFirstMove = 0.0f;


    private float timer;

    private float moveTimer; 
    private Vector3 originPos;
    private Vector3 targetPos;

    private bool beginRecording = false;
    private Vector3 holdPos;
    private float initialTimer = 0.0f; 

    private void Start()
    {
        timer = 0.0f;
        moveTimer = 0.0f;

        originPos = target.position;
        targetPos = target.position;
        holdPos   = target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CheckIfStart())
            return;

        RecordData();

        if (initialTimer <= pauseOnFirstMove)
        {
            initialTimer += Time.deltaTime;
            return;
        }

        ReplayMovement();
    }

    bool CheckIfStart()
    {
        if (beginRecording)
            return true;

        // Checks if there is a difference between hold and target 
        return (holdPos - target.position).sqrMagnitude > float.Epsilon;
    }


    void RecordData()
    {
        timer += Time.deltaTime;

        if (timer > pointUpdateRate)
        {
            points.Add(target.position);
            timer = 0;
        }
    }

    void ReplayMovement()
    {
        moveTimer += replaySpeed * Time.deltaTime;

        // Skip points if movetimer overflows 
        while (moveTimer > pointUpdateRate)
        {
            moveTimer = 0.0f;

            if (points.Count > 0)
            {
                // Points still exist
                originPos = targetPos;
                targetPos = points[0];
                points.RemoveAt(0);
            }
        }


        this.transform.position = Vector3.Lerp(originPos, targetPos, moveTimer / pointUpdateRate);
    }
}
