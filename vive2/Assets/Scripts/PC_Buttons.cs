using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// <author email="andrew.ajemian@gmail.com">Andrew L. Ajemian</author>
/// This code is a terrible hack, but solves the problem.
/// </summary>
public class PC_Buttons : MonoBehaviour {

    [SerializeField]
    private CameraControllerPC ccpc;


    public void SetStateNone()
    {
        ccpc.building_placing = CameraControllerPC.Building.None;
    }
    public void SetStateWall()
    {
        ccpc.building_placing = CameraControllerPC.Building.Wall;
    }
    public void SetStateTurret()
    {
        ccpc.building_placing = CameraControllerPC.Building.Turret;
    }
    public void SetStateTepee()
    {
        ccpc.building_placing = CameraControllerPC.Building.Tepee;
    }


}
