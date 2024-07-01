using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Gun : MonoBehaviour
{
    [SerializeField]
    public GameObject bullet;
    [SerializeField]
    public GameObject leftGun, rightGun;
    [SerializeField]
    public GameObject bulletParent;
    public GameObject gControl;
    private bool isLeftGun = false;

    public void Fire()
    {
        int ammo = gameControl.ammoMount;
        gControl.SendMessage("MsgHandle", "Fire");
        if (ammo <= 0) return;
        Vector3 pos,forward;
        if (isLeftGun)
        {
            pos = leftGun.transform.position;
            forward = leftGun.transform.forward;
            isLeftGun = false;
        }
        else
        {
            pos = rightGun.transform.position;
            forward = rightGun.transform.forward;
            isLeftGun = true;
        }
        GameObject newOb = Instantiate(bullet, pos, Quaternion.identity);
        newOb.transform.parent = bulletParent.transform;
        newOb.transform.forward = forward;
    }
}
