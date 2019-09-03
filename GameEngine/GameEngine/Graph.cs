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
        private static readonly List<string[]> Binding_Obj = new List<string[]>();

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
        public static void Delete_Binding_Animation(string obj_name) //解除绑定动画
        {
            Binding_Obj.Remove(Binding_Obj.FirstOrDefault(t => t[1] == obj_name));

        }


        private static readonly Stopwatch Animation_Watch = new Stopwatch(); //帧率计时器
        private static readonly int Animation__Interval_Time = 500; //动画输出间隔ms(不保证准确输出间隔)
        private static bool Enable_Animation = false; //动画是否启动
        public static void Start_Animation() //启动动画
        {
            Enable_Animation = true;
        }
        public static void Close_Animation() //关闭动画
        {
            Enable_Animation = false;
        }

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

                //Add_String(All_Graphs["V"], "        ");Add_String(All_Graphs["V"], Animation_Watch.ElapsedMilliseconds.ToString());

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
        public static char[,] String_To_Graph(string s) //字符串转图片 一行高
        {
            char[,] OutP = new char[s.Length, 1];

            for (int i = 0; i < s.Length; i++)
            {
                OutP[i, 0] = s[i];
            }

            return OutP;
        }
        public static char[,] String_To_Graph(string s, int x, int y) //字符串转图片 指定大小自动折行
        {
            char[,] OutP = new char[x, y];
            int yy = 0; int xx = 0;

            for (int i = 0; i < s.Length; i++)
            {
                if (yy < y)
                {
                    if (xx < x)
                    {
                        OutP[xx++, yy] = s[i];
                    }
                    else
                    {
                        xx = 0;
                        yy++;
                    }
                }
                
            }

            return OutP;
        }
        public static char[,] Add_Graph(Tuple<int, int> size, params Tuple<int, int, char[,]>[] p) //图片添加到一起导出(图片位置一一对应) size为目标输出尺寸 先添加的在下
        {
            char[,] OutP = new char[size.Item1, size.Item2];
            
            foreach (Tuple<int, int, char[,]> t in p)
            {
                for (int i = 0; i < t.Item3.GetLength(1); i++)
                {
                    for (int j = 0; j < t.Item3.GetLength(0); j++)
                    {
                        if (t.Item1 + i < OutP.GetLength(0) && t.Item2 + j < OutP.GetLength(1))
                        {
                            if (t.Item3[i, j] != '\0')
                            {
                                OutP[t.Item1 + i, t.Item2 + j] = t.Item3[i, j];
                            }
                        }
                    }
                }

            }

            return OutP;
        }
        public static char[,] Add_Graph(params Tuple<int, int, char[,]>[] p) //图片添加到一起导出(图片位置一一对应) 不指定size自动导出最大矩形
        {
            int[] xx = new int[p.Length];
            int[] yy = new int[p.Length];
            for(int i=0;i<p.Length;i++)
            {
                xx[i] = p[i].Item1 + p[i].Item3.GetLength(0);
                yy[i] = p[i].Item2 + p[i].Item3.GetLength(1);
            }

            char[,] OutP = new char[xx.Max(), yy.Max()];

            foreach (Tuple<int, int, char[,]> t in p)
            {
                for (int i = 0; i < t.Item3.GetLength(1); i++)
                {
                    for (int j = 0; j < t.Item3.GetLength(0); j++)
                    {
                        if (t.Item3[i, j] != '\0')
                        {
                            OutP[t.Item1 + i, t.Item2 + j] = t.Item3[i, j];
                        }
                    }
                }

            }

            return OutP;
        }
        
        public static void Mix_Graph(char[,] look, char[,] g, int x = 0, int y = 0) //将图片叠加
        {
            if (y < look.GetLength(1))
            {
                for (int i = 0; i < g.GetLength(0); i++)
                {
                    for (int j = 0; j < g.GetLength(1); j++)
                    {
                        if (x + i < look.GetLength(0) && y + j < look.GetLength(1))
                        {
                            if (g[i, j] != '\0')
                            {
                                look[x + i, y + j] = g[i, j];
                            }
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
                    if (g[i, j] != '\0')
                    {
                        look[x + i, y + j] = g[i, j];
                    }
                }
            }
            
        }
        public static void Mix_Graph_H(char[,] look, char[,] g)
        {
            for (int i = 0; i < g.GetLength(0); i++)
            {
                for (int j = 0; j < g.GetLength(1); j++)
                {
                    if (g[i, j] != '\0')
                    {
                        look[i, j] = g[i, j];
                    }
                }
            }

        }
        public static void Replace_Pixel(char[,] b,char Initial_p,char Aims_p) //将b中的Initial_p替换成Aims_p
        {
            Parallel.For(0, b.GetLength(1), (i) =>
            {
                for (int j = 0; j < b.GetLength(0); j++)
                {
                    if (b[j, i] == Initial_p)
                    {
                        b[j, i] = Aims_p;
                    }
                }
            });

        }


        //文件操作
        private static string Graph_Path; //进行操作的位置
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
        public static string[] Read_Prerendered_Frame(string filename) //读取预渲染文件
        {
            using (System.IO.StreamReader file = new System.IO.StreamReader(Graph_Path + filename))
            {
                List<string> fms = new List<string>();
                
                string line;string siz="0 0";
                while ((line = file.ReadLine()) != null)
                {
                    if (line == "-----")
                    {
                        string[] message = file.ReadLine().Split(' ');
                        string L = "";
                        siz = message[1] +" "+ message[2];
                        
                        for (int i = 0; i < int.Parse(message[2]); i++)
                        {
                            L += file.ReadLine();
                        }

                        fms.Add(L);
                    }
                }

                fms.Add(siz);

                return fms.ToArray();
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
        }

        public static void Add_File_To_Graphs(string name) //将文件中的图片添加到缓存中
        {
            ReadFile(name).ToList().ForEach(x => All_Graphs.Add(x.Key, x.Value));
        }
        
        
    }
}
