using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab2._3
{
    class Program
    {
        static bool flag = false;
        static int counterOverFlow = 0;
        static void Main(string[] args)
        {
            float numb1, numb2;

            numb1 = float.Parse(Console.ReadLine());
            numb2 = float.Parse(Console.ReadLine());
            
            FloatPointNumb first = new FloatPointNumb();
            FloatPointNumb second = new FloatPointNumb();

            FloatPointNumb result = new FloatPointNumb();

            string numb1InBin = FloatToBinary(numb1);
            string numb2InBin = FloatToBinary(numb2);

            first.Sign = Convert.ToInt32(numb1InBin.Split(' ')[0]);
            first.Mantissa = numb1InBin.Split(' ')[2].Select(c => Int32.Parse(c.ToString())).ToList();
            first.Exponent = numb1InBin.Split(' ')[1].Select(c => Int32.Parse(c.ToString())).ToList();
            Console.WriteLine($"{numb1}\t{first.Sign} {GetListAsStr(first.Exponent)} {GetListAsStr(first.Mantissa)} ");
            first.Exponent = Subtract(first.Exponent, new List<int>() { 0, 1, 1, 1, 1, 1, 1, 1 });

            second.Sign = Convert.ToInt32(numb2InBin.Split(' ')[0]);
            second.Mantissa = numb2InBin.Split(' ')[2].Select(c => Int32.Parse(c.ToString())).ToList();
            second.Exponent = numb2InBin.Split(' ')[1].Select(c => Int32.Parse(c.ToString())).ToList();
            Console.WriteLine($"{numb2}\t{second.Sign} {GetListAsStr(second.Exponent)} {GetListAsStr(second.Mantissa)}");
            second.Exponent = Subtract(second.Exponent, new List<int>() { 0, 1, 1, 1, 1, 1, 1, 1 });

            result.Sign = ((first.Sign == 0 && second.Sign == 0) || (first.Sign == 1 && second.Sign == 1)) ? 0 : 1;
            result.Exponent = ADD(first.Exponent, second.Exponent, 0);
            Console.WriteLine($"E1+E2\t {GetListAsStr(result.Exponent)}");
            
            result.Mantissa = multiplicate(first.Mantissa, second.Mantissa);
            if (counterOverFlow >= 1 && (first.Sign == 0 && second.Sign == 0))
               result.Exponent = ADD(result.Exponent, new List<int>() { 0, 0, 0, 0, 0, 0, 0, 1}, 0);
             result.Exponent = ADD(result.Exponent, new List<int>() { 0, 1, 1, 1, 1, 1, 1, 1 }, 0);
            string resultStr = result.Sign + GetListAsStr(result.Exponent) + GetListAsStr(result.Mantissa);
            
            Console.WriteLine($"result: {result.Sign} {GetListAsStr(result.Exponent)} {GetListAsStr(result.Mantissa)}");

            float res = BinaryStringToSingle(resultStr);
            Console.WriteLine(res);
            
        }
        static List<int> Subtract(List<int> exponent, List<int> numb)
        {
            List<int> result = new List<int>();

            for (int i = exponent.Count - 1; i >= 0; i--)
            {
                if (exponent[i] == 0 && numb[i] == 0)
                {
                    result.Add(0);
                }
                else if (exponent[i] == 1 && numb[i] == 1)
                {
                    result.Add(0);
                }
                else if (exponent[i] == 1 && numb[i] == 0)
                {
                    result.Add(1);
                }
                else
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (exponent[j] == 1)
                        {
                            for (int k = j + 1; k <= i; k++)
                            {
                                if (exponent[k] == 0)
                                    exponent[k] = 1;
                            }
                            exponent[j] = 0;
                            result.Add(1);
                            break;
                        }
                    }
                }
            }

            result.Reverse();
            return result;
        }
        static string GetListAsStr(List<int> list)
        {
            string result = string.Join("", list.ToArray());
            return result;
        }
        static float BinaryStringToSingle(string s)
        {
            long i = Convert.ToInt64(s, 2);
            byte[] b = BitConverter.GetBytes(i);
            return BitConverter.ToSingle(b, 0);
        }
        static List<int> multiplicate(List<int> first, List<int> second)
        {
            List<int> result = new List<int>();
            bool flag = false;

            List<int> help = new List<int>() { 1 };
            help.AddRange(first);
            first.Clear();
            first.AddRange(help);
            help.Clear();
            help.Add(1);
            help.AddRange(second);
            second.Clear();
            second.AddRange(help);

            List<int> helper1 = new List<int>();
            List<int> helper2 = new List<int>();

            if (second[second.Count - 1] == 0)
            {
                helper1.AddRange(new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            }
            else
            {
                helper1.AddRange(first);
            }

            List<int> helper = new List<int>();
            int shift = 1;
            for(int i = second.Count - 2; i >= 0; i--)
            {
                helper2.Clear();
                if (second[i] == 0)
                {
                    helper2.AddRange(new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
                }
                else
                {
                    helper2.AddRange(first);
                }
                helper.AddRange(helper1);
                helper1.Clear();
                shift = helper.Count - helper2.Count + 1;
                helper1.AddRange(ADD(helper, helper2, shift));
                //Console.WriteLine($"First addition: {GetListAsStr(helper)}   Second addition: {GetListAsStr(helper2)}   Sum: {GetListAsStr(helper1)}");
                helper.Clear();
            }

            for (int i = 0; i < helper1.Count; i++)
            {
                while(helper1[0] == 0)
                {
                    helper1.RemoveAt(0);
                }
                helper1.RemoveAt(0);
                break;
            }
            List<int> helperForHelp = new List<int>();
            helperForHelp.AddRange(helper1);
            helper1.Clear();
            for(int i = 0; i < 23; i++)
            {
                helper1.Add(helperForHelp[i]);
            }
            Console.WriteLine($"M1*M2 \t{GetListAsStr(helper1)}");

            return helper1;
        }
        static List<int> ADD(List<int> first, List<int> second, int shift)
        {
            List<int> result = new List<int>();

            if (shift > 0 && !Program.flag)
            {
                List<int> help = new List<int>() { 0 };
                help.AddRange(first);
                first.Clear();
                first.AddRange(help);
            }
            Program.flag = false;

            while(shift > 0)
            {
                second.Add(0);
                shift--;
            }

            int mind = 0;
            for (int i = first.Count - 1; i >= 0; i--)
            {
                if (first[i] == 0 && second[i] == 0 && mind == 0)
                {
                    result.Add(0);
                }
                else if ((first[i] == 0 && second[i] == 0 && mind == 1) ||
                        (first[i] == 0 && second[i] == 1 && mind == 0) ||
                        (first[i] == 1 && second[i] == 0 && mind == 0))
                {
                    result.Add(1);
                    mind = 0;
                }
                else if ((first[i] == 0 && second[i] == 1 && mind == 1) ||
                        (first[i] == 1 && second[i] == 0 && mind == 1) ||
                        (first[i] == 1 && second[i] == 1 && mind == 0))
                {
                    result.Add(0);
                    mind = 1;
                }
                else
                {
                    result.Add(1);
                    mind = 1;
                }
            }
            if (mind == 1)
            {
                result.Add(1);
                Program.flag = true;
                Program.counterOverFlow++;
            }

            result.Reverse();
            return result;
        }
        static string FloatToBinary(float f)
        {
            StringBuilder sb = new StringBuilder();
            Byte[] ba = BitConverter.GetBytes(f);
            foreach (Byte b in ba)
                for (int i = 0; i < 8; i++)
                {
                    sb.Insert(0, ((b >> i) & 1) == 1 ? "1" : "0");
                }
            string s = sb.ToString();
            string r = s.Substring(0, 1) + " " + s.Substring(1, 8) + " " + s.Substring(9);
            return r;
        }
    }
    class FloatPointNumb
    {
        public List<int> Exponent { get; set; }
        public List<int> Mantissa { get; set; }
        public int Sign { get; set; }
    }
}
