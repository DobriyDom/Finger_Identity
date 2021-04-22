using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Finger_identity
{
    class Skeletization
    {
        Bitmap image;
        int[,] ar;
        List<int[]> t_noise;
        List<int[]> T;
        public Skeletization(Bitmap image)
        {
            this.image = image;

            T = new List<int[]>()
            {new int[]{ 1, 1, 0, 0, 1, 0 },
            new int[]{ 1, 1, 1, 0, 0, 0 },
            new int[]{ 0, 1, 0, 0, 1, 1 },
            new int[]{ 0, 0, 0, 1, 1, 1 },
            new int[]{ 1, 1, 1, 0, 0, 0, 0 },
            new int[]{ 1, 0, 1, 0, 0, 1, 0 },
            new int[]{ 0, 0, 0, 0, 1, 1, 1 },
            new int[]{ 0, 1, 0, 0, 1, 0, 1 }};

            t_noise = new List<int[]>()
               {new int[]{1,1,1,1,0,1,1,1,1},
               new int[]{1,1,1,1,0,1,1,0,0},
               new int[]{1,1,1,0,0,1,0,1,1},
               new int[]{0,0,1,1,0,1,1,1,1},
               new int[]{1,1,0,1,0,0,1,1,1},
               new int[]{1,1,1,1,0,1,0,0,1},
               new int[]{0,1,1,0,0,1,1,1,1},
               new int[]{1,0,0,1,0,1,1,1,1},
               new int[]{1,1,1,1,0,0,1,1,0},
               new int[]{1,1,1,1,0,1,0,0,0},
               new int[]{0,1,1,0,0,1,0,1,1},
               new int[]{0,0,0,1,0,1,1,1,1},
               new int[]{1,1,0,1,0,0,1,1,0}};

            bitmap_toBW();
            bitmap_toarr();
            skeletization();
        }
        public Bitmap get_image()
        {
            Bitmap image = new Bitmap(this.image.Width, this.image.Height);
            for (int i = 0; i < image.Height; i++)
                for (int j = 0; j < image.Width; j++)
                    if (ar[i, j] == 0)
                        image.SetPixel(j, i, Color.Black);
                    else
                        image.SetPixel(j, i, Color.White);
            return image;
        }
        private void bitmap_toBW()
        {
            double treshold = 0.7;

            Bitmap Black_White = new Bitmap(image.Width, image.Height);

            for (int i = 0; i < image.Width; i++)
                for (int j = 0; j < image.Height; j++)
                    Black_White.SetPixel(i, j, image.GetPixel(i, j).GetBrightness() < treshold ? Color.Black : Color.White);
                
            image = Black_White;
        }
        private void bitmap_toarr()
        {
            ar = new int[image.Height, image.Width];
            for (int i = 0; i < image.Height; i++)
                for (int j = 0; j < image.Width; j++)
                    if (image.GetPixel(j, i).ToArgb() == Color.White.ToArgb())
                        ar[i, j] = 1;
                    else
                        ar[i, j] = 0;

            StreamWriter file = new StreamWriter("1.txt", false, System.Text.Encoding.UTF8);
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                    file.Write(ar[i, j]);
                file.WriteLine();
            }
            file.Close();
        }
        
        private bool equal(int[] a1,int[] a2)
        {
            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;
            return true;
        }
        private void skeletization()
        {
            int w = image.Width;
            int h = image.Height;
            int count = 1;
            while(count != 0)
            {
                count = delete(w, h);
                if (count > 0)
                    delete_noise(w, h);
            }
        }
        private int delete(int w , int h)
        {
            int count = 0;
            for (int i = 0; i < h - 1; i++)
                for (int j = 0; j < w - 1; j++)
                    if (ar[i, j] == 0)
                        if (table(j, i))
                        {
                            ar[i, j] = 1;
                            count++;
                        }
            return count;
        }
        private void delete_noise(int w, int h)
        {
            for (int i = 0; i < h - 1; i++)
                for (int j = 0; j < w - 1; j++)
                    if (ar[i, j] == 0)
                        if (table_noise(j, i))
                        {
                            ar[i, j] = 1;
                        }
        }
        private bool check(int[] a)
        {
            int[] t = { a[1], a[2], a[3], a[4], a[5], a[7]};
            if (equal(t,T[0]))
                return true;
            t = new int[] { a[0],a[1],a[3],a[4],a[5],a[7]};
            if (equal(t,T[1]))
                return true;
            t = new int[] { a[1], a[3], a[4], a[5], a[6], a[7] };
            if (equal(t,T[2]))
                return true;
            t = new int[] { a[1], a[3], a[4], a[5], a[7], a[8] };
            if (equal(t,T[3]))
                return true;
            t = new int[] { a[0], a[1], a[2], a[3], a[4], a[5], a[7] };
            if (equal(t,T[4]))
                return true;
            t = new int[] { a[1], a[3], a[4], a[5], a[6], a[7], a[8] };
            if (equal(t,T[5]))
                return true;
            t = new int[] { a[0], a[1], a[3], a[4], a[5], a[6], a[7] };
            if (equal(t,T[6]))
                return true;
            t = new int[] { a[1], a[2], a[3], a[4], a[5], a[7], a[8] };
            if (equal(t,T[7]))
                return true;
            return false;
        }
        private bool check_noise(int[] a)
        {
            foreach(int[] p in t_noise)
                if (equal(a,p))
                    return true;
            return false;
        }
        private bool table(int x , int y)
        {
            List<int> a = new List<int>();
            for (int i = y - 1; i < y + 2; i++)
                for (int j = x - 1; j < x + 2; j++)
                    a.Add(ar[i,j]);
            return check(a.ToArray());
        }
        private bool table_noise(int x, int y)
        {
            List<int> a = new List<int>();
            for (int i = y - 1; i < y + 2; i++)
                for (int j = x - 1; j < x + 2; j++)
                    a.Add(ar[i, j]);
            return check_noise(a.ToArray());
        }
        
    }
}
