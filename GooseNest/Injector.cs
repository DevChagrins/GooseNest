using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GooseNest
{
    public class Injector
    {
        public static void Inject()
        {
            bool flag = GameObject.Find("Goosling");
            if (flag)
            {
                foreach (object obj in Goosling._UDO)
                {
                    UnityEngine.Object @object = (UnityEngine.Object)obj;
                    UnityEngine.Object.Destroy(@object);
                }
                UnityEngine.Object.Destroy(GameObject.Find("Goosling").gameObject);
            }
            GameObject gameObject = new GameObject("Goosling");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<Goosling>();
        }

        public static void Unhook()
        {
            foreach (object obj in Goosling._UDO)
            {
                UnityEngine.Object @object = (UnityEngine.Object)obj;
                UnityEngine.Object.Destroy(@object);
            }
            UnityEngine.Object.Destroy(GameObject.Find("Goosling").gameObject);
        }
    }
}
