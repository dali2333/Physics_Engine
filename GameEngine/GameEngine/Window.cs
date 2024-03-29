﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameEngine
{
    public static class Window
    {
        #region 关闭快速编辑模式、插入模式
        const int STD_INPUT_HANDLE = -10;
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        const uint ENABLE_INSERT_MODE = 0x0020;
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int hConsoleHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);

        public static void DisbleQuickEditMode()
        {
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            uint mode;
            GetConsoleMode(hStdin, out mode);
            mode &= ~ENABLE_QUICK_EDIT_MODE;//移除快速编辑模式
            mode &= ~ENABLE_INSERT_MODE;      //移除插入模式
            SetConsoleMode(hStdin, mode);
        }
        #endregion
        #region 设置窗体始终最前
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(
               IntPtr hWnd,
               IntPtr hWndInsertAfter,
               int x,
               int y,
               int cx,
               int cy,
               int uFlags);

        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private static void SetFront()
        {
            IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;

            SetWindowPos(hWnd,
                new IntPtr(HWND_TOPMOST),
                0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE);
        }
        #endregion

        //控制台
        public static int Size_X;
        public static int Size_Y;

        private static StringBuilder Window_Buffer; //屏幕输出缓存
        private static char[,] Window_Buffer_0; //原始缓冲区0
        private static char[,] Window_Buffer_1; //原始缓冲区1
        private static bool Switch_Buffer = true; //双重缓存切换

        private static readonly Stopwatch Frame_Watch = new Stopwatch(); //帧率计时器
        public static readonly long Refresh_Dely = 30; //每帧输出时间ms fps = 1000/Refresh_Dely

        //背景
        public static char[,] BackGround; //背景画面 默认都是 '\0'
        public static void Change_BackGround(char[,] b) //修改背景(尺寸不同与背景则贴合左上角,超出窗口自动去除)
        {
            if(b.GetLength(0)== Size_X && b.GetLength(1) == Size_Y)
            {
                BackGround = b;
            }
            else
            {
                BackGround = Graph.Add_Graph(new Tuple<int, int>(Size_X, Size_Y), new Tuple<int, int, char[,]>(0, 0, b));
            }
            
        }
        public static void Add_To_BackGround(char[,] b,int x=0,int y=0) //将图片绘制到背景上 x,y是图片左上角位置
        {
            Graph.Mix_Graph(BackGround, b, x, y);
        }
        public static void Clean_BackGround() //清空背景内容 '\0'
        {
            BackGround = new char[Size_X, Size_Y];
        }

        //游戏体
        public static readonly Dictionary<string,GameOBJ> All_Obj = new Dictionary<string, GameOBJ>(GameOBJ.Max_Objs); //所有物体
        public static readonly Dictionary<string, SpliceOBJ> All_SpliceOBJ = new Dictionary<string, SpliceOBJ>(SpliceOBJ.Max_SpliceObjs); //所有组合体
        public static bool Add_Obj(string name,GameOBJ obj)
        {
            try
            {
                obj.Name = name;
                obj.Judge_Visible();
                All_Obj.Add(name,obj);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool Remove_Obj(string name)
        {
            try
            {
                All_Obj.Remove(name);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool Add_SpliceObj(string name, SpliceOBJ obj)
        {
            try
            {
                obj.Name = name;
                obj.Judge_Visible();
                All_SpliceOBJ.Add(name, obj);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool Remove_SpliceObj(string name)
        {
            try
            {
                All_SpliceOBJ.Remove(name);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //预渲染
        public static bool If_Show_Prerendered_Frame = false; //是否输出预渲染
        public static void Stop_Prerendered() //强行打断预渲染
        {
            If_Show_Prerendered_Frame = false;
        }
        public static void Show_Prerendered_Frame(string path,long refresh_dely=30,string show_while_out="") //输出预渲染画面
        {
            If_Show_Prerendered_Frame = true;

            string[] frames = Graph.Read_Prerendered_Frame(path);
            string[] vv = frames.Last().Split(' ');

            Console.WindowWidth = Convert.ToInt32(vv[0]);
            Console.WindowHeight = Convert.ToInt32(vv[1]) + 1;
            Console.BufferWidth = Convert.ToInt32(vv[0]);
            Console.BufferHeight = Convert.ToInt32(vv[1]) + 1;

            foreach (string f in frames)
            {
                if (!If_Show_Prerendered_Frame)
                {
                    break;
                }

                Frame_Watch.Restart();

                Console.Write(f);

                Console.Write("Press esc to exit " + Frame_Watch.ElapsedMilliseconds.ToString());

                Console.SetCursorPosition(0, 0);

                if (Frame_Watch.ElapsedMilliseconds <= refresh_dely)
                {
                    Thread.Sleep((ushort)(refresh_dely - Frame_Watch.ElapsedMilliseconds));
                }

            }

            Console.Clear();

            Console.WindowWidth = Size_X;
            Console.WindowHeight = Size_Y + 1;
            Console.BufferWidth = Size_X;
            Console.BufferHeight = Size_Y + 1;

            Console.Write(show_while_out);
        }
        public static void Out_Prerendered_Frame(string filename,int n) //输出画面到文件(和物理效果同时打开才有动作)
        {
            Switch_Buffer = true;
            Dictionary<string, char[,]> graph = new Dictionary<string, char[,]>();
            

            for (int ii = 0; ii < n; ii++)
            {
                Frame_Watch.Restart();

                Synthetic_Cache();

                char[,] BB = new char[Size_X, Size_Y];
                Array.Copy(Window_Buffer_1, BB, BB.Length);
                graph.Add("PF" + ii.ToString(), BB);

                Window_Buffer.Clear();
                for (int i = 0; i < Window_Buffer_1.GetLength(1); i++)
                {
                    for (int j = 0; j < Window_Buffer_1.GetLength(0); j++)
                    {
                        Window_Buffer.Append(Window_Buffer_1[j, i]);
                    }
                }
                Console.Write(Window_Buffer);

                Console.Write("Prerendered:" + ii.ToString());
                Console.SetCursorPosition(0, 0);

                if (Frame_Watch.ElapsedMilliseconds <= Refresh_Dely)
                {
                    Thread.Sleep((ushort)(Refresh_Dely - Frame_Watch.ElapsedMilliseconds));
                }
                
            }

            Console.Clear();
            Console.WriteLine("Output Frame...");

            Graph.WriteFile(filename, graph);
            
            Console.WriteLine("Output Completed!");
        }

        //屏幕显示
        private static void Show_Buffer() //缓存输出到画面
        {
            Window_Buffer.Clear();

            if (Switch_Buffer)
            {
                for (int i = 0; i < Window_Buffer_0.GetLength(1); i++)
                {
                    for (int j = 0; j < Window_Buffer_0.GetLength(0); j++)
                    {
                        Window_Buffer.Append(Window_Buffer_0[j,i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Window_Buffer_1.GetLength(1); i++)
                {
                    for (int j = 0; j < Window_Buffer_1.GetLength(0); j++)
                    {
                        Window_Buffer.Append(Window_Buffer_1[j,i]);
                    }
                }
            }

            Console.Write(Window_Buffer);
            Console.SetCursorPosition(0, 0);
        }
        private static void Synthetic_Cache() //绘制画面到缓存( '\0'为透明 )
        {
            if (Switch_Buffer)
            {
                Array.Copy(BackGround, Window_Buffer_1, BackGround.Length);


                Parallel.ForEach(All_Obj.Values, (obj) =>
                {
                    if (obj.Visible)
                    {
                        for (int i = 0; i < obj.Look.GetLength(1); i++)
                        {
                            for (int j = 0; j < obj.Look.GetLength(0); j++)
                            {
                                if(obj.Look[j, i] != '\0')
                                {
                                    Window_Buffer_1[j + (int)obj.X, i + (int)obj.Y] = obj.Look[j, i];
                                }
                                
                            }
                        }
                    }
                    
                });

            }
            else
            {
                Array.Copy(BackGround, Window_Buffer_0, BackGround.Length);

                Parallel.ForEach(All_Obj.Values, (obj) =>
                {
                    if (obj.Visible)
                    {
                        for (int i = 0; i < obj.Look.GetLength(1); i++)
                        {
                            for (int j = 0; j < obj.Look.GetLength(0); j++)
                            {
                                Window_Buffer_0[j + (int)obj.X, i + (int)obj.Y] = obj.Look[j, i];
                            }
                        }
                    }
                });

            }

        }

        private static bool Window_Open = true;//是否启用
        public static void Close() //关闭线程
        {
            Window_Open = false;

            Window_Buffer.Clear();

            Frame_Watch.Reset();
        }
        private static void Main_Loop() //主循环
        {
            while (Window_Open)
            {
                Frame_Watch.Restart();

                //计算并输出画面
                Parallel.Invoke(() => Show_Buffer(), () => Synthetic_Cache());
                Switch_Buffer = !Switch_Buffer;
                
                //控制帧数
                if (Frame_Watch.ElapsedMilliseconds<= Refresh_Dely)
                {
                    Thread.Sleep((ushort)(Refresh_Dely - Frame_Watch.ElapsedMilliseconds));
                }
                else //帧数不足
                {
                    //Console.Write('!');  Console.SetCursorPosition(0, 0);
                }

                //输出帧速率
                //Console.Write(Frame_Watch.ElapsedMilliseconds);
            }
        }

        //开始
        public static void Loading(int size_X,int size_Y)
        {
            Size_X = size_X; Size_Y = size_Y;
            Window_Buffer = new StringBuilder(new string(' ', size_X * size_Y), size_X * size_Y);
            Window_Buffer_0 = new char[size_X, size_Y];
            Window_Buffer_1 = new char[size_X, size_Y];
            BackGround = new char[size_X, size_Y];
            for (int i = 0; i < size_X; i++)
            {
                for (int j = 0; j < size_Y; j++)
                {
                    Window_Buffer_0[i, j] = ' ';
                    Window_Buffer_1[i, j] = ' ';
                    //BackGround[i, j] = ' ';
                }
            }

            Console.Title = "Window";
            Console.WindowWidth = size_X;
            Console.WindowHeight = size_Y+1;
            Console.BufferWidth = size_X;
            Console.BufferHeight = size_Y+1;
            Console.CursorVisible = false;
            DisbleQuickEditMode(); //关闭快速编辑模式
            SetFront(); //设置窗体始终最前

        }
        public static void Start_Up()
        {
            //
            Window_Open = true;

            //主循环
            Main_Loop();
        }
    }
}
