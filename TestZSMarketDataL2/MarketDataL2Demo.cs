using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDFAPI;
using ZSMarketData;

namespace TestZSMarketDataL2
{
    class MarketDataL2Demo : MarketDataL2
    {

        /// <summary>
        ///  构造函数（非代理连接设置）
        /// </summary>
        /// <param name="openSetting"></param>
        public MarketDataL2Demo(string ip, int port, string userName, string password)
            : base(ip, port, userName, password)
        {
        }



        /// <summary>
        /// 行情数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override int OnMarketedDataUpdate(TDFMarketData data)
        {
            // 数据存入队列
            this.marketDataList.Enqueue(data);

            // 输出
            string value = String.Format("个股 {0} {1:0.00} {2:##:##:##:###}", data.Code, (double)data.Match / 10000, data.Time);

            Console.WriteLine(value);

            return 0;
        }

        /// <summary>
        /// 指数数据更新
        /// </summary>
        /// <returns></returns>
        public override int OnIndexDataUpdate(TDFIndexData data)
        {
            // 数据存入队列
            this.indexDataList.Enqueue(data);

            //
            index300Data = data;

            // 输出
            string value = String.Format("指数 {0} {1:0.00} {2:##:##:##:###}", data.Code, (double)data.LastIndex / 10000, data.Time);

            Console.WriteLine(value);

            return 0;
        }

        /// <summary>
        /// 指数数据更新
        /// </summary>
        /// <returns></returns>
        public override int OnFutureDataUpdate(TDFFutureData data)
        {
            FutureData futureData = new FutureData();

            futureData.futureData = data;
            futureData.indexData = this.index300Data;
            if (data.Time > 92500000)
            {
                futureData.deltaValue = (int)futureData.futureData.Match - futureData.indexData.LastIndex;
            }

            // 数据存入队列
            futureDataList.Enqueue(futureData);

            // 输出
            //string value = "期货 " + data.Code + " " + data.Match + " " + data.Time;
            string value = String.Format("期货 {0} {1:0.00}  {2:0.00} {3:##:##:##:###} {4:0.00} {5:##:##:##:###}",
                data.Code,
                (double)futureData.deltaValue / 10000, 
                (double)data.Match / 10000,
                data.Time, 
                (double)futureData.indexData.LastIndex / 10000,
                futureData.indexData.Time);

           // Console.WriteLine(value);
            Program.log.Debug(value);

            return 0;
        }

        /// <summary>
        /// 网络连接
        /// </summary>
        public override int OnConnectted(TDFConnectResult connectResult)
        {
            string value = connectResult.Username + "已连接";

            Console.WriteLine(value);
            Program.log.Info(value);

            return 0;
        }

        /// <summary>
        /// 网络断开
        /// </summary>
        /// <returns></returns>
        public override int OnDisconnnectted()
        {

            string value = "网络已断开";
            Program.log.Info(value);

            return 0;
        }

        /// <summary>
        ///  登录成功
        /// </summary>
        /// <returns></returns>
        public override int OnLogin(TDFLoginResult loginResult)
        {
            string value = loginResult.Info;
            if (loginResult.LoginResult)
            {
                value += "已登录";
            }
            else
            {
                value += "已登出";
            }

            Program.log.Info(value);
            return 0;
        }

        /// <summary>
        /// 行情日期改变
        /// </summary>
        /// <returns></returns>
        public override int OnQuotationDateChanged(TDFQuotationDateChange quotationChange)
        {

            string value = "OnQuotationDateChanged" +  quotationChange.Market.ToString() + quotationChange.NewDate + " " + quotationChange.OldDate;

            Program.log.Info(value);

            return 0;
        }

        /// <summary>
        /// 闭市
        /// </summary>
        /// <returns></returns>
        public override int OnMarketClose(TDFMarketClose marketClose)
        {

            string value = string.Format("{0} {1:##:##:##:###} {2}", marketClose.Market, marketClose.Time , marketClose.Info);

            Program.log.Info(value);

            return 0;
        }

        // 实时指数行情
        private TDFIndexData index300Data;
        // 期货指数数据
        public Queue<FutureData> futureDataList = new Queue<FutureData>();

        // 委托队列数据
        public Queue<TDFIndexData> indexDataList = new Queue<TDFIndexData>();

        // 行情队列数据
        public Queue<TDFMarketData> marketDataList = new Queue<TDFMarketData>();
    }
}

public struct FutureData
{
    // 期货数据
    public TDFFutureData futureData;
    // 指数数据
    public TDFIndexData indexData;
    // 差值
    public int deltaValue;

}
