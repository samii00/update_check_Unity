﻿using System;
using UnityEngine;


//attach this script to a gameObject
public class StartCheck : MonoBehaviour
{
	public void Start()
	{
        var updatCheker = UpdateAppCheck.Instacne;
		updatCheker.updateCheck();
	}

}
