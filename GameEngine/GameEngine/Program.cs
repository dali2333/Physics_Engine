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
        //形状逆时针转90度输出
        public static char[,] oo = { { ' ' },{ ' ' }, { ' ' },{ ' ' }, { ' ' } } ;

        public static char[,] l =  { { '#', '#', '#' }, { '#', ' ', '#' },{ '#', '#', '#' } };
        public static char[,] l2 = { { '*', '*', '*' }, { '*', ' ', '*' }, { '*', '*', '*' } };
        public static char[,] l3 = { { '@', '@', '@' }, { '@', ' ', '@' }, { '@', '@', '@' } };
        public static char[,] l4 = { { ' ', '#', ' ' }, { '#', '#', '#' }, { ' ', '#', ' ' } };
        public static char[,] l5 = new char[100,2];
        public static char[,] l6 = new char[3, 32];

        public static char[,] l7 = { { '#', '#', '#', '#' }, { '#', '#', '#', '#' } };
        public static char[,] l8 = { { '#', '#' },{ '#', '#' }, { '#', '#' },{ '#', '#' } };
        public static char[,] l9 = { { '$' }, { '$' } };
        public static char[,] l10 = { { '#', '#', '#', '#' }, { '#', '#', '#', '#' }, { '#', '#', '#', '#' }, { '#', '#', '#', '#' } };

        static void Pretreatment()
        {
            for (int i = 0; i < 100; i++)
            {
                l5[i,0] = '&';
                l5[i,1] = '&';
            }
            for (int i = 0; i < 32; i++)
            {
                l6[0 ,i] = '!';
                l6[1 ,i] = '!';
                l6[2, i] = '!';
            }
        }

        //所有事件(高频调用，不要阻塞)
        static void Get_Touched(GameOBJ rc1, GameOBJ rc2) //碰撞事件
        {
            
        }
        static void Out_Border(GameOBJ rc1) //物体超出边界事件
        {
            
        }
        

        static void Main(string[] args)
        {
            //数据预处理
            Pretreatment();

            //初始化显示器
            Window.Loading(120, 50);

            //载入物体
            float Elastic = 0.8f; float Friction = 0.5f;

            Physics.Made_Obj("A", 20, 34, 3, 3, l);  Physics.Change_Obj_Physics("A", 1f, 40, 0, 0,0, Elastic, Friction); Physics.Change_Obj_Physics_S("A", true,true);
            Physics.Made_Obj("B", 40, 20, 3, 3, l);  Physics.Change_Obj_Physics("B", 1f, 10, -10, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("B", true, true);
            Physics.Made_Obj("C", 50, 20, 3, 3, l2); Physics.Change_Obj_Physics("C", 1f, 20, 0, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("C", true, true);
            Physics.Made_Obj("D", 30, 20, 3, 3, l3); Physics.Change_Obj_Physics("D", 1f, 0, 0, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("D", true, true);
            Physics.Made_Obj("E", 50, 30, 3, 3, l4); Physics.Change_Obj_Physics("E", 1, -10, -10, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("E", true, true);
            Physics.Made_Obj("F", 80, 17, 3, 3, l7); Physics.Change_Obj_Physics("F", 1, -10, -10, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("F", true, true);
            Physics.Made_Obj("G", 10, 17, 3, 2, l8); Physics.Change_Obj_Physics("G", 1, 30, 0, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("G", true, true);
            Physics.Made_Obj("H", 80, 32, 2, 1, l9); Physics.Change_Obj_Physics("H", 1, -10, 0, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("H", true, true);
            Physics.Made_Obj("I", 30, 30, 4, 4, l10); Physics.Change_Obj_Physics("I", 5f, 10, -15, 0, 0, Elastic, Friction); Physics.Change_Obj_Physics_S("I", true, true);

            Physics.Made_Obj("FU", 10, 10, 100, 2, l5); Physics.Change_Obj_Physics("FU", 1, 0, 0, 0, 0); Physics.Change_Obj_Physics_S("FU", false,true);
            Physics.Made_Obj("FD", 10, 40, 100, 2, l5); Physics.Change_Obj_Physics("FD", 1, 0, 0, 0, 0); Physics.Change_Obj_Physics_S("FD", false, true);
            Physics.Made_Obj("FL", 7, 10, 3, 32, l6);  Physics.Change_Obj_Physics("FL", 1, 0, 0, 0, 0); Physics.Change_Obj_Physics_S("FL", false, true);
            Physics.Made_Obj("FR", 110, 10, 3, 32, l6);Physics.Change_Obj_Physics("FR", 1, 0, 0, 0, 0); Physics.Change_Obj_Physics_S("FR", false, true);

            //设置物理环境
            Physics.Global_GY = 40;
            Physics.Global_Max_Speed = 100f;

            //注册事件
            Physics.Touched += new Physics.OBJTouch_Events(Get_Touched);
            Physics.Out_Of_Bounds += new Physics.OBJOut_Events(Out_Border);
            
            //键盘绑定物体
            KeyBoard.Control_Obj("A");

            //启动 -> 显示器 物理效果 键盘输入
            Parallel.Invoke(() => Window.Start_Up(), () => Physics.Start_Up(), () => KeyBoard.Start_Up());
            
        }
    }
}
