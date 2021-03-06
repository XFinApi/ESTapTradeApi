﻿/*******************************************************
* ESTapTradeApiSimpleDemoCSharp2015
* www.xfinapi.com
*******************************************************/
using System;
using System.Threading;
using XFinApi.TradeApi;

namespace ESTapTradeApiSimpleDemoCSharp2015
{
    class Program
    {
        //////////////////////////////////////////////////////////////////////////////////
        //配置信息
        class Config
        {
            //注册易盛内盘9.0仿真交易账号，http://www.esunny.com.cn/index.php?m=content&c=index&a=lists&catid=49

            //地址
            public string MarketAddress = "123.161.206.213:6161";
            public string TradeAddress = "123.161.206.213:6160";

            //账户
            public string MarketAuthCode = "B112F916FE7D27BCE7B97EB620206457946CED32E26C1EAC946CED32E26C1EAC946CED32E26C1EAC946CED32E26C1EAC5211AF9FEE541DDE9D6F622F72E25D5DEF7F47AA93A738EF5A51B81D8526AB6A9D19E65B41F59D6A946CED32E26C1EACCAF8D4C61E28E2B1ABD9B8F170E14F8847D3EA0BF4E191F5DCB1B791E63DC196D1576DEAF5EC563CA3E560313C0C3411B45076795F550EB050A62C4F74D5892D2D14892E812723FAC858DEBD8D4AF9410729FB849D5D8D6EA48A1B8DC67E037381A279CE9426070929D5DA085659772E24A6F5EA52CF92A4D403F9E46083F27B19A88AD99812DADA44100324759F9FD1964EBD4F2F0FB50B51CD31C0B02BB437";
            public string TradeAuthCode = "67EA896065459BECDFDB924B29CB7DF1946CED32E26C1EAC946CED32E26C1EAC946CED32E26C1EAC946CED32E26C1EAC5211AF9FEE541DDE41BCBAB68D525B0D111A0884D847D57163FF7F329FA574E7946CED32E26C1EAC946CED32E26C1EAC733827B0CE853869ABD9B8F170E14F8847D3EA0BF4E191F5D97B3DFE4CCB1F01842DD2B3EA2F4B20CAD19B8347719B7E20EA1FA7A3D1BFEFF22290F4B5C43E6C520ED5A40EC1D50ACDF342F46A92CCF87AEE6D73542C42EC17818349C7DEDAB0E4DB16977714F873D505029E27B3D57EB92D5BEDA0A710197EB67F94BB1892B30F58A3F211D9C3B3839BE2D73FD08DD776B9188654853DDA57675EBB7D6FBBFC";
            public string MarketUserName = "ES";
            public string MarketPassword = "123456";
            public string TradeUserName = "Q1203070045";//公用测试账户。为了测试准确，请注册使用您自己的账户。
            public string TradePassword = "a123456";

            //合约
            public string ExchangeID = "ZCE";
            public string ProductID = "MA";
            public string InstrumentID = "812";

            //行情
            public double SellPrice1 = -1;
            public double BuyPrice1 = -1;

        };

        //////////////////////////////////////////////////////////////////////////////////
        // 变量
        static Config Cfg = new Config();
        static IMarket market;
        static ITrade trade;
        static MarketEvent marketEvent;
        static TradeEvent tradeEvent;

        //////////////////////////////////////////////////////////////////////////////////
        // 输出
        static void PrintNotifyInfo(NotifyParams param)
        {
            string strs = "";
            for (int i = 0; i < param.CodeInfos.Count; i++)
            {
                CodeInfo info = param.CodeInfos[i];
                strs += "(Code=" + info.Code +
            ";LowerCode=" + info.LowerCode +
            ";LowerMessage=" + info.LowerMessage + ")";
            }

            Console.WriteLine(string.Format(" OnNotify: Action={0:d}, Result={1:d}{2}", param.ActionType, param.ResultType, strs));
        }

        static void PrintSubscribedInfo(QueryParams instInfo)
        {
            Console.WriteLine(string.Format("- OnSubscribed: {0} {1} {2}", instInfo.ExchangeID, instInfo.ProductID, instInfo.InstrumentID));
        }

        static void PrintUnsubscribedInfo(QueryParams instInfo)
        {
            Console.WriteLine(string.Format("- OnUnsubscribed: {0} {1} {2}", instInfo.ExchangeID, instInfo.ProductID, instInfo.InstrumentID));
        }

        static void PrintTickInfo(Tick tick)
        {
            Console.WriteLine(string.Format(" Tick,{0} {1} {2}, HighestPrice={3:g}, LowestPrice={4:g}, BidPrice0={5:g}, BidVolume0={6:d}, AskPrice0={7:g}, AskVolume0={8:d}, LastPrice={9:g}, LastVolume={10:d}, TradingDay={11}, TradingTime={12}",
                tick.ExchangeID,
                tick.ProductID,
                tick.InstrumentID,
                tick.HighestPrice,
                tick.LowestPrice,
                tick.GetBidPrice(0),
                tick.GetBidVolume(0),
                tick.GetAskPrice(0),
                tick.GetAskVolume(0),
                tick.LastPrice,
                tick.LastVolume,
                tick.TradingDay,
                tick.TradingTime));
        }

        static void PrintOrderInfo(Order order)
        {
            Console.WriteLine(string.Format("  ProductType={0:d}, Ref{1}, ID={2}, InstID={3} {4} {5}, Price={6:g}, Volume={7:d}, NoTradedVolume={8:d}, Direction={9:d}, OpenCloseType={10:d}, PriceCond={11:d}, TimeCond={12:d}, VolumeCond={13:d}, Status={14:d}, Msg={15}, {16}",
                order.ProductType,
                order.OrderRef, order.OrderID,
                order.ExchangeID, order.ProductID, order.InstrumentID,
                order.Price, order.Volume, order.NoTradedVolume,
                order.Direction, order.OpenCloseType,
                order.PriceCond,
                order.TimeCond,
                order.VolumeCond,
                order.Status,
                order.StatusMsg,
                order.OrderTime));
        }

        static void PrintTradeInfo(TradeOrder trade)
        {
            Console.WriteLine(string.Format("  ID={0}, OrderID={1}, InstID={2} {3} {4}, Price={5:g}, Volume={6:d}, Direction={7:d}, OpenCloseType={8:d}, {9}",
                trade.TradeID, trade.OrderID,
                trade.ExchangeID, trade.ProductID, trade.InstrumentID,
                trade.Price, trade.Volume,
                trade.Direction, trade.OpenCloseType,
                trade.TradeTime));
        }

        static void PrintInstrumentInfo(Instrument inst)
        {
            Console.WriteLine(string.Format(" ExchangeID={0}, ProductID={1}, ID={2}",
                inst.ExchangeID, inst.ProductID,
                inst.InstrumentID));
        }

        static void PrintPositionInfo(Position pos)
        {
            long buyposition = ITradeApi.IsDefaultValue(pos.BuyPosition) ? -1 : pos.BuyPosition;
            long sellposition = ITradeApi.IsDefaultValue(pos.SellPosition) ? -1 : pos.SellPosition;
            long netposition = ITradeApi.IsDefaultValue(pos.NetPosition) ? -1 : pos.NetPosition;
            Console.WriteLine(string.Format(" InstID={0} {1} {2}, BuyPosition={3:d}, SellPosition={4:d}, NetPosition={5:d}",
                pos.ExchangeID, pos.ProductID, pos.InstrumentID,
                buyposition, sellposition, netposition
                ));
        }

        static void PrintAccountInfo(Account acc)
        {
            Console.WriteLine(string.Format("   Balance={0:g}, Available={1:g}, FrozenCommission={2:g}, FrozenMargin={3:g}, Commission={4:g}, MaintenanceMargin={5:g}, PositionProfit={6:g}\n",
                acc.Balance, acc.Available,
                acc.FrozenCommission, acc.FrozenMargin, acc.Commission, acc.MaintenanceMargin,
                acc.PositionProfit));
        }

        //////////////////////////////////////////////////////////////////////////////////
        //API 创建失败错误码的含义，其他错误码的含义参见XTA_W32\Cpp\ApiEnum.h文件
        static string[] StrCreateErrors = {
            "无错误",
            "头文件与接口版本不匹配",
            "头文件与实现版本不匹配",
            "实现加载失败",
            "实现入口未找到",
            "创建实例失败",
            "无授权文件",
            "授权版本不符",
            "最后一次通信超限",
            "机器码错误",
            "认证文件到期",
            "认证超时"

        };

        //////////////////////////////////////////////////////////////////////////////////
        //行情事件
        public class MarketEvent : MarketListener
        {
            public override void OnNotify(NotifyParams notifyParams)
            {

                Console.WriteLine("* Market");

                PrintNotifyInfo(notifyParams);

                //连接成功后可订阅合约
                if ((int)XFinApi.TradeApi.ActionKind.Open == notifyParams.ActionType &&
                    (int)ResultKind.Success == notifyParams.ResultType && market != null)
                {
                    //订阅
                    QueryParams param = new QueryParams();
                    param.ExchangeID = Cfg.ExchangeID;
                    param.ProductID = Cfg.ProductID;
                    param.InstrumentID = Cfg.InstrumentID;
                    market.Subscribe(param);
                }

                //ToDo ...
            }

            public override void OnSubscribed(QueryParams instInfo)
            {

                PrintSubscribedInfo(instInfo);

                //ToDo ...
            }

            public override void OnUnsubscribed(QueryParams instInfo)
            {

                PrintUnsubscribedInfo(instInfo);

                //ToDo ...
            }

            public override void OnTick(Tick tick)
            {
                if (Cfg.SellPrice1 <= 0 && Cfg.BuyPrice1 <= 0)
                    PrintTickInfo(tick);

                Cfg.SellPrice1 = tick.GetAskPrice(0);
                Cfg.BuyPrice1 = tick.GetBidPrice(0);

                //ToDo ...
            }
        };

        //////////////////////////////////////////////////////////////////////////////////
        //交易事件
        public class TradeEvent : TradeListener
        {
            public override void OnNotify(NotifyParams notifyParams)
            {

                Console.WriteLine("* Trade");

                PrintNotifyInfo(notifyParams);

                //ToDo ...
            }

            public override void OnUpdateOrder(Order order)
            {

                Console.WriteLine("- OnUpdateOrder:");

                PrintOrderInfo(order);

                //ToDo ...
            }

            public override void OnUpdateTradeOrder(TradeOrder trade)
            {

                Console.WriteLine("- OnUpdateTradeOrder:");

                PrintTradeInfo(trade);

                //ToDo ...
            }

            public override void OnQueryOrder(OrderList orders)
            {

                Console.WriteLine("- OnQueryOrder:");

                for (int i = 0; i < orders.Count; i++)
                {
                    Order order = orders[i];
                    PrintOrderInfo(order);

                    //ToDo ...
                }
            }

            public override void OnQueryTradeOrder(TradeOrderList trades)
            {

                Console.WriteLine("- OnQueryTradeOrder:");

                for (int i = 0; i < trades.Count; i++)
                {
                    TradeOrder trade = trades[i];
                    PrintTradeInfo(trade);

                    //ToDo ...
                }
            }

            public override void OnQueryInstrument(InstrumentList insts)
            {

                Console.WriteLine("- OnQueryInstrument:");

                for (int i = 0; i < insts.Count; i++)
                {
                    Instrument inst = insts[i];
                    PrintInstrumentInfo(inst);

                    //ToDo ...
                }
            }

            public override void OnQueryPosition(PositionList posInfos)
            {
                Console.WriteLine("- OnQueryPosition:");
                for (int i = 0; i < posInfos.Count; i++)
                {
                    Position pos = posInfos[i];
                    PrintPositionInfo(pos);
                }

                //ToDo ...
            }

            public override void OnQueryAccount(Account accInfo)
            {
                Console.WriteLine("- OnQueryAccount:");

                PrintAccountInfo(accInfo);

                //ToDo ...
            }
        };

        //////////////////////////////////////////////////////////////////////////////////
        //行情测试
        static void MarketTest()
        {
            //创建 IMarket
            // char* path 指 xxx.exe 同级子目录中的 xxx.dll 文件
            int err = -1;

            market = ITradeApi.XFinApi_CreateMarketApi("XTA_W32/Api/ESTap_v9.0.3.11/XFinApi.ESTapTradeApi.dll", out err);


            if (err > 0 || market == null)
            {
                Console.WriteLine(string.Format("* Market XFinApiCreateError={0};", StrCreateErrors[err]));
                return;
            }

            //注册事件
            marketEvent = new MarketEvent();
            market.SetListener(marketEvent);

            //连接服务器
            OpenParams openParams = new OpenParams();
            openParams.Configs.Add("AuthCode", Cfg.MarketAuthCode);
            openParams.HostAddress = Cfg.MarketAddress;
            openParams.UserID = Cfg.MarketUserName;
            openParams.Password = Cfg.MarketPassword;
            market.Open(openParams);

            /*
            连接成功后才能执行订阅行情等操作，检测方法有两种：
            1、IMarket.IsOpened()=true
            2、MarketListener.OnNotify中
            (int)XFinApi.TradeApi.ActionKind.Open == notifyParams.ActionType &&
            (int)ResultKind.Success == notifyParams.ResultType
            */

            /* 行情相关方法
            while (!market.IsOpened())
                 Thread.Sleep(1000);

            //订阅行情，已在MarketEvent.OnNotify中订阅
            XFinApi.QueryParams param = new XFinApi.QueryParams();
            param.ExchangeID = Cfg.Exception;
            param.ProductID = Cfg.ProductID;
            param.InstrumentID = Cfg.InstrumentID;
            market.Subscribe(param);

            //取消订阅行情
            market.Unsubscribe(param);
            */
        }

        //////////////////////////////////////////////////////////////////////////////////
        //交易测试
        static void TradeTest()
        {
            //创建 ITrade
            // char* path 指 xxx.exe 同级子目录中的 xxx.dll 文件
            int err = -1;

            trade = ITradeApi.XFinApi_CreateTradeApi("XTA_W32/Api/ESTap_v9.0.3.11/XFinApi.ESTapTradeApi.dll", out err);

            if (err > 0 || trade == null)
            {
                Console.WriteLine(string.Format("* Trade XFinApiCreateError={0};", StrCreateErrors[err]));
                return;
            }

            //注册事件
            tradeEvent = new TradeEvent();
            trade.SetListener(tradeEvent);

            //连接服务器
            OpenParams openParams = new OpenParams();
            openParams.Configs.Add("AuthCode", Cfg.TradeAuthCode);
            openParams.HostAddress = Cfg.TradeAddress;
            openParams.UserID = Cfg.TradeUserName;
            openParams.Password = Cfg.TradePassword;
            openParams.IsUTF8 = true;
            trade.Open(openParams);

            /*
            //连接成功后才能执行查询、委托等操作，检测方法有两种：
            1、ITrade.IsOpened()=true
            2、TradeListener.OnNotify中
            (int)XFinApi.ActionType.Open == notifyParams.ActionType &&
            (int)XFinApi.Result.Success == notifyParams.Result
             */

            while (!trade.IsOpened())
                Thread.Sleep(1000);

            QueryParams qryParam = new QueryParams();

            //查询委托单
            Thread.Sleep(1000);//有些接口查询有间隔限制，如：CTP查询间隔为1秒
            Console.WriteLine("Press any key to QueryOrder.");
            Console.ReadKey();
            trade.QueryOrder(qryParam);

            //查询成交单
            Thread.Sleep(3000);
            Console.WriteLine("Press any key to QueryTradeOrder.");
            Console.ReadKey();
            trade.QueryTradeOrder(qryParam);

            //查询合约
            Thread.Sleep(3000);
            qryParam.ExchangeID = Cfg.ExchangeID;
            qryParam.ProductID = Cfg.ProductID;
            Console.WriteLine("Press any key to QueryInstrument.");
            Console.ReadKey();
            trade.QueryInstrument(qryParam);

            //查询持仓
            Thread.Sleep(3000);
            Console.WriteLine("Press any key to QueryPosition.");
            Console.ReadKey();
            trade.QueryPosition(qryParam);

            //查询账户
            Thread.Sleep(3000);
            Console.WriteLine("Press any key to QueryAccount.");
            Console.ReadKey();
            trade.QueryAccount(qryParam);

            //委托下单
            Thread.Sleep(1000);
            Console.WriteLine("Press any key to OrderAction.");
            Console.ReadKey();
            Order order = new Order();
            order.ExchangeID = Cfg.ExchangeID;
            order.ProductID = Cfg.ProductID;
            order.InstrumentID = Cfg.InstrumentID;
            order.Price = Cfg.SellPrice1;
            order.Volume = 1;
            order.Direction = DirectionKind.Buy;
            order.OpenCloseType = OpenCloseKind.Open;

            //下单高级选项，可选择性设置
            order.ActionType = OrderActionKind.Insert;//下单
            order.OrderType = OrderKind.Order;//标准单
            order.PriceCond = PriceConditionKind.LimitPrice;//限价
            order.VolumeCond = VolumeConditionKind.AnyVolume;//任意数量
            order.TimeCond = TimeConditionKind.GFD;//当日有效
            order.ContingentCond = ContingentCondKind.Immediately;//立即
            order.HedgeType = HedgeKind.Speculation;//投机
            order.ExecResult = ExecResultKind.NoExec;//没有执行

            trade.OrderAction(order);
        }

        static void Main(string[] args)
        {
            //可在Config类中修改用户名、密码、合约等信息
            MarketTest();
            TradeTest();

            Thread.Sleep(2000);
            Console.WriteLine("Press any key to close.");
            Console.ReadKey();

            //关闭连接
            if (market != null)
            {
                market.Close();
                ITradeApi.XFinApi_ReleaseMarketApi(market);//必须释放资源
            }
            if (trade != null)
            {
                trade.Close();
                ITradeApi.XFinApi_ReleaseTradeApi(trade);//必须释放资源
            }

            Console.WriteLine("Closed.");
            Console.ReadKey();
        }
    }
}

