using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqlManager : MonoBehaviour
{
    public static SqlManager SqlManagerInstance { get; set; }
    // Start is called before the first frame update
    void Awake()
    {
        SqlManagerInstance = this;
        DontDestroyOnLoad(this);
    }
}
