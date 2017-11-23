//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public struct PointerEventArgs
{
    public uint controllerIndex;
    public uint flags;
    public float distance;
    public Transform target;
}

public delegate void PointerEventHandler(object sender, PointerEventArgs e);

public class SteamVR_LaserPointer : MonoBehaviour
{
    public bool active = true;
    public Color color;
    public float thickness = 0.002f;
    public GameObject holder;
    public GameObject pointer;
    bool isActive = false;
    public bool addRigidBody = false;
    public Transform reference;
    public event PointerEventHandler PointerIn;
    public event PointerEventHandler PointerOut;

    Transform previousContact = null;

    public Button aButton;
    public VRButton vrButton;

    bool triggerJustPressed = false, triggerPressedPrev = false;

    // Use this for initialization
    void Start()
    {
        holder = new GameObject();
        holder.transform.parent = this.transform;
        holder.transform.localPosition = Vector3.zero;
        holder.transform.localRotation = Quaternion.identity;

        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.transform.parent = holder.transform;
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
        pointer.transform.localRotation = Quaternion.identity;
        BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (addRigidBody)
        {
            if (collider)
            {
                collider.isTrigger = true;
            }
            Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }
        else
        {
            if (collider)
            {
                Object.Destroy(collider);
            }
        }
        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", color);
        pointer.GetComponent<MeshRenderer>().material = newMaterial;
    }

    public virtual void OnPointerIn(PointerEventArgs e)
    {
        if (PointerIn != null)
            PointerIn(this, e);
    }

    public virtual void OnPointerOut(PointerEventArgs e)
    {
        if (PointerOut != null)
            PointerOut(this, e);
    }


    // Update is called once per frame
    void Update()
    {

        if (!isActive)
        {
            isActive = true;
            this.transform.GetChild(0).gameObject.SetActive(true);
        }

        float dist = 100f;

        SteamVR_TrackedController controller = GetComponent<SteamVR_TrackedController>();
        
        if (controller.triggerPressed && !triggerPressedPrev)
        {
            triggerJustPressed = true;
            //Debug.Log("trigger just pressed");
        }
        else
        {
            triggerJustPressed = false;
        }
        triggerPressedPrev = controller.triggerPressed;
        
        Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit);

        if (previousContact && previousContact != hit.transform)
        {
            PointerEventArgs args = new PointerEventArgs();
            if (controller != null)
            {
                args.controllerIndex = controller.controllerIndex;
            }
            args.distance = 0f;
            args.flags = 0;
            args.target = previousContact;
            OnPointerOut(args);
            previousContact = null;
        }
        if (bHit && previousContact != hit.transform)
        {
            PointerEventArgs argsIn = new PointerEventArgs();
            if (controller != null)
            {
                argsIn.controllerIndex = controller.controllerIndex;
            }
            argsIn.distance = hit.distance;
            argsIn.flags = 0;
            argsIn.target = hit.transform;
            OnPointerIn(argsIn);
            previousContact = hit.transform;
        }

        if (!bHit)
        {
            previousContact = null;
        }
        if (bHit && hit.distance < 100f)
        {
            dist = hit.distance;
        }

        if (bHit)
        {
            bool hasVRButton = false, hasButton = false;

            VRButton hitVRButton = hit.transform.GetComponent<VRButton>();
            Button hitButton = hit.transform.GetComponent<Button>();

            if (hitVRButton != null)
            {
                hasVRButton = true;
                vrButton = hitVRButton;
            }

            if (hitButton != null)
            {
                hasButton = true;
                aButton = hitButton;
            }

            //controller.triggerPressed;

            if (controller != null && triggerJustPressed)
            {
                pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);

                if (hasButton)
                    aButton.onClick.Invoke(); //.GetComponent<Button>()            

                if (hasVRButton)
                {
                    vrButton.OnClick();
                    Debug.Log("VR BUTTON CLICKED");
                }
            }
            else
            {

                if (hasVRButton)
                    vrButton.OnHover();
                pointer.transform.localScale = new Vector3(thickness, thickness, dist);
            }
        }
        pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);
    }
}
