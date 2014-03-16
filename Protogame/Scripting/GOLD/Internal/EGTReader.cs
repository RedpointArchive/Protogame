// =============================================================== //
// This GOLD engine implementation is a C# port of the .NET        //
// engine by Devin Cook.  Licensed under the GOLD open source      //
// license agreement (based on zlib/libpng).  See                  //
// http://goldparser.org/about/license.htm for more information.   //
// =============================================================== //
namespace GOLD.Internal
{
    using System;
    using System.IO;

    internal class EGTReader
    {
        // M
        private const byte KRecordContentMulti = 77;

        private int m_EntriesRead;

        private int m_EntryCount;

        private string m_FileHeader;

        private BinaryReader m_Reader;

        ~EGTReader()
        {
            this.Close();
        }

        public enum EntryType : byte
        {
            Empty = 69, 

            // E
            UInt16 = 73, 

            // I - Unsigned, 2 byte
            String = 83, 

            // S - Unicode format
            Boolean = 66, 

            // B - 1 Byte, Value is 0 or 1
            Byte = 98, 

            // b
            Error = 0
        }

        // Current record 
        public void Close()
        {
            if (this.m_Reader != null)
            {
                this.m_Reader.Close();
                this.m_Reader = null;
            }
        }

        public bool EndOfFile()
        {
            return this.m_Reader.BaseStream.Position == this.m_Reader.BaseStream.Length;
        }

        public short EntryCount()
        {
            return (short)this.m_EntryCount;
        }

        public bool GetNextRecord()
        {
            bool success;

            // ==== Finish current record
            while (this.m_EntriesRead < this.m_EntryCount)
            {
                this.RetrieveEntry();
            }

            // ==== Start next record
            var id = this.m_Reader.ReadByte();

            if (id == KRecordContentMulti)
            {
                this.m_EntryCount = this.RawReadUInt16();
                this.m_EntriesRead = 0;
                success = true;
            }
            else
            {
                success = false;
            }

            return success;
        }

        public string Header()
        {
            return this.m_FileHeader;
        }

        public void Open(BinaryReader reader)
        {
            this.m_Reader = reader;

            this.m_EntryCount = 0;
            this.m_EntriesRead = 0;
            this.m_FileHeader = this.RawReadCString();
        }

        public void Open(string path)
        {
            this.Open(new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)));
        }

        public bool RecordComplete()
        {
            return this.m_EntriesRead >= this.m_EntryCount;
        }

        public bool RetrieveBoolean()
        {
            var e = this.RetrieveEntry();
            if (e.Type == EntryType.Boolean)
            {
                return (bool)e.Value;
            }

            throw new EGTReadException(e.Type, this.m_Reader);
        }

        public byte RetrieveByte()
        {
            var e = this.RetrieveEntry();
            if (e.Type == EntryType.Byte)
            {
                return (byte)e.Value;
            }

            throw new EGTReadException(e.Type, this.m_Reader);
        }

        public Entry RetrieveEntry()
        {
            var result = new Entry();

            if (this.RecordComplete())
            {
                result.Type = EntryType.Empty;
                result.Value = string.Empty;
            }
            else
            {
                this.m_EntriesRead += 1;
                var type = this.m_Reader.ReadByte();

                // Entry Type Prefix
                result.Type = (EntryType)type;

                switch ((EntryType)type)
                {
                    case EntryType.Empty:
                        result.Value = string.Empty;

                        break;
                    case EntryType.Boolean:

                        var b = this.m_Reader.ReadByte();
                        result.Value = b == 1;

                        break;
                    case EntryType.UInt16:
                        result.Value = this.RawReadUInt16();

                        break;
                    case EntryType.String:
                        result.Value = this.RawReadCString();

                        break;
                    case EntryType.Byte:
                        result.Value = this.m_Reader.ReadByte();

                        break;
                    default:
                        result.Type = EntryType.Error;
                        result.Value = string.Empty;
                        break;
                }
            }

            return result;
        }

        public int RetrieveInt16()
        {
            var e = this.RetrieveEntry();
            if (e.Type == EntryType.UInt16)
            {
                return (ushort)e.Value;
            }

            throw new EGTReadException(e.Type, this.m_Reader);
        }

        public string RetrieveString()
        {
            var e = this.RetrieveEntry();
            if (e.Type == EntryType.String)
            {
                return (string)e.Value;
            }

            throw new EGTReadException(e.Type, this.m_Reader);
        }

        private string RawReadCString()
        {
            var text = string.Empty;
            var done = false;

            while (!done)
            {
                var char16 = this.RawReadUInt16();
                if (char16 == 0)
                {
                    done = true;
                }
                else
                {
                    text += (char)char16;
                }
            }

            return text;
        }

        private ushort RawReadUInt16()
        {
            // Read a uint in little endian. This is the format already used
            // by the .NET BinaryReader. However, it is good to specificially
            // define this given byte order can change depending on platform.
            int b0 = this.m_Reader.ReadByte();

            // Least significant byte first
            int b1 = this.m_Reader.ReadByte();

            return (ushort)((b1 << 8) + b0);
        }

        public class Entry
        {
            public Entry()
            {
                this.Type = EntryType.Empty;
                this.Value = string.Empty;
            }

            public Entry(EntryType type, object value)
            {
                this.Type = type;
                this.Value = value;
            }

            public EntryType Type { get; set; }

            public object Value { get; set; }
        }

        public class EGTReadException : Exception
        {
            public EGTReadException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            public EGTReadException(EntryType type, BinaryReader reader)
                : base("Type mismatch in file. Read '" + type + "' at " + reader.BaseStream.Position)
            {
            }
        }
    }
}