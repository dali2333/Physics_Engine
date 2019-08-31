using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class GameOBJ
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
        public float Elastic = 1f; //弹性系数
        public float Friction = 0f;//动摩擦因数

        public float Speed_X = 0;//速度
        public float Speed_Y = 0;
        public float F_X = 0;//受力
        public float F_Y = 0;



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
            if (Speed_Y > Physics.Global_Max_Speed) { Speed_Y = Physics.Global_Max_Speed; }
        }


        public void Move(int mx, int my) //相对移动
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

    }
}
