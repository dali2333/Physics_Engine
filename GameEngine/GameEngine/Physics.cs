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
        public static float Global_Max_Speed = 100f;//全局最大速度

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

        //物理演算部分
        private static bool Judge_Touched(GameOBJ rc1, GameOBJ rc2) //判断是否碰撞
        {
            if (rc1.X + rc1.SX > rc2.X &&
                rc2.X + rc2.SX > rc1.X &&
                rc1.Y + rc1.SY > rc2.Y &&
                rc2.Y + rc2.SY > rc1.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static void Overlap_Friction(GameOBJ rc1) //碰撞-摩擦效果
        {
            //遍历其余物体
            Parallel.ForEach(Window.All_Obj.Values, (rc2) =>
            {
                //找出符合标准的物体
                if (rc2.Collisible && rc1.Name != rc2.Name)
                {
                    //判断是否碰撞
                    if (rc1.X + rc1.SX > rc2.X &&
                    rc2.X + rc2.SX > rc1.X &&
                    rc1.Y + rc1.SY > rc2.Y &&
                    rc2.Y + rc2.SY > rc1.Y)
                    {
                        //计算碰撞后反应
                        if (rc1.Speed_Y > 0)
                        {
                            if (rc2.Y - rc1.Y + rc1.SY / 2 > 0)
                            {
                                //弹力
                                rc1.Speed_Y = -(rc1.Elastic + rc2.Elastic) * rc1.Speed_Y;//rc2.Weight / rc1.Weight * (Math.Abs(rc1.Speed_Y) + Math.Abs(rc2.Speed_Y));
                            }
                        }
                        else if (rc1.Speed_Y < 0)
                        {
                            if (rc2.Y + rc2.SY - rc1.Y - rc1.SY / 2 < 0)
                            {
                                //弹力
                                rc1.Speed_Y = -(rc1.Elastic + rc2.Elastic) * rc1.Speed_Y;//rc2.Weight / rc1.Weight * (Math.Abs(rc1.Speed_Y) + Math.Abs(rc2.Speed_Y));
                            }
                        }
                        else
                        {
                            //支持力
                            /*
                            if(rc2.Y - rc1.Y + rc1.SY / 2 > 0 || rc2.Y + rc2.SY - rc1.Y - rc1.SY / 2 < 0)
                            {
                                //rc2.F_Y = -rc2.Weight * Global_GY;

                                //rc2.Speed_Y += -rc2.Weight * Global_GY * Physics_Interval_Time;

                                //Console.Write('A');
                            }
                            */

                            //摩擦力
                            if (rc2.Speed_X != 0)
                            {
                                rc2.Speed_X += -rc2.Friction * Math.Abs(Global_GY) * rc2.Weight * rc2.Speed_X / Math.Abs(rc2.Speed_X) * Physics_Interval_Time;
                            }
                        }

                        if (rc1.Speed_X > 0)
                        {
                            if (rc2.X - rc1.X + rc1.SX / 2 > 0)
                            {
                                rc1.Speed_X = -(rc1.Elastic + rc2.Elastic) * rc1.Speed_X;//rc2.Weight / rc1.Weight * (Math.Abs(rc1.Speed_Y) + Math.Abs(rc2.Speed_Y));
                            }
                        }
                        else if (rc1.Speed_X < 0)
                        {
                            if (rc2.X + rc2.SX - rc1.X - rc1.SX / 2 < 0)
                            {
                                rc1.Speed_X = -(rc1.Elastic + rc2.Elastic) * rc1.Speed_X;//rc2.Weight / rc1.Weight * (Math.Abs(rc1.Speed_Y) + Math.Abs(rc2.Speed_Y));

                            }
                        }
                        else
                        {
                            //支持力
                            /*
                            if (rc2.X - rc1.X + rc1.SX / 2 > 0 || rc2.X + rc2.SX - rc1.X - rc1.SX / 2 < 0)
                            {
                                //rc1.F_X = -rc1.Weight * Global_GX;
                                //rc2.Speed_X += -rc2.Weight * Global_GY * Physics_Interval_Time;
                                //Console.Write('A');
                            }
                            */

                            //摩擦力
                            if (rc2.Speed_Y != 0)
                            {
                                rc2.Speed_Y += -rc2.Friction * Math.Abs(Global_GX) * rc2.Weight * rc2.Speed_Y / Math.Abs(rc2.Speed_Y) * Physics_Interval_Time;
                            }
                        }

                        //输出碰撞事件
                        Touched.Invoke(rc1, rc2);

                    }
                }
            });

        }

        private static void Equal_Interval_Callback(object state) //等时间间隔计算
        {
            //物理算法
            Parallel.ForEach(Window.All_Obj.Values, (obj) =>
            {
                //组合体
                if (obj.Is_Spliced) 
                {
                    SpliceOBJ so = Window.All_SpliceOBJ[obj.Splice_Name];

                    if (obj.Collisible)
                    {
                        ///////////////////////////////////

                        
                        
                        ///////////////////////////////////

                    }

                    if (so.Movable) //位移-速度计算
                    {
                        ///////////////////////////////////
                        so.Move(so.Speed_X * Physics_Interval_Time, so.Speed_Y * Physics_Interval_Time);

                        so.Speed_X += (so.F_X / so.Weight + Global_GX) * Physics_Interval_Time;
                        so.Speed_Y += (so.F_Y / so.Weight + Global_GY) * Physics_Interval_Time;

                        if (so.Speed_X > Global_Max_Speed) { so.Speed_X = Global_Max_Speed; }
                        if (so.Speed_Y > Global_Max_Speed) { so.Speed_Y = Global_Max_Speed; }

                        so.Judge_Visible();
                        so.Judge_Speed();

                        ///////////////////////////////////
                    }
                    else
                    {
                        so.Speed_X = 0; so.Speed_Y = 0;
                    }

                }
                //非组合体
                else
                {
                    //碰撞-摩擦计算
                    if (obj.Collisible)
                    {
                        ///////////////////////////////////

                        Overlap_Friction(obj); //可更换模块

                        ///////////////////////////////////

                    }
                    //位移-速度计算
                    if (obj.Movable) 
                    {
                        ///////////////////////////////////
                        obj.X += obj.Speed_X * Physics_Interval_Time;
                        obj.Y += obj.Speed_Y * Physics_Interval_Time;

                        obj.Speed_X += (obj.F_X / obj.Weight + Global_GX) * Physics_Interval_Time;
                        obj.Speed_Y += (obj.F_Y / obj.Weight + Global_GY) * Physics_Interval_Time;

                        
                        obj.Judge_Speed();
                        //obj.Judge_Visible();

                        if (obj.X + obj.SX >= Window.Size_X - 1 || obj.X < 0 || obj.Y + obj.SY >= Window.Size_Y || obj.Y < 0)
                        {
                            Out_Of_Bounds.Invoke(obj);

                            obj.Visible = false;

                        }
                        else
                        {
                            obj.Visible = true;
                        }

                        ///////////////////////////////////
                    }
                    else
                    {
                        obj.Speed_X = 0; obj.Speed_Y = 0;
                    }

                }

            });

            //信息显示
            Graph.Add_String(Graph.All_Graphs["V"], "        "); Graph.Add_String(Graph.All_Graphs["V"], Physics_Time_Watch.ElapsedMilliseconds.ToString() + " " + Game_Time_Watch.ElapsedMilliseconds.ToString());

            //动态延迟时间
            Physics_Interval_Time = (float)Physics_Time_Watch.ElapsedMilliseconds / 1000;

            Physics_Time_Watch.Restart();
        }

        //总控
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

        
        public static void Start_Up()
        {
            //启动准备
            Physics_Timer = new Timer(Equal_Interval_Callback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            Physics_Timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(Timer_Interval_Time));
            Is_Running = true;
            Game_Time_Watch.Start();

            //主循环
            Main_Loop();
        }
    }
}
