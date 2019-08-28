using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    static class Graph
    {
        //所有存储的图片
        public static Dictionary<string, char[,]> All_Graphs = new Dictionary<string, char[,]>();

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
        }

        public static void Add_File_To_Graphs(string name) //将文件中的图片添加到缓存中
        {
            ReadFile(name).ToList().ForEach(x => All_Graphs.Add(x.Key, x.Value));
        }
    }
}
