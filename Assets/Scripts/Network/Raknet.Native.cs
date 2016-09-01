using System;
using System.Runtime.InteropServices;

namespace Networking
{
    namespace Raknet
    {
        unsafe public class Native
        {
            [DllImport("RustNative")]
            public static extern IntPtr NET_Create();

            [DllImport("RustNative")]
            public static extern void NET_Close(IntPtr nw);

            [DllImport("RustNative")]
            public static extern Int32 NET_StartClient(IntPtr nw, string hostName, Int32 port, Int32 retries, Int32 retryDelay, Int32 timeout);

            [DllImport("RustNative")]
            public static extern Int32 NET_StartServer(IntPtr nw, string ip, Int32 port, Int32 maxConnections);

            [DllImport("RustNative")]
            public static extern void NET_StartLogging(IntPtr nw, string filename);

            [DllImport("RustNative")]
            public static extern void NET_StopLogging(IntPtr nw);

            [DllImport("RustNative")]
            public static extern IntPtr NET_LastStartupError(IntPtr nw);

            [DllImport("RustNative")]
            public static extern Boolean NET_Receive(IntPtr nw);

            [DllImport("RustNative")]
            public static extern UInt64 NETRCV_GUID(IntPtr nw);

            [DllImport("RustNative")]
            public static extern Int32 NETRCV_LengthBits(IntPtr nw);

            [DllImport("RustNative")]
            public static extern Int32 NETRCV_UnreadBits(IntPtr nw);

            [DllImport("RustNative")]
            public static extern float NETRCV_GetAge(IntPtr nw);

            [DllImport("RustNative")]
            public static extern bool NETRCV_ReadBytes(IntPtr nw, byte* data, int length);

            [DllImport("RustNative")]
            public static extern void NETSND_Start(IntPtr nw);

            [DllImport("RustNative")]
            public static extern void NETSND_WriteBytes(IntPtr nw, byte* data, Int32 length);

            [DllImport("RustNative")]
            public static extern UInt32 NETSND_Size(IntPtr nw);

            [DllImport("RustNative")]
            public static extern UInt32 NETSND_Broadcast(IntPtr nw, Int32 priority, Int32 reliability, Int32 channel);

            [DllImport("RustNative")]
            public static extern UInt32 NETSND_Send(IntPtr nw, UInt64 connectionID, Int32 priority, Int32 reliability, Int32 channel);

            [DllImport("RustNative")]
            public static extern void NET_CloseConnection(IntPtr nw, UInt64 connectionID);

            [DllImport("RustNative")]
            public static extern IntPtr NET_GetAddress(IntPtr nw, UInt64 connectionID);

            [DllImport("RustNative")]
            public static extern IntPtr NET_GetStatisticsString(IntPtr nw, UInt64 connectionID);

            [DllImport("RustNative")]
            public static extern bool NET_GetStatistics(IntPtr nw, UInt64 connectionID, ref Native.RaknetStats data, Int32 dataLength);

            [DllImport("RustNative")]
            public static extern Int32 NET_GetAveragePing(IntPtr nw, UInt64 connectionID);

            [DllImport("RustNative")]
            public static extern Int32 NET_GetLastPing(IntPtr nw, UInt64 connectionID);

            [DllImport("RustNative")]
            public static extern Int32 NET_GetLowestPing(IntPtr nw, UInt64 connectionID);

            [DllImport("RustNative")]
            public static extern void NET_ErrorDump(string headerName);

            public enum Metrics : int
            {
                /// How many bytes per pushed via a call to RakPeerInterface::Send()
                USER_MESSAGE_BYTES_PUSHED,

                /// How many user message bytes were sent via a call to RakPeerInterface::Send(). This is less than or equal to USER_MESSAGE_BYTES_PUSHED.
                /// A message would be pushed, but not yet sent, due to congestion control
                USER_MESSAGE_BYTES_SENT,

                /// How many user message bytes were resent. A message is resent if it is marked as reliable, and either the message didn't arrive or the message ack didn't arrive.
                USER_MESSAGE_BYTES_RESENT,

                /// How many user message bytes were received, and returned to the user successfully.
                USER_MESSAGE_BYTES_RECEIVED_PROCESSED,

                /// How many user message bytes were received, but ignored due to data format errors. This will usually be 0.
                USER_MESSAGE_BYTES_RECEIVED_IGNORED,

                /// How many actual bytes were sent, including per-message and per-datagram overhead, and reliable message acks
                ACTUAL_BYTES_SENT,

                /// How many actual bytes were received, including overead and acks.
                ACTUAL_BYTES_RECEIVED,

                /// \internal
                RNS_PER_SECOND_METRICS_COUNT
            };

            public enum PacketPriority : int
            {
                /// The highest possible priority. These message trigger sends immediately, and are generally not buffered or aggregated into a single datagram.
                IMMEDIATE_PRIORITY,

                /// For every 2 IMMEDIATE_PRIORITY messages, 1 HIGH_PRIORITY will be sent.
                /// Messages at this priority and lower are buffered to be sent in groups at 10 millisecond intervals to reduce UDP overhead and better measure congestion control. 
                HIGH_PRIORITY,

                /// For every 2 HIGH_PRIORITY messages, 1 MEDIUM_PRIORITY will be sent.
                /// Messages at this priority and lower are buffered to be sent in groups at 10 millisecond intervals to reduce UDP overhead and better measure congestion control. 
                MEDIUM_PRIORITY,

                /// For every 2 MEDIUM_PRIORITY messages, 1 LOW_PRIORITY will be sent.
                /// Messages at this priority and lower are buffered to be sent in groups at 10 millisecond intervals to reduce UDP overhead and better measure congestion control. 
                LOW_PRIORITY,

                /// \internal
                NUMBER_OF_PRIORITIES
            };

            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct RaknetStats
            {
                public unsafe fixed System.UInt64 valueOverLastSecond[7];
                public unsafe fixed System.UInt64 runningTotal[7];

                /// When did the connection start?
                /// \sa RakNet::GetTimeUS()
                public unsafe System.UInt64 connectionStartTime;

                /// Is our current send rate throttled by congestion control?
                /// This value should be true if you send more data per second than your bandwidth capacity
                public unsafe System.Byte isLimitedByCongestionControl;

                /// If \a isLimitedByCongestionControl is true, what is the limit, in bytes per second?
                public unsafe System.UInt64 BPSLimitByCongestionControl;

                /// Is our current send rate throttled by a call to RakPeer::SetPerConnectionOutgoingBandwidthLimit()?
                public unsafe System.Byte isLimitedByOutgoingBandwidthLimit;

                /// If \a isLimitedByOutgoingBandwidthLimit is true, what is the limit, in bytes per second?
                public unsafe System.UInt64 BPSLimitByOutgoingBandwidthLimit;

                /// For each priority level, how many messages are waiting to be sent out?
                public unsafe fixed System.UInt32 messageInSendBuffer[4];

                /// For each priority level, how many bytes are waiting to be sent out?
                public unsafe fixed System.Double bytesInSendBuffer[4];

                /// How many messages are waiting in the resend buffer? This includes messages waiting for an ack, so should normally be a small value
                /// If the value is rising over time, you are exceeding the bandwidth capacity. See BPSLimitByCongestionControl 
                public unsafe System.UInt32 messagesInResendBuffer;

                /// How many bytes are waiting in the resend buffer. See also messagesInResendBuffer
                public unsafe System.UInt64 bytesInResendBuffer;

                /// Over the last second, what was our packetloss? This number will range from 0.0 (for none) to 1.0 (for 100%)
                public unsafe System.Single packetlossLastSecond;

                /// What is the average total packetloss over the lifetime of the connection?
                public unsafe System.Single packetlossTotal;
            }
        }

        public static class PacketType
        {
            public const byte NEW_INCOMING_CONNECTION = 19;
            public const byte CONNECTION_REQUEST_ACCEPTED = 16;
            public const byte CONNECTION_ATTEMPT_FAILED = 17;
            public const byte DISCONNECTION_NOTIFICATION = 21;
            public const byte CONNECTION_LOST = 22;
            public const byte CONNECTION_BANNED = 23;
        }

        internal class Defines
        {
            public const byte lowestUserPacket = 140;
        }
    }
}