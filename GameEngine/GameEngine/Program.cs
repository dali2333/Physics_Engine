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
        

        static void Main(string[] args)
        {
            //加载图像
            Graph.Loading(@"C:\Users\liush\source\repos\Physics_Engine\Graph\"); //存放数据的文件夹，后面读取和写入使用的地址都是从它开始
            
            Graph.Add_File_To_Graphs("A.txt"); //读取该文件内的图片并存在Graph.All_Graphs中

            //初始化显示器
            Window.Loading(120, 50);

            //载入物体
            float Elastic = 0.8f; float Friction = 0.5f; //直接Change_Obj_Physics带数就行,这里是为了方便

            //用于显示文字的标签
            Physics.Made_Obj("O", 0, 0, 8, 1, Graph.All_Graphs["V"]); Physics.Change_Obj_Physics_S("O", false, false);

            Physics.Made_Obj("A", 20, 34, 3, 3, Graph.All_Graphs["l"]); //外观和形状
            Physics.Change_Obj_Physics("A", 1f, 40, 0, 0,0, Elastic, Friction);//物理性质
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

            //注册事件
            Physics.Touched += new Physics.OBJTouch_Events(Get_Touched); //碰撞事件
            Physics.Out_Of_Bounds += new Physics.OBJOut_Events(Out_Border); //物体超出屏幕事件
            
            //键盘绑定物体
            KeyBoard.Control_Obj("A"); //参数是被控制物体的名称

            //加载动画(一个文件一个动画)
            Graph.Load_Animation("An1", "An1.txt"); 
            Graph.Load_Animation("An2", "An2.txt");
            //动画绑定物体的贴图(均为名称,先构建物体再绑定.动画与物体1对1,不可重复)
            Graph.Binding_Animation_To_Obj("An1", "A");
            Graph.Binding_Animation_To_Obj("An2", "B");
            //启动动画系统
            Graph.Enable_Animation = true;

            //启动 -> 显示器 物理效果 键盘输入
            Parallel.Invoke(() => Window.Start_Up(), () => Physics.Start_Up(), () => KeyBoard.Start_Up());

        }
    }
}
