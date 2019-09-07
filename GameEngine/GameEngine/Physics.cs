using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameEngine
{

    static class Physics
    {
        //参数
        public static float Global_GX = 0f;//全局重力
        public static float Global_GY = 0f;
        public static float Global_Max_Speed = 100;//全局最大速度

        public static int Main_Loop_Sleep = 100;//主循环延迟ms

        private static readonly float Timer_Interval_Time = 0.05f; //物理定时器间隔
        private static float Physics_Interval_Time = Timer_Interval_Time; //两次计算的间隔时间s(动态)
        private static bool Is_Running = true; //时钟是否开启

        private static readonly Stopwatch Game_Time_Watch = new Stopwatch();//游戏时间计时器
        private static readonly Stopwatch Physics_Time_Watch = new Stopwatch(); //演算时长计时器
        private static Timer Physics_Timer; //演算定时器

        private static readonly List<long> Alarm_Contents = new List<long>();//闹钟触发时间
        private static bool Alarm_Open = true; //是否开启闹钟

        //应用方法
        public static void Stop_Physics() //暂停物理演算(暂停时钟)
        {
            if (Is_Running)
            {
                Physics_Timer.Change(Timeout.Infinite, Timeout.Infinite);
                Physics_Time_Watch.Stop();

                Game_Time_Watch.Stop();

                Is_Running = false;
            }

        }
        public static void Restart_Physics() //重新开始物理演算(先停止才有效)
        {
            if (!Is_Running)
            {
                Physics_Timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(Timer_Interval_Time));
                Physics_Time_Watch.Start();

                Game_Time_Watch.Start();

                Is_Running = true;
            }

        }
        public static long Get_Game_Time() //获取游戏运行时间(ms)
        {
            return Game_Time_Watch.ElapsedMilliseconds;
        }
        public static void Restart_Game_Time() //重新计算游戏运行时间
        {
            if (Is_Running)
            {
                Game_Time_Watch.Restart();
            }
            else
            {
                Game_Time_Watch.Reset();
            }

        }
        public static void Add_Alarm(long t) //添加闹钟
        {
            if (!Alarm_Contents.Contains(t))
            {
                Alarm_Contents.Add(t);

                Alarm_Contents.Sort();
            }

        }
        public static void Delete_Alarm(long t) //删除闹钟
        {
            if (Alarm_Contents.Contains(t))
            {
                Alarm_Contents.Remove(t);
            }

        }
        public static void Clean_Alarm() //清空闹钟
        {
            Alarm_Contents.Clear();
        }
        public static void Close_Alarm() //关闭闹钟
        {
            Alarm_Open = false;
        }

        //物理事件
        public delegate void OBJTouch_Events(GameOBJ rc1, GameOBJ rc2o);//碰撞
        public static event OBJTouch_Events Touched;
        public delegate void OBJOut_Events(GameOBJ rc1);//出界
        public static event OBJOut_Events Out_Of_Bounds;
        public delegate void Alarm_Events(long t, long rt);//闹钟触发
        public static event Alarm_Events Alarm_Clock;

        //闹钟
        private static long Game_Time_Now = 0;
        private static void Alarm_On() //闹钟检测 闹钟时间如果小于 Physics.Main_Loop_Sleep*2 可能会不触发
        {
            if (Alarm_Open)
            {
                Game_Time_Now = Game_Time_Watch.ElapsedMilliseconds;

                List<long> outs = new List<long>();
                foreach (long t in Alarm_Contents)
                {
                    if (Game_Time_Now >= t)
                    {
                        Alarm_Clock.Invoke(t, Game_Time_Now);

                        outs.Add(t);
                    }
                    else
                    {
                        break;
                    }

                }

                foreach (long o in outs)
                {
                    Alarm_Contents.Remove(o);
                }

            }

        }

        //物理输出
        private static void Show()
        {

            Graph.Add_String(Graph.All_Graphs["V"], "AAA");
        }

        private static void Equal_Interval_Callback(object state) //等时间间隔计算
        {
            //物理算法
            Parallel.ForEach(Window.All_Obj.Values, (rc1) =>
            {
                //组合体
                if (rc1.Is_Spliced)
                {
                    SpliceOBJ so = Window.All_SpliceOBJ[rc1.Splice_Name];

                }
                //非组合体
                else
                {
                    if (rc1.Movable)
                    {
                        //位移-速度计算
                        rc1.NX = rc1.X + rc1.Speed_X * Physics_Interval_Time;
                        rc1.NY = rc1.Y + rc1.Speed_Y * Physics_Interval_Time;

                        rc1.Speed_X += (rc1.F_X / rc1.Weight + Global_GX) * Physics_Interval_Time;
                        rc1.Speed_Y += (rc1.F_Y / rc1.Weight + Global_GY) * Physics_Interval_Time;

                        //碰撞-摩擦计算
                        if (rc1.Collisible)
                        {
                            //遍历其余物体
                            Parallel.ForEach(Window.All_Obj.Values, (rc2) =>
                            {
                                //找出符合标准的物体
                                if (rc2.Collisible && rc1.Name != rc2.Name)
                                {
                                    //判断是否碰撞 Graph.Add_String(Graph.All_Graphs["V"], "AAA");
                                    if (rc1.Speed_X > 0)
                                    {
                                        if (rc1.Speed_Y > 0)
                                        {
                                            if (rc1.NX + rc1.SX > rc2.X &&
                                                rc2.X + rc2.SX > rc1.X &&
                                                rc1.NY + rc1.SY > rc2.Y &&
                                                rc2.Y + rc2.SY > rc1.Y)
                                            {
                                                if(rc2.X < rc1.X && rc1.X < rc2.PX && rc1.Y < rc2.Y && rc2.Y < rc1.NPY)
                                                {
                                                    rc1.Speed_Y *= -(rc1.Elastic + rc2.Elastic);

                                                    rc1.NY = rc2.Y - rc1.SY;
                                                }

                                                if (rc1.PX < rc2.X && rc2.X < rc1.NPX && rc2.Y < rc1.Y && rc1.Y < rc2.PY)
                                                {
                                                    rc1.Speed_X *= -(rc1.Elastic + rc2.Elastic);

                                                    rc1.NX = rc2.X - rc1.SX;
                                                }

                                            }
                                        }
                                        else if (rc1.Speed_Y < 0)
                                        {
                                            if (rc1.NX + rc1.SX > rc2.X &&
                                                rc2.X + rc2.SX > rc1.X &&
                                                rc1.Y + rc1.SY > rc2.Y &&
                                                rc2.Y + rc2.SY > rc1.NY)
                                            {
                                                if (rc2.X < rc1.X && rc1.X < rc2.PX && rc1.NY < rc2.PY && rc2.PY < rc1.Y)
                                                {
                                                    rc1.Speed_Y *= -(rc1.Elastic + rc2.Elastic);

                                                    rc1.NY = rc2.Y + rc2.SY;
                                                }

                                                if (rc1.PX < rc2.X && rc2.X < rc1.NPX && rc2.Y < rc1.Y && rc1.Y < rc2.PY)
                                                {
                                                    rc1.Speed_X *= -(rc1.Elastic + rc2.Elastic);

                                                    rc1.NX = rc2.X - rc1.SX;
                                                }

                                            }

                                        }
                                        else
                                        {
                                            if (rc1.NX + rc1.SX > rc2.X &&
                                                rc2.X + rc2.SX > rc1.X &&
                                                rc1.Y + rc1.SY > rc2.Y &&
                                                rc2.Y + rc2.SY > rc1.Y)
                                            {
                                                rc1.Speed_X *= -(rc1.Elastic + rc2.Elastic);

                                                rc1.NX = rc2.X - rc1.SX;
                                            }
                                        }
                                    }
                                    else if (rc1.Speed_X < 0)
                                    {
                                        if (rc1.Speed_Y > 0)
                                        {
                                            if (rc1.X + rc1.SX > rc2.X &&
                                                rc2.X + rc2.SX > rc1.NX &&
                                                rc1.NY + rc1.SY > rc2.Y &&
                                                rc2.Y + rc2.SY > rc1.Y)
                                            {
                                                if (rc2.X < rc1.X && rc1.X < rc2.PX && rc1.Y < rc2.Y && rc2.Y < rc1.NPY)
                                                {
                                                    rc1.Speed_Y *= -(rc1.Elastic + rc2.Elastic);

                                                    rc1.NY = rc2.Y - rc1.SY;
                                                }

                                                if (rc1.NX < rc2.PX && rc2.PX < rc1.X && rc2.Y < rc1.Y && rc1.Y < rc2.PY)
                                                {
                                                    rc1.Speed_X *= -(rc1.Elastic + rc2.Elastic);

                                                    rc1.NX = rc2.X + rc2.SX;
                                                }
                                            }
                                        }
                                        else if (rc1.Speed_Y < 0)
                                        {
                                            if (rc1.X + rc1.SX > rc2.X &&
                                                rc2.X + rc2.SX > rc1.NX &&
                                                rc1.Y + rc1.SY > rc2.Y &&
                                                rc2.Y + rc2.SY > rc1.NY)
                                            {
                                                if (rc2.X < rc1.X && rc1.X < rc2.PX && rc1.NY < rc2.PY && rc2.PY < rc1.Y)
                                                {
                                                    rc1.Speed_Y *= -(rc1.Elastic + rc2.Elastic);

                                                    rc1.NY = rc2.Y + rc2.SY;
                                                }

                                                if (rc1.NX < rc2.PX && rc2.PX < rc1.X && rc2.Y < rc1.Y && rc1.Y < rc2.PY)
                                                {
                                                    rc1.Speed_X *= -(rc1.Elastic + rc2.Elastic);

                                                    rc1.NX = rc2.X + rc2.SX;
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (rc1.X + rc1.SX > rc2.X &&
                                                rc2.X + rc2.SX > rc1.NX &&
                                                rc1.Y + rc1.SY > rc2.Y &&
                                                rc2.Y + rc2.SY > rc1.Y)
                                            {
                                                rc1.Speed_X *= -(rc1.Elastic + rc2.Elastic);

                                                rc1.NX = rc2.X + rc2.SX;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (rc1.Speed_Y > 0)
                                        {
                                            if (rc1.X + rc1.SX > rc2.X &&
                                                rc2.X + rc2.SX > rc1.X &&
                                                rc1.NY + rc1.SY > rc2.Y &&
                                                rc2.Y + rc2.SY > rc1.Y)
                                            {
                                                rc1.Speed_Y *= -(rc1.Elastic + rc2.Elastic);

                                                rc1.NY = rc2.Y - rc1.SY;
                                            }
                                        }
                                        else if (rc1.Speed_Y < 0)
                                        {
                                            if (rc1.X + rc1.SX > rc2.X &&
                                                rc2.X + rc2.SX > rc1.X &&
                                                rc1.Y + rc1.SY > rc2.Y &&
                                                rc2.Y + rc2.SY > rc1.NY)
                                            {
                                                rc1.Speed_Y *= -(rc1.Elastic + rc2.Elastic);

                                                rc1.NY = rc2.Y + rc2.SY;
                                            }
                                        }
                                        else
                                        {
                                            if (rc1.X + rc1.SX > rc2.X &&
                                                rc2.X + rc2.SX > rc1.X &&
                                                rc1.Y + rc1.SY > rc2.Y &&
                                                rc2.Y + rc2.SY > rc1.Y)
                                            {
                                                //静止状态被碰到
                                            }
                                        }
                                    }
                                }
                            });

                            rc1.X = rc1.NX;
                            rc1.Y = rc1.NY;
                        }

                        //
                        rc1.Judge_Speed();
                        //rc1.Judge_Visible();
                        if (rc1.X + rc1.SX >= Window.Size_X - 1 || rc1.X < 0 || rc1.Y + rc1.SY >= Window.Size_Y || rc1.Y < 0)
                        {
                            Out_Of_Bounds.Invoke(rc1);

                            rc1.Visible = false;

                        }
                        else
                        {
                            rc1.Visible = true;
                        }

                        ///////////////////////////////////
                    }
                    else
                    {
                        //rc1.Speed_X = 0; rc1.Speed_Y = 0;
                    }



                }

            });

            //信息显示
            Graph.Add_String(Graph.All_Graphs["V"], "        "); Graph.Add_String(Graph.All_Graphs["V"], Physics_Time_Watch.ElapsedMilliseconds.ToString() + " " + Game_Time_Watch.ElapsedMilliseconds.ToString());

            //动态延迟时间
            Physics_Interval_Time = (float)Physics_Time_Watch.ElapsedMilliseconds / 1000;

            Physics_Time_Watch.Restart();
        }
        
        private static bool Physics_Open = true;//是否启用
        public static void Close() //关闭线程
        {
            Physics_Open = false;

            Game_Time_Watch.Reset();
            Physics_Time_Watch.Reset();

            Physics_Timer.Dispose();
        }
        private static void Main_Loop() //主循环 用作特殊效果
        {
            while (Physics_Open)
            {
                Alarm_On();//闹钟响应

                Graph.Animation_Out(); //输出动画

                Thread.Sleep(Main_Loop_Sleep);
            }
        }
       
        //开始
        public static void Start_Up()
        {
            //启动准备
            Physics_Timer = new Timer(Equal_Interval_Callback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            Physics_Timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(Timer_Interval_Time));
            Is_Running = true;
            Game_Time_Watch.Start();

            Thread.Sleep(500);
            
            //主循环
            Main_Loop();
        }
    }
}
