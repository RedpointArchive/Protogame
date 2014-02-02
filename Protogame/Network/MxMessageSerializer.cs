namespace Protogame
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using ProtoBuf;
    using ProtoBuf.Meta;

    /// <summary>
    /// This is a Protobuf serializer that serializes and deserializes the MxMessage class.
    /// </summary>
    /// <remarks>
    /// This code is generated from the precompile tool, decompiled with ILSpy and copied back into
    /// the Protogame code base.  This is done so that we can serialize and deserialize on mobile platforms
    /// and so that we don't introduce a new DLL from the result of precompile.  If the structure of
    /// MxMessage or MxPayload ever changes, this code will need to be regenerated.
    /// </remarks>
    public sealed class MxMessageSerializer : TypeModel
    {
        /// <summary>
        /// The known types.
        /// </summary>
        private static readonly Type[] knownTypes = { typeof(MxMessage), typeof(MxPayload) };

        /// <summary>
        /// The deserialize.
        /// </summary>
        /// <param name="num">
        /// The num.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="protoReader">
        /// The proto reader.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        protected override object Deserialize(int num, object obj, ProtoReader protoReader)
        {
            switch (num)
            {
                case 0:
                    return Read((MxMessage)obj, protoReader);
                case 1:
                    return Read((MxPayload)obj, protoReader);
                default:
                    return null;
            }
        }

        /// <summary>
        /// The get key impl.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected override int GetKeyImpl(Type value)
        {
            return ((IList)knownTypes).IndexOf(value);
        }

        /// <summary>
        /// The serialize.
        /// </summary>
        /// <param name="num">
        /// The num.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="protoWriter">
        /// The proto writer.
        /// </param>
        protected override void Serialize(int num, object obj, ProtoWriter protoWriter)
        {
            switch (num)
            {
                case 0:
                    Write((MxMessage)obj, protoWriter);
                    return;
                case 1:
                    Write((MxPayload)obj, protoWriter);
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="mxMessage">
        /// The mx message.
        /// </param>
        /// <param name="protoReader">
        /// The proto reader.
        /// </param>
        /// <returns>
        /// The <see cref="MxMessage"/>.
        /// </returns>
        private static MxMessage Read(MxMessage mxMessage, ProtoReader protoReader)
        {
            int num;
            while ((num = protoReader.ReadFieldHeader()) > 0)
            {
                if (num != 1)
                {
                    if (num != 2)
                    {
                        if (num != 3)
                        {
                            if (num != 4)
                            {
                                if (num != 6)
                                {
                                    if (mxMessage == null)
                                    {
                                        var expr_170 = new MxMessage();
                                        ProtoReader.NoteObject(expr_170, protoReader);
                                        mxMessage = expr_170;
                                    }

                                    protoReader.SkipField();
                                }
                                else
                                {
                                    if (mxMessage == null)
                                    {
                                        var expr_D9 = new MxMessage();
                                        ProtoReader.NoteObject(expr_D9, protoReader);
                                        mxMessage = expr_D9;
                                    }

                                    MxPayload[] payloads = mxMessage.Payloads;
                                    var list = new List<MxPayload>();
                                    int num2 = protoReader.FieldNumber;
                                    do
                                    {
                                        List<MxPayload> arg_111_0 = list;
                                        MxPayload arg_104_0 = null;
                                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                                        MxPayload arg_111_1 = Read(arg_104_0, protoReader);
                                        ProtoReader.EndSubItem(token, protoReader);
                                        arg_111_0.Add(arg_111_1);
                                    }
                                    while (protoReader.TryReadFieldHeader(num2));
                                    MxPayload[] expr_124 = payloads;
                                    var array =
                                        new MxPayload[(num2 = (expr_124 != null) ? expr_124.Length : 0) + list.Count];
                                    if (num2 != 0)
                                    {
                                        payloads.CopyTo(array, 0);
                                    }

                                    list.CopyTo(array, num2);
                                    array = array;
                                    if (array != null)
                                    {
                                        mxMessage.Payloads = array;
                                    }
                                }
                            }
                            else
                            {
                                if (mxMessage == null)
                                {
                                    var expr_A9 = new MxMessage();
                                    ProtoReader.NoteObject(expr_A9, protoReader);
                                    mxMessage = expr_A9;
                                }

                                uint num3 = protoReader.ReadUInt32();
                                mxMessage.AckBitfield = num3;
                            }
                        }
                        else
                        {
                            if (mxMessage == null)
                            {
                                var expr_79 = new MxMessage();
                                ProtoReader.NoteObject(expr_79, protoReader);
                                mxMessage = expr_79;
                            }

                            uint num3 = protoReader.ReadUInt32();
                            mxMessage.Ack = num3;
                        }
                    }
                    else
                    {
                        if (mxMessage == null)
                        {
                            var expr_49 = new MxMessage();
                            ProtoReader.NoteObject(expr_49, protoReader);
                            mxMessage = expr_49;
                        }

                        uint num3 = protoReader.ReadUInt32();
                        mxMessage.Sequence = num3;
                    }
                }
                else
                {
                    if (mxMessage == null)
                    {
                        var expr_19 = new MxMessage();
                        ProtoReader.NoteObject(expr_19, protoReader);
                        mxMessage = expr_19;
                    }

                    uint num3 = protoReader.ReadUInt32();
                    mxMessage.ProtocolID = num3;
                }
            }

            if (mxMessage == null)
            {
                var expr_198 = new MxMessage();
                ProtoReader.NoteObject(expr_198, protoReader);
                mxMessage = expr_198;
            }

            return mxMessage;
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="mxPayload">
        /// The mx payload.
        /// </param>
        /// <param name="protoReader">
        /// The proto reader.
        /// </param>
        /// <returns>
        /// The <see cref="MxPayload"/>.
        /// </returns>
        private static MxPayload Read(MxPayload mxPayload, ProtoReader protoReader)
        {
            int num;
            while ((num = protoReader.ReadFieldHeader()) > 0)
            {
                if (num != 1)
                {
                    if (mxPayload == null)
                    {
                        var expr_49 = new MxPayload();
                        ProtoReader.NoteObject(expr_49, protoReader);
                        mxPayload = expr_49;
                    }

                    protoReader.SkipField();
                }
                else
                {
                    if (mxPayload == null)
                    {
                        var expr_19 = new MxPayload();
                        ProtoReader.NoteObject(expr_19, protoReader);
                        mxPayload = expr_19;
                    }

                    byte[] array = ProtoReader.AppendBytes(mxPayload.Data, protoReader);
                    if (array != null)
                    {
                        mxPayload.Data = array;
                    }
                }
            }

            if (mxPayload == null)
            {
                var expr_71 = new MxPayload();
                ProtoReader.NoteObject(expr_71, protoReader);
                mxPayload = expr_71;
            }

            return mxPayload;
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="mxMessage">
        /// The mx message.
        /// </param>
        /// <param name="protoWriter">
        /// The proto writer.
        /// </param>
        private static void Write(MxMessage mxMessage, ProtoWriter protoWriter)
        {
            if (mxMessage.GetType() != typeof(MxMessage))
            {
                ThrowUnexpectedSubtype(typeof(MxMessage), mxMessage.GetType());
            }

            uint expr_2D = mxMessage.ProtocolID;
            if (expr_2D != 0u)
            {
                ProtoWriter.WriteFieldHeader(1, WireType.Variant, protoWriter);
                ProtoWriter.WriteUInt32(expr_2D, protoWriter);
            }

            uint expr_4A = mxMessage.Sequence;
            if (expr_4A != 0u)
            {
                ProtoWriter.WriteFieldHeader(2, WireType.Variant, protoWriter);
                ProtoWriter.WriteUInt32(expr_4A, protoWriter);
            }

            uint expr_67 = mxMessage.Ack;
            if (expr_67 != 0u)
            {
                ProtoWriter.WriteFieldHeader(3, WireType.Variant, protoWriter);
                ProtoWriter.WriteUInt32(expr_67, protoWriter);
            }

            uint expr_84 = mxMessage.AckBitfield;
            if (expr_84 != 0u)
            {
                ProtoWriter.WriteFieldHeader(4, WireType.Variant, protoWriter);
                ProtoWriter.WriteUInt32(expr_84, protoWriter);
            }

            MxPayload[] expr_A1 = mxMessage.Payloads;
            if (expr_A1 != null)
            {
                MxPayload[] array = expr_A1;
                for (int i = 0; i < array.Length; i++)
                {
                    MxPayload expr_B5 = array[i];
                    if (expr_B5 != null)
                    {
                        ProtoWriter.WriteFieldHeader(6, WireType.String, protoWriter);
                        SubItemToken token = ProtoWriter.StartSubItem(expr_B5, protoWriter);
                        Write(expr_B5, protoWriter);
                        ProtoWriter.EndSubItem(token, protoWriter);
                    }
                }
            }
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="mxPayload">
        /// The mx payload.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private static void Write(MxPayload mxPayload, ProtoWriter writer)
        {
            if (mxPayload.GetType() != typeof(MxPayload))
            {
                ThrowUnexpectedSubtype(typeof(MxPayload), mxPayload.GetType());
            }

            byte[] expr_2D = mxPayload.Data;
            if (expr_2D != null)
            {
                ProtoWriter.WriteFieldHeader(1, WireType.String, writer);
                ProtoWriter.WriteBytes(expr_2D, writer);
            }
        }
    }
}