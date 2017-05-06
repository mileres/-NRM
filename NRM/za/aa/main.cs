using System;
using System.Net;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using log4net;
using IPNumbers;

namespace IpHlpApidotnet
{
    public enum Protocol { TCP, UDP, None };
    /// <summary>
    /// Store information concerning single TCP/UDP connection
    /// </summary>
    public class TCPUDPConnection
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TCPUDPConnection));

        private int _dwState = -1;
        public int iState
        {
            get { return _dwState; }
            set 
            {
                if (_dwState != value)
                {
                    _dwState = value;
                }
            }
        }

        private static TCPUDPConnections _conns = null;
        public TCPUDPConnection(TCPUDPConnections conns) : base()
        {
            _conns = conns;
            this._Stopped = DateTime.Now;
        }

        public TCPUDPConnections Parent
        {
            get { return _conns; }
        }

        private bool _IsRefresh = false;
        public bool IsRefresh
        {
            get { return _IsRefresh; }
            set
            {
                if (_IsRefresh != value)
                {
                    _IsRefresh = value;
                    ClearParams(value);
                }
            }
        }

        protected virtual void ClearParams(bool Refresh)
        {

        }

        private DateTime _Started;
        public DateTime Started
        {
            get { return this._Started; }
            internal set { this._Started = value; }
        }

        private DateTime _Stopped;
        public DateTime Stopped
        {
            get { return this._Stopped; }
            set { this._Stopped = value; }
        }

        private bool _IsDead = false;
        public bool IsDead
        {
            get { return this._IsDead; }
            internal set { this._IsDead = value; }
        }

        public string GetHostName(IPEndPoint HostAddress)
        {
            return Utils.GetHostName(HostAddress, _conns.LocalHostName);
        }

        private bool _IsResolveIP = true;
        public bool IsResolveIP
        {
            get { return _IsResolveIP; }
            set { _IsResolveIP = value; }
        }

        private Protocol _Protocol;
        public Protocol Protocol
        {
            get { return _Protocol; }
            set { _Protocol = value; }
        }

        private string _State = String.Empty; 
        public string State
        {
            get 
            {
                if (_State == String.Empty)
                    _State = Utils.StateToStr(this._dwState);
                return _State; 
            }
            //internal set { _State = value; }
        }

        private IPEndPoint _OldLocalHostName;
        private IPEndPoint _OldRemoteHostName;
        private string _LocalAddress = String.Empty;
        private string _RemoteAddress = String.Empty;
        private void SaveHostName(bool IsLocalHostName)
        {
            if (IsLocalHostName)
            {
                this._LocalAddress = GetHostName(this._Local);
                this._OldLocalHostName = this._Local;
            }
            else
            {
                this._RemoteAddress = GetHostName(this._Remote);
                this._OldRemoteHostName = this._Remote;
            }
        }

        public string LocalAddress
        {
            get
            {
                if (this._OldLocalHostName == this._Local)
                {
                    if (this._LocalAddress.Trim() == String.Empty)
                    {
                        this.SaveHostName(true);
                    }
                }
                else
                {
                    this.SaveHostName(true);
                }
                return this._LocalAddress;
            }
        }

        public string RemoteAddress
        {
            get
            {
                if (this._OldRemoteHostName == this._Remote)
                {
                    if (this._RemoteAddress.Trim() == String.Empty)
                    {
                        this.SaveHostName(false);
                    }
                }
                else
                {
                    this.SaveHostName(false);
                }
                return this._RemoteAddress;
            }
        }

        private IPEndPoint _Local = null;
        public IPEndPoint Local  //LocalAddress
        {
            get { return this._Local; }
            set
            {
                if (this._Local != value)
                {
                    this._Local = value;
                }
            }
        }

        private IPEndPoint _Remote;
        public IPEndPoint Remote //RemoteAddress
        {
            get { return this._Remote; }
            set 
            {
                if (this._Remote != value)
                {
                    this._Remote = value;
                }
            }
        }

        private int _dwOwningPid = -1;
        public int PID
        {
            get { return this._dwOwningPid; }
            set
            {
                if (this._dwOwningPid != value)
                {
                    if (value != -1)
                    {
                        if (!(value == 0 && this._dwOwningPid > -1))
                            this._dwOwningPid = value;
                    }
                    else
                    {
                        if (this._dwOwningPid == -1 && value == -1)
                            this._dwOwningPid = value;
                    }
               }
            }
        }

        private void SaveProcessID()
        {
            this._ProcessName = Utils.GetProcessNameByPID(this._dwOwningPid);
            if (this._ProcessName.StartsWith("Unknown"))
            {
                this._ProcessName += "_" + this.Local.Port.ToString();
            }
            this._OldProcessID = this._dwOwningPid;
        }

        private int _OldProcessID = -1;
        private string _ProcessName = String.Empty;
        public string ProcessName
        {
            get
            {
                if (this._OldProcessID == this._dwOwningPid)
                {
                    if (this._ProcessName.Trim() == String.Empty)
                    {
                        this.SaveProcessID();
                    }
                }
                else
                {
                    this.SaveProcessID();
                }
                return this._ProcessName;
            }
            internal set 
            {
                if (this._ProcessName != value)
                {
                    this._ProcessName = value;
                }
            }
        }

        private DateTime _WasActiveAt = DateTime.MinValue;
        public DateTime WasActiveAt
        {
            get { return _WasActiveAt; }
            internal set { _WasActiveAt = value; }
        }

        private Object _Tag = null;
        public Object Tag
        {
            get { return this._Tag; }
            set { this._Tag = value; }
        }
    }

    public class SortConnectionsByLocalAndRemoteAddress : IComparer<TCPUDPConnection>
    {
        /// <summary>
        /// Method is used to compare two <seealso cref="TCPUDPConnection"/>. 
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public virtual int CompareConnections(TCPUDPConnection first, TCPUDPConnection second)
        {
            int i;
            i = Utils.CompareIPEndPoints(first.Local, second.Local);
            if (i != 0)
                    return i;
            if (first.Protocol == Protocol.TCP &&
                second.Protocol == Protocol.TCP)
            {
                i = Utils.CompareIPEndPoints(first.Remote, second.Remote);
                if (i != 0)
                    return i;
            }
            //i = first.PID - second.PID;
            //if (i != 0)
            //    return i;
            if (first.Protocol == second.Protocol)
                return 0;
            if (first.Protocol == Protocol.TCP)
                return -1;
            else
                return 1;
        }

        #region IComparer<TCPUDPConnection> Members

        public int Compare(TCPUDPConnection x, TCPUDPConnection y)
        {
            return this.CompareConnections(x, y);
        }

        #endregion
    }

    public class SortConnectionsByLocalAddress : IComparer<TCPUDPConnection>
    {
        /// <summary>
        /// Method is used to compare two <seealso cref="TCPUDPConnection"/>. 
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public virtual int CompareConnections(TCPUDPConnection first, TCPUDPConnection second)
        {
            int i;
            i = Utils.CompareIPEndPoints(first.Local, second.Local);
            if (i != 0)
                return i;
            if (first.Protocol == second.Protocol)
                return 0;
            if (first.Protocol == Protocol.TCP)
                return -1;
            else
                return 1;
        }

        #region IComparer<TCPUDPConnection> Members

        public int Compare(TCPUDPConnection x, TCPUDPConnection y)
        {
            return this.CompareConnections(x, y);
        }

        #endregion
    }

    /// <summary>
    /// Store information concerning TCP/UDP connections
    /// </summary>
    public class TCPUDPConnections : IEnumerable<TCPUDPConnection>  
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TCPUDPConnections));

        protected List<TCPUDPConnection> _list;
        System.Timers.Timer _timer = null;
        private int _DeadConnsMultiplier = 1*60; //Collect dead connections each ... sec.
        //private int _RefreshConnsMultiplier = 1*60; //Refresh list multiplier
        private bool _IsEnableDeadConnRemover = true;
        private int _DeadConnsTimerCounter = -1;
        //private int _RefreshConnsTimerCounter = -1;
        private string _LocalHostName = String.Empty;
        private IPList _iplist = new IPList();


        public TCPUDPConnections()
        {
            _iplist.ExcludeLocalNetwork();
            _LocalHostName = Utils.GetLocalHostName();
            _list = new List<TCPUDPConnection>();
            this.Refresh(Protocol.None);
            _timer = new System.Timers.Timer();
            _timer.Interval = 1000; // Refresh every 1 sec.
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();
        }

        public virtual bool IsCalculateCosts(System.Net.IPAddress RemoteIP)
        {
            return !_iplist.CheckNumber(RemoteIP.ToString());
        }

        public bool IsEnableDeadConnRemover
        {
            get { return _IsEnableDeadConnRemover; }
            set { _IsEnableDeadConnRemover = value; }
        }

        public delegate void ItemAddedEvent(Object sender, TCPUDPConnection item);
        /// <summary>
        /// Event occures when <seealso cref="TCPUDPConnection"/> deleted.
        /// </summary>
        public event ItemAddedEvent ItemAdded;
        protected void ItemAddedEventHandler(TCPUDPConnection item)
        {
            if (ItemAdded != null)
            {
                ItemAdded(this, item);
            }
        }

        public delegate void ItemChangedEvent(Object sender, TCPUDPConnection item, int Pos);
        /// <summary>
        /// Event occures when <seealso cref="TCPUDPConnection"/> changed.
        /// </summary>
        public event ItemChangedEvent ItemChanged;
        protected void ItemChangedEventHandler(TCPUDPConnection item, int Pos)
        {
            if (ItemChanged != null)
            {
                ItemChanged(this, item, Pos);
            }
        }

        public delegate void ItemInsertedEvent(Object sender, TCPUDPConnection item, int Position);
        /// <summary>
        /// Event occures when <seealso cref="TCPUDPConnection"/> inserted into list.
        /// </summary>
        public event ItemInsertedEvent ItemInserted;
        protected void ItemInsertedEventHandler(TCPUDPConnection item, int Position)
        {
            if (ItemInserted != null)
            {
                ItemInserted(this, item, Position);
            }
        }

        public delegate void ItemDeletedEvent(Object sender, TCPUDPConnection item, int Position);
        /// <summary>
        /// Event occures when <seealso cref="TCPUDPConnection"/> deleted from list. 
        /// </summary>
        public event ItemDeletedEvent ItemDeleted;
        protected void ItemDeletedEventHandler(TCPUDPConnection item, int Position)
        {
            if (ItemDeleted != null)
            {
                ItemDeleted(this, item, Position);
            }
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _DeadConnsTimerCounter++;
            if (_DeadConnsTimerCounter >= _DeadConnsMultiplier)
            {
                this.CheckForClosedConnections();
                this._DeadConnsTimerCounter = -1;
            }
            //this.Refresh();
        }

        /// <summary>
        /// Refresh connections list.
        /// </summary>
        public void Refresh(Protocol protocol)
        {
//            lock (this) //Consume a lot of processor time. 
            {
                //_RefreshConnsTimerCounter++;
                //if (_RefreshConnsMultiplier >= this._RefreshConnsTimerCounter)
                //{
                if (protocol == Protocol.TCP || protocol == Protocol.None)
                {
                    this._LastRefreshDateTimeTcp = DateTime.Now;
                    this.GetTcpConnections();
                }
                if (protocol == Protocol.UDP || protocol == Protocol.None)
                {
                    this._LastRefreshDateTimeUdp = DateTime.Now;
                    this.GetUdpConnections();
                }
               //     this._RefreshConnsTimerCounter = -1;
               // }
            }
        }


        public DateTime GetLastRefreshTime(Protocol protocol)
        {
            if (protocol == Protocol.TCP)
            {
                return _LastRefreshDateTimeTcp;
            }
            else
            {
                if (protocol == Protocol.UDP)
                {
                    return _LastRefreshDateTimeUdp;
                }
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// Get last refresh <seealso cref="DateTime"/>.
        /// </summary>
        private DateTime _LastRefreshDateTimeTcp = DateTime.MinValue;
        public DateTime LastRefreshDateTimeTcp
        {
            get { return _LastRefreshDateTimeTcp; }
        }

        /// <summary>
        /// Get last refresh <seealso cref="DateTime"/>.
        /// </summary>
        private DateTime _LastRefreshDateTimeUdp = DateTime.MinValue;
        public DateTime LastRefreshDateTimeUdp
        {
            get { return _LastRefreshDateTimeUdp; }
        }

        public void StopAutoRefresh()
        {
            _timer.Stop();
        }

        public void StartAutoRefresh()
        {
            _timer.Start();
        }

        /// <summary>
        /// Enable or Disable connections list auto refresh.
        /// </summary>
        public bool AutoRefresh
        {
            get { return _timer.Enabled; }
            set { _timer.Enabled = value; }
        }

        /// <summary>
        /// Skip item from adding to list. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual bool SkipItem(TCPUDPConnection item)
        {
            return false;
        }

        private void AddItem(ref TCPUDPConnection item, int Pos)
        {
            item.WasActiveAt = GetLastRefreshTime(item.Protocol);
            if ((Pos > -1) && (Pos < this.Count))
            {
                this.Insert(Pos, item);
            }
            else
            {
                _list.Add(item);
                ItemAddedEventHandler(item);
            }
        }

        //private int NestingDepth = -1;
        /// <summary>
        /// Add new <seealso cref="TCPUDPConnection"/> connection.
        /// </summary>
        /// <param name="item"></param>
        public virtual TCPUDPConnection Add(TCPUDPConnection item)
        {
            int Pos = 0;
            TCPUDPConnection conn = IndexOf(item, out Pos);
            if (conn == null)
            {
                if (SkipItem(item)) //Skip items specified some criteria.
                {
                    return null;
                }
                if (item.PID < 0)
                {
                    //NestingDepth++;
                    //if (NestingDepth > 0)
                    //{
                    //    item.ProcessName = "Unknown_" + item.Local.Port.ToString();
                    //    _list.Add(item);
                    //    ItemAddedEventHandler(item);
                    //    NestingDepth = -1;
                    //    log.Error("Method [" + new StackFrame(0).GetMethod().Name + "]. Nesting depth more then 1.");
                    //    return item;
                    //}
                    TCPUDPConnection tmp_conn = null;
                    if (this.RefreshByLocalAddress(item, out tmp_conn))
                    {
                        this.Add(item);
                    }
                    else
                    {
                        item.ProcessName = "Unknown_" + item.Local.Port.ToString();
                        this.AddItem(ref item, Pos);
                        return item;
                    }
                }
                else
                {
                    this.AddItem(ref item, Pos);
                }
                return item;
            }
            else
            {
                if (conn.ProcessName.StartsWith("Unknown"))
                {
                    TCPUDPConnection tmp_conn = null;
                    if (this.RefreshByLocalAddress(conn, out tmp_conn))
                    {
                        item.PID = tmp_conn.PID;
                    }
                }
                if (SkipItem(conn) || conn.IsDead) //Skip conn specified some criteria.
                {
                    return null;
                }
                if ((item.PID != -1) && (conn.PID != item.PID)) //Exclude records when local address and port the same, but ProcessID was changed.
                {
                    this[Pos].IsDead = true;
                    this[Pos].Stopped = DateTime.Now;
                    this.Add(item);
                    return null;
                }
                this[Pos].WasActiveAt = GetLastRefreshTime(this[Pos].Protocol);
                if (ChangeItem(item, ref conn, Pos))
                {
                    ItemChangedEventHandler(conn, Pos);
                }
                return conn;
            }
        }

        protected virtual bool ChangeItem(TCPUDPConnection src, ref TCPUDPConnection dst, int Position)
        {
            //_list[Pos].WasActiveAt = DateTime.Now;
            if (src.iState != dst.iState ||
                src.PID != dst.PID ||
                src.IsRefresh != dst.IsRefresh ||
                src.IsDead != dst.IsDead)
            {
                _list[Position].iState = src.iState;
                _list[Position].IsRefresh = src.IsRefresh;
                _list[Position].PID = src.PID;
                _list[Position].IsDead = src.IsDead;
                dst = _list[Position];
                return true;
            }
            return false;
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public TCPUDPConnection this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        private IComparer<TCPUDPConnection> _connComp;
        public void Sort()
        {
            _list.Sort(_connComp);
        }

        public virtual int CompareConnections(TCPUDPConnection first, TCPUDPConnection second)
        {
            _connComp = new SortConnectionsByLocalAddress();
            return _connComp.Compare(first, second);
        }

        public virtual TCPUDPConnection IndexOf(TCPUDPConnection item, out int Pos)
        {
            int index = -1;
            foreach (TCPUDPConnection conn in _list)
            {
                index++;
                if (conn.IsDead)
                    continue;
                int i = CompareConnections(item, conn);
                if (i == 0)
                {
                    Pos = index;
                    return conn;
                }
                if (i > 0) // If current an item more then conn, try to compare with next one until finding equal or less.
                {
                    continue; //Skip
                }
                if (i < 0) // If there is an item in list with row less then current, insert current before this one.
                {
                    Pos = index;
                    return null;
                }
            }
            Pos = -1;
            return null;
        }

        /// <summary>
        /// Method detects and remove all dead connections from the list .
        /// </summary>
        public virtual void CheckForClosedConnections()
        {
            long interval = (long)_timer.Interval * this._DeadConnsMultiplier;
            //Remove item from the end of the list
            for (int index = _list.Count - 1; index >= 0; index--)
            {
                if (this[index].IsDead)
                {
                    if (_IsEnableDeadConnRemover)
                    {
                        this.Remove(index);
                    }
                    continue;
                }

                if (this[index].WasActiveAt != DateTime.MinValue)
                {
                    TimeSpan diff = (this.GetLastRefreshTime(this[index].Protocol) - this[index].WasActiveAt);
                    long interval1 = Math.Abs((long)diff.TotalMilliseconds);
                    if (interval1 > interval)
                    {
                        this[index].Stopped = this.GetLastRefreshTime(this[index].Protocol);
                        this[index].IsDead = true;
                        ItemChangedEventHandler(this[index], index);
                        /*if (_IsEnableDeadConnRemover)
                        {
                            this.Remove(index);
                        }*/
                    }
                }
            }
        }

        public void Remove(int index)
        {
            TCPUDPConnection conn = this[index];
            _list.RemoveAt(index);
            this.ItemDeletedEventHandler(conn, index);
        }

        public virtual void Insert(int index, TCPUDPConnection item)
        {
            _list.Insert(index, item);
            ItemInsertedEventHandler(item, index);
        }

        public bool RefreshByLocalAddress(TCPUDPConnection item, out TCPUDPConnection connection)
        {
            connection = null;
            if (item.Protocol == Protocol.TCP)
            {
                this._LastRefreshDateTimeTcp = DateTime.Now;
                return GetExTcpConnections(item.Local, out connection);
            }
            else
            {
                if (item.Protocol == Protocol.UDP)
                {
                    this._LastRefreshDateTimeUdp = DateTime.Now;
                    return GetUdpConnections(item.Local, out connection);
                }
            }
            return true;
        }

        public bool GetTcpConnections()
        {
            TCPUDPConnection conn = null;
            return GetTcpConnections(null, out conn);
        }

        public bool GetTcpConnections(IPEndPoint Local, out TCPUDPConnection connection)
        {
            bool flag = false;
            connection = null;
            int AF_INET = 2;    // IP_v4
            int buffSize = 20000;
            byte[] buffer = new byte[buffSize];
            int res = IPHlpAPI32Wrapper.GetExtendedTcpTable(buffer, out buffSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
            if (res != Utils.NO_ERROR) //If there is no enouth memory to execute function
            {
                buffer = new byte[buffSize];
                res = IPHlpAPI32Wrapper.GetExtendedTcpTable(buffer, out buffSize, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
                if (res != Utils.NO_ERROR)
                {
                    return false;
                }
            }
            int nOffset = 0;
            // number of entry in the
            int NumEntries = Convert.ToInt32(buffer[nOffset]);
            nOffset += 4;
            for (int i = 0; i < NumEntries; i++)
            {
                TCPUDPConnection row = NewTCPUDPConnection();
                // state
                int st = Convert.ToInt32(buffer[nOffset]);
                // state  by ID
                row.iState = st;
                nOffset += 4;
                row.Protocol = Protocol.TCP;
                row.Local = Utils.BufferToIPEndPoint(buffer, ref nOffset, false);
                row.Remote = Utils.BufferToIPEndPoint(buffer, ref nOffset, true);
                row.PID = Utils.BufferToInt(buffer, ref nOffset);
                row.IsRefresh = true;
                if (Local == null)
                {
                    this.Add(row);
                    flag = true;
                }
                else
                {
                    if ((Local != null &&
                        (row.Local.Address.Address == Local.Address.Address) &&
                        (row.Local.Port == Local.Port)))
                    {
                        this.Add(row);
                        connection = row;
                        return true;
                    }
                }
            }
            return flag;
        }

        public bool GetExTcpConnections()
        {
            TCPUDPConnection conn = null;
            return GetExTcpConnections(null, out conn);
        }

        public bool GetExTcpConnections(IPEndPoint Local, out TCPUDPConnection connection)
        {
            bool flag = false;
            connection = null;
            // the size of the MIB_EXTCPROW struct =  6*DWORD
            int rowsize = 24;
            int BufferSize = 100000;
            // allocate a dumb memory space in order to retrieve  nb of connection
            IntPtr lpTable = Marshal.AllocHGlobal(BufferSize);
            //getting infos
            int res = IPHlpAPI32Wrapper.AllocateAndGetTcpExTableFromStack(ref lpTable, true, IPHlpAPI32Wrapper.GetProcessHeap(), 0, 2);
            if (res != Utils.NO_ERROR)
            {
                log.Error(IPHlpAPI32Wrapper.GetAPIErrorMessageDescription(res) + " " + res);
                return false; // Error. You should handle it
            }
            int CurrentIndex = 0;
            //get the number of entries in the table
            int NumEntries = (int)Marshal.ReadIntPtr(lpTable);
            lpTable = IntPtr.Zero;
            // free allocated space in memory
            Marshal.FreeHGlobal(lpTable);
            ///////////////////
            // calculate the real buffer size nb of entrie * size of the struct for each entrie(24) + the dwNumEntries
            BufferSize = (NumEntries * rowsize) + 4;
            // Allocate memory
            lpTable = Marshal.AllocHGlobal(BufferSize);
            res = IPHlpAPI32Wrapper.AllocateAndGetTcpExTableFromStack(ref lpTable, true, IPHlpAPI32Wrapper.GetProcessHeap(), 0, 2);
            if (res != Utils.NO_ERROR)
            {
                log.Error(IPHlpAPI32Wrapper.GetAPIErrorMessageDescription(res) + " " + res);
                return false; // Error. You should handle it
            }
            // New pointer of iterating throught the data
            IntPtr current = lpTable;
            CurrentIndex = 0;
            // get the (again) the number of entries
            NumEntries = (int)Marshal.ReadIntPtr(current);
            // iterate the pointer of 4 (the size of the DWORD dwNumEntries)
            CurrentIndex += 4;
            current = (IntPtr)((int)current + CurrentIndex);
            // for each entries
            for (int i = 0; i < NumEntries; i++)
            {

                // The state of the connection (in string)
                TCPUDPConnection row = NewTCPUDPConnection();
                row.Protocol = Protocol.TCP;
                // state
                row.iState = (int)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // get the local address of the connection
                UInt32 localAddr = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // get the local port of the connection
                UInt32 localPort = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // Store the local endpoint in the struct and convertthe port in decimal (ie convert_Port())
                row.Local = new IPEndPoint(localAddr, (int)Utils.ConvertPort(localPort));
                // get the remote address of the connection
                UInt32 RemoteAddr = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                UInt32 RemotePort = 0;
                // if the remote address = 0 (0.0.0.0) the remote port is always 0
                // else get the remote port
                if (RemoteAddr != 0)
                {
                    RemotePort = (UInt32)Marshal.ReadIntPtr(current);
                    RemotePort = Utils.ConvertPort(RemotePort);
                }
                current = (IntPtr)((int)current + 4);
                // store the remote endpoint in the struct  and convertthe port in decimal (ie convert_Port())
                row.Remote = new IPEndPoint(RemoteAddr, (int)RemotePort);
                // store the process ID
                row.PID = (int)Marshal.ReadIntPtr(current);
                current = (IntPtr)((int)current + 4);
                row.IsRefresh = true;
                if (Local == null)
                {
                    this.Add(row);
                    flag = true;
                }
                else
                {
                    if ((Local != null &&
                        (row.Local.Address.Address == Local.Address.Address) &&
                        (row.Local.Port == Local.Port)))
                    {
                        this.Add(row);
                        connection = row;
                        return true;
                    }
                }
            }
            // free the buffer
            Marshal.FreeHGlobal(lpTable);
            // re init the pointer
            current = IntPtr.Zero;
            return flag;
        }

        protected virtual TCPUDPConnection NewTCPUDPConnection()
        {
            return new TCPUDPConnection(this);
        }

        public string LocalHostName
        {
            get { return _LocalHostName; }
        }

        public void GetUdpConnections()
        {
            TCPUDPConnection conn = null; 
            GetUdpConnections(null, out conn);
        }

        public bool GetUdpConnections(IPEndPoint Local, out TCPUDPConnection connection)
        {
            bool flag = false;
            connection = null;
            int AF_INET = 2;    // IP_v4
            int buffSize = 20000;
            byte[] buffer = new byte[buffSize];
            int res = IPHlpAPI32Wrapper.GetExtendedUdpTable(buffer, out buffSize, true, AF_INET, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
            if (res != Utils.NO_ERROR)
            {
                buffer = new byte[buffSize];
                res = IPHlpAPI32Wrapper.GetExtendedUdpTable(buffer, out buffSize, true, AF_INET, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
                if (res != Utils.NO_ERROR)
                {
                    return false;
                }
            }
            int nOffset = 0;
            int NumEntries = Convert.ToInt32(buffer[nOffset]);
            nOffset += 4;
            for (int i = 0; i < NumEntries; i++)
            {
                TCPUDPConnection row = NewTCPUDPConnection();
                row.Protocol = Protocol.UDP;
                row.Local = Utils.BufferToIPEndPoint(buffer, ref nOffset, false);
                row.PID = Utils.BufferToInt(buffer, ref nOffset);
                row.IsRefresh = true;
                if (Local == null)
                {
                    this.Add(row);
                    flag = true;
                }
                else
                {
                    if ((Local != null &&
                        (row.Local.Address.Address == Local.Address.Address) &&
                        (row.Local.Port == Local.Port)))
                    {
                        this.Add(row);
                        connection = row;
                        return true;
                    }
                }
            }
            return flag;
        }

        public bool GetExUdpConnections()
        {
            TCPUDPConnection conn = null;
            return GetExUdpConnections(null, out conn);
        }

        public bool GetExUdpConnections(IPEndPoint Local, out TCPUDPConnection connection)
        {
            bool flag = false;
            connection = null;
            // the size of the MIB_EXTCPROW struct =  4*DWORD
            int rowsize = 12;
            int BufferSize = 100000;
            // allocate a dumb memory space in order to retrieve  nb of connection
            IntPtr lpTable = Marshal.AllocHGlobal(BufferSize);
            //getting infos
            int res = IPHlpAPI32Wrapper.AllocateAndGetUdpExTableFromStack(ref lpTable, true, IPHlpAPI32Wrapper.GetProcessHeap(), 0, 2);
            if (res != Utils.NO_ERROR)
            {
                log.Error(IPHlpAPI32Wrapper.GetAPIErrorMessageDescription(res) + " " + res);
                return false; // Error. You should handle it
            }
            int CurrentIndex = 0;
            //get the number of entries in the table
            int NumEntries = (int)Marshal.ReadIntPtr(lpTable);
            lpTable = IntPtr.Zero;
            // free allocated space in memory
            Marshal.FreeHGlobal(lpTable);

            ///////////////////
            // calculate the real buffer size nb of entrie * size of the struct for each entrie(24) + the dwNumEntries
            BufferSize = (NumEntries * rowsize) + 4;
            // Allocate memory
            lpTable = Marshal.AllocHGlobal(BufferSize);
            res = IPHlpAPI32Wrapper.AllocateAndGetUdpExTableFromStack(ref lpTable, true, IPHlpAPI32Wrapper.GetProcessHeap(), 0, 2);
            if (res != Utils.NO_ERROR)
            {
                log.Error(IPHlpAPI32Wrapper.GetAPIErrorMessageDescription(res) + " " + res);
                return false; // Error. You should handle it
            }
            // New pointer of iterating throught the data
            IntPtr current = lpTable;
            CurrentIndex = 0;
            // get the (again) the number of entries
            NumEntries = (int)Marshal.ReadIntPtr(current);
            // iterate the pointer of 4 (the size of the DWORD dwNumEntries)
            CurrentIndex += 4;
            current = (IntPtr)((int)current + CurrentIndex);
            // for each entries
            for (int i = 0; i < NumEntries; i++)
            {
                TCPUDPConnection row = NewTCPUDPConnection();
                row.Protocol = Protocol.UDP;
                // get the local address of the connection
                UInt32 localAddr = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // get the local port of the connection
                UInt32 localPort = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // Store the local endpoint in the struct and convertthe port in decimal (ie convert_Port())
                row.Local = new IPEndPoint(localAddr, Utils.ConvertPort(localPort));
                // store the process ID
                row.PID = (int)Marshal.ReadIntPtr(current);
                current = (IntPtr)((int)current + 4);
                if (Local == null)
                {
                    this.Add(row);
                    flag = true;
                }
                else
                {
                    if ((Local != null &&
                        (row.Local.Address.Address == Local.Address.Address) &&
                        (row.Local.Port == Local.Port)))
                    {
                        this.Add(row);
                        connection = row;
                        return true;
                    }
                }
            }
            // free the buffer
            Marshal.FreeHGlobal(lpTable);
            // re init the pointer
            current = IntPtr.Zero;
            return flag;
        }
        
        #region IEnumerable<TCPUDPConnection> Members

        public IEnumerator<TCPUDPConnection> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }

    public class IPHelper
    {
        /*
         * Tcp Struct
         * */
        public IpHlpApidotnet.MIB_TCPTABLE TcpConnections;
        public IpHlpApidotnet.MIB_TCPSTATS TcpStats;
        public IpHlpApidotnet.MIB_EXTCPTABLE TcpExConnections;
        //public IpHlpApidotnet.MIB_TCPTABLE_OWNER_PID TcpExAllConnections;

        /*
         * Udp Struct
         * */
        public IpHlpApidotnet.MIB_UDPSTATS UdpStats;
        public IpHlpApidotnet.MIB_UDPTABLE UdpConnections;
        public IpHlpApidotnet.MIB_EXUDPTABLE UdpExConnections;
        //public IpHlpApidotnet.MIB_UDPTABLE_OWNER_PID UdpExAllConnections;

        public TCPUDPConnections Connections; 

        public IPHelper()
        {

        }

        #region Tcp Functions

        public void GetTcpStats()
        {
            TcpStats = new MIB_TCPSTATS();
            IPHlpAPI32Wrapper.GetTcpStatistics(ref TcpStats);
        }

        public void GetExTcpConnections()
        {
            // the size of the MIB_EXTCPROW struct =  6*DWORD
            int rowsize = 24;
            int BufferSize = 100000;
            // allocate a dumb memory space in order to retrieve  nb of connection
            IntPtr lpTable = Marshal.AllocHGlobal(BufferSize);
            //getting infos
            int res = IPHlpAPI32Wrapper.AllocateAndGetTcpExTableFromStack(ref lpTable, true, IPHlpAPI32Wrapper.GetProcessHeap(), 0, 2);
            if (res != Utils.NO_ERROR)
            {
                Debug.WriteLine("Error : " + IPHlpAPI32Wrapper.GetAPIErrorMessageDescription(res) + " " + res);
                return; // Error. You should handle it
            }
            int CurrentIndex = 0;
            //get the number of entries in the table
            int NumEntries = (int)Marshal.ReadIntPtr(lpTable);
            lpTable = IntPtr.Zero;
            // free allocated space in memory
            Marshal.FreeHGlobal(lpTable);
            ///////////////////
            // calculate the real buffer size nb of entrie * size of the struct for each entrie(24) + the dwNumEntries
            BufferSize = (NumEntries * rowsize) + 4;
            // make the struct to hold the resullts
            TcpExConnections = new IpHlpApidotnet.MIB_EXTCPTABLE();
            // Allocate memory
            lpTable = Marshal.AllocHGlobal(BufferSize);
            res = IPHlpAPI32Wrapper.AllocateAndGetTcpExTableFromStack(ref lpTable, true, IPHlpAPI32Wrapper.GetProcessHeap(), 0, 2);
            if (res != Utils.NO_ERROR)
            {
                Debug.WriteLine("Error : " + IPHlpAPI32Wrapper.GetAPIErrorMessageDescription(res) + " " + res);
                return; // Error. You should handle it
            }
            // New pointer of iterating throught the data
            IntPtr current = lpTable;
            CurrentIndex = 0;
            // get the (again) the number of entries
            NumEntries = (int)Marshal.ReadIntPtr(current);
            TcpExConnections.dwNumEntries = NumEntries;
            // Make the array of entries
            TcpExConnections.table = new MIB_EXTCPROW[NumEntries];
            // iterate the pointer of 4 (the size of the DWORD dwNumEntries)
            CurrentIndex += 4;
            current = (IntPtr)((int)current + CurrentIndex);
            // for each entries
            for (int i = 0; i < NumEntries; i++)
            {

                // The state of the connection (in string)
                TcpExConnections.table[i].StrgState = Utils.StateToStr((int)Marshal.ReadIntPtr(current));
                // The state of the connection (in ID)
                TcpExConnections.table[i].iState = (int)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // get the local address of the connection
                UInt32 localAddr = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // get the local port of the connection
                UInt32 localPort = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // Store the local endpoint in the struct and convertthe port in decimal (ie convert_Port())
                TcpExConnections.table[i].Local = new IPEndPoint(localAddr, (int)Utils.ConvertPort(localPort));
                // get the remote address of the connection
                UInt32 RemoteAddr = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                UInt32 RemotePort = 0;
                // if the remote address = 0 (0.0.0.0) the remote port is always 0
                // else get the remote port
                if (RemoteAddr != 0)
                {
                    RemotePort = (UInt32)Marshal.ReadIntPtr(current);
                    RemotePort = Utils.ConvertPort(RemotePort);
                }
                current = (IntPtr)((int)current + 4);
                // store the remote endpoint in the struct  and convertthe port in decimal (ie convert_Port())
                TcpExConnections.table[i].Remote = new IPEndPoint(RemoteAddr, (int)RemotePort);
                // store the process ID
                TcpExConnections.table[i].dwProcessId = (int)Marshal.ReadIntPtr(current);
                // Store and get the process name in the struct
                TcpExConnections.table[i].ProcessName = Utils.GetProcessNameByPID(TcpExConnections.table[i].dwProcessId);
                current = (IntPtr)((int)current + 4);

            }
            // free the buffer
            Marshal.FreeHGlobal(lpTable);
            // re init the pointer
            current = IntPtr.Zero;
        }

        public TcpConnectionInformation[] GetTcpConnectionsNative()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            return properties.GetActiveTcpConnections();
        }

        public IPEndPoint[] GetUdpListeners()
        {
            return IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners();
        }

        public IPEndPoint[] GetTcpListeners()
        {
            return IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
        }

        public void GetTcpConnections()
        {
            byte[] buffer = new byte[20000]; // Start with 20.000 bytes left for information about tcp table
            int pdwSize = 20000;
            int res = IPHlpAPI32Wrapper.GetTcpTable(buffer, out pdwSize, true);
            if (res != Utils.NO_ERROR)
            {
                buffer = new byte[pdwSize];
                res = IPHlpAPI32Wrapper.GetTcpTable(buffer, out pdwSize, true);
                if (res != 0)
                    return;     // Error. You should handle it
            }

            TcpConnections = new IpHlpApidotnet.MIB_TCPTABLE();

            int nOffset = 0;
            // number of entry in the
            TcpConnections.dwNumEntries = Convert.ToInt32(buffer[nOffset]);
            nOffset += 4;
            TcpConnections.table = new MIB_TCPROW[TcpConnections.dwNumEntries];

            for (int i = 0; i < TcpConnections.dwNumEntries; i++)
            {
                // state
                int st = Convert.ToInt32(buffer[nOffset]);
                // state in string
                ((MIB_TCPROW)(TcpConnections.table[i])).StrgState = Utils.StateToStr(st);
                // state  by ID
                ((MIB_TCPROW)(TcpConnections.table[i])).iState = st;
                nOffset += 4;
                // local address
                ((MIB_TCPROW)(TcpConnections.table[i])).Local = Utils.BufferToIPEndPoint(buffer, ref nOffset, false);
                // remote address
                ((MIB_TCPROW)(TcpConnections.table[i])).Remote = Utils.BufferToIPEndPoint(buffer, ref nOffset, true);
            }
        }

        #endregion

        #region Udp Functions

        public void GetUdpStats()
        {

            UdpStats = new MIB_UDPSTATS();
            IPHlpAPI32Wrapper.GetUdpStatistics(ref UdpStats);
        }

        public void GetUdpConnections()
        {
            byte[] buffer = new byte[20000]; // Start with 20.000 bytes left for information about tcp table
            int pdwSize = 20000;
            int res = IPHlpAPI32Wrapper.GetUdpTable(buffer, out pdwSize, true);
            if (res != Utils.NO_ERROR)
            {
                buffer = new byte[pdwSize];
                res = IPHlpAPI32Wrapper.GetUdpTable(buffer, out pdwSize, true);
                if (res != Utils.NO_ERROR)
                    return;     // Error. You should handle it
            }

            UdpConnections = new IpHlpApidotnet.MIB_UDPTABLE();

            int nOffset = 0;
            // number of entry in the
            UdpConnections.dwNumEntries = Convert.ToInt32(buffer[nOffset]);
            nOffset += 4;
            UdpConnections.table = new MIB_UDPROW[UdpConnections.dwNumEntries];
            for (int i = 0; i < UdpConnections.dwNumEntries; i++)
            {
                ((MIB_UDPROW)(UdpConnections.table[i])).Local = Utils.BufferToIPEndPoint(buffer, ref nOffset, false);//new IPEndPoint(IPAddress.Parse(LocalAdrr), LocalPort);
            }
        }

        public void GetExUdpConnections()
        {
            // the size of the MIB_EXTCPROW struct =  4*DWORD
            int rowsize = 12;
            int BufferSize = 100000;
            // allocate a dumb memory space in order to retrieve  nb of connection
            IntPtr lpTable = Marshal.AllocHGlobal(BufferSize);
            //getting infos
            int res = IPHlpAPI32Wrapper.AllocateAndGetUdpExTableFromStack(ref lpTable, true, IPHlpAPI32Wrapper.GetProcessHeap(), 0, 2);
            if (res != Utils.NO_ERROR)
            {
                Debug.WriteLine("Error : " + IPHlpAPI32Wrapper.GetAPIErrorMessageDescription(res) + " " + res);
                return; // Error. You should handle it
            }
            int CurrentIndex = 0;
            //get the number of entries in the table
            int NumEntries = (int)Marshal.ReadIntPtr(lpTable);
            lpTable = IntPtr.Zero;
            // free allocated space in memory
            Marshal.FreeHGlobal(lpTable);

            ///////////////////
            // calculate the real buffer size nb of entrie * size of the struct for each entrie(24) + the dwNumEntries
            BufferSize = (NumEntries * rowsize) + 4;
            // make the struct to hold the resullts
            UdpExConnections = new IpHlpApidotnet.MIB_EXUDPTABLE();
            // Allocate memory
            lpTable = Marshal.AllocHGlobal(BufferSize);
            res = IPHlpAPI32Wrapper.AllocateAndGetUdpExTableFromStack(ref lpTable, true, IPHlpAPI32Wrapper.GetProcessHeap(), 0, 2);
            if (res != Utils.NO_ERROR)
            {
                Debug.WriteLine("Error : " + IPHlpAPI32Wrapper.GetAPIErrorMessageDescription(res) + " " + res);
                return; // Error. You should handle it
            }
            // New pointer of iterating throught the data
            IntPtr current = lpTable;
            CurrentIndex = 0;
            // get the (again) the number of entries
            NumEntries = (int)Marshal.ReadIntPtr(current);
            UdpExConnections.dwNumEntries = NumEntries;
            // Make the array of entries
            UdpExConnections.table = new MIB_EXUDPROW[NumEntries];
            // iterate the pointer of 4 (the size of the DWORD dwNumEntries)
            CurrentIndex += 4;
            current = (IntPtr)((int)current + CurrentIndex);
            // for each entries
            for (int i = 0; i < NumEntries; i++)
            {
                // get the local address of the connection
                UInt32 localAddr = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // get the local port of the connection
                UInt32 localPort = (UInt32)Marshal.ReadIntPtr(current);
                // iterate the pointer of 4
                current = (IntPtr)((int)current + 4);
                // Store the local endpoint in the struct and convertthe port in decimal (ie convert_Port())
                UdpExConnections.table[i].Local = new IPEndPoint(localAddr, Utils.ConvertPort(localPort));
                // store the process ID
                UdpExConnections.table[i].dwProcessId = (int)Marshal.ReadIntPtr(current);
                // Store and get the process name in the struct
                UdpExConnections.table[i].ProcessName = Utils.GetProcessNameByPID(UdpExConnections.table[i].dwProcessId);
                current = (IntPtr)((int)current + 4);

            }
            // free the buffer
            Marshal.FreeHGlobal(lpTable);
            // re init the pointer
            current = IntPtr.Zero;
        }

        #endregion
    }

    public static class Utils
    {
        public const int NO_ERROR = 0;
        public const int MIB_TCP_STATE_CLOSED = 1;
        public const int MIB_TCP_STATE_LISTEN = 2;
        public const int MIB_TCP_STATE_SYN_SENT = 3;
        public const int MIB_TCP_STATE_SYN_RCVD = 4;
        public const int MIB_TCP_STATE_ESTAB = 5;
        public const int MIB_TCP_STATE_FIN_WAIT1 = 6;
        public const int MIB_TCP_STATE_FIN_WAIT2 = 7;
        public const int MIB_TCP_STATE_CLOSE_WAIT = 8;
        public const int MIB_TCP_STATE_CLOSING = 9;
        public const int MIB_TCP_STATE_LAST_ACK = 10;
        public const int MIB_TCP_STATE_TIME_WAIT = 11;
        public const int MIB_TCP_STATE_DELETE_TCB = 12;

        #region helper function

        public static UInt16 ConvertPort(UInt32 dwPort)
        {
            byte[] b = new Byte[2];
            // high weight byte
            b[0] = byte.Parse((dwPort >> 8).ToString());
            // low weight byte
            b[1] = byte.Parse((dwPort & 0xFF).ToString());
            return BitConverter.ToUInt16(b, 0);
        }

        public static int BufferToInt(byte[] buffer, ref int nOffset)
        {
            int res = (((int)buffer[nOffset])) + (((int)buffer[nOffset + 1]) << 8) +
                (((int)buffer[nOffset + 2]) << 16) + (((int)buffer[nOffset + 3]) << 24);
            nOffset += 4;
            return res;
        }

        public static string StateToStr(int state)
        {
            string strg_state = "";
            switch (state)
            {
                case MIB_TCP_STATE_CLOSED: strg_state = "CLOSED"; break;
                case MIB_TCP_STATE_LISTEN: strg_state = "LISTEN"; break;
                case MIB_TCP_STATE_SYN_SENT: strg_state = "SYN_SENT"; break;
                case MIB_TCP_STATE_SYN_RCVD: strg_state = "SYN_RCVD"; break;
                case MIB_TCP_STATE_ESTAB: strg_state = "ESTAB"; break;
                case MIB_TCP_STATE_FIN_WAIT1: strg_state = "FIN_WAIT1"; break;
                case MIB_TCP_STATE_FIN_WAIT2: strg_state = "FIN_WAIT2"; break;
                case MIB_TCP_STATE_CLOSE_WAIT: strg_state = "CLOSE_WAIT"; break;
                case MIB_TCP_STATE_CLOSING: strg_state = "CLOSING"; break;
                case MIB_TCP_STATE_LAST_ACK: strg_state = "LAST_ACK"; break;
                case MIB_TCP_STATE_TIME_WAIT: strg_state = "TIME_WAIT"; break;
                case MIB_TCP_STATE_DELETE_TCB: strg_state = "DELETE_TCB"; break;
            }
            return strg_state;
        }

        public static IPEndPoint BufferToIPEndPoint(byte[] buffer, ref int nOffset, bool IsRemote)
        {
            //address
            Int64 m_Address = ((((buffer[nOffset + 3] << 0x18) | (buffer[nOffset + 2] << 0x10)) | (buffer[nOffset + 1] << 8)) | buffer[nOffset]) & ((long)0xffffffff);
            nOffset += 4;
            int m_Port = 0;
            m_Port = (IsRemote && (m_Address == 0))? 0 : 
                        (((int)buffer[nOffset]) << 8) + (((int)buffer[nOffset + 1])) + (((int)buffer[nOffset + 2]) << 24) + (((int)buffer[nOffset + 3]) << 16);
            nOffset += 4;

            // store the remote endpoint
            IPEndPoint temp = new IPEndPoint(m_Address, m_Port);
            if (temp == null)
                Debug.WriteLine("Parsed address is null. Addr=" + m_Address.ToString() + " Port=" + m_Port + " IsRemote=" + IsRemote.ToString());
            return temp;
        }


        public static string TrafficToStr(int UsefulTraffic, int RealTraffic)
        {
            return Utils.TrafficToStr(UsefulTraffic) + "/" + Utils.TrafficToStr(RealTraffic);
        }

        public static string TrafficToStr(int Bytes)
        {
            return Utils.TrafficToStr((Double)Bytes, false);
        }

        public static string TrafficToStr(double Bytes, bool IsSpeed)
        {
            double tmp = Bytes / 1024;
            string speed = "";
            if (IsSpeed)
                speed = "/s";

            if (tmp < 1)
            {
                return Bytes.ToString("0.##");
            }
            Double tmp1 = tmp / 1024;
            if (tmp1 < 1)
            {
                return tmp.ToString("0.##") + " Kb" + speed;
            }
            Double tmp2 = tmp1 / 1024;
            if (tmp2 < 1)
            {
                return tmp1.ToString("0.##") + " Mb" + speed;
            }
            Double tmp3 = tmp2 / 1024;
            if (tmp2 < 1)
            {
                return tmp2.ToString("0.##") + " Gb" + speed;
            }
            return Bytes.ToString();
        }

        public static string GetHostName(IPEndPoint HostAddress, string LocalHostName)
        {
            try
            {
                if (HostAddress.Address.Address == 0)//ToString() == "0.0.0.0")
                {
                    if (HostAddress.Port > 0)
                        return LocalHostName + ":" + HostAddress.Port.ToString();
                    else
                        return "Anyone";
                }
                return Dns.GetHostEntry(HostAddress.Address).HostName + ":" + HostAddress.Port.ToString();
            }
            catch
            {
                return HostAddress.ToString();
            }
        }

        public static string GetLocalHostName()
        {
            //IPGlobalProperties.GetIPGlobalProperties().DomainName +"." + IPGlobalProperties.GetIPGlobalProperties().HostName
            return Dns.GetHostEntry("localhost").HostName;
        }

        private static IPHostEntry _hostentry;
        private static bool _hostentryflag;
        public static IPHostEntry UseDNS()
        {
            if (!_hostentryflag)
            {
                string hostName = Dns.GetHostName();
                _hostentry = Dns.GetHostEntry(hostName);
            }
            return _hostentry;
        }

        public static System.Net.IPAddress[] LocalAddressList = null; 
        public static System.Net.IPAddress[] GetAddressList(bool Refresh)
        {
            if (LocalAddressList == null || LocalAddressList.Length < 0 || Refresh)
            {
                LocalAddressList = UseDNS().AddressList;
            }
            return LocalAddressList;
        }

        public static bool IsLocalIPAddress(System.Net.IPAddress IP)
        {
            try
            {
                if (IP.ToString() == "255.255.255.255")
                    return false;
                if (System.Net.IPAddress.IsLoopback(IP))
                    return true;
                foreach (System.Net.IPAddress ip in GetAddressList(false))
                {
                    if (ip.Address == IP.Address)
                        return true;
                }
                return false;
            }
            catch 
            { 
                return false; 
            }
        }

        public static int CompareIPEndPoints(IPEndPoint first, IPEndPoint second)
        {
            int i;
            byte[] _first = first.Address.GetAddressBytes();
            byte[] _second = second.Address.GetAddressBytes();
            for (int j = 0; j < _first.Length; j++)
            {
                i = _first[j] - _second[j];
                if (i != 0)
                    return i;
            }
            i = first.Port - second.Port;
            if (i != 0)
                return i;
            return 0;
        }

        public static string GetProcessNameByPID(int processID)
        {
            //could be an error here if the process die before we can get his name
            try
            {
                if (processID == -1)
                    return "Unknown";
                return Process.GetProcessById(processID).ProcessName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "Unknown";
            }
        }
        #endregion
    }
}
