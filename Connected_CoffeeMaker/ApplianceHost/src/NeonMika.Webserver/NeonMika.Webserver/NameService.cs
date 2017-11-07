using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.NetworkInformation;

namespace NeonMika.Webserver
{
    /// <summary>
    /// NetBios NameService class for .NET Micro Framework.
    /// Programmed by Wouter Huysentruit
    /// Copyright (C) 2011 Fastload-Media
    ///
    /// This program is free software: you can redistribute it and/or modify
    /// it under the terms of the GNU General Public License as published by
    /// the Free Software Foundation, either version 3 of the License, or
    /// any later version.
    ///
    /// This program is distributed in the hope that it will be useful,
    /// but WITHOUT ANY WARRANTY; without even the implied warranty of
    /// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    /// GNU General Public License for more details.
    ///
    /// You should have received a copy of the GNU General Public License
    /// along with this program. If not, see <see cref="http://www.gnu.org/licenses"/>.
    /// </summary>
    public class NameService : IDisposable
    {
        #region Consts

        private const ushort NAME_TRN_ID = 0x6703; // unique transaction id
        private const int BCAST_REQ_RETRY_TIMEOUT = 250;
        private const int BCAST_REQ_RETRY_COUNT = 3;
        private const int BCAST_NS_PORT = 137;
        private const int NAME_UPDATE_INTERVAL_MS = 10 * 60 * 1000; // 10 minutes, represented as ms

        #endregion

        #region Definitions

        /// <summary>
        /// Available name types.
        /// </summary>
        public enum NameType
        {
            /// <summary>
            /// Unique name. (F.e. workstation)
            /// </summary>
            Unique,

            /// <summary>
            /// Group name. (F.e. domain)
            /// </summary>
            Group
        }

        /// <summary>
        /// Represents a single name as added to the namelist.
        /// </summary>
        private struct Name
        {
            public string UncompressedName;
            public NameType Type;
        }

        /// <summary>
        /// Most used Microsoft suffixes for NetBIOS names.
        /// </summary>
        public enum MsSuffix : byte
        {
            /// <summary>
            /// Unique workstation computername or default group name.
            /// </summary>
            Default = 0x00,

            /// <summary>
            /// Group name for master browser (<\\--__MSBROWSE__>).
            /// </summary>
            MasterBrowserGroup = 0x01,

            /// <summary>
            /// Unique domainname.
            /// </summary>
            MasterBrowserUnique = 0x1D,

            /// <summary>
            /// Group domain name.
            /// </summary>
            BrowserServiceElections = 0x1E,

            /// <summary>
            /// Unique computername for file server.
            /// </summary>
            FileServerService = 0x20,
        }

        /// <summary>
        /// Available flags used in the header.
        /// </summary>
        //[Flags]
        private enum HeaderFlags : byte
        {
            /// <summary>
            /// Broadcast flag.
            /// </summary>
            Broadcast = 0x01,

            /// <summary>
            /// Recursion available flag. Only valid in responses from a NetBIOS Name Server.
            /// Must be zero in all other responses.
            /// </summary>
            RecursionAvailable = 0x08,

            /// <summary>
            /// Recursion desired flag. May only be set on a request to a NetBIOS Name Server.
            /// </summary>
            RecursionDesired = 0x10,

            /// <summary>
            /// Truncation flag.
            /// Set if this message was truncated because the datagram carrying it would be greater than 576 bytes in length.
            /// </summary>
            Truncation = 0x20,

            /// <summary>
            /// Must be zero if Response is false.
            /// If set, then the node responing is an authority for the domain name.
            /// </summary>
            AuthoritativeAnswer = 0x40
        }

        /// <summary>
        /// Available opcodes used in the header.
        /// </summary>
        private enum HeaderOpcode : byte
        {
            Query = 0,
            Registration = 5,
            Release = 6,
            WACK = 7,
            Refresh = 8,
            Update = 255 // Special case: is a name registration request with RD bit cleared
        }

        /// <summary>
        /// Packet header.
        /// </summary>
        private struct Header
        {
            /// <summary>
            /// Transaction ID (chosen by requestor).
            /// </summary>
            public ushort NameTrnId;
            /// <summary>
            /// True for response, false for request.
            /// </summary>
            public bool Response;
            /// <summary>
            /// Operation code.
            /// </summary>
            public HeaderOpcode Opcode;
            /// <summary>
            /// Flags.
            /// </summary>
            public HeaderFlags Flags;
            /// <summary>
            /// Result codes of request.
            /// </summary>
            public byte Rcode;
            /// <summary>
            /// Number of entries in the question section of a Name Service packet.
            /// Always zero for response. Must be non-zero for all NetBIOS Name requests.
            /// </summary>
            public ushort QdCount;
            /// <summary>
            /// Number of resource records in the answer section of a Name Service packet.
            /// </summary>
            public ushort AnCount;
            /// <summary>
            /// Number of resource records in the authority section of a Name Service packet.
            /// </summary>
            public ushort NsCount;
            /// <summary>
            /// Number of resource records in the additional records section of a Name Service packet.
            /// </summary>
            public ushort ArCount;

            /// <summary>
            /// Parse a header represented as a byte array.
            /// Useful when receiving messages.
            /// </summary>
            /// <param name="data">The byte array of the header.</param>
            /// <returns>A header object.</returns>
            public static Header Parse(byte[] data)
            {
                if (data.Length < 12)
                    throw new ArgumentException("Minimum 12 bytes are required");

                Header header = new Header();
                int offset = 0;
                header.NameTrnId = (ushort)((data[offset++] << 8) + data[offset++]);
                ushort temp = (ushort)((data[offset++] << 8) + data[offset++]);
                header.Response = temp >= 0x8000;
                header.Opcode = (HeaderOpcode)((temp >> 11) & 0x0F);
                header.Flags = (HeaderFlags)((temp >> 4) & 0x7F);
                header.Rcode = (byte)(temp & 0x0F);
                header.QdCount = (ushort)((data[offset++] << 8) + data[offset++]);
                header.AnCount = (ushort)((data[offset++] << 8) + data[offset++]);
                header.NsCount = (ushort)((data[offset++] << 8) + data[offset++]);
                header.ArCount = (ushort)((data[offset++] << 8) + data[offset++]);
                return header;
            }

            /// <summary>
            /// Gets the number of bytes that will be returned by the ToArray method.
            /// </summary>
            public int ByteSize
            {
                get { return 12; }
            }

            /// <summary>
            /// Convert a header to a byte array.
            /// Useful for sending messages.
            /// </summary>
            /// <returns>The byte array representation of the header.</returns>
            public byte[] ToArray()
            {
                byte[] data = new byte[ByteSize];
                int offset = 0;
                data[offset++] = (byte)(NameTrnId >> 8);
                data[offset++] = (byte)NameTrnId;
                ushort temp = (ushort)(((ushort)Opcode << 11) + ((ushort)Flags << 4) + Rcode);
                if (Response) temp += 0x8000;
                data[offset++] = (byte)(temp >> 8);
                data[offset++] = (byte)temp;
                data[offset++] = (byte)(QdCount >> 8);
                data[offset++] = (byte)QdCount;
                data[offset++] = (byte)(AnCount >> 8);
                data[offset++] = (byte)AnCount;
                data[offset++] = (byte)(NsCount >> 8);
                data[offset++] = (byte)NsCount;
                data[offset++] = (byte)(ArCount >> 8);
                data[offset++] = (byte)ArCount;
                return data;
            }
        }

        /// <summary>
        /// Available question types.
        /// </summary>
        private enum QuestionType : ushort
        {
            /// <summary>
            /// NetBIOS general Name Service Resource Record.
            /// </summary>
            NB = 0x0020,
            /// <summary>
            /// NetBIOS NODE STATUS Resource Record.
            /// </summary>
            NBSTAT = 0x0021
        }

        /// <summary>
        /// Available question classes.
        /// </summary>
        private enum QuestionClass : ushort
        {
            /// <summary>
            /// Internet class.
            /// </summary>
            IN = 0x0001
        }

        /// <summary>
        /// Packet question name record.
        /// </summary>
        private struct QuestionName
        {
            /// <summary>
            /// The NetBIOS name for the request.
            /// </summary>
            public string UncompressedName;
            /// <summary>
            /// The type of request.
            /// </summary>
            public QuestionType Type;
            /// <summary>
            /// The class of request.
            /// </summary>
            public QuestionClass Class;

            /// <summary>
            /// Extract an uncompressed name from an array.
            /// </summary>
            /// <param name="data">The byte array.</param>
            /// <param name="offset">The offset to start extracting from.</param>
            /// <returns>The uncompressed name.</returns>
            internal static string ExtractName(byte[] data, ref int offset)
            {
                string result = "";

                while (true)
                {
                    byte length = data[offset++];
                    if (length == 0)
                        break; // Reached end of record.

                    if (result.Length > 0)
                    { // Add a '.' in uncompressed format
                        result += "CO";
                    }

                    if ((length & 0xC0) != 0x00)
                    { // Whooo, we have a pointer
                        int address = (ushort)(((length & 0x3F) << 8) + data[offset++]);
                        return ExtractName(data, ref address);
                    }

                    for (int i = 0; i < length; i++)
                        result += (char)data[offset++];
                }

                return result;
            }

            /// <summary>
            /// Parse a QuestionName represented as a byte array.
            /// </summary>
            /// <param name="data">The byte array.</param>
            /// <param name="offset">The offset to start parsing from.</param>
            /// <returns>A parsed QuestionName object.</returns>
            public static QuestionName Parse(byte[] data, ref int offset)
            {
                QuestionName name = new QuestionName();
                name.UncompressedName = ExtractName(data, ref offset);
                name.Type = (QuestionType)((data[offset++] << 8) + data[offset++]);
                name.Class = (QuestionClass)((data[offset++] << 8) + data[offset++]);
                return name;
            }

            /// <summary>
            /// Gets the number of bytes that will be returned by the ToArray method.
            /// </summary>
            public int ByteSize
            {
                get
                {
                    if (UncompressedName == null)
                        throw new Exception("UncompressedName not set");
                    return UncompressedName.Length + 2 + 4;
                }
            }

            /// <summary>
            /// Convert a QuestionName to a byte array.
            /// Useful for sending messages.
            /// </summary>
            /// <returns>The byte array representation of the QuestionName.</returns>
            public byte[] ToArray()
            { // TODO: support '.' in names? Not needed though for normal netbios name services
                if (UncompressedName == null)
                    throw new Exception("UncompressedName not set");
                byte[] result = new byte[ByteSize];
                int offset = 0;
                result[offset++] = (byte)UncompressedName.Length;
                for (int i = 0; i < UncompressedName.Length; i++)
                    result[offset++] = (byte)UncompressedName[i];
                result[offset++] = 0;
                result[offset++] = (byte)((ushort)Type >> 8);
                result[offset++] = (byte)Type;
                result[offset++] = (byte)((ushort)Class >> 8);
                result[offset++] = (byte)Class;

                if (offset != result.Length)
                    throw new Exception("Length mismatch");

                return result;
            }
        }

        /// <summary>
        /// Available resource record types.
        /// </summary>
        private enum ResourceRecordType : ushort
        {
            /// <summary>
            /// IP address record.
            /// </summary>
            A = 0x0001,
            /// <summary>
            /// Name Server record.
            /// </summary>
            NS = 0x0002,
            /// <summary>
            /// NULL resource record (waiting for acknowledgement response).
            /// </summary>
            NULL = 0x000A,
            /// <summary>
            /// General NetBIOS record.
            /// </summary>
            NB = 0x0020,
            /// <summary>
            /// NetBIOS Node Status resource record.
            /// </summary>
            NBSTAT = 0x0021
        }

        /// <summary>
        /// Available resource record classes.
        /// </summary>
        private enum ResourceRecordClass : ushort
        {
            /// <summary>
            /// Internet class.
            /// </summary>
            IN = 0x0001
        }

        /// <summary>
        /// Packet resource record.
        /// </summary>
        private struct ResourceRecord
        {
            /// <summary>
            /// The NetBIOS name corresponding to this resource record.
            /// </summary>
            public string UncompressedName;
            /// <summary>
            /// The record type code.
            /// </summary>
            public ResourceRecordType Type;
            /// <summary>
            /// The record class code.
            /// </summary>
            public ResourceRecordClass Class;
            /// <summary>
            /// Time to Live of the resource record's name.
            /// </summary>
            public uint TTL;
            /// <summary>
            /// Class and Type dependent field. Contains the resource information.
            /// </summary>
            public byte[] Data;

            /// <summary>
            /// Parse a ResourceRecord represented as a byte array.
            /// </summary>
            /// <param name="data">The byte array.</param>
            /// <param name="offset">The offset to start parsing from.</param>
            /// <returns>A parsed ResourceRecord.</returns>
            public static ResourceRecord Parse(byte[] data, ref int offset)
            {
                ResourceRecord record = new ResourceRecord();
                record.UncompressedName = QuestionName.ExtractName(data, ref offset);
                record.Type = (ResourceRecordType)((data[offset++] << 8) + data[offset++]);
                record.Class = (ResourceRecordClass)((data[offset++] << 8) + data[offset++]);
                record.TTL = (uint)((data[offset++] << 24) + (data[offset++] << 16) + (data[offset++] << 8) + data[offset++]);
                int length = (ushort)((data[offset++] << 8) + data[offset++]);
                if (length > 0)
                {
                    record.Data = new byte[length];
                    for (int i = 0; i < length; i++)
                        record.Data[i] = data[offset++];
                }
                else
                    record.Data = null;
                return record;
            }

            /// <summary>
            /// Gets the number of bytes that will be returned by the ToArray method.
            /// </summary>
            public int ByteSize
            {
                get
                {
                    if (UncompressedName == null)
                        throw new Exception("UncompressedName not set");
                    return UncompressedName.Length + 2 + 4 + 4 + 2 + (Data != null ? Data.Length : 0);
                }
            }

            /// <summary>
            /// Convert a ResourceRecord to a byte array.
            /// Useful for sending messages.
            /// </summary>
            /// <returns>The byte array representation of the ResourceRecord.</returns>
            public byte[] ToArray()
            {
                if (UncompressedName == null)
                    throw new Exception("UncompressedName not set");
                byte[] result = new byte[ByteSize];
                int offset = 0;
                result[offset++] = (byte)UncompressedName.Length;
                for (int i = 0; i < UncompressedName.Length; i++)
                    result[offset++] = (byte)UncompressedName[i];
                result[offset++] = 0;
                result[offset++] = (byte)((ushort)Type >> 8);
                result[offset++] = (byte)Type;
                result[offset++] = (byte)((ushort)Class >> 8);
                result[offset++] = (byte)Class;
                result[offset++] = (byte)(TTL >> 24);
                result[offset++] = (byte)(TTL >> 16);
                result[offset++] = (byte)(TTL >> 8);
                result[offset++] = (byte)TTL;
                int length = Data != null ? Data.Length : 0;
                result[offset++] = (byte)(length >> 8);
                result[offset++] = (byte)length;
                for (int i = 0; i < length; i++)
                    result[offset++] = Data[i];

                if (offset != result.Length)
                    throw new Exception("Length mismatch");

                return result;
            }
        }

        /// <summary>
        /// Full packet (header + data).
        /// </summary>
        private class Packet
        {
            public Header Header;
            public QuestionName[] QuestionEntries;
            public ResourceRecord[] AnswerResourceRecords;
            public ResourceRecord[] AuthorityResourceRecords;
            public ResourceRecord[] AdditionalResourceRecords;

            /// <summary>
            /// Parses a packet from incomming data.
            /// </summary>
            /// <param name="data">Byte array containing the incomming data.</param>
            /// <returns>A parsed Packet.</returns>
            public static Packet Parse(byte[] data)
            {
                Packet packet = new Packet();
                packet.Header = Header.Parse(data);
                int offset = packet.Header.ByteSize;

                if (packet.Header.QdCount > 0)
                {
                    packet.QuestionEntries = new QuestionName[packet.Header.QdCount];
                    for (int i = 0; i < packet.Header.QdCount; i++)
                        packet.QuestionEntries[i] = QuestionName.Parse(data, ref offset);
                }
                else
                    packet.QuestionEntries = null;

                if (packet.Header.AnCount > 0)
                {
                    packet.AnswerResourceRecords = new ResourceRecord[packet.Header.AnCount];
                    for (int i = 0; i < packet.Header.AnCount; i++)
                        packet.AnswerResourceRecords[i] = ResourceRecord.Parse(data, ref offset);
                }
                else
                    packet.AnswerResourceRecords = null;

                if (packet.Header.NsCount > 0)
                {
                    packet.AuthorityResourceRecords = new ResourceRecord[packet.Header.NsCount];
                    for (int i = 0; i < packet.Header.NsCount; i++)
                        packet.AuthorityResourceRecords[i] = ResourceRecord.Parse(data, ref offset);
                }
                else
                    packet.AuthorityResourceRecords = null;

                if (packet.Header.ArCount > 0)
                {
                    packet.AdditionalResourceRecords = new ResourceRecord[packet.Header.ArCount];
                    for (int i = 0; i < packet.Header.ArCount; i++)
                        packet.AdditionalResourceRecords[i] = ResourceRecord.Parse(data, ref offset);
                }
                else
                    packet.AdditionalResourceRecords = null;

                return packet;
            }

            /// <summary>
            /// Gets the number of bytes that will be returned by the ToArray method.
            /// </summary>
            public int ByteSize
            {
                get
                {
                    int result = Header.ByteSize;

                    if (QuestionEntries != null)
                        foreach (QuestionName name in QuestionEntries)
                            result += name.ByteSize;

                    if (AnswerResourceRecords != null)
                        foreach (ResourceRecord record in AnswerResourceRecords)
                            result += record.ByteSize;

                    if (AuthorityResourceRecords != null)
                        foreach (ResourceRecord record in AuthorityResourceRecords)
                            result += record.ByteSize;

                    if (AdditionalResourceRecords != null)
                        foreach (ResourceRecord record in AdditionalResourceRecords)
                            result += record.ByteSize;

                    return result;
                }
            }

            /// <summary>
            /// Prepares a packet for transmission.
            /// </summary>
            /// <returns>A byte array containing the packet data.</returns>
            public byte[] ToArray()
            {
                byte[] result = new byte[ByteSize];
                int offset = 0;

                byte[] source = Header.ToArray();
                Array.Copy(source, 0, result, offset, source.Length);
                offset += source.Length;

                if (QuestionEntries != null)
                    foreach (QuestionName name in QuestionEntries)
                    {
                        source = name.ToArray();
                        Array.Copy(source, 0, result, offset, source.Length);
                        offset += source.Length;
                    }

                if (AnswerResourceRecords != null)
                    foreach (ResourceRecord record in AnswerResourceRecords)
                    {
                        source = record.ToArray();
                        Array.Copy(source, 0, result, offset, source.Length);
                        offset += source.Length;
                    }

                if (AuthorityResourceRecords != null)
                    foreach (ResourceRecord record in AuthorityResourceRecords)
                    {
                        source = record.ToArray();
                        Array.Copy(source, 0, result, offset, source.Length);
                        offset += source.Length;
                    }

                if (AdditionalResourceRecords != null)
                    foreach (ResourceRecord record in AdditionalResourceRecords)
                    {
                        source = record.ToArray();
                        Array.Copy(source, 0, result, offset, source.Length);
                        offset += source.Length;
                    }

                if (offset != result.Length)
                    throw new Exception("Length mismatch");

                return result;
            }
        }

        #endregion

        #region Declarations

        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private EndPoint broadcastEndPoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), BCAST_NS_PORT);
        private byte[] localIP = null;
        private byte[] localMacAddress = null;
        private Thread thread;
        private ExtendedTimer updateTimer;
        private bool terminate = false;

        private ArrayList nameList = new ArrayList();

        private bool capture = false;
        private bool denyCaptured = false;

        #endregion

        #region Construction / destruction

        /// <summary>
        /// Creates a brand new name service object.
        /// Since there is only one UDP 137 port, you should use this class as singleton.
        /// </summary>
        public NameService()
        {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                localIP = IPAddress.Parse(networkInterface.IPAddress).GetAddressBytes();
                localMacAddress = networkInterface.PhysicalAddress;
                break;
            }

            socket.Bind(new IPEndPoint(IPAddress.Any, BCAST_NS_PORT));
            updateTimer = new ExtendedTimer(new TimerCallback(OnUpdate), null, Timeout.Infinite, Timeout.Infinite);
            thread = new Thread(new ThreadStart(SocketThread));
            thread.Start();
            thread.Priority = ThreadPriority.AboveNormal;
            Thread.Sleep(0);
        }

        /// <summary>
        /// Releases used resources.
        /// </summary>
        public void Dispose()
        {
            if (updateTimer != null)
            {
                updateTimer.Dispose();
                updateTimer = null;
            }

            // Shut down socket first, so the ReceiveFrom method in thread gets unblocked
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }

            if (thread != null)
            {
                terminate = true;
                thread.Join();
                thread = null;
            }
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Converts an uncompressed NetBIOS name to a compressed, human-readable name.
        /// </summary>
        /// <param name="name">The uncompressed NetBIOS name.</param>
        /// <param name="suffix">The name suffix as introduced by Microsoft.</param>
        /// <returns>The compressed, human-readable name.</returns>
        private static string CompressName(string name, out MsSuffix suffix)
        {
            if (name.Length != 32)
                throw new ArgumentException("Unsupported name length, should be 32 characters", "name");

            suffix = MsSuffix.Default;

            int offset = 0;
            char[] result = new char[15];

            for (int i = 0; i < 16; i++)
            {
                byte b1 = (byte)(name[offset++] - 'A');
                byte b2 = (byte)(name[offset++] - 'A');

                if ((b1 > 15) || (b2 > 15))
                    throw new ArgumentException("Invalid characters in name", "name");

                b1 <<= 4;
                b1 += b2;

                if (i < 15)
                    result[i] = (char)b1;
                else
                    suffix = (MsSuffix)b1;
            }

            return new string(result).TrimEnd(new char[] { ' ' });
        }

        /// <summary>
        /// Converts a human-readable name to an uncompressed NetBIOS name.
        /// </summary>
        /// <param name="name">The compressed, human-readable name.</param>
        /// <param name="suffix">The name suffix as introduced by Microsoft.</param>
        /// <returns>The uncompressed NetBIOS name.</returns>
        private static string UncompressName(string name, MsSuffix suffix)
        {
            if (name.Length > 15)
                throw new ArgumentException("Name cannot contain more than 15 characters");

            char[] result = new char[32];
            int offset = 0;

            for (int i = 0; i < 15; i++)
            {
                char c = i < name.Length ? name[i] : ' ';
                result[offset++] = (char)('A' + (c >> 4));
                result[offset++] = (char)('A' + (c & 15));
            }
            result[offset++] = (char)('A' + ((byte)suffix >> 4));
            result[offset++] = (char)('A' + ((byte)suffix & 15));

            return new string(result);
        }

        #endregion

        #region Thread & timer

        private void SocketThread()
        {
            byte[] buffer = new byte[1024];
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 137);

            while (!terminate)
            {
                int count = socket.ReceiveFrom(buffer, ref remoteEndPoint); // Blocking call, returns 0 when socket is closed

                if (count == 0)
                    break; // Socket has been closed

                // Don't check own messages
                if ((remoteEndPoint as IPEndPoint).Address.Equals(IPAddress.Loopback))
                    continue;

                ProcessReceivedPacket(buffer, count, remoteEndPoint);
            }
        }

        private void OnUpdate(object o)
        {
            lock (nameList)
                foreach (Name name in nameList)
                    Request(name, HeaderOpcode.Refresh);

            updateTimer.Change(NAME_UPDATE_INTERVAL_MS, Timeout.Infinite);
        }

        #endregion

        #region Private methods

        private void ProcessReceivedPacket(byte[] data, int size, EndPoint remoteEndPoint)
        {
            // TODO: verify the size one day
            Packet packet = Packet.Parse(data);
            //Header header = Header.Parse(data); // Only capture header initially and not the full packet (need 4 speed)

            if (capture && (packet.Header.NameTrnId == NAME_TRN_ID) && (packet.Header.Opcode != 0))
                denyCaptured = true; // Other node denied our registration request

            if (!packet.Header.Response && (packet.Header.QdCount > 0))
            { // We received a request
                switch (packet.Header.Opcode)
                {
                    case HeaderOpcode.Query:
                        if (packet.QuestionEntries[0].UncompressedName == "CKAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")
                        { // "*" received
                            StatusResponse(packet.Header.NameTrnId, remoteEndPoint);
                        }
                        else
                        {
                            lock (nameList)
                            {
                                foreach (Name name in nameList)
                                {
                                    if (name.UncompressedName == packet.QuestionEntries[0].UncompressedName)
                                    { // We own the name
                                        Response(name, HeaderOpcode.Query, 0, packet.Header.NameTrnId, remoteEndPoint);
                                    }
                                }
                            }
                        }
                        break;
                    case HeaderOpcode.Registration:
                        lock (nameList)
                        {
                            foreach (Name name in nameList)
                            {
                                if (name.Type == NameType.Group)
                                    continue;

                                if (name.UncompressedName == packet.QuestionEntries[0].UncompressedName)
                                { // We own the name, send negative response (0x06 ACT_ERR : Active error, name owned)
                                    Response(name, HeaderOpcode.Registration, 6, packet.Header.NameTrnId, remoteEndPoint);
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void StartCapture()
        {
            denyCaptured = false;
            capture = true;
        }

        private bool StopCapture()
        {
            capture = false;
            return !denyCaptured;
        }

        private void Request(Name name, HeaderOpcode opcode)
        {
            bool recursionDesired = true;

            switch (opcode)
            {
                case HeaderOpcode.Update:
                    opcode = HeaderOpcode.Registration;
                    recursionDesired = false;
                    break;
                case HeaderOpcode.Release:
                case HeaderOpcode.WACK:
                case HeaderOpcode.Refresh:
                    recursionDesired = false;
                    break;
            }

            Packet packet = new Packet();
            packet.Header.NameTrnId = NAME_TRN_ID;
            packet.Header.Opcode = opcode;
            packet.Header.Flags = HeaderFlags.Broadcast;
            if (recursionDesired) packet.Header.Flags |= HeaderFlags.RecursionDesired;
            packet.Header.Rcode = 0;
            packet.Header.QdCount = 1;
            packet.Header.ArCount = 1;

            packet.QuestionEntries = new QuestionName[1];
            packet.QuestionEntries[0].UncompressedName = name.UncompressedName;
            packet.QuestionEntries[0].Type = QuestionType.NB;
            packet.QuestionEntries[0].Class = QuestionClass.IN;

            packet.AdditionalResourceRecords = new ResourceRecord[1];
            packet.AdditionalResourceRecords[0].UncompressedName = name.UncompressedName; // TODO: Should use a pointer here
            packet.AdditionalResourceRecords[0].Type = ResourceRecordType.NB;
            packet.AdditionalResourceRecords[0].Class = ResourceRecordClass.IN;
            packet.AdditionalResourceRecords[0].TTL = 0;

            byte[] data = new byte[6];
            data[0] = (byte)(name.Type == NameType.Group ? 0x80 : 0x00); // NB_FLAGS / bit 15: Group name flag, bit 16-15: 00 B node, 01 P node, 10 M node, 11 reserved, bit 14-8: reserved
            data[1] = 0x00; // NB_FLAGS
            // NB_ADDRESS
            for (int i = 0; i < 4; i++)
                data[i + 2] = localIP[i];

            packet.AdditionalResourceRecords[0].Data = data;

            data = packet.ToArray();

            lock (socket)
                socket.SendTo(data, broadcastEndPoint);
        }

        private void Response(Name name, HeaderOpcode opcode, byte rcode, ushort nameTrnId, EndPoint remoteEndPoint)
        {
            Packet packet = new Packet();
            packet.Header.Response = true;
            packet.Header.NameTrnId = nameTrnId;
            packet.Header.Opcode = opcode;
            packet.Header.Flags = HeaderFlags.AuthoritativeAnswer | HeaderFlags.RecursionDesired;
            packet.Header.Rcode = rcode;
            packet.Header.AnCount = 1;

            packet.AnswerResourceRecords = new ResourceRecord[1];
            packet.AnswerResourceRecords[0].UncompressedName = name.UncompressedName;
            packet.AnswerResourceRecords[0].Type = ResourceRecordType.NB;
            packet.AnswerResourceRecords[0].Class = ResourceRecordClass.IN;
            packet.AnswerResourceRecords[0].TTL = 0;

            byte[] data = new byte[6];
            data[0] = (byte)(name.Type == NameType.Group ? 0x80 : 0x00); // NB_FLAGS / bit 15: Group name flag, bit 16-15: 00 B node, 01 P node, 10 M node, 11 reserved, bit 14-8: reserved
            data[1] = 0x00; // NB_FLAGS
            // NB_ADDRESS
            for (int i = 0; i < 4; i++)
                data[i + 2] = localIP[i];

            packet.AnswerResourceRecords[0].Data = data;

            data = packet.ToArray();

            try
            {
                lock (socket)
                    socket.SendTo(data, remoteEndPoint);
            }
            catch
            {
                // Handles situations where the remote host is not accessable through the network.
            }
        }

        private void StatusResponse(ushort nameTrnId, EndPoint remoteEndPoint)
        {
            lock (nameList)
                if (nameList.Count == 0)
                    return;

            Packet packet = new Packet();
            packet.Header.Response = true;
            packet.Header.NameTrnId = nameTrnId;
            packet.Header.Opcode = (HeaderOpcode)0;
            packet.Header.Flags = HeaderFlags.AuthoritativeAnswer;
            packet.Header.Rcode = 0;
            packet.Header.AnCount = 1;

            packet.AnswerResourceRecords = new ResourceRecord[1];
            packet.AnswerResourceRecords[0].UncompressedName = "CKAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";//uncompressedName;
            packet.AnswerResourceRecords[0].Type = ResourceRecordType.NBSTAT;
            packet.AnswerResourceRecords[0].Class = ResourceRecordClass.IN;
            packet.AnswerResourceRecords[0].TTL = 0;

            int length = 0;
            byte[] data;
            lock (nameList)
            {
                length += 1; // NUM_NAMES
                foreach (Name name in nameList)
                {
                    length += 16;
                    length += 2; // NAME_FLAGS
                }
                length += 46; // STATISTICS

                int offset = 0;
                data = new byte[length];
                data[offset++] = (byte)nameList.Count; // NUM_NAMES

                bool first = true;
                foreach (Name name in nameList)
                {
                    MsSuffix suffix;
                    string value = CompressName(name.UncompressedName, out suffix);
                    while (value.Length < 15)
                        value += ' ';

                    Array.Copy(Encoding.UTF8.GetBytes(value), 0, data, offset, 15);
                    offset += 15;
                    data[offset++] = (byte)suffix;

                    // NAME_FLAGS
                    //+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
                    //| G | ONT |DRG|CNF|ACT|PRM| RESERVED |
                    //+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+

                    data[offset++] = (byte)((name.Type == NameType.Group ? 0x80 : 0x00) + 0x04 + (first ? 0x02 : 0x00)); // treat first name as permanent name
                    data[offset++] = 0;

                    first = false;
                }

                // Statistics - only fill in mac address
                Array.Copy(localMacAddress, 0, data, offset, localMacAddress.Length);
                offset += localMacAddress.Length;
            }

            packet.AnswerResourceRecords[0].Data = data;

            data = packet.ToArray();

            try
            {
                lock (socket)
                    socket.SendTo(data, remoteEndPoint);
            }
            catch
            {
                // Handles situations where the remote host is not accessable through the network.
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Add a netbios name.
        /// </summary>
        /// <param name="name">The name to add.</param>
        /// <param name="type">The type to use.</param>
        /// <param name="suffix">The suffix to use.</param>
        /// <returns>True on success, false on failure.</returns>
        public bool AddName(string name, NameType type, MsSuffix suffix)
        {
            Name node = new Name();
            node.UncompressedName = UncompressName(name, suffix);
            node.Type = type;

            // Send request
            StartCapture();

            for (int i = 0; i < BCAST_REQ_RETRY_COUNT; i++)
            {
                Request(node, HeaderOpcode.Registration);
                Thread.Sleep(3 * BCAST_REQ_RETRY_TIMEOUT); // Three times, otherwise FEZ can't follow :(

                if (denyCaptured)
                    break;
            }

            if (!StopCapture())
                return false; // Name in use

            Request(node, HeaderOpcode.Update);

            lock (nameList)
                nameList.Add(node);

            updateTimer.Change(NAME_UPDATE_INTERVAL_MS, Timeout.Infinite);

            return true;
        }

        /// <summary>
        /// Remove a netbios name.
        /// </summary>
        /// <param name="name">The name to remove.</param>
        /// <param name="type">The type used.</param>
        /// <param name="suffix">The suffix used.</param>
        /// <returns>True on success, false on failure.</returns>
        public bool RemoveName(string name, NameType type, MsSuffix suffix)
        {
            Name node = new Name();
            node.UncompressedName = UncompressName(name, suffix);
            node.Type = type;

            lock (nameList)
            {
                int i = 0;
                for (; i < nameList.Count; i++)
                {
                    Name n = (Name)nameList[i];
                    if ((n.UncompressedName == node.UncompressedName) && (n.Type == node.Type))
                        break;
                }

                if (i >= nameList.Count)
                    return false; // Name not found

                nameList.RemoveAt(i);

                if (nameList.Count == 0)
                    updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            // Send request
            for (int i = 0; i < BCAST_REQ_RETRY_COUNT; i++)
            {
                Request(node, HeaderOpcode.Release);
                Thread.Sleep(BCAST_REQ_RETRY_TIMEOUT);
            }

            return true;
        }

        #endregion
    }
}