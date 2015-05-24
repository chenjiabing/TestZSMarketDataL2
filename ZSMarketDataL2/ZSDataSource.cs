using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDFAPI;

namespace ZSMarketData
{
    public class ZSDataSource : TDFDataSource
    {
        MarketDataL2 mdl2Inter;

        /// <summary>
        ///  构造函数（非代理连接设置）
        /// </summary>
        /// <param name="openSetting"></param>
        public ZSDataSource(MarketDataL2 mdl2, TDFOpenSetting openSetting)
            : base(openSetting)
        {
            mdl2Inter = mdl2;
        }

        /// <summary>
        /// 接收系统消息
        /// 1. 不要在这个函数里做耗时操作
        /// 2. 只在这个函数里做数据获取工作 -- 将数据复制到其它数据缓存区，由其它线程做业务逻辑处理
        /// </summary>
        /// <param name="msg"></param>
        public override void OnRecvSysMsg(TDFMSG msg)
        {
            // 处理数据接收消息
            switch (msg.MsgID)
            {
                // 网络连接回调
                case TDFMSGID.MSG_SYS_CONNECT_RESULT:
                    {
                        TDFConnectResult connectResult = msg.Data as TDFConnectResult;

                        // 网络连接结果
                        if (connectResult.ConnResult)
                        {
                            // 网络连接成功回调
                            mdl2Inter.OnConnectted(connectResult);
                        }
                        else
                        {
                            // 网络断开回调
                            mdl2Inter.OnDisconnnectted();
                        }

                        break;
                    }
                // 网络断开消息
                case TDFMSGID.MSG_SYS_DISCONNECT_NETWORK:
                    {
                        // 网络断开回调
                        mdl2Inter.OnDisconnnectted();
                        break;
                    }
                // 登录回调
                case TDFMSGID.MSG_SYS_LOGIN_RESULT:
                    {
                        TDFLoginResult loginResult = msg.Data as TDFLoginResult;

                        // 登录结果回调
                        mdl2Inter.OnLogin(loginResult);

                        break;
                    }
                // 交易日期改变回调
                case TDFMSGID.MSG_SYS_QUOTATIONDATE_CHANGE:
                    {
                        //行情日期变更。
                        TDFQuotationDateChange quotationChange = msg.Data as TDFQuotationDateChange;

                        // 行情日期变更回调
                        mdl2Inter.OnQuotationDateChanged(quotationChange);

                        break;
                    }
                // 闭市通知回调
                case TDFMSGID.MSG_SYS_MARKET_CLOSE:
                    {
                        TDFMarketClose marketClose = msg.Data as TDFMarketClose;

                        // 闭市回调
                        mdl2Inter.OnMarketClose(marketClose);

                        break;
                    }
                default:
                    {
                        Console.WriteLine("OnRecvSysMsg 未知消息 {0]", msg.MsgID);
                        break;
                    }

            }
        }

        /// <summary>
        /// 接收行情数据
        /// </summary>
        /// <param name="msg"></param>
        public override void OnRecvDataMsg(TDFMSG msg)
        {
            // 处理数据接收消息
            switch (msg.MsgID)
            {
                // 行情数据更新回调
                case TDFMSGID.MSG_DATA_MARKET:
                    {
                        // 行情消息
                        TDFMarketData[] marketDataArr = msg.Data as TDFMarketData[];

                        // 逐个数据回调
                        foreach (TDFMarketData data in marketDataArr)
                        {
                            mdl2Inter.OnMarketedDataUpdate(data);
                        }

                        break;
                    }
                // 指数数据接收
                case TDFMSGID.MSG_DATA_INDEX:
                    {
                        // 指数消息
                        TDFIndexData[] indexDataArr = msg.Data as TDFIndexData[];
                        // 逐个数据回调
                        foreach (TDFIndexData data in indexDataArr)
                        {
                            mdl2Inter.OnIndexDataUpdate(data);
                        }

                        break;
                    }
                // 期货数据接收
                case TDFMSGID.MSG_DATA_FUTURE:
                    {
                        //期货行情消息
                        TDFFutureData[] futureDataArr = msg.Data as TDFFutureData[];
                        foreach (TDFFutureData data in futureDataArr)
                        {
                            mdl2Inter.OnFutureDataUpdate(data);
                        }

                        break;
                    }
                // 逐笔成交数据更新回调
                case TDFMSGID.MSG_DATA_TRANSACTION:
                    {
                        // 逐笔成交
                        TDFTransaction[] transactionDataArr = msg.Data as TDFTransaction[];
                        foreach (TDFTransaction data in transactionDataArr)
                        {
                            mdl2Inter.OnTransactionDataUpdate(data);
                        }

                        break;
                    }
                // 逐笔委托数据更新回调
                case TDFMSGID.MSG_DATA_ORDER:
                    {
                        //逐笔委托
                        TDFOrder[] orderDataArr = msg.Data as TDFOrder[];
                        foreach (TDFOrder data in orderDataArr)
                        {
                            mdl2Inter.OnOrderDataUpdate(data);
                        }

                        break;
                    }
                default:
                    {
                        Console.WriteLine("OnRecvDataMsg: 未知消息 {0]", msg.MsgID);
                        break;
                    }
            }
        }
    }
}
