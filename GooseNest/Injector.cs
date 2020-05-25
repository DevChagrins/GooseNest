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
            bool flag = GameObject.Find("GooseNest");
            if (flag)
            {
                foreach (object obj in GooseNest._UDO)
                {
                    UnityEngine.Object @object = (UnityEngine.Object)obj;
                    UnityEngine.Object.Destroy(@object);
                }
                UnityEngine.Object.Destroy(GameObject.Find("GooseNest").gameObject);
            }
            GameObject gameObject = new GameObject("GooseNest");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<NestMenu>();
            gameObject.AddComponent<GooseNest>();
        }

        public static void Unhook()
        {
            foreach (object obj in GooseNest._UDO)
            {
                UnityEngine.Object @object = (UnityEngine.Object)obj;
                UnityEngine.Object.Destroy(@object);
            }
            UnityEngine.Object.Destroy(GameObject.Find("GooseNest").gameObject);
        }
    }
}
