using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class GameOBJ //基础游戏体
    {
        //位置尺寸
        public float X;//位置
        public float Y;
        public int SX;//尺寸
        public int SY;
        //外观性质
        public bool Visible = true;//可见性
        public char[,] Look;//外观
        public string Name;//名称
        //物理性质
        public bool Movable = true; //可移动性(关闭不移动物体的可移动性可以提升性能)
        public bool Collisible = true; //可碰撞性
        public float Weight = 1f;
        public float Elastic = 0f; //弹性系数
        public float Friction = 0f;//动摩擦因数

        public float Speed_X = 0;//速度
        public float Speed_Y = 0;
        public float F_X = 0;//受力
        public float F_Y = 0;

        public bool Is_Spliced = false;//是否被组合
        public string Splice_Name = "";//组合体名称
        public void Change_Weight_When_Spliced(float w) //当物体被绑定到组合体中,使用这个方法改变物体质量,同时变化组合体质量
        {
            if (Is_Spliced)
            {
                Window.All_SpliceOBJ[Splice_Name].Weight += w - Weight;

                Weight = w;
            }
            else
            {
                Weight = w;
            }
        }

        public void Add_Physics(float weight, float vx = 0, float vy = 0, float fx = 0, float fy = 0, float elastic = 0, float friction = 0)
        {
            Weight = weight;
            Speed_X = vx;
            Speed_Y = vy;
            F_X = fx;
            F_Y = fy;
            Elastic = elastic;
            Friction = friction;
        }

        public GameOBJ(int x, int y, int sx, int sy, char[,] look, bool visible = true)
        {
            //MX = x;MY = y;
            X = x; Y = y; SX = sx; SY = sy; Look = look;
            Visible = visible;

        }
        public void Judge_Visible()
        {
            if (X + SX >= Window.Size_X - 1 || X < 0 || Y + SY >= Window.Size_Y || Y < 0)
            {
                Visible = false;
            }
            else
            {
                Visible = true;
            }
        }
        public void Judge_Speed()
        {
            if (Speed_X > Physics.Global_Max_Speed) { Speed_X = Physics.Global_Max_Speed; }
            else if(Speed_X < -Physics.Global_Max_Speed) { Speed_X = -Physics.Global_Max_Speed; }

            if (Speed_Y > Physics.Global_Max_Speed) { Speed_Y = Physics.Global_Max_Speed; }
            else if (Speed_Y < -Physics.Global_Max_Speed) { Speed_Y = -Physics.Global_Max_Speed; }

        }


        public void Move(int mx, int my) //相对移动
        {
            X += mx;
            Y += my;

            Judge_Visible();
        }
        public void Move(float mx, float my) //相对移动float
        {
            X += mx;
            Y += my;

            Judge_Visible();
        }
        public void Move_To(int x, int y) //移动到
        {
            X = x;
            Y = y;

            Judge_Visible();
        }

        //静态创建方法
        public readonly static int Max_Objs = 20;//最大游戏体数

        public static bool Made_Obj(string name, int x, int y, int sx, int sy, char[,] look) //创建物体
        {
            if (sx <= 0 || sy <= 0 || Window.All_Obj.Count > Max_Objs)
            {
                return false;
            }
            else
            {
                if (Window.Add_Obj(name, new GameOBJ(x, y, sx, sy, look)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static bool Change_Obj_Physics(string name, float weight = 1, float vx = 0, float vy = 0, float fx = 0, float fy = 0, float elastic = 1, float friction = 0) //物体物理性质
        {
            if (weight <= 0 || !Window.All_Obj.ContainsKey(name))
            {
                return false;
            }
            else
            {
                Window.All_Obj[name].Add_Physics(weight, vx, vy, fx, fy, elastic, friction);

                return true;
            }
        }
        public static void Change_Obj_Physics_S(string name, bool Movable = true, bool Collisible = true)
        {
            Window.All_Obj[name].Movable = Movable;
            Window.All_Obj[name].Collisible = Collisible;
        }

    }

    public class SpliceOBJ //组合体
    {
        public readonly static int Max_Splice = 10; //最大组合内元素数

        private readonly List<GameOBJ> All_OBJ_In = new List<GameOBJ>(Max_Splice); //组合内的物体

        public string Name;//组合体名称

        //支持内部成员单独修改的属性 => X,Y,SX,SY,Look,Visible,Elastic,Friction,Weight(需调函数)

        private bool visible = true;//可见性(支持局部可见,修改将统一修改)
        private bool movable = true;//可移动性(关闭不移动物体的可移动性可以提升性能)
        private bool collisible = true;//可碰撞性
        private float speed_X = 0;//速度
        private float speed_Y = 0;
        private float f_X = 0;//受力
        private float f_Y = 0;
        private float weight = 0; //总重量 主动修改会使所有物体重量变为总重的均分

        public bool Visible
        {
            get { return visible; }

            set
            {
                visible = value;
                foreach (GameOBJ o in All_OBJ_In)
                {
                    o.Visible = value;
                }
            }
        }
        public bool Movable
        {
            get { return movable; }

            set
            {
                movable = value;
                foreach (GameOBJ o in All_OBJ_In)
                {
                    o.Movable = value;
                }
            }
        } 
        public bool Collisible
        {
            get { return collisible; }

            set
            {
                collisible = value;
                foreach (GameOBJ o in All_OBJ_In)
                {
                    o.Collisible = value;
                }
            }
        }
        public float Speed_X
        {
            get { return speed_X; }

            set
            {
                speed_X = value;
                foreach (GameOBJ o in All_OBJ_In)
                {
                    o.Speed_X = value;
                }
            }
        }
        public float Speed_Y
        {
            get { return speed_Y; }

            set
            {
                speed_Y = value;
                foreach (GameOBJ o in All_OBJ_In)
                {
                    o.Speed_Y = value;
                }
            }
        }
        public float F_X
        {
            get { return f_X; }

            set
            {
                f_X = value;
                
            }
        }
        public float F_Y
        {
            get { return f_Y; }

            set
            {
                f_Y = value;
                
            }
        }
        public float Weight
        {
            get { return weight; }
            set
            {
                if (value > 0)
                {
                    weight = value;
                    foreach (GameOBJ o in All_OBJ_In)
                    {
                        o.Weight = value / All_OBJ_In.Count;
                    }
                }
               
            }
        }
        public float Elastic
        {
            set
            {
                foreach (GameOBJ o in All_OBJ_In)
                {
                    o.Elastic = value;
                }
            }
        }
        public float Friction
        {
            set
            {
                foreach (GameOBJ o in All_OBJ_In)
                {
                    o.Friction = value;
                }
            }
        }
        public char[,] Look
        {
            get
            {
                Tuple<int, int, char[,]>[] p = new Tuple<int, int, char[,]>[All_OBJ_In.Count];

                for(int i=0;i< All_OBJ_In.Count;i++)
                {
                    p[i] = new Tuple<int, int, char[,]>((int)(All_OBJ_In[i].X + 0.5), (int)(All_OBJ_In[i].Y+0.5), All_OBJ_In[i].Look);
                }

                return Graph.Add_Graph(p);
            }
        } //外观 返回包含所有物体的最大包围矩形(消耗性能大,且每次都重新生成)

        public void Add_Physics(float weight, float vx = 0, float vy = 0, float fx = 0, float fy = 0, float elastic = 1, float friction = 0)
        {
            Weight = weight;
            Speed_X = vx;
            Speed_Y = vy;
            F_X = fx;
            F_Y = fy;

            Elastic = elastic;
            Friction = friction;
        }

        public void Judge_Visible()
        {
            foreach (GameOBJ o in All_OBJ_In)
            {
                o.Judge_Visible();
            }
        }
        public void Judge_Speed()
        {
            foreach (GameOBJ o in All_OBJ_In)
            {
                o.Judge_Speed();
            }
        }

        public void Move(int mx, int my) //相对移动
        {
            foreach (GameOBJ o in All_OBJ_In)
            {
                o.Move(mx, my);
            }
        }
        public void Move(float mx, float my) //相对移动float
        {
            foreach (GameOBJ o in All_OBJ_In)
            {
                o.Move(mx, my);
            }
        }

        public SpliceOBJ(string name, params string[] oBJs) 
        {
            Name = name;

            if (oBJs.Length > 0 && oBJs.Length <= Max_Splice)
            {
                foreach (string o in oBJs)
                {
                    if (!Window.All_Obj[o].Is_Spliced)
                    {
                        All_OBJ_In.Add(Window.All_Obj[o]);

                        Window.All_Obj[o].Is_Spliced = true;
                        Window.All_Obj[o].Splice_Name = name;

                        weight += Window.All_Obj[o].Weight;
                    }
                    
                }
            }
            
        }
        public void Start_Splice(string s) //添加绑定
        {
            if (!All_OBJ_In.Contains(Window.All_Obj[s]))
            {
                All_OBJ_In.Add(Window.All_Obj[s]);

                Window.All_Obj[s].Is_Spliced = true;
                Window.All_Obj[s].Splice_Name = Name;
                weight += Window.All_Obj[s].Weight;
            }
        }
        public void Stop_Splice(string s) //解除绑定
        {
            if (All_OBJ_In.Contains(Window.All_Obj[s]))
            {
                All_OBJ_In.Remove(Window.All_Obj[s]);

                Window.All_Obj[s].Is_Spliced = false;
                Window.All_Obj[s].Splice_Name = "";

                if (weight > Window.All_Obj[s].Weight)
                {
                    weight -= Window.All_Obj[s].Weight;
                }

            }
            
        }

        //静态创建方法
        public static bool Made_Obj(string name, params string[] oBJs) //创建物体
        {

            if (Window.Add_SpliceObj(name, new SpliceOBJ(name, oBJs)))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public static bool Change_Obj_Physics(string name, float weight = 1, float vx = 0, float vy = 0, float fx = 0, float fy = 0, float elastic = 1, float friction = 0) //物体物理性质
        {
            if (weight <= 0 || !Window.All_SpliceOBJ.ContainsKey(name))
            {
                return false;
            }
            else
            {
                Window.All_SpliceOBJ[name].Add_Physics(weight, vx, vy, fx, fy, elastic, friction);

                return true;
            }
        }
        public static void Change_Obj_Physics_S(string name, bool Movable = true, bool Collisible = true)
        {
            Window.All_SpliceOBJ[name].Movable = Movable;
            Window.All_SpliceOBJ[name].Collisible = Collisible;
        }

    }


}
