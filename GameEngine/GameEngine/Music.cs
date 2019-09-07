using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    static class Music
    {
        private static readonly SoundPlayer Player = new SoundPlayer();

        private static string Music_Path = "";

        public static void Load_Music(string filename) //加载音乐
        {
            Player.SoundLocation = Music_Path + filename+".wav";
            Player.Load();
        }

        public static void Play_Music(string filename) //加载+播放
        {
            Player.SoundLocation = Music_Path + filename + ".wav";
            Player.Load();
            Player.Play();
        }
        public static void Play_Music() //播放加载项
        {
            Player.Play();
        }
        public static void Stop_Music() //停止播放
        {
            Player.Stop();
        }

        public static void Loading(string path="")
        {
            if (Directory.Exists(path))
            {
                Music_Path = path;
            }
            else
            {
                throw new Exception("路径不存在");
            }
            
        }
    }
}
