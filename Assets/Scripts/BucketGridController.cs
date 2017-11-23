using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The Bucket Grid Controller creates a 2D array of Lists of game object transforms which can be used to optimize things like finding nearest objects, polygon detail levels, collisions, ect..
//To access any bucket in the bucketgrid, use getBucket to access that cell

public class BucketGridController : MonoBehaviour {

    public int bucketWidth = 20, bucketHeight = 20, hbuckets, vbuckets, bucketableScanPos = 0;

    public Transform gridPrefab;

    List<Transform>[,] bucketGrid; //A 2d array of Lists
    Transform[,] bucketLabelGrid; //A 2d array of transforms for the labels of each bucket

    List<Transform> bucketables;

    public bool showBuckets = false;

    void Awake()
    {
        bucketWidth = 10;
        bucketHeight = 10;

        hbuckets = 24; // 17; //13 at 20
        vbuckets = 24; // 17;

        bucketables = new List<Transform>();
        bucketGrid = new List<Transform>[hbuckets, vbuckets];
        bucketLabelGrid = new Transform[hbuckets, vbuckets];

        for (int ix = 0; ix < hbuckets; ix++)
        {
            for (int iy = 0; iy < vbuckets; iy++)
            {
                bucketGrid[ix,iy] = new List<Transform>();
                if (showBuckets)
                {
                    bucketLabelGrid[ix, iy] = Instantiate(gridPrefab, new Vector3(ix * bucketWidth, .1f, iy * bucketHeight), Quaternion.identity);
                    bucketLabelGrid[ix, iy].GetChild(0).GetComponent<TextMesh>().text = "OBJECTS["+ix+"," + iy + "]";
                }
            }
        }
    }

    //Add an object to the bucket grid
    public void addBucketable(Transform obj)
    {
        //Add the bucketable to the 1d list
        bucketables.Add(obj);

        //Calculate the bucketables position in the bucketgrid
        int bucketX, bucketZ;
        bucketX = (int) obj.position.x / bucketWidth;
        bucketZ = (int) obj.position.z / bucketHeight;

        bucketX = Math.Max(bucketX, 0);
        bucketZ = Math.Max(bucketZ, 0);
        bucketX = Math.Min(bucketX, hbuckets);
        bucketZ = Math.Min(bucketZ, vbuckets);

        Bucketable objBucketable = obj.GetComponent<Bucketable>();

        //Set the position of the bucketable within the bucketgrid
        objBucketable.setBucketPosition(bucketX, bucketZ);
        if (showBuckets)
        {
            bucketLabelGrid[bucketX, bucketZ].GetChild(0).GetComponent<TextMesh>().text += "\n" + obj.name;
        }
        //Debug.Log("Added object " + obj.name + " at position (" + bucketX + "," + bucketZ + ")");
        bucketGrid[bucketX,bucketZ].Add(obj);
    }

    public void removeBucketable(Transform obj) //, int bx, int bz)
    {
        int bx, bz;
        bx = (int)(obj.position.x / bucketWidth);
        bz = (int)(obj.position.z / bucketHeight);
        bucketGrid[bx, bz].Remove(obj);

        bx = obj.GetComponent<Bucketable>().bucketX;
        bz = obj.GetComponent<Bucketable>().bucketZ;
        bucketGrid[bx, bz].Remove(obj);

        //Update debug text for this bucket
        if (showBuckets)
            updateBucketDebugText(bx, bz);
    }

    //Update an object in the bucket grid
    public void updateBucketable(Transform obj)
    {
        int bucketX, bucketZ;
        bucketX = (int) obj.position.x / bucketWidth;
        bucketZ = (int) obj.position.z / bucketHeight;

        bucketX = Math.Max(bucketX, 0);
        bucketZ = Math.Max(bucketZ, 0);
        bucketX = Math.Min(bucketX, hbuckets);
        bucketZ = Math.Min(bucketZ, vbuckets);

        Bucketable objBucketable = obj.GetComponent<Bucketable>();

        //If the object is already in a different bucket, remove it from it's previous bucket
        if (objBucketable.positionSet && (bucketX != objBucketable.bucketX || bucketZ != objBucketable.bucketZ))
        {
            bucketGrid [ objBucketable.bucketX , objBucketable.bucketZ ].Remove(obj);

            //Update debug text for previous bucket
            if (showBuckets)
                updateBucketDebugText( objBucketable.bucketX , objBucketable.bucketZ );
            
            //Debug.Log("Updated object " + obj.name + " from position (" + objBucketable.bucketX + "," + objBucketable.bucketZ + ") to position (" + bucketX + "," + bucketZ + ")");

            //Update the bucketable objects current bucket position
            objBucketable.setBucketPosition( bucketX, bucketZ );

            //Add the object to the new bucket
            bucketGrid[ bucketX, bucketZ ].Add(obj);

            //Update debug text for this bucket
            if (showBuckets)
                updateBucketDebugText( bucketX, bucketZ );
        }
        
    }

    void updateBucketDebugText(int bx, int bz)
    {
        if (bucketLabelGrid[bx, bz] != null)
        { 
        bucketLabelGrid[bx, bz].GetChild(0).GetComponent<TextMesh>().text = "bucket["+bx+","+bz+"]";
        for (int i = 0; i < bucketGrid[bx, bz].Count; i++)
            {
                if (bucketGrid[bx, bz][i] != null)
                    bucketLabelGrid[bx, bz].GetChild(0).GetComponent<TextMesh>().text += "\n" + bucketGrid[bx, bz][i].name;
                else
                    bucketGrid[bx, bz].RemoveAt(i);
            }
        }
    }

    //Return the bucket at the bucket coordinates
    public List<Transform> getBucket(int bx, int by)
    {
        bx = Math.Max(bx, 0);
        by = Math.Max(by, 0);
        bx = Math.Min(bx, hbuckets - 1);
        by = Math.Min(by, vbuckets - 1);

        return bucketGrid[bx, by];
    }
    
    //Return the bucket at the world coordinates (world coordinates automatically converted to bucket coordinates)
    public List<Transform> getBucket(float bxf, float byf)
    {
        int bx = (int)bxf / bucketWidth;
        int by = (int)byf / bucketHeight;

        bx = Math.Max(bx, 0);
        by = Math.Max(by, 0);
        bx = Math.Min(bx, hbuckets - 1);
        by = Math.Min(by, vbuckets - 1);

        return bucketGrid[bx, by];
    }
    
    public Transform getNearestObject(Vector2 point, float maxTestRadius, List<string> findTag)
    {
        Transform nearestObject, nextNearestObject;
        
        nearestObject = getNearestObject(point, maxTestRadius, findTag[0]);

        float nearestDistance = float.MaxValue;

        if (nearestObject != null)
            nearestDistance = Vector2.Distance(new Vector2(point.x, point.y), new Vector2(nearestObject.position.x, nearestObject.position.z));

        for (int i = 1; i < findTag.Count; i++)
        {
            nextNearestObject = getNearestObject(point, maxTestRadius, findTag[i]);
            if (nextNearestObject != null)
            {
                float nextNearestDistance = Vector2.Distance(new Vector2(point.x, point.y), new Vector2(nextNearestObject.position.x, nextNearestObject.position.z));
                if (nextNearestDistance < nearestDistance)
                {
                    nearestObject = nextNearestObject;
                }
            }
        }
        return nearestObject;
    }

    //Returns a list of nearby objects to the given point within the given distance
    //TODO: write this method

    //Returns nearest object to the given point within the given distance
    public Transform getNearestObject(Vector2 point, float maxTestRadius, string findTag)
    {
        //List<Transform> nearbyObjects = new List<Transform>();

        float maxTestRadiusBuckets = maxTestRadius; // Mathf.Max(maxTestRadius / (float)bucketWidth, 2) + 2;

        int bucketX, bucketZ, radius, ix, iy;
        float smallestDistance, maxRadius;

        bucketX = Math.Min(Math.Max((int)(point.x / bucketWidth), 0), hbuckets);
        bucketZ = Math.Min(Math.Max((int)(point.y / bucketHeight), 0), vbuckets);

        Transform closest = getNearestObjectInBucket(bucketX, bucketZ, point, findTag);

        //Max radius is the limit at which it will stop searching, will dynamically expand if no object is found
        maxRadius = Math.Max(hbuckets, vbuckets);

        if (closest != null)
            smallestDistance = Vector2.Distance(point, new Vector2(closest.position.x, closest.position.z));

        radius = 1;

        ix = Math.Max(bucketX - 1, 0);
        iy = Math.Max(bucketZ - 1, 0);

        int bestix = -100, bestiy = -100;

        while (radius <= Math.Min(hbuckets, Math.Min(maxRadius, maxTestRadiusBuckets)))
        {
            //Debug.Log("maxRadius: " + maxRadius);
            //Debug.Log("maxTestRadiusBuckets: " + maxTestRadiusBuckets);
            //Debug.Log("ix: " + ix);
            //Debug.Log("iy: " + iy);
            if (maxTestRadius == -1)
                maxTestRadius = maxRadius;

            int c = 5;

            Transform testObj = getNearestObjectInBucket(ix, iy, point, findTag);

            /*
            if (testObj == null)
                Debug.Log("testObj["+ix+","+iy+"] = null");
            else
                Debug.Log("testObj[" + ix + "," + iy + "] = " + testObj);
                */

            if (testObj != null && testObj.tag == findTag)
            {
                float newDist = Vector2.Distance(point, new Vector2(testObj.position.x, testObj.position.z));
                if (closest == null)
                {
                    bestix = ix;
                    bestiy = iy;

                    closest = testObj;
                    maxRadius = radius + 1;
                    smallestDistance = newDist;
                }
            }
            
            int firstCol, lastCol, firstRow, lastRow;

            firstCol = Math.Max(bucketX - radius, 0);
            firstRow = Math.Max(bucketZ - radius, 0);
            lastCol = Math.Min(bucketX + radius, hbuckets - 1);
            lastRow = Math.Min(bucketZ + radius, vbuckets - 1);

            //HANDLE TOP AND BOTTOM ROWS (where every column's cell is scanned)
            if (iy == bucketZ - radius || iy == bucketZ + radius)
            {
                if (ix < lastCol)          //if hasn't reached rightmost column
                {
                    ix += 1;               //move right
                }
                else                       //else if reached rightmost column
                {
                    ix = firstCol;         //go to beginning column
                    if (iy == lastRow)     //if this is the last row
                    {
                        radius += 1;                         //increase search radius
                        ix = Math.Max(bucketX - radius, 0);  //reset scan position to top left corner
                        iy = Math.Max(bucketZ - radius, 0);
                    }
                    else                   //if this isn't the last row
                    {
                        iy += 1;           //go to next row
                    }
                }
            }
            else //HANDLE CENTER ROWS (where only leftmost and righmost column cell is scanned)
            {
                if (ix == firstCol)                     //if this is the first column
                {
                    ix = lastCol;                       //go to last column
                    if (lastCol != bucketX + radius)    //if last column is out of bounds
                    {
                        ix = firstCol;          //go to beginning column
                        iy += 1;                //go to next row
                    }
                }
                else                                    //else if this is the last column
                {
                    ix = firstCol;                  //go to beginning column
                    iy += 1;                        //go to next row
                }
            }

            if (iy == lastRow + 1)
            {
                //Expand buckets to check
                radius += 1;
                ix = Math.Max(bucketX - radius, 0);
                iy = Math.Max(bucketZ - radius, 0);
            }

            
        }
        //Debug.Log("bestix: " + bestix);
        //Debug.Log("bestiy: " + bestiy);

        /*
        if (closest != null)
            Debug.Log("closest found");
        else
            Debug.Log("no closest found");
            */
        return closest;
    }
    
    //Returns nearest object to point in the bucket (bx, by)
    Transform getNearestObjectInBucket(int bx, int by, Vector2 point, string findTag)
    {
        List<Transform> nearbyObjects; // = new List<Transform>();

        int bucketX, bucketZ;

        //bucketX = (int)point.x / bucketWidth;
        //bucketZ = (int)point.y / bucketHeight;

        bucketX = bx; // Math.Min(Math.Max((int)point.x / bucketWidth, 0), hbuckets);
        bucketZ = by; // Math.Min(Math.Max((int)point.y / bucketHeight, 0), vbuckets);
        
        nearbyObjects = bucketGrid[bucketX, bucketZ];

        int closest = -1;
        float smallestDistance = float.MaxValue;

        if (nearbyObjects.Count > 0)
        smallestDistance = Vector2.Distance(point, new Vector2(nearbyObjects[0].position.x, nearbyObjects[0].position.z));

        //Debug.Log("made it to for loop[bucketX = " + bucketX + ",bucketZ = " + bucketZ + "]: " + nearbyObjects.Count);
        //Debug.Log("made it to for loop["+bx+","+by+"]: "+ nearbyObjects.Count);
        for (int i = 0; i < nearbyObjects.Count ; i++)
        {
            //Debug.Log("made it into for loop");
            if (nearbyObjects[i] != null)
                {
                if (nearbyObjects[i].tag == findTag)
                {
                    float currentDistance = Vector2.Distance(point, new Vector2(nearbyObjects[i].position.x, nearbyObjects[i].position.z));
                    if (currentDistance <= smallestDistance)
                    {
                        closest = i;
                        smallestDistance = currentDistance;
                    }
                }
                else {
                    //Debug.Log("wrong tag");
                }
            }
            else
            {
                //Debug.Log("nearby object is null");
            }
        }
        if (closest == -1)
        {
            //Debug.Log("no closest found");
            return null;
        }
        if (nearbyObjects.Count == 0)
        {
            //Debug.Log("nearbyObjects list is empty");
            return null;
        }

        //Debug.Log("returned nearest object");
        return nearbyObjects[closest];
    }
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (bucketableScanPos < bucketables.Count)
        {
            if (bucketables[bucketableScanPos] != null)
            {
                updateBucketable(bucketables[bucketableScanPos]);
                bucketableScanPos++;
            }
            else
            {
                bucketables.RemoveAt(bucketableScanPos);
            }
        }
        else
        {
            bucketableScanPos = 0;
        }
	}
}
