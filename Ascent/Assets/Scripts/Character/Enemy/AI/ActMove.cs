﻿// Developed by Mana Khamphanpheng 2013

// Dependencies
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Action;

[RAINAction]
public class ActMove : RAINAction 
{
	public ActMove()
    {
		actionName = "ActMove";
    }

    public override void Start(AI ai)
    {
        base.Start(ai);
    }

    public override ActionResult Execute(AI ai)
    {
		ai.Motor.Move();
        return ActionResult.SUCCESS;
    }

    public override void Stop(AI ai)
    {
        base.Stop(ai);
    }	
	
	
}