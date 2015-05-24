using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using TDFAPI;
using ZSMarketData;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)] 
namespace TestZSMarketDataL2
{
    class Program
    {
        static public ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            int status = 0;
            string[] stocks = { "000300.sh;IF1506.cf" };

            string ip       = System.Configuration.ConfigurationManager.AppSettings["IP"];
            int    port      = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);
            string user      =  System.Configuration.ConfigurationManager.AppSettings["Username"];
            string password  = System.Configuration.ConfigurationManager.AppSettings["Password"];

            log.Info("测试程序已启动");

            MarketDataL2Demo mdl2 = new MarketDataL2Demo(ip, port, user, password);

            // 启动行情
            status = mdl2.Init();
            if (status != 0)
            {
                log.Info("行情初始化失败,连接地址:" + ip + " 端口:" + port +  "用户名:" + user  + "，错误代码:" + status);
            }
            else
            {
                log.Info("行情初始化成功" + ip + " 端口:" + port +  "用户名:" + user  + ", 按q键退出.");
            }

            // 订阅
            status = mdl2.MarketDataSubscriptionSet(stocks);
            if (status != 0)
            {
                log.Info("行情订阅失败，错误代码:" + status);
            }

            string input;
            while (true)
            {
                input = Console.ReadLine();
                switch (input[0])
                {
                    case 'd':
                        {
                            mdl2.MarketDataSubscriptionDelete(stocks);
                            break;
                        }
                    case 'a':
                        {
                            mdl2.MarketDataSubscriptionAdd(stocks);
                            break;
                        }
                    case 'f':
                        {
                            mdl2.MarketDataSubsribtionFull();
                            break;
                        }
                    case 's':
                        {
                            string[] stocks2 = { "600000.SH", "000002.SZ" };
                            mdl2.MarketDataSubscriptionSet(stocks2);
                            break;
                        }
                    case 'p':
                        {
                            //foreach(FutureData fd in mdl2.futureDataList.r)
                            // 打印队列数据
                            while (mdl2.futureDataList.Count > 0)
                            {
                                FutureData data = mdl2.futureDataList.Dequeue();
                                string value = String.Format("期货 {0} {1:0.00}  {2:0.00} {3:##:##:##:###} {4:0.00} {5:##:##:##:###}", 
                                                            data.futureData.Code,
                                                            (double)data.deltaValue / 10000,
                                                            (double)data.futureData.Match / 10000, 
                                                            data.futureData.Time, 
                                                            (double)data.indexData.LastIndex / 10000,
                                                            data.indexData.Time);

                                log.Debug(value);
                            }

                            break;
                        }
                    case 'q':
                    {
                        mdl2.UnInit();
                        return;
                    }
                }
            }

        }
    }
}
