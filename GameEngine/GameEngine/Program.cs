using System;
using System.Collections;
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
        static void T()
        {
            //加载图像
            Graph.Loading(@"C:\Users\liush\source\repos\Physics_Engine\Graph\"); //存放数据的文件夹，后面读取和写入使用的地址都是从它开始

            Graph.Add_File_To_Graphs("A.txt"); //读取该文件内的图片并存在Graph.All_Graphs中
            //Graph.WriteFile();//将图片写入到文件中

            //加载音乐
            Music.Loading(@"C:\Users\liush\source\repos\Physics_Engine\Music\");
            //Music.Load_Music("地上bgm");//只加载
            //Music.Play_Music("地上bgm");//加载+播放
            //Music.Play_Music();//播放加载项
            //Music.Stop__Music();//停止播放
            
            //初始化显示器
            Window.Loading(120, 50);
            //Window.Change_BackGround(v); //安全更改背景
            //Window.Add_To_BackGround(v,20,20);//将图片添加到背景中
            

            

            //载入物体 两者碰撞,弹力求和
            float Elastic = 0.4f; float Friction = 0.5f; //直接Change_Obj_Physics带数就行,这里是为了方便

            //用于显示文字的标签
            GameOBJ.Made_Obj("O", 0, 0, 8, 1, Graph.All_Graphs["V"]); GameOBJ.Change_Obj_Physics_S("O", false, false);
            //物体
            GameOBJ.Made_Obj("A", 20, 20, 3, 3, Graph.All_Graphs["l"]); //外观和形状
            GameOBJ.Change_Obj_Physics("A", 1f, 0, 0, 0, 0, Elastic, Friction);//物理性质
            GameOBJ.Change_Obj_Physics_S("A", true, true);//可移动性和可碰撞性开关

            GameOBJ.Made_Obj("B", 25, 23, 3, 3, Graph.All_Graphs["l"]);
            GameOBJ.Change_Obj_Physics("B", 1f, 0, 0, 0, 0, Elastic, Friction);
            GameOBJ.Change_Obj_Physics_S("B", true, true);

            GameOBJ.Made_Obj("C", 60, 21, 3, 3, Graph.All_Graphs["l2"]);
            GameOBJ.Change_Obj_Physics("C", 1f, -20, 0, 0, 0, Elastic, Friction);
            GameOBJ.Change_Obj_Physics_S("C", true, true);

            //Physics.Made_Obj("D", 30, 20, 3, 3, Graph.All_Graphs["l3"]); Physics.Change_Obj_Physics("D", 1f, 0, 0, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("D", true, true);
            //Physics.Made_Obj("E", 50, 30, 3, 3, Graph.All_Graphs["l4"]); Physics.Change_Obj_Physics("E", 1, -10, -10, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("E", true, true);
            //Physics.Made_Obj("F", 80, 17, 3, 3, Graph.All_Graphs["l7"]); Physics.Change_Obj_Physics("F", 1, -10, -10, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("F", true, true);
            //Physics.Made_Obj("G", 10, 17, 3, 2, Graph.All_Graphs["l8"]); Physics.Change_Obj_Physics("G", 1, 30, 0, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("G", true, true);
            //Physics.Made_Obj("H", 90, 35, 2, 1, Graph.All_Graphs["l9"]); Physics.Change_Obj_Physics("H", 1, -5, -5, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("H", true, true);
            //Physics.Made_Obj("I", 30, 30, 4, 4, Graph.All_Graphs["l10"]); Physics.Change_Obj_Physics("I", 5f, 10, -15, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("I", true, true);

            GameOBJ.Made_Obj("FU", 10, 10, 100, 2, Graph.All_Graphs["l5"]); GameOBJ.Change_Obj_Physics("FU", 1, 0, 0, 0, 0, 0.5f, 0); GameOBJ.Change_Obj_Physics_S("FU", false,true);
            GameOBJ.Made_Obj("FD", 10, 40, 100, 2, Graph.All_Graphs["l5"]); GameOBJ.Change_Obj_Physics("FD", 1, 0, 0, 0, 0, 0.5f, 0); GameOBJ.Change_Obj_Physics_S("FD", false, true);
            GameOBJ.Made_Obj("FL", 7, 10, 3, 32, Graph.All_Graphs["l6"]); GameOBJ.Change_Obj_Physics("FL", 1, 0, 0, 0, 0, 0.5f, 0); GameOBJ.Change_Obj_Physics_S("FL", false, true);
            GameOBJ.Made_Obj("FR", 110, 10, 3, 32, Graph.All_Graphs["l6"]); GameOBJ.Change_Obj_Physics("FR", 1, 0, 0, 0, 0, 0.5f, 0); GameOBJ.Change_Obj_Physics_S("FR", false, true);

            //组合体 创建完物体后可以绑定为组合体,物理性质不修改(不能调用Change_Obj_Physics)将保留原来的
            //绑定为组合体后,单独修改某个内部物体的速度、受力等属性是不生效的(重量需调用Change_Weight_When_Spliced)
            //SpliceOBJ类的全部属性方法都和GameOBJ一一对应,直接调用可整体修改 例:Window.All_SpliceOBJ["HH"].Speed_X = 10;
            SpliceOBJ.Made_Obj("HH", "A", "B");//名称,后接绑定物体的名称(最多添加SpliceOBJ.Max_Splice个)
            SpliceOBJ.Change_Obj_Physics("HH", 1f, 0, 0, 0, 0, Elastic, Friction);//物理性质(所有内部物体同时修改)
            SpliceOBJ.Change_Obj_Physics_S("HH",true,true);//可移动性和可碰撞性开关(所有内部物体同时修改)

            //Window.All_SpliceOBJ["HH"].Stop_Splice("B"); //绑定
            //Window.All_SpliceOBJ["HH"].Start_Splice("C");//解绑

            //设置物理环境
            Physics.Global_GY = 40; //全局重力
            Physics.Global_GX = 0;
            Physics.Global_Max_Speed = 50; //全局最大速度

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
            //Graph.Load_Animation("An1", "An1.txt");
            //Graph.Load_Animation("An2", "An2.txt");
            //动画绑定物体的贴图(均为名称,先构建物体再绑定.动画与物体1对1,不可重复)
            //Graph.Binding_Animation_To_Obj("An1", "A");
            //Graph.Binding_Animation_To_Obj("An2", "B");
            //Graph.Delete_Binding_Animation("B"); //解除绑定
            Graph.Start_Animation();//启动动画系统
            //Graph.Close_Animation();//关闭动画系统

            //预渲染画面并保存到文件 Out_Prerendered_Frame(文件名,总渲染帧数) 与Physics.Start_Up()同时运行才有物理效果
            //Parallel.Invoke(() => Window.Out_Prerendered_Frame("Prerendered_Frame1.txt", 200), () => Physics.Start_Up(), () => KeyBoard.Start_Up());

            //从文件输出预渲染画面 Show_Prerendered_Frame(文件名,帧时间) PS:帧时间比渲染时的Refresh_Dely大画面将变慢,反之亦然
            //Window.Show_Prerendered_Frame("Prerendered_Frame1.txt",10);
            //Window.Stop_Prerendered() //输出预渲染中调用可以强行停止

            //添加闹钟(ms) PS:闹钟时间如果小于 Physics.Main_Loop_Sleep*2 可能会不触发
            //Physics.Add_Alarm(5000);//5秒钟触发
            //Physics.Add_Alarm(2000);
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
            //Parallel.Invoke(() => Window.Show_Prerendered_Frame("Prerendered_Frame1.txt", 10, "Press esc to exit..."), () => KeyBoard.Start_Up());

            //播放背景音乐
            //Music.Play_Music("地上bgm");

            //启动 -> 显示器 物理效果 键盘输入
            Parallel.Invoke(() => Window.Start_Up(), () => Physics.Start_Up(), () => KeyBoard.Start_Up());

        }

        static void G()
        {
            //初始化显示器
            Window.Loading(126, 31);

            //加载图像
            Graph.Loading(@"C:\Users\liush\source\repos\Physics_Engine\Graph\");

            Graph.Add_File_To_Graphs("封面.txt");

            Graph.Show_Look(Graph.All_Graphs["Feng0"]);

            Thread.Sleep(500); //正式启动前最好暂停一小会等待数据加载

            //欢迎画面
            Window.Show_Prerendered_Frame("开始.txt", 25);
            Window.Show_Prerendered_Frame("开始.txt", 25);

            Graph.Add_File_To_Graphs("马里奥.txt");
            Graph.Add_File_To_Graphs("环境组件.txt");

            //加载音乐
            Music.Loading(@"C:\Users\liush\source\repos\Physics_Engine\Music\");

            //载入物体
            Window.Loading(130, 50);

            GameOBJ.Made_Obj("O", 0, 0, 8, 1, Graph.All_Graphs["WORD"]); GameOBJ.Change_Obj_Physics_S("O", false, false);

            GameOBJ.Made_Obj("Mario", 20, 40, 8, 6, Graph.All_Graphs["StandSmall"]);
            GameOBJ.Change_Obj_Physics("Mario", 1f, 0, 0, 0, 0, 0.01f, 0.8f);
            GameOBJ.Change_Obj_Physics_S("Mario", true, true);

            GameOBJ.Made_Obj("FD", 10, 45, 100, 3, Graph.All_Graphs["Heng"]);
            GameOBJ.Change_Obj_Physics("FD", 1f, 0, 0, 0, 0, 0.01f, 1);
            GameOBJ.Change_Obj_Physics_S("FD", false, true);

            GameOBJ.Made_Obj("FR", 110, 13, 3, 32, Graph.All_Graphs["Shu"]);
            GameOBJ.Change_Obj_Physics("FR", 1f, 0, 0, 0, 0, 1, 1);
            GameOBJ.Change_Obj_Physics_S("FR", false, true);

            //设置物理环境
            Physics.Global_GY = 50; //全局重力
            Physics.Global_GX = 0;
            Physics.Global_Max_Speed = 40f; //全局最大速度

            //键盘绑定物体
            KeyBoard.Control_Obj("Mario"); //参数是被控制物体的名称

            //注册事件
            Physics.Touched += new Physics.OBJTouch_Events(Get_Touched); //碰撞事件
            Physics.Out_Of_Bounds += new Physics.OBJOut_Events(Out_Border); //物体超出屏幕事件
            Physics.Alarm_Clock += new Physics.Alarm_Events(Alarm); //闹钟响应事件
            KeyBoard.KeyDowm += new KeyBoard.KeyDowm_Events(Key_Down); //按键响应事件

            //物理演算主循环的特殊效果延迟ms PS:闹钟、动画...
            Physics.Main_Loop_Sleep = 10;

            //加载动画(一个文件一个动画)
            //Graph.Load_Animation("An1", "封面.txt"); 
            //Graph.Binding_Animation_To_Obj("An1", "A");
            Graph.Start_Animation();//启动动画系统

            //预渲染画面并保存到文件 Out_Prerendered_Frame(文件名,总渲染帧数) 与Physics.Start_Up()同时运行才有物理效果
            //Parallel.Invoke(() => Window.Out_Prerendered_Frame("123.txt", 30), () => Physics.Start_Up(), () => KeyBoard.Start_Up());


            //播放背景音乐
            Music.Play_Music("地上bgm");

            //启动 -> 显示器 物理效果 键盘输入
            Parallel.Invoke(() => Window.Start_Up(), () => Physics.Start_Up(), () => KeyBoard.Start_Up());

        }

        //所有事件(高频调用，不要阻塞)
        static void Get_Touched(GameOBJ rc1, GameOBJ rc2) //碰撞事件
        {
            if(rc1.Name== "Mario" && rc2.Name == "FD" && !Is_Land)
            {
                Is_Land = true;
            }

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
                        if(Is_Land && KeyBoard.Controlling_Obj.Speed_Y > -Physics.Global_Max_Speed)
                        {
                            Is_Land = false;

                            KeyBoard.Controlling_Obj.Speed_Y += -20;

                        }
                        
                        break;
                    case "DownArrow":
                        if (KeyBoard.Controlling_Obj.Speed_Y < Physics.Global_Max_Speed)
                        {
                            if (Is_Land)
                            {
                                //这里应该蹲下
                            }
                            else
                            {
                                KeyBoard.Controlling_Obj.Speed_Y += 10;
                            }
                        }
                            
                        //KeyBoard.Controlling_Obj.Move(0, 1);
                        break;
                    case "LeftArrow":
                        if (KeyBoard.Controlling_Obj.Speed_X > 0)
                        {
                            KeyBoard.Controlling_Obj.Speed_X = -20;
                        }
                        else if (KeyBoard.Controlling_Obj.Speed_X > -Physics.Global_Max_Speed)
                        {
                            KeyBoard.Controlling_Obj.Speed_X -= 10;
                        }
                            
                        //KeyBoard.Controlling_Obj.Move(-1, 0);
                        break;
                    case "RightArrow":
                        if (KeyBoard.Controlling_Obj.Speed_X < 0)
                        {
                            KeyBoard.Controlling_Obj.Speed_X = 20;
                        }
                        else if (KeyBoard.Controlling_Obj.Speed_X < Physics.Global_Max_Speed)
                        {
                            KeyBoard.Controlling_Obj.Speed_X += 10;
                        }
                            
                        //KeyBoard.Controlling_Obj.Move(1, 0);
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

        static bool Is_Land = false;

        static void Main(string[] args)
        {
            T();

            //Console.ReadKey();
        }
    }
}
