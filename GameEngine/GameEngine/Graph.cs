using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameEngine
{
    static class Graph
    {
        //动画类
        public class Animation
        {
            public string Name = ""; //名称

            public int X = 0; //尺寸
            public int Y = 0;

            public int Length; //帧数

            private char[][,] All_Frame;//全部帧

            public Animation(string name,string path)
            {
                Name = name;

                Dictionary<string, char[,]> fms = Graph.ReadFile(path);
                char[,] kvp = fms.FirstOrDefault().Value;
                X = kvp.GetLength(0);
                Y = kvp.GetLength(1);
                Length = fms.Count;
                All_Frame = new char[fms.Count][ ,];

                int n = 0;
                foreach(char[,]c in fms.Values)
                {
                    All_Frame[n] = new char[kvp.GetLength(0), kvp.GetLength(1)];
                    for (int i=0;i< c.GetLength(0); i++)
                    {
                        for (int j = 0; j < c.GetLength(1); j++)
                        {
                             All_Frame[n][i,j] = c[i,j];
                        }

                    }
                    
                    n++;
                }

                Enum_Frame = GetEnumerator();
            }

            private IEnumerator<char[,]> GetEnumerator()
            {
                while (true)
                {
                    for (int i = 0; i < Length; i++)
                    {
                        yield return All_Frame[i];
                    }
                }
                
            }
            private IEnumerator<char[,]> Enum_Frame;
            public char[,] Get_Next_Frame() //循环获取帧
            {
                Enum_Frame.MoveNext();
                
                return Enum_Frame.Current;
            }

        }

        //所有存储的图片
        public static Dictionary<string, char[,]> All_Graphs = new Dictionary<string, char[,]>();
        //所有存储的动画
        public static Dictionary<string, Animation> All_Animations = new Dictionary<string, Animation>();
        //动画绑定物体
        private static List<string[]> Binding_Obj = new List<string[]>();
        
        public static void Load_Animation(string name,string path) //创建并添加动画
        {
            All_Animations[name] = new Animation(name,path);
        }
        public static void Binding_Animation_To_Obj(string animation_name, string obj_name) //动画绑定物体的贴图(均为名称,先构建物体再绑定.动画与物体1对1,不可重复)
        {
            if(All_Animations.ContainsKey(animation_name) && Window.All_Obj.ContainsKey(obj_name))
            {
                Binding_Obj.Add(new string[] { animation_name, obj_name });
            }

        }


        private static readonly Stopwatch Animation_Watch = new Stopwatch(); //帧率计时器
        private static readonly int Animation__Interval_Time = 500; //动画输出间隔ms(不保证准确输出间隔)
        public static bool Enable_Animation = false; //启动动画

        public static void Animation_Out() //逐帧输出动画(非同步变化)
        {
            if (Enable_Animation) 
            {
                Parallel.ForEach(Binding_Obj, (b) =>
                {
                    Window.All_Obj[b[1]].Look = All_Animations[b[0]].Get_Next_Frame();
                });
                
                if (Animation_Watch.ElapsedMilliseconds <= Animation__Interval_Time)
                {
                    Thread.Sleep((ushort)(Animation__Interval_Time - Animation_Watch.ElapsedMilliseconds));
                }
                else
                {

                }

                //Add_String(All_Graphs["V"], "        ");
                //Add_String(All_Graphs["V"], Animation_Watch.ElapsedMilliseconds.ToString());

                Animation_Watch.Restart();
            }
        }

        //图片操作
        public static void Show_Look(char[,] Look) //显示图片
        {
            for (int i = 0; i < Look.GetLength(1); i++)
            {
                for (int j = 0; j < Look.GetLength(0); j++)
                {
                    Console.Write(Look[j,i]);
                }
                Console.Write('\n');
            }
        }
        public static void Add_String(char[,] look, string s, int x = 0, int y = 0) //将字符串绘制到图片上 超出图片的部分不显示
        {
            if (y < look.GetLength(1))
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (x + i < look.GetLength(0))
                    {
                        look[x + i, y] = s[i];
                    }
                }
            }

        }
        public static void Add_String_H(char[,] look, string s, int x, int y) //快速将字符串绘制到图片上 超出图片会报错
        {
            for (int i = 0; i < s.Length; i++)
            {
                look[x + i, y] = s[i];
            }
        }
        public static void Add_String_H(char[,] look, string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                look[i, 0] = s[i];
            }

        }
        public static void Mix_Graph(char[,] look, char[,] g, int x = 0, int y = 0) //将图片叠加
        {
            if (y < look.GetLength(1))
            {
                for (int i = 0; i < g.GetLength(0); i++)
                {
                    for (int j = 0; j < g.GetLength(1); j++)
                    {
                        if (x + i < look.GetLength(0))
                        {
                            look[x + i, y + j] = g[i, j];
                        }

                    }
                }
            }

        }
        public static void Mix_Graph_H(char[,] look, char[,] g, int x, int y) //图片快速叠加(超出图片会报错)
        {
            for (int i = 0; i < g.GetLength(0); i++)
            {
                for (int j = 0; j < g.GetLength(1); j++)
                {
                    look[x + i, y + j] = g[i, j];

                }
            }
            
        }
        public static void Mix_Graph_H(char[,] look, char[,] g)
        {
            for (int i = 0; i < g.GetLength(0); i++)
            {
                for (int j = 0; j < g.GetLength(1); j++)
                {
                    look[i, j] = g[i, j];

                }
            }

        }

        //文件操作
        public static string Graph_Path; //进行操作的位置
        public static Dictionary<string, char[,]> ReadFile(string filename) //读取图片文件
        {
            using (System.IO.StreamReader file = new System.IO.StreamReader(Graph_Path + filename))
            {
                Dictionary<string, char[,]> All_Data = new Dictionary<string, char[,]>();

                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (line == "-----")
                    {
                        string[] message = file.ReadLine().Split(' ');
                        char[,] look = new char[int.Parse(message[1]), int.Parse(message[2])];

                        for (int i = 0; i < int.Parse(message[2]); i++)
                        {
                            int j = 0;
                            foreach (char l in file.ReadLine())
                            {
                                look[j, i] = l;
                                j++;
                            }
                        }
                        
                        All_Data.Add(message[0], look);
                    }
                }

                return All_Data;
            }
                
        }
        public static void WriteFile(string filename, string name, char[,] look,bool append=true) //写入一张图片 append=false是覆盖写入,true是接在下方
        {

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Graph_Path + filename, append))
            {
                file.WriteLine("-----");
                file.WriteLine("" + name + " " + look.GetLength(0).ToString() + " " + look.GetLength(1).ToString());
                
                for (int j = 0; j < look.GetLength(1); j++)
                {
                    for (int i = 0; i < look.GetLength(0); i++)
                    {
                        file.Write(look[i, j]);
                    }
                    file.WriteLine();
                }

            }
                
            
        }
        public static void WriteFile(string filename, Dictionary<string, char[,]> graph) //写入多张图片 必定接续写入
        {
            foreach (string name in graph.Keys)
            {
                WriteFile(filename, name, graph[name], true);
            }

        }

        //高级方法
        public static void Loading(string path="") //输入path后每次调用都以该目录为基准，不然每次用全局路径也行
        {
            Graph_Path = path;
            Enable_Animation = false;
        }

        public static void Add_File_To_Graphs(string name) //将文件中的图片添加到缓存中
        {
            ReadFile(name).ToList().ForEach(x => All_Graphs.Add(x.Key, x.Value));
        }
        
        
    }
}
