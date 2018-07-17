using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace JudgeTime
{
    class Time
    {
        //获取当前的年、月、日、时、分、秒
        public string startYear = DateTime.Now.Year.ToString();
        public string startMonth = DateTime.Now.Month.ToString();
        public string startDay = DateTime.Now.Day.ToString();
        public string startHour = DateTime.Now.Hour.ToString();
        public string startMinute = DateTime.Now.Minute.ToString();
        public string startSecond = DateTime.Now.Second.ToString();

        public DateTime endDT;//定义截止时间

        //通过输入日期，来计算输出Deadline  格式为 2018/7/13 16:00:00
        public DateTime IsOverdue(string tempStr)
        {

            //分割输入的字符串
            startYear = tempStr.Split(' ')[0].Split('/')[0];    
            startMonth = tempStr.Split(' ')[0].Split('/')[1];
            startDay = tempStr.Split(' ')[0].Split('/')[2];
            startHour = tempStr.Split(' ')[1].Split(':')[0];
            startMinute = tempStr.Split(' ')[1].Split(':')[1];
            startSecond = tempStr.Split(' ')[1].Split(':')[2];

            //输入的时间
            DateTime dt1 = new DateTime(int.Parse(startYear), int.Parse(startMonth), int.Parse(startDay), int.Parse(startHour), int.Parse(startMinute), int.Parse(startSecond));

            //当天的下班时间
            DateTime dt2 = new DateTime(int.Parse(startYear), int.Parse(startMonth), int.Parse(startDay), 17, 30, 00);

            double m = (dt2 - dt1).TotalMinutes;//派单时间距离下班时间的时间差，单位：分钟

            double n = 0;//下一天的逾期前的时间差，单位：分钟
			
			double space = 180;//时间差为180分钟，单位：分钟

            if (m >= space)//时间差大于3个小时，则在当天截止时间
            {
                endDT = dt1.AddMinutes(space);
            }
            else if (m < space)//时间差小于3个小时，则在下一天截止
            {
                n = space - m;
            }

            DateTime temp = dt1;
            while (n > 0)
            {
                temp = temp.AddDays(1); //加一天
                string strTempDate = temp.ToString().Split(' ')[0]; //取下一天的日期
                string[] st = strTempDate.Split('/');
                string year = st[0];
                string month = "";
                string day = "";
                if (st[1].Length == 1)
                {
                    month = "0" + st[1];
                }
                else
                {
                    month = st[1];
                }
                if (st[2].Length == 1)
                {
                    day = "0" + st[2];
                }
                else
                {
                    day = st[2];
                }
                string s = year + month + day;
                //节假日 API http://api.goseek.cn/
                string strUrl = "http://api.goseek.cn/Tools/holiday?date=" + s;
                Uri uri = new Uri(strUrl);
                WebRequest req = WebRequest.Create(uri);
                WebResponse resp = req.GetResponse();
                StreamReader reader = new StreamReader(resp.GetResponseStream(), Encoding.ASCII);
                string strTemp = reader.ReadToEnd();
                strTemp = strTemp.Split(',')[1].Split(':')[1];
                //0 为工作日， 1 为休息日，2 为节假日
                if (strTemp.Contains('0'))
                {
					//当天的上班时间
                    DateTime dt3 = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), 8, 30, 00);
                    endDT = dt3.AddMinutes(n);
                    break;
                }
            }
            return endDT;
        }

        //Main方法
        static void Main(string[] args)
        {

            Console.WriteLine("输入日期（示例：2018/7/13 16:00:00），按回车结束：");
            string str = Console.ReadLine();
            Time time = new Time();
            time.IsOverdue(str);
            Console.WriteLine();
            Console.WriteLine("Deadlin：");
            Console.WriteLine(time.endDT);

            Console.ReadKey();
        }
    }
}
