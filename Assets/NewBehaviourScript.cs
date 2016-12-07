using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Squirrel;
using SQ = Squirrel.SquirrelDLL;
using System;

public class NewBehaviourScript : MonoBehaviour
{
    [MonoPInvokeCallbackAttribute(typeof(Squirrel.SQFUNCTION))]
    public static int log(IntPtr v)
    {
        var argc = SQ.sq_gettop(v);
        var sb = new StringBuilder();
        for (var i = 2; i <= argc; ++i)
        {
            if (SQ.SQ_SUCCEEDED(SQ.sq_tostring(v, i)))
            {
                string str;
                if (SQ.SQ_SUCCEEDED(SQ.sq_getstring(v, -1, out str)))
                {
                    sb.Append(str);
                    sb.Append(' ');
                }
                SQ.sq_poptop(v);
            }
        }
        Debug.Log(sb.ToString());
        return 0;
    }

    // Use this for initialization
    void Start()
    {
        var vm = SQ.sq_open(1024);

        SQ.sq_pushroottable(vm);
        SQ.sq_pushstring(vm, "log");
        SQ.sq_newclosure(vm, log, 0);
        SQ.sq_newslot(vm, -3, SQ.SQTrue);
        SQ.sq_poptop(vm);

        var script = @"
//////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////
class BaseVector {
    constructor(...)
    {
        if(vargv.len() >= 3) {
            x = vargv[0];
            y = vargv[1];
            z = vargv[2];
        }
    }


    x = 0;
    y = 0;
    z = 0;
}

class Vector3 extends BaseVector {
    function _add(other)
    {
        if(other instanceof this.getclass())
            return ::Vector3(x+other.x,y+other.y,z+other.z);
        else
            throw ""wrong parameter"";
    }
    function Print()
    {
        ::log(x,y,z);
    }
}

local v0 = Vector3(1,2,3)
local v1 = Vector3(11,12,13)
local v2 = v0 + v1;
v2.Print();

FakeNamespace <- {
    Utils = {}
}

class FakeNamespace.Utils.SuperClass {
    constructor()
    {
        ::log(""FakeNamespace.Utils.SuperClass"")
    }
}

local testy = FakeNamespace.Utils.SuperClass();

for (local i = 0; i < 3; ++i) {
    ::log(i, ""测试123哈哈"")
}
";

        if (SQ.SQ_SUCCEEDED(SQ.sq_compilebuffer(vm, script, "(buffer)", SQ.SQFalse)))
        {
            SQ.sq_pushroottable(vm);
            if (SQ.SQ_SUCCEEDED(SQ.sq_call(vm, 1, SQ.SQFalse, SQ.SQFalse)))
            {
                Debug.Log("pass");
            }
            else
            {
                Debug.LogError("call error");
            }
            SQ.sq_poptop(vm);
        }
        else
        {
            Debug.LogError("compile error");
        }

        SQ.sq_close(vm);
    }
}
