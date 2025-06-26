using System;

namespace DEF
{
    public static class Utils2
    {
        //������ǰ�õ����������ȡӳ���µ��������
        public static float GetRandomFloat(int min_src, int max_src, int rd_src, float min_dst, float max_dst)
        {
            float result = ((max_dst - min_dst) * (max_src - rd_src) / (max_src - min_src - 1)) + min_dst;
            return result;
        }

        //������ǰ�õ����������ȡӳ���µ��������
        public static int GetRandomInt(int min_src, int max_src, int rd_src, int min_dst, int max_dst)
        {
            int result = (int)((float)(max_dst - min_dst - 1) * (max_src - rd_src) / (max_src - min_src - 1)) + min_dst;
            return result;
        }
    }
}