using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Windows.Input;

namespace GameEngine
{
    class Program
    {
        //所有事件(高频调用，不要阻塞)
        static void Get_Touched(GameOBJ rc1, GameOBJ rc2) //碰撞事件
        {
            
        }
        static void Out_Border(GameOBJ rc1) //物体超出边界事件
        {
            
        }
        static void Alarm(long t,long real_t) //闹钟事件 t是设置的时间,real_t是实际触发的时间
        {

        }

        static void Key_Down(string Pressed_Key) //键盘按下事件 Pressed_Key为键值
        {
            
            if (KeyBoard.Is_Control_Obj) //绑定了物体
            {
                switch (Pressed_Key)
                {
                    case "0": break;

                    case "UpArrow":
                        //Controlling_Obj.Speed_Y += -5;
                        KeyBoard.Controlling_Obj.Move(0, -1);
                        break;
                    case "DownArrow":
                        KeyBoard.Controlling_Obj.Move(0, 1);
                        break;
                    case "LeftArrow":
                        KeyBoard.Controlling_Obj.Move(-1, 0);
                        break;
                    case "RightArrow":
                        KeyBoard.Controlling_Obj.Move(1, 0);
                        break;

                    case "S":
                        Physics.Restart_Physics();
                        break;
                    case "D":
                        Physics.Stop_Physics();
                        break;
                    case "F":
                        Graph.Start_Animation();
                        break;
                    case "G":
                        Graph.Close_Animation();
                        break;
                    case "A":
                        Physics.Restart_Game_Time();
                        break;
                    case "Escape":
                        if (Window.If_Show_Prerendered_Frame)
                        {
                            //结束预渲染
                            Window.Stop_Prerendered();
                            KeyBoard.Close();
                        }
                        else
                        {
                            //结束全部进程
                            Physics.Close();
                            Window.Close();
                            KeyBoard.Close();
                        }
                        break;

                    default: break;
                }
            }
            else //没绑定物体
            {
                switch (Pressed_Key)
                {
                    case "0": break;

                    default: break;
                }
            }

        }


        static void Main(string[] args)
        {
            //加载图像
            Graph.Loading(@"C:\Users\liush\source\repos\Physics_Engine\Graph\"); //存放数据的文件夹，后面读取和写入使用的地址都是从它开始

            Graph.Add_File_To_Graphs("A.txt"); //读取该文件内的图片并存在Graph.All_Graphs中
            //Graph.WriteFile();//将图片写入到文件中

            //初始化显示器
            Window.Loading(120, 50);

            //背景图片 PS:没有内容需填充空格,不修改自动空格 不能透明
            //Window.BackGround;

            //载入物体
            float Elastic = 0.8f; float Friction = 0.5f; //直接Change_Obj_Physics带数就行,这里是为了方便

            //用于显示文字的标签
            Physics.Made_Obj("O", 0, 0, 8, 1, Graph.All_Graphs["V"]); Physics.Change_Obj_Physics_S("O", false, false);
            //物体
            Physics.Made_Obj("A", 20, 34, 3, 3, Graph.All_Graphs["l"]); //外观和形状
            Physics.Change_Obj_Physics("A", 1f, 10, 0, 0,0, Elastic, Friction);//物理性质
            Physics.Change_Obj_Physics_S("A", true,true);//可移动性和可碰撞性开关

            Physics.Made_Obj("B", 40, 20, 3, 3, Graph.All_Graphs["l"]);  Physics.Change_Obj_Physics("B", 1f, 10, -10, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("B", true, true);
            Physics.Made_Obj("C", 50, 20, 3, 3, Graph.All_Graphs["l2"]); Physics.Change_Obj_Physics("C", 1f, 20, 0, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("C", true, true);
            Physics.Made_Obj("D", 30, 20, 3, 3, Graph.All_Graphs["l3"]); Physics.Change_Obj_Physics("D", 1f, 0, 0, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("D", true, true);
            Physics.Made_Obj("E", 50, 30, 3, 3, Graph.All_Graphs["l4"]); Physics.Change_Obj_Physics("E", 1, -10, -10, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("E", true, true);
            Physics.Made_Obj("F", 80, 17, 3, 3, Graph.All_Graphs["l7"]); Physics.Change_Obj_Physics("F", 1, -10, -10, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("F", true, true);
            Physics.Made_Obj("G", 10, 17, 3, 2, Graph.All_Graphs["l8"]); Physics.Change_Obj_Physics("G", 1, 30, 0, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("G", true, true);
            Physics.Made_Obj("H", 90, 35, 2, 1, Graph.All_Graphs["l9"]); Physics.Change_Obj_Physics("H", 1, -5, -5, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("H", true, true);
            Physics.Made_Obj("I", 30, 30, 4, 4, Graph.All_Graphs["l10"]); Physics.Change_Obj_Physics("I", 5f, 10, -15, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("I", true, true);

            Physics.Made_Obj("FU", 10, 10, 100, 2, Graph.All_Graphs["l5"]); Physics.Change_Obj_Physics("FU", 1, 0, 0, 0, 0); Physics.Change_Obj_Physics_S("FU", false,true);
            Physics.Made_Obj("FD", 10, 40, 100, 2, Graph.All_Graphs["l5"]); Physics.Change_Obj_Physics("FD", 1, 0, 0, 0, 0); Physics.Change_Obj_Physics_S("FD", false, true);
            Physics.Made_Obj("FL", 7, 10, 3, 32, Graph.All_Graphs["l6"]);  Physics.Change_Obj_Physics("FL", 1, 0, 0, 0, 0); Physics.Change_Obj_Physics_S("FL", false, true);
            Physics.Made_Obj("FR", 110, 10, 3, 32, Graph.All_Graphs["l6"]);Physics.Change_Obj_Physics("FR", 1, 0, 0, 0, 0); Physics.Change_Obj_Physics_S("FR", false, true);

            //设置物理环境
            Physics.Global_GY = 40; //全局重力
            Physics.Global_GX = 0;
            Physics.Global_Max_Speed = 100f; //全局最大速度

            //键盘绑定物体
            KeyBoard.Control_Obj("A"); //参数是被控制物体的名称
            //KeyBoard.Stop_Control_Obj();//解绑物体
            //KeyBoard.KeyBoard_Delay=10;//按键后延迟ms
            
            //注册事件
            Physics.Touched += new Physics.OBJTouch_Events(Get_Touched); //碰撞事件
            Physics.Out_Of_Bounds += new Physics.OBJOut_Events(Out_Border); //物体超出屏幕事件
            Physics.Alarm_Clock += new Physics.Alarm_Events(Alarm); //闹钟响应事件

            KeyBoard.KeyDowm += new KeyBoard.KeyDowm_Events(Key_Down); //按键响应事件


            //物理演算主循环的特殊效果延迟ms PS:闹钟、动画...
            //Physics.Main_Loop_Sleep = 100;

            //加载动画(一个文件一个动画)
            Graph.Load_Animation("An1", "An1.txt"); 
            Graph.Load_Animation("An2", "An2.txt");
            //动画绑定物体的贴图(均为名称,先构建物体再绑定.动画与物体1对1,不可重复)
            Graph.Binding_Animation_To_Obj("An1", "A");
            Graph.Binding_Animation_To_Obj("An2", "B");
            //Graph.Delete_Binding_Animation("B"); //解除绑定
            Graph.Start_Animation();//启动动画系统
            //Graph.Close_Animation();//关闭动画系统

            //预渲染画面并保存到文件 Out_Prerendered_Frame(文件名,总渲染帧数) 与Physics.Start_Up()同时运行才有物理效果
            //Parallel.Invoke(() => Window.Out_Prerendered_Frame("Prerendered_Frame1.txt", 200), () => Physics.Start_Up(), () => KeyBoard.Start_Up());

            //从文件输出预渲染画面 Show_Prerendered_Frame(文件名,帧时间) PS:帧时间比渲染时的Refresh_Dely大画面将变慢,反之亦然
            //Window.Show_Prerendered_Frame("Prerendered_Frame1.txt",10);
            //Window.Stop_Prerendered() //输出预渲染中调用可以强行停止

            //添加闹钟(ms) PS:闹钟时间如果小于 Physics.Main_Loop_Sleep*2 可能会不触发
            Physics.Add_Alarm(5000);//5秒钟触发
            Physics.Add_Alarm(2000);
            //Physics.Delete_Alarm(1000);//删除闹钟
            //Physics.Clean_Alarm();//清空闹钟
            //Physics.Close_Alarm();//关闭闹钟

            //Physics.Stop_Physics();//暂停物理演算
            //Physics.Restart_Physics() //重新开始物理演算
            //Physics.Get_Game_Time() //获取游戏运行时间(ms)
            //Physics.Restart_Game_Time() //重新计算游戏运行时间

            //Window.Start_Up();//启动Window进程
            //Physics.Start_Up();//启动Physics进程
            //KeyBoard.Start_Up();//启动键盘进程
            //Window.Close();//结束Window进程 PS:结束后需等待一会才能再次启动
            //Physics.Close();//结束Physics进程
            //KeyBoard..Close();//结束键盘进程


            //Thread.Sleep(1000); //正式启动前最好暂停一小会等待数据加载

            //演示一段预渲染画面,按Esc跳过
            Parallel.Invoke(() => Window.Show_Prerendered_Frame("Prerendered_Frame1.txt", 10, "Press esc to exit..."), () => KeyBoard.Start_Up());

            //启动 -> 显示器 物理效果 键盘输入
            Parallel.Invoke(() => Window.Start_Up(), () => Physics.Start_Up(), () => KeyBoard.Start_Up());

        }
    }
}
