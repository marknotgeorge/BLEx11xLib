using System;

namespace BLED112Lib
{
    [Flags]
    public enum ConnectionStatusFlags
    {
        Connected = 0x01,
        Encrypted = 0x02,
        Completed = 0x04,
        ParametersChanged = 0x08
    }

    public enum GAPAdType
    {
        None = 0,
        Flags = 1,
        Services16BitMore = 2,
        Services16BitAll = 3,
        Services32BitMore = 4,
        Services32BitAll = 5,
        Services128BitMore = 6,
        Services128BitAll = 7,
        LocalNameShort = 8,
        LocalNameComplete = 9,
        TXPower = 10
    }

    [Flags]
    public enum GAPAdFlags
    {
        LimitedDiscoverable = 0x01,
        GeneralDiscoverable = 0x02,
        BREDRNotSupported = 0x04,
        BREDRController = 0x10,
        BREDRHost = 0x20,
        Mask = 0x1f
    }

    public enum GAPAdPolicy
    {
        All = 0,
        WhitelistScan = 1,
        WhitelistConnect = 2,
        WhitelistAll = 3
    }

    public enum GAPAddressType
    {
        Public = 0,
        Random = 1
    }

    public enum GAPConnectableMode: byte  
    {
        NonConnectable = 0,
        DirectedConnectable = 1,
        UndirectedConnectable = 2,
        ScannableConnectable = 3
    }

    public enum GAPDiscoverableMode: byte
    {
        NonDiscoverable = 0,
        LimitedDiscoverable = 1,
        GeneralDiscoverable = 2,
        Broadcast = 3,
        UserData = 4,
        EnhancedBroadcasting = 0x80
    }

    public enum GAPDiscoverMode: byte
    {
        Limited = 0,
        Generic = 1,
        Observation = 2
    }
   
    public enum GAPScanHeaderFlags
    {
        ConnectableUndirectedAdvertising = 0,
        ConnectableDirectedAdvertising = 1,
        NonConnectableUndirectedAdvertising = 2,
        ScanRequest = 3,
        ScanResponse = 4, 
        ConnectRequest = 5,
        Discover = 6
    }

    public enum GAPScanPolicy
    {
        All = 0,
        Whitelist = 1
    }

    public enum DisconnectedReason: ushort
    {
        DisconnectedByLocalUser = 0
    }

    public enum SMBondableMode: byte
    {
        False = 0,
        True = 1
    }


}