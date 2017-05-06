using System;
using System.Collections.Generic;
using System.Text;
using IpHlpApidotnet;
using Tamir.IPLib;
using Tamir.IPLib.Packets;
using log4net;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.IO;
using IPNumbers;

namespace AppTrafficMonitor
{
    public enum Direction {Upload, Download, None};

    public class ExTCPUDPConnection : TCPUDPConnection
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ExTCPUDPConnection));


        public ExTCPUDPConnection(TCPUDPConnections conns, Packet packet)
            : base(conns)
        {
            _Packet = packet;
            this._IsValidConnection = FillConnection(packet);
        }

        public ExTCPUDPConnection(TCPUDPConnections conns)
            : base(conns)
        {

        }

        protected override void ClearParams(bool Refresh)
        {
            if (Refresh)
            {
                this._DownloadedReal = 0;
                this._DownloadedUseful = 0;
                this._UploadedReal = 0;
                this._UploadedUseful = 0;
            }
        }

        public virtual bool IsCalculateCosts(System.Net.IPAddress RemoteIP)
        {
            this._IsCalcCosts = this.Parent.IsCalculateCosts(RemoteIP);
            return this._IsCalcCosts;
        }

        private bool _IsCalcCosts = false;
        public bool IsCalcCosts
        {
            get { return _IsCalcCosts; }
        }

        private Packet _Packet; 
        public Packet Packet
        {
            get { return _Packet; }
        }

        private bool _IsValidConnection = true;
        public bool IsValidConnection
        {
            get { return _IsValidConnection; }
        }

        private int _UploadedReal = 0;
        public int UploadedReal
        {
            get { return _UploadedReal; }
        }

        private int _DownloadedReal = 0;
        public int DownloadedReal
        {
            get { return _DownloadedReal; }
        }

        private int _DownloadedUseful = 0;
        public int DownloadedUseful
        {
            get { return _DownloadedUseful; }
        }

        private int _UploadedUseful = 0;
        public int UploadedUseful
        {
            get { return _UploadedUseful; }
        }

        private int _UploadedRealTotal = 0;
        public int UploadedRealTotal
        {
            get { return _UploadedRealTotal; }
        }

        private int _DownloadedRealTotal = 0;
        public int DownloadedRealTotal
        {
            get { return _DownloadedRealTotal; }
        }

        private int _DownloadedUsefulTotal = 0;
        public int DownloadedUsefulTotal
        {
            get { return _DownloadedUsefulTotal; }
        }

        private int _UploadedUsefulTotal = 0;
        public int UploadedUsefulTotal
        {
            get { return _UploadedUsefulTotal; }
        }

        private int _PacketsIn = 0;
        public int PacketsIn
        {
            get { return _PacketsIn; }
        }

        private int _PacketsOut = 0;
        public int PacketsOut
        {
            get { return _PacketsOut; }
        }

        private DateTime _Time = DateTime.MinValue;
        public DateTime Time
        {
            get { return _Time; }
        }

        private Direction _Direction;
        public Direction Direction
        {
            get { return _Direction; }
        }

        public void Clone(ExTCPUDPConnection conn)
        {
            this.WasActiveAt = conn.WasActiveAt;
            this.iState = conn.iState;
            //this.PID = conn.PID;
            //this.ProcessName = conn.ProcessName;
            this._IsValidConnection = this.FillConnection(conn.Packet);
        }

        public bool FillConnection(Packet packet)
        {
            if (packet == null)
                return false;
            IPEndPoint srcAddress = null;
            IPEndPoint dstAddress = null;
            int UsefulTraffic = 0;//Length;
            int RealTraffic = 0;//Length;
            this._Direction = Direction.None;
            this._IsValidConnection = false;
            this._Time = packet.PcapHeader.Date;

            if (packet is TCPPacket)
            {
                srcAddress = new IPEndPoint(new System.Net.IPAddress((packet as TCPPacket).SourceAddressBytes), (packet as TCPPacket).SourcePort);
                dstAddress = new IPEndPoint(new System.Net.IPAddress((packet as TCPPacket).DestinationAddressBytes), (packet as TCPPacket).DestinationPort);

                if (Utils.IsLocalIPAddress(srcAddress.Address) &&
                    Utils.IsLocalIPAddress(dstAddress.Address))
                {
                    return this._IsValidConnection;
                }
                UsefulTraffic = (packet as TCPPacket).PayloadDataLength;//Length;
                RealTraffic = (packet as TCPPacket).Length;//Length;
                this.Protocol = Protocol.TCP;
            }
            else
            {
                if (packet is UDPPacket)
                {
                    srcAddress = new IPEndPoint(new System.Net.IPAddress((packet as UDPPacket).SourceAddressBytes), (packet as UDPPacket).SourcePort);
                    dstAddress = new IPEndPoint(new System.Net.IPAddress((packet as UDPPacket).DestinationAddressBytes), (packet as UDPPacket).DestinationPort);

                    UsefulTraffic = (packet as UDPPacket).Length - (packet as UDPPacket).UDPHeader.Length;//Length;
                    RealTraffic = (packet as UDPPacket).Length;//Length;
                    this.Protocol = Protocol.UDP;
                }
            }

            if (Utils.IsLocalIPAddress(srcAddress.Address))
            {
                if (srcAddress.Port == 0)
                {
                    return this._IsValidConnection;
                }
                this.Local = srcAddress;
                this.Remote = dstAddress;
                this._Direction = Direction.Upload;
                this._IsValidConnection = true;
                this.IsCalculateCosts(this.Remote.Address);
                this.CalcTraffic(UsefulTraffic, RealTraffic);
                return this._IsValidConnection;
            }
            else
            {
                if (Utils.IsLocalIPAddress(dstAddress.Address))
                {
                    if (dstAddress.Port == 0)
                    {
                        return this._IsValidConnection;
                    }
                    this.Local = dstAddress;
                    this.Remote = srcAddress;
                    this._Direction = Direction.Download;
                    this._IsValidConnection = true;
                    this.IsCalculateCosts(this.Remote.Address);
                    this.CalcTraffic(UsefulTraffic, RealTraffic);
                    return this._IsValidConnection;
                }
            }
            return this._IsValidConnection;
        }

        public void CalcTraffic(int UsefulLength, int RealLength)
        {
            if (UsefulLength != 0 || RealLength != 0)
            {
                if (this.Direction == Direction.Upload)
                {
                    this._UploadedReal = RealLength;
                    this._UploadedUseful = UsefulLength;
                    this._UploadedUsefulTotal += UsefulLength;
                    this._UploadedRealTotal += RealLength;
                    this._PacketsOut++;
                }
                else
                {
                    this._DownloadedReal = RealLength;
                    this._DownloadedUseful = UsefulLength;
                    this._DownloadedUsefulTotal += UsefulLength;
                    this._DownloadedRealTotal += RealLength;
                    this._PacketsIn++;
                }
            }
        }
    }

    public class AppTCPUDPConnection
    {
        SpeedCalculator _speedcalc = null;

        List<int> _usedtcpports = new List<int>();
        List<int> _usedudpports = new List<int>();
        MonitoredApplications _parent = null;

        public AppTCPUDPConnection(MonitoredApplications Parent)
        {
            _parent = Parent;
            _speedcalc = new SpeedCalculator(this);
        }

        private string _UsedPorts = String.Empty;
        public string UsedPorts
        {
            get 
            {
                this.MakeUsedPorts();
                return _UsedPorts;
            }
        }

        private void MakeUsedPorts()
        {
            string prefix = String.Empty;
            _UsedPorts = String.Empty;
            foreach (int port in this.UsedTcpPortsList)
            {
                if (_UsedPorts != String.Empty)
                    prefix = ", ";
                _UsedPorts += prefix + "T" + port.ToString();
            }
            foreach (int port in this.UsedUdpPortsList)
            {
                if (_UsedPorts != String.Empty)
                    prefix = ", ";
                _UsedPorts += prefix + "U" + port.ToString();
            }
        }

        public void AddUsedPort(IPEndPoint addr, Protocol protocol)
        {
            if (protocol == Protocol.TCP)
            {
                if (_usedtcpports.IndexOf(addr.Port) < 0)
                {
                    _usedtcpports.Add(addr.Port);
                }
            }
            else
                if (protocol == Protocol.UDP)
                {
                    if (_usedudpports.IndexOf(addr.Port) < 0)
                    {
                        _usedudpports.Add(addr.Port);
                    }
                }

            /*if (!_UsedPorts.Contains(tmp))
            {
                string prefix = String.Empty;
                if (_UsedPorts != String.Empty)
                    prefix = ", ";
                _UsedPorts += prefix + tmp;
            }*/
        }

        public List<int> UsedTcpPortsList
        {
            get 
            {
                _usedtcpports.Sort();
                return _usedtcpports; 
            }
        }

        public List<int> UsedUdpPortsList
        {
            get
            {
                _usedudpports.Sort();
                return _usedudpports;
            }
        }

        private DateTime _Time;
        public DateTime Time
        {
            get { return _Time; }
            set { _Time = value; }
        }

        public SpeedCalculator SpeedCalculator
        {
            get { return _speedcalc; }
        }

        private int _UploadedReal = 0;
        public int UploadedReal
        {
            get { return _UploadedReal; }
        }

        private int _DownloadedReal = 0;
        public int DownloadedReal
        {
            get { return _DownloadedReal; }
        }

        private int _DownloadedUseful = 0;
        public int DownloadedUseful
        {
            get { return _DownloadedUseful; }
        }

        private int _UploadedUseful = 0;
        public int UploadedUseful
        {
            get { return _UploadedUseful; }
        }

        int coeff = 1024 * 1024;

        private double _OldUploadedTarifficatedTraffic = 0;
        private double _UploadedTarifficatedTraffic = 0;
        private double _UploadedCosts = 0;
        /// <summary>
        /// Uploaded traffic costs.
        /// </summary>
        public double UploadedCosts
        {
            get 
            {
                if (_OldUploadedTarifficatedTraffic != _UploadedTarifficatedTraffic)
                {
                    if (this.IsMonitored)
                    {
                        _parent.UploadedCosts -= _UploadedCosts;
                        _UploadedCosts = (_UploadedTarifficatedTraffic / coeff) * _parent.UploadPricePerMegabyte;
                        _parent.UploadedCosts += _UploadedCosts;
                        _OldUploadedTarifficatedTraffic = _UploadedTarifficatedTraffic;
                    }
                }
                return _UploadedCosts;
            }
        }

        private double _OldDownloadedTarifficatedTraffic = 0;
        private double _DownloadedTarifficatedTraffic = 0;
        private double _DownloadedCosts = 0;
        /// <summary>
        /// Downloaded traffic costs.
        /// </summary>
        public double DownloadedCosts
        {
            get 
            {
                if (_OldDownloadedTarifficatedTraffic != _DownloadedTarifficatedTraffic)
                {
                    if (this.IsMonitored)
                    {
                        _parent.DownloadedCosts -= _DownloadedCosts;
                        _DownloadedCosts = (_DownloadedTarifficatedTraffic / coeff) * _parent.DownloadPricePerMegabyte;
                        _parent.DownloadedCosts += _DownloadedCosts;
                        _OldDownloadedTarifficatedTraffic = _DownloadedTarifficatedTraffic;
                    }
                }
                return _DownloadedCosts;
            }
        }

        private double _DownloadingUsefulSpeed = 0;
        public double DownloadingUsefulSpeed
        {
            get { return _DownloadingUsefulSpeed; }
            internal set { _DownloadingUsefulSpeed = value; }
        }

        private double _DownloadingRealSpeed = 0;
        public double DownloadingRealSpeed
        {
            get { return _DownloadingRealSpeed; }
            internal set { _DownloadingRealSpeed = value; }
        }

        private double _UploadingUsefulSpeed = 0;
        public double UploadingUsefulSpeed
        {
            get { return _UploadingUsefulSpeed; }
            internal set { _UploadingUsefulSpeed = value; }
        }

        private double _UploadingRealSpeed = 0;
        public double UploadingRealSpeed
        {
            get { return _UploadingRealSpeed; }
            internal set { _UploadingRealSpeed = value; }
        }

        private string _UploadingSpeed = String.Empty;
        public string UploadingSpeed
        {
            get { return _UploadingSpeed; }
        }

        private string _DownloadingSpeed = String.Empty;
        public string DownloadingSpeed
        {
            get { return _DownloadingSpeed; }
        }

        public void AddDownloadingSpeeed(double UsefulSpeed, double RealSpeed)
        {
            this._DownloadingUsefulSpeed = UsefulSpeed;
            this._DownloadingRealSpeed = RealSpeed;
            this._DownloadingSpeed = Utils.TrafficToStr(UsefulSpeed, true) + "/" + Utils.TrafficToStr(RealSpeed, true);
        }

        public void AddUploadingSpeeed(double UsefulSpeed, double RealSpeed)
        {
            this._UploadingUsefulSpeed = UsefulSpeed;
            this._UploadingRealSpeed = RealSpeed;
            this._UploadingSpeed = Utils.TrafficToStr(UsefulSpeed, true) + "/" + Utils.TrafficToStr(RealSpeed, true);
        }

        private int _PacketsIn = -1;
        public int PacketsIn
        {
            get { return _PacketsIn; }
        }

        private int _PacketsOut = -1;
        public int PacketsOut
        {
            get { return _PacketsOut; }
        }

        private bool _IsMonitored = true;
        public bool IsMonitored
        {
            get { return _IsMonitored; }
            set 
            {
                if (this._IsMonitored != value)
                {
                    _IsMonitored = value;
                    ItemChangedEventHandler();
                }
            }
        }

        private string _ProcessName = String.Empty;
        public string ProcessName
        {
            get { return _ProcessName; }
            set 
            {
                if (this._ProcessName != value)
                {
                    _ProcessName = value;
                    ItemChangedEventHandler();
                }
            }
        }

        public void ClearValues()
        {
            this._DownloadedReal = 0;
            this._DownloadedUseful = 0;
            this._UploadedReal = 0;
            this._UploadedUseful = 0;
            this._PacketsIn = 0;
            this._PacketsOut = 0;
            ItemChangedEventHandler();
        }

        public delegate void ItemChangedEvent(Object sender);
        /// <summary>
        /// Event occures when ... changed.
        /// </summary>
        public event ItemChangedEvent ItemChanged;
        protected void ItemChangedEventHandler()
        {
            if (ItemChanged != null)
            {
                ItemChanged(this);
            }
        }

        public void Clone(TCPUDPConnection conn) //Ex
        {
//            if (conn.IsValidConnection)
            //{
                if (conn is ExTCPUDPConnection)
                {
                    this._UploadedReal = (conn as ExTCPUDPConnection).UploadedRealTotal;
                    this._UploadedUseful = (conn as ExTCPUDPConnection).UploadedUsefulTotal;
                    this._DownloadedReal = (conn as ExTCPUDPConnection).DownloadedRealTotal;
                    this._DownloadedUseful = (conn as ExTCPUDPConnection).DownloadedUsefulTotal;
                    this._PacketsIn = (conn as ExTCPUDPConnection).PacketsIn;
                    this._PacketsOut = (conn as ExTCPUDPConnection).PacketsOut;
                    this.Time = (conn as ExTCPUDPConnection).Time;
                }
                this._ProcessName = conn.ProcessName;
                this.AddUsedPort(conn.Local, conn.Protocol);
            //}
        }

        public void Calc(ExTCPUDPConnection conn)
        {
            if (conn.IsValidConnection)
            {
                if (conn.Direction == Direction.Download)
                {
                    this._DownloadedUseful += conn.DownloadedUseful;
                    this._DownloadedReal += conn.DownloadedReal;
                    this._PacketsIn++;
                    if (conn.IsCalcCosts)
                    {
                        _DownloadedTarifficatedTraffic += conn.DownloadedReal;
                    }
                }
                else
                {
                    this._UploadedUseful += conn.UploadedUseful;
                    this._UploadedReal += conn.UploadedReal;
                    this._PacketsOut++;
                    if (conn.IsCalcCosts)
                    {
                        _UploadedTarifficatedTraffic += conn.UploadedReal;
                    }
                }
            }
        }
    }

    public class MonitoredApplications : IEnumerable<AppTCPUDPConnection>
    {
        private List<AppTCPUDPConnection> _list;

        public MonitoredApplications()
        {
            _list = new List<AppTCPUDPConnection>();
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public delegate void ItemAddedEvent(Object sender, AppTCPUDPConnection item);
        /// <summary>
        /// Event occures when <seealso cref="AppTCPUDPConnection"/> deleted.
        /// </summary>
        public event ItemAddedEvent ItemAdded;
        protected void ItemAddedEventHandler(AppTCPUDPConnection item)
        {
            if (ItemAdded != null)
            {
                ItemAdded(this, item);
            }
        }

        public delegate void ItemChangedEvent(Object sender, AppTCPUDPConnection item, int Pos);
        /// <summary>
        /// Event occures when <seealso cref="AppTCPUDPConnection"/> changed.
        /// </summary>
        public event ItemChangedEvent ItemChanged;
        public void ItemChangedEventHandler(AppTCPUDPConnection item, int Pos)
        {
            if (ItemChanged != null)
            {
                if (item == null)
                    item = this[Pos];
                ItemChanged(this, item, Pos);
            }
        }

        public delegate void ItemInsertedEvent(Object sender, AppTCPUDPConnection item, int Position);
        /// <summary>
        /// Event occures when <seealso cref="AppTCPUDPConnection"/> inserted into list.
        /// </summary>
        public event ItemInsertedEvent ItemInserted;
        protected void ItemInsertedEventHandler(AppTCPUDPConnection item, int Position)
        {
            if (ItemInserted != null)
            {
                ItemInserted(this, item, Position);
            }
        }

        protected double _UploadPricePerMegabyte = 0;
        public double UploadPricePerMegabyte
        {
            get { return _UploadPricePerMegabyte; }
            set 
            {
                if (_UploadPricePerMegabyte != value)
                {
                    _UploadPricePerMegabyte = value;
                }
            }
        }

        protected double _DownloadPricePerMegabyte = 0;
        public double DownloadPricePerMegabyte
        {
            get { return _DownloadPricePerMegabyte; }
            set 
            {
                if (_DownloadPricePerMegabyte != value)
                {
                    _DownloadPricePerMegabyte = value;
                }
            }
        }

        private double _UploadedCosts = 0;
        /// <summary>
        /// Uploaded traffic costs.
        /// </summary>
        public double UploadedCosts
        {
            get { return _UploadedCosts; }
            internal set { _UploadedCosts = value; }
        }

        private double _DownloadedCosts = 0;
        /// <summary>
        /// Downloaded traffic costs.
        /// </summary>
        public double DownloadedCosts
        {
            get { return _DownloadedCosts; }
            internal set { _DownloadedCosts = value; }
        }

        public bool IsProcessMonitored(string ProcessName)
        {
            int i = IndexOf(ProcessName);
            if (i > -1)
            {
                return this[i].IsMonitored;
            }
            return true;
        }

        public void Add(TCPUDPConnection item) //Ex
        {
            int Pos = -1;
            //if (item.PID == -1)
            //    return;
            AppTCPUDPConnection conn = this.IndexOf(item, out Pos);
            if (conn == null)
            {
                if (Pos == -1)
                {
                    conn = new AppTCPUDPConnection(this);
                    conn.Clone(item);

                    _list.Add(conn);
                    if (!item.IsRefresh)
                        this[this.Count - 1].SpeedCalculator.Calculate();
                    this.ItemAddedEventHandler(conn);
                }
                else if (Pos >= 0 && Pos < this.Count)
                {
                    this.Insert(item, Pos);
                    if (!item.IsRefresh)
                        this[Pos].SpeedCalculator.Calculate();
                }
            }
            else
            {
                //change item
                //this[Pos] = conn;
                //ItemChangedEventHandler(conn, Pos);
                this[Pos].AddUsedPort(item.Local, item.Protocol);
                if (item is ExTCPUDPConnection)
                {
                    if (((item as ExTCPUDPConnection).DownloadedReal != 0) ||
                        ((item as ExTCPUDPConnection).UploadedReal != 0))
                    {
                        conn.Time = (item as ExTCPUDPConnection).Time;
                        //conn.AddUsedPort(item.Local.Port);
                        conn.Calc((item as ExTCPUDPConnection));
                        this[Pos] = conn;
                        if (!item.IsRefresh)
                            this[Pos].SpeedCalculator.Calculate();
                        ItemChangedEventHandler(conn, Pos);
                    }
                }
            }
        }

        public virtual void Insert(TCPUDPConnection item, int Pos) //Ex
        {
            AppTCPUDPConnection conn = new AppTCPUDPConnection(this);
            conn.Clone(item);
            _list.Insert(Pos, conn);
             this.ItemInsertedEventHandler(conn, Pos);
        }

        protected virtual int CompareByProcessName(TCPUDPConnection first, AppTCPUDPConnection second) //Ex
        {
            return String.Compare(first.ProcessName, second.ProcessName, true);
        }

        public int IndexOf(string ProcessName)
        {
            if (ProcessName == String.Empty)
                return -1;
            int index = -1;
            foreach (AppTCPUDPConnection conn in _list)
            {
                index++;
                int i = String.Compare(ProcessName, conn.ProcessName, true);
                if (i == 0) //Change item
                {
                    return index;
                }
                if (i > 0)
                {
                    continue;
                }
                if (i < 0)
                {
                    return -1;
                }
            }
            return -1;
        }

        public AppTCPUDPConnection IndexOf(TCPUDPConnection item, out int Pos) //Ex
        {
            int index = -1;
            foreach (AppTCPUDPConnection conn in _list)
            {
                index++;
                int i = CompareByProcessName(item, conn);
                if (i == 0) //Change item
                {
                    Pos = index;
                    return conn;
                }
                if (i > 0)
                {
                    continue;
                }
                if (i < 0)
                {
                    Pos = index;
                    return null;
                }
            }
            Pos = -1;
            return null;
        }

        public AppTCPUDPConnection this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        #region IEnumerable<AppTCPUDPConnection> Members

        public IEnumerator<AppTCPUDPConnection> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }

    public class SpeedCalculatorRecord
    {
        private int _DownloadedTrafficReal = 0;
        private int _DownloadedTrafficUseful = 0;
        private int _UploadedTrafficReal = 0;
        private int _UploadedTrafficUseful = 0;
        private DateTime _TimerTick;

        public int DownloadedTrafficReal
        {
            get { return _DownloadedTrafficReal; }
            set { _DownloadedTrafficReal = value; }
        }

        public int DownloadedTrafficUseful
        {
            get { return _DownloadedTrafficUseful; }
            set { _DownloadedTrafficUseful = value; }
        }

        public int UploadedTrafficReal
        {
            get { return _UploadedTrafficReal; }
            set { _UploadedTrafficReal = value; }
        }

        public int UploadedTrafficUseful
        {
            get { return _UploadedTrafficUseful; }
            set { _UploadedTrafficUseful = value; }
        }

        public DateTime Time
        {
            get { return _TimerTick; }
            set { _TimerTick = value; }
        }
    }

    public class SpeedCalculator : IEnumerable<SpeedCalculatorRecord>
    {
        private List<SpeedCalculatorRecord> _list = null;
        AppTCPUDPConnection _conn = null;

        public SpeedCalculator(AppTCPUDPConnection item)
        {
            _list = new List<SpeedCalculatorRecord>();
            _conn = item;
        }

        public SpeedCalculatorRecord this[int i]
        {
            get { return _list[i]; }
            set { _list[i] = value; }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        private int _MaxQueueLength = 10;
        public int MaxQueueLength
        {
            get { return _MaxQueueLength; }
            set { _MaxQueueLength = value; }
        }

        private double GetDiff(int first, int last)
        {
            double tmp = last - first;
            return (tmp > 0) ? tmp : 0;
        }

        public void Add(SpeedCalculatorRecord row)
        {
            if (_list.Count > _MaxQueueLength - 1)
            {
                _list.RemoveAt(0);
            }
            _list.Add(row);
            if (this.Count > 0)
            {
                GetSpeed();
            }
        }

        public void Calculate()
        {
            SpeedCalculatorRecord record = new SpeedCalculatorRecord();
            record.DownloadedTrafficReal = _conn.DownloadedReal;
            record.DownloadedTrafficUseful = _conn.DownloadedUseful;
            record.UploadedTrafficReal = _conn.UploadedReal;
            record.UploadedTrafficUseful = _conn.UploadedUseful;
            record.Time = _conn.Time;
            this.Add(record);
        }

        private double _UploadingRealSpeed = 0;
        private double _UploadingUsefulSpeed = 0;
        private double _DownloadingRealSpeed = 0;
        private double _DownloadingUsefulSpeed = 0;

        private double _UploadingRealDiff = 0;
        private double _UploadingUsefulDiff = 0;
        private double _DownloadingRealDiff = 0;
        private double _DownloadingUsefulDiff = 0;

        private void ClearValues()
        {
            _UploadingRealSpeed = 0;
            _UploadingUsefulSpeed = 0;
            _DownloadingRealSpeed = 0;
            _DownloadingUsefulSpeed = 0;
        }

        public double UploadingRealSpeed
        {
            get { return _UploadingRealSpeed; }
        }

        public double UploadingUsefulSpeed
        {
            get { return _UploadingUsefulSpeed; }
        }

        public double DownloadingUsefulSpeed
        {
            get { return _DownloadingUsefulSpeed; }
        }

        public double DownloadingRealSpeed
        {
            get { return _DownloadingRealSpeed; }
        }

        public void Clear()
        {
            _list.Clear();
        }

        public void GetSpeed()
        {
            _DownloadingRealDiff = GetDiff(this[0].DownloadedTrafficReal, this[this.Count - 1].DownloadedTrafficReal);
            _DownloadingUsefulDiff = GetDiff(this[0].DownloadedTrafficUseful, this[this.Count - 1].DownloadedTrafficUseful);
            _UploadingUsefulDiff = GetDiff(this[0].UploadedTrafficUseful, this[this.Count - 1].UploadedTrafficUseful);
            _UploadingRealDiff = GetDiff(this[0].UploadedTrafficReal, this[this.Count - 1].UploadedTrafficReal);

            TimeSpan timespan = this[this.Count - 1].Time - this[0].Time;
            double diff = timespan.TotalSeconds;
            if (diff > 0)
            {
                _UploadingRealSpeed = _UploadingRealDiff / diff;
                _UploadingUsefulSpeed = _UploadingUsefulDiff / diff;
                _DownloadingRealSpeed = _DownloadingRealDiff / diff;
                _DownloadingUsefulSpeed = _DownloadingUsefulDiff / diff;

                _conn.AddDownloadingSpeeed(_DownloadingUsefulSpeed, _DownloadingRealSpeed);
                _conn.AddUploadingSpeeed(_UploadingUsefulSpeed, _UploadingRealSpeed);
            }
        }

        public IEnumerator<SpeedCalculatorRecord> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    public class ConnectionsWatcher : TCPUDPConnections
    {
        private PcapDeviceList _devices = SharpPcap.GetAllDevices();
        private static readonly ILog log = LogManager.GetLogger(typeof(ConnectionsWatcher));
        private MonitoredApplications _monapps = new MonitoredApplications();

        public ConnectionsWatcher()
            : base()
        {
             _currentfilter = _defaultfilter;
            //_iplist.Add(
            //_monapps = new MonitoredApplications();
            //this.StopAutoRefresh();
        }

        public MonitoredApplications MonitoredApplications
        {
            get { return _monapps; }
        }

        protected override bool SkipItem(TCPUDPConnection item)
        {
            if (_monapps != null)
            {
                return !_monapps.IsProcessMonitored(item.ProcessName);
            }
            return false;
        }

        public double DownloadPricePerMegabyte
        {
            get { return _monapps.DownloadPricePerMegabyte; }
            set { _monapps.DownloadPricePerMegabyte = value; }
        }

        public double UploadPricePerMegabyte
        {
            get { return _monapps.UploadPricePerMegabyte; }
            set { _monapps.UploadPricePerMegabyte = value; }
        }

        public override TCPUDPConnection Add(TCPUDPConnection item)
        {
            ExTCPUDPConnection conn = base.Add(item) as ExTCPUDPConnection;
            if (conn != null && 
                //(conn is ExTCPUDPConnection) && 
                //(conn as ExTCPUDPConnection).IsValidConnection &&
                _monapps != null)
            {
                _monapps.Add(conn); // as ExTCPUDPConnection
            }
            return conn;
        }

        public void CloseDevices()
        {
            try
            {
                this.Stop();
                foreach (PcapDevice device in _devices)
                {
                    if (device.PcapOpened)
                    {
                        device.PcapClose();
                    }
                }
            }
            catch (Exception err)
            {
                log.Error("Method [" + new StackFrame(0).GetMethod().Name + "]. Error " + err.Message);
            }
        }

        private System.Timers.Timer _dumptimer = new System.Timers.Timer();
        private bool _IsDumpToFile = false;
        public bool IsDumpToFile
        {
            get { return _IsDumpToFile; }
            set 
            {
                if (_IsDumpToFile != value)
                {
                    _IsDumpToFile = value;
                    if (value)
                    {
                        this.KillOldDumpFile();
                        _dumptimer.Interval = 60000;
                        _dumptimer.Elapsed += new System.Timers.ElapsedEventHandler(_dumptimer_Elapsed);
                        _dumptimer.Start();
                    }
                    else
                    {
                        _dumptimer.Stop();
                    }
                    this.Start();
                }
            }
        }

        private PcapDevice _offlinedevice = null;
        void _dumptimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //Get an offline file pcap device
                _offlinedevice = SharpPcap.GetPcapOfflineDevice(this.FileName);
                //Open the device for capturing
                _offlinedevice.PcapOpen();
                //_offlinedevice.PcapOnPacketArrival += new SharpPcap.PacketArrivalEvent(_offlinedevice_PcapOnPacketArrival);
                //Start capture 'INFINTE' number of packets
                //This method will return when EOF reached.
                //_offlinedevice.PcapCapture(SharpPcap.INFINITE);
                //Keep capture packets using PcapGetNextPacket()
                Packet packet = null;
                while ((packet = _offlinedevice.PcapGetNextPacket()) != null)
                {
                    try
                    {
                        this.AddPacket(packet);
                    }
                    catch (Exception err)
                    {
                        log.Error("Method [" + new StackFrame(0).GetMethod().Name + "]. Error " + err.Message);
                    }
                }
                //Close the pcap device
                _offlinedevice.PcapClose();
            }
            catch
            {

            }
        }

        /*void _offlinedevice_PcapOnPacketArrival(object sender, Packet packet)
        {
            try
            {
                this.AddPacket(packet);
            }
            catch (Exception err)
            {
                log.Error("Method [" + new StackFrame(0).GetMethod().Name + "]. Error " + err.Message);
            }
        }
        */
        public void Stop()
        {
            try
            {
                foreach (PcapDevice device in _devices)
                {
                    if (device.PcapStarted)
                        device.PcapStopCapture();
                }
            }
            catch (Exception err)
            {
                log.Error("Method [" + new StackFrame(0).GetMethod().Name + "]. Error " + err.Message);
            }
        }

        public void Start()
        {
            ChangeFilter(_currentfilter);
        }

        protected override TCPUDPConnection NewTCPUDPConnection()
        {
            return new ExTCPUDPConnection(this);
        }

        private string _defaultfilter = "(tcp or udp) and host " + Utils.GetLocalHostName()+ 
            " and (not port 53) and (not port 137) and (not port 445) and (not port 139)";
        private string _currentfilter;

        private void AddPacket(Packet packet) //string srcIp, int srcPort, string dstIp, int dstPort
        {
            ExTCPUDPConnection conn = new ExTCPUDPConnection(this, packet);
            if (conn.IsValidConnection)
            {
                conn.IsRefresh = false;
                this.Add(conn);
            }
        }

        public void ChangeMonitoring(int Position)
        {

        }

        protected override bool ChangeItem(TCPUDPConnection src, ref TCPUDPConnection dst, int Position)
        {
            bool tmp = base.ChangeItem(src, ref dst, Position);
            ExTCPUDPConnection _src = (src as ExTCPUDPConnection);
            //ExTCPUDPConnection _dst = (dst as ExTCPUDPConnection);
            if (//_src.PacketsIn != _dst.PacketsIn ||
                //_src.PacketsOut != _dst.PacketsOut ||
                _src.UploadedReal != 0 ||
                _src.DownloadedReal != 0)
            {
                (_list[Position] as ExTCPUDPConnection).Clone(_src);
                dst = _list[Position];
                return true;
            }
            return tmp;
        }

        /// <summary>
        /// Prints the time, length, src ip, src port, dst ip and dst port
        /// for each TCP/IP packet received on the network
        /// </summary>
        private void device_PcapOnPacketArrival(object sender, Packet packet)
        {
            try
            {
                PcapDevice device = (PcapDevice)sender;
                if (device.PcapDumpOpened)
                {
                    //dump the packet to the file
                    device.PcapDump(packet);
                }
                else
                {
                    this.AddPacket(packet);
                }
            }
            catch (Exception err)
            {
                log.Error("Method [" + new StackFrame(0).GetMethod().Name + "]. Error " + err.Message);
            }
        }

        private void KillOldDumpFile()
        {
            try
            {
                string filepath = Path.GetDirectoryName(this.FileName);
                foreach (string file in Directory.GetFileSystemEntries(filepath, "*.dmp"))
                {
                    File.Delete(file);
                }
            }
            catch { }
        }

        private string _filename = String.Empty;
        public string FileName
        {
            get
            {
                if (_filename == String.Empty)
                {
                    string _path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                    _filename = _path + "\\" + Guid.NewGuid().ToString() + ".dmp";
                }
                return _filename;
            }
            internal set { _filename = value; }
        }

        public bool SkipMonitoringProcess(string ProcessName)
        {
            return true;
        }

        public void ChangeFilter(string Filter)
        {
            /*If no device exists, print error */
            try
            {
                if (_devices.Count < 1)
                {
                    MessageBox.Show("No device found on this machine");
                    return;
                }

                if (Filter != null &&
                    Filter.Trim() != "")
                {
                    try
                    {
                        foreach (PcapDevice device in _devices)
                        {
                            if (device.PcapStarted)
                            {
                                device.PcapStopCapture();
                                //device.PcapOnPacketArrival -= new SharpPcap.PacketArrivalEvent(device_PcapOnPacketArrival);
                            }
                            if (device.PcapOpened)
                            {
                                device.PcapClose();
                            }
                            device.PcapOnPacketArrival += new SharpPcap.PacketArrivalEvent(device_PcapOnPacketArrival);
                            device.PcapOpen(true, 1000);
                            device.PcapSetFilter(Filter);
                            if (this._IsDumpToFile)
                            {
                                //Open or create a capture output file
                                device.PcapDumpOpen(this.FileName);
                            }
                            device.PcapStartCapture();
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                        log.Error("Method [" + new StackFrame(0).GetMethod().Name + "]. Error " + err.Message);
                        //log.Error(err.Message);
                    }
                }
            }
            catch (Exception err)
            {
                log.Error("Method [" + new StackFrame(0).GetMethod().Name + "]. Error " + err.Message);
            }
        }
    }
}
