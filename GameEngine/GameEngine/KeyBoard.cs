using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameEngine
{
    static class KeyBoard
    {
        public static int KeyBoard_Delay = 50; //按键延迟
        private static bool Is_Open = true;//是否启动

        public static bool Is_Control_Obj = false; //键盘是否绑定控制对象
        public static GameOBJ Controlling_Obj; //控制中的对象

        public static bool Control_Obj(string name) //绑定对象
        {
            if (Window.All_Obj.ContainsKey(name))
            {
                Is_Control_Obj = true;
                Controlling_Obj = Window.All_Obj[name];
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public static void Stop_Control_Obj() //解除绑定
        {
            Is_Control_Obj = false;
        }

        public delegate void KeyDowm_Events(string key);//按键事件
        public static event KeyDowm_Events KeyDowm;

        public static void Close() //停止运行
        {
            Is_Open = false;
        }

        private static void Main_Loop() //主循环
        {
            while (Is_Open)
            {
                //if (Console.KeyAvailable)
                KeyDowm.Invoke(Console.ReadKey(true).Key.ToString());
                
                Thread.Sleep(KeyBoard_Delay);
            }
        }
        public static void Start_Up()
        {
            Is_Open = true;
            //主循环
            Main_Loop();
        }
    }
}
