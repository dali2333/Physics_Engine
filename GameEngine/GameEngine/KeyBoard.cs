﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameEngine
{
    static class KeyBoard
    {
        public static string Pressed_Key; //按下的键值
        public static int KeyBoard_Delay = 10; //按键延迟

        private static bool Is_Control_Obj = false; //键盘是否绑定控制对象
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

        private static void Key_Down()
        {
            ///////////////////////////////////

            if (Is_Control_Obj)
            {
                switch (Pressed_Key)
                {
                    case "0": break;

                    case "UpArrow":
                        //Controlling_Obj.Speed_Y += -5;
                        Controlling_Obj.Move(0,-1); 
                        break;
                    case "DownArrow":
                        Controlling_Obj.Move(0, 1);
                        break;
                    case "LeftArrow":
                        Controlling_Obj.Move(-1, 0);
                        break;
                    case "RightArrow":
                        Controlling_Obj.Move(1, 0);
                        break;

                    default: break;
                }
            }
            else
            {
                switch (Pressed_Key)
                {
                    case "0": break;

                    default: break;
                }
            }

            ///////////////////////////////////
            
        }

        private static void Main_Loop() //主循环
        {
            while (true)
            {
                Pressed_Key = Console.ReadKey(true).Key.ToString();

                Key_Down();

                Thread.Sleep(KeyBoard_Delay);
            }
        }
        public static void Start_Up()
        {
            
            //主循环
            Main_Loop();
        }
    }
}