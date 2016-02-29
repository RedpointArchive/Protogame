namespace Protogame
{
    using System;
    using System.Collections;
    using ProtoBuf;
    using ProtoBuf.Meta;

    /// <summary>
    /// This is a Protobuf serializer that serializes and deserializes the CompiledAsset class.
    /// </summary>
    /// <remarks>
    /// This code is generated from the precompile tool, decompiled with ILSpy and copied back into
    /// the Protogame code base.  This is done so that we can serialize and deserialize on mobile platforms
    /// and so that we don't introduce a new DLL from the result of precompile.  If the structure of
    /// CompiledAsset, PlatformData or TargetPlatform ever changes, this code will need to be regenerated.
    /// </remarks>
    public sealed class CompiledAssetSerializer : TypeModel
    {
        /// <summary>
        /// The known types.
        /// </summary>
        private static readonly string[] knownTypes =
        {
            typeof(CompiledAsset).FullName,
            typeof(PlatformData).FullName, 
            typeof(TargetPlatform).FullName
        };

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
        protected internal override object Deserialize(int num, object obj, ProtoReader protoReader)
        {
            switch (num)
            {
                case 0:
                    return Read((CompiledAsset)obj, protoReader);
                case 1:
                    return Read((PlatformData)obj, protoReader);
                case 2:
                    return _2(obj, protoReader);
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
            return ((IList)knownTypes).IndexOf(value.FullName);
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
        protected internal override void Serialize(int num, object obj, ProtoWriter protoWriter)
        {
            switch (num)
            {
                case 0:
                    Write((CompiledAsset)obj, protoWriter);
                    return;
                case 1:
                    Write((PlatformData)obj, protoWriter);
                    return;
                case 2:
                    Write((TargetPlatform)obj, protoWriter);
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="compiledAsset">
        /// The compiled asset.
        /// </param>
        /// <param name="protoReader">
        /// The proto reader.
        /// </param>
        /// <returns>
        /// The <see cref="CompiledAsset"/>.
        /// </returns>
        private static CompiledAsset Read(CompiledAsset compiledAsset, ProtoReader protoReader)
        {
            int num;
            while ((num = protoReader.ReadFieldHeader()) > 0)
            {
                if (num != 1)
                {
                    if (num != 3)
                    {
                        if (compiledAsset == null)
                        {
                            var expr_8A = new CompiledAsset();
                            ProtoReader.NoteObject(expr_8A, protoReader);
                            compiledAsset = expr_8A;
                        }

                        protoReader.SkipField();
                    }
                    else
                    {
                        if (compiledAsset == null)
                        {
                            var expr_4C = new CompiledAsset();
                            ProtoReader.NoteObject(expr_4C, protoReader);
                            compiledAsset = expr_4C;
                        }

                        PlatformData arg_63_0 = compiledAsset.PlatformData;
                        SubItemToken token = ProtoReader.StartSubItem(protoReader);
                        PlatformData arg_6F_0 = Read(arg_63_0, protoReader);
                        ProtoReader.EndSubItem(token, protoReader);
                        PlatformData platformData = arg_6F_0;
                        if (platformData != null)
                        {
                            compiledAsset.PlatformData = platformData;
                        }
                    }
                }
                else
                {
                    if (compiledAsset == null)
                    {
                        var expr_19 = new CompiledAsset();
                        ProtoReader.NoteObject(expr_19, protoReader);
                        compiledAsset = expr_19;
                    }

                    string text = protoReader.ReadString();
                    if (text != null)
                    {
                        compiledAsset.Loader = text;
                    }
                }
            }

            if (compiledAsset == null)
            {
                var expr_B2 = new CompiledAsset();
                ProtoReader.NoteObject(expr_B2, protoReader);
                compiledAsset = expr_B2;
            }

            return compiledAsset;
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="platformData">
        /// The platform data.
        /// </param>
        /// <param name="protoReader">
        /// The proto reader.
        /// </param>
        /// <returns>
        /// The <see cref="PlatformData"/>.
        /// </returns>
        private static PlatformData Read(PlatformData platformData, ProtoReader protoReader)
        {
            int num;
            while ((num = protoReader.ReadFieldHeader()) > 0)
            {
                if (num != 1)
                {
                    if (num != 2)
                    {
                        if (platformData == null)
                        {
                            var expr_164 = new PlatformData();
                            ProtoReader.NoteObject(expr_164, protoReader);
                            platformData = expr_164;
                        }

                        protoReader.SkipField();
                    }
                    else
                    {
                        if (platformData == null)
                        {
                            var expr_134 = new PlatformData();
                            ProtoReader.NoteObject(expr_134, protoReader);
                            platformData = expr_134;
                        }

                        byte[] array = ProtoReader.AppendBytes(platformData.Data, protoReader);
                        if (array != null)
                        {
                            platformData.Data = array;
                        }
                    }
                }
                else
                {
                    if (platformData == null)
                    {
                        var expr_19 = new PlatformData();
                        ProtoReader.NoteObject(expr_19, protoReader);
                        platformData = expr_19;
                    }

                    int num2 = protoReader.ReadInt32();
                    var targetPlatform = TargetPlatform.Windows;
                    if (num2 != 0)
                    {
                        if (num2 != 1)
                        {
                            if (num2 != 2)
                            {
                                if (num2 != 3)
                                {
                                    if (num2 != 4)
                                    {
                                        if (num2 != 5)
                                        {
                                            if (num2 != 6)
                                            {
                                                if (num2 != 7)
                                                {
                                                    if (num2 != 8)
                                                    {
                                                        if (num2 != 9)
                                                        {
                                                            if (num2 != 10)
                                                            {
                                                                if (num2 != 11)
                                                                {
                                                                    if (num2 != 12)
                                                                    {
                                                                        protoReader.ThrowEnumException(
                                                                            typeof(TargetPlatform), 
                                                                            num2);
                                                                    }
                                                                    else
                                                                    {
                                                                        targetPlatform = TargetPlatform.RaspberryPi;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    targetPlatform = TargetPlatform.WindowsPhone8;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                targetPlatform = TargetPlatform.PlayStationMobile;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            targetPlatform = TargetPlatform.Ouya;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        targetPlatform = TargetPlatform.NativeClient;
                                                    }
                                                }
                                                else
                                                {
                                                    targetPlatform = TargetPlatform.WindowsStoreApp;
                                                }
                                            }
                                            else
                                            {
                                                targetPlatform = TargetPlatform.MacOSX;
                                            }
                                        }
                                        else
                                        {
                                            targetPlatform = TargetPlatform.Linux;
                                        }
                                    }
                                    else
                                    {
                                        targetPlatform = TargetPlatform.Android;
                                    }
                                }
                                else
                                {
                                    targetPlatform = TargetPlatform.iOS;
                                }
                            }
                            else
                            {
                                targetPlatform = TargetPlatform.WindowsPhone;
                            }
                        }
                        else
                        {
                            targetPlatform = TargetPlatform.Xbox360;
                        }
                    }
                    else
                    {
                        targetPlatform = TargetPlatform.Windows;
                    }

                    platformData.Platform = targetPlatform;
                }
            }

            if (platformData == null)
            {
                var expr_18C = new PlatformData();
                ProtoReader.NoteObject(expr_18C, protoReader);
                platformData = expr_18C;
            }

            return platformData;
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="targetPlatform">
        /// The target platform.
        /// </param>
        /// <param name="protoReader">
        /// The proto reader.
        /// </param>
        /// <returns>
        /// The <see cref="TargetPlatform"/>.
        /// </returns>
        private static TargetPlatform Read(TargetPlatform targetPlatform, ProtoReader protoReader)
        {
            int num = protoReader.ReadInt32();
            var result = TargetPlatform.Windows;
            if (num != 0)
            {
                if (num != 1)
                {
                    if (num != 2)
                    {
                        if (num != 3)
                        {
                            if (num != 4)
                            {
                                if (num != 5)
                                {
                                    if (num != 6)
                                    {
                                        if (num != 7)
                                        {
                                            if (num != 8)
                                            {
                                                if (num != 9)
                                                {
                                                    if (num != 10)
                                                    {
                                                        if (num != 11)
                                                        {
                                                            if (num != 12)
                                                            {
                                                                protoReader.ThrowEnumException(
                                                                    typeof(TargetPlatform), 
                                                                    num);
                                                            }
                                                            else
                                                            {
                                                                result = TargetPlatform.RaspberryPi;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            result = TargetPlatform.WindowsPhone8;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        result = TargetPlatform.PlayStationMobile;
                                                    }
                                                }
                                                else
                                                {
                                                    result = TargetPlatform.Ouya;
                                                }
                                            }
                                            else
                                            {
                                                result = TargetPlatform.NativeClient;
                                            }
                                        }
                                        else
                                        {
                                            result = TargetPlatform.WindowsStoreApp;
                                        }
                                    }
                                    else
                                    {
                                        result = TargetPlatform.MacOSX;
                                    }
                                }
                                else
                                {
                                    result = TargetPlatform.Linux;
                                }
                            }
                            else
                            {
                                result = TargetPlatform.Android;
                            }
                        }
                        else
                        {
                            result = TargetPlatform.iOS;
                        }
                    }
                    else
                    {
                        result = TargetPlatform.WindowsPhone;
                    }
                }
                else
                {
                    result = TargetPlatform.Xbox360;
                }
            }
            else
            {
                result = TargetPlatform.Windows;
            }

            return result;
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="compiledAsset">
        /// The compiled asset.
        /// </param>
        /// <param name="protoWriter">
        /// The proto writer.
        /// </param>
        private static void Write(CompiledAsset compiledAsset, ProtoWriter protoWriter)
        {
            if (compiledAsset.GetType() != typeof(CompiledAsset))
            {
                ThrowUnexpectedSubtype(typeof(CompiledAsset), compiledAsset.GetType());
            }

            string expr_2D = compiledAsset.Loader;
            if (expr_2D != null)
            {
                ProtoWriter.WriteFieldHeader(1, WireType.String, protoWriter);
                ProtoWriter.WriteString(expr_2D, protoWriter);
            }

            PlatformData expr_4A = compiledAsset.PlatformData;
            if (expr_4A != null)
            {
                ProtoWriter.WriteFieldHeader(3, WireType.String, protoWriter);
                SubItemToken token = ProtoWriter.StartSubItem(expr_4A, protoWriter);
                Write(expr_4A, protoWriter);
                ProtoWriter.EndSubItem(token, protoWriter);
            }
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="platformData">
        /// The platform data.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private static void Write(PlatformData platformData, ProtoWriter writer)
        {
            if (platformData.GetType() != typeof(PlatformData))
            {
                ThrowUnexpectedSubtype(typeof(PlatformData), platformData.GetType());
            }

            TargetPlatform expr_2D = platformData.Platform;
            if (expr_2D != TargetPlatform.Windows)
            {
                ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
                TargetPlatform targetPlatform = expr_2D;
                if (targetPlatform != TargetPlatform.Windows)
                {
                    if (targetPlatform != TargetPlatform.Xbox360)
                    {
                        if (targetPlatform != TargetPlatform.WindowsPhone)
                        {
                            if (targetPlatform != TargetPlatform.iOS)
                            {
                                if (targetPlatform != TargetPlatform.Android)
                                {
                                    if (targetPlatform != TargetPlatform.Linux)
                                    {
                                        if (targetPlatform != TargetPlatform.MacOSX)
                                        {
                                            if (targetPlatform != TargetPlatform.WindowsStoreApp)
                                            {
                                                if (targetPlatform != TargetPlatform.NativeClient)
                                                {
                                                    if (targetPlatform != TargetPlatform.Ouya)
                                                    {
                                                        if (targetPlatform != TargetPlatform.PlayStationMobile)
                                                        {
                                                            if (targetPlatform != TargetPlatform.WindowsPhone8)
                                                            {
                                                                if (targetPlatform != TargetPlatform.RaspberryPi)
                                                                {
                                                                    ProtoWriter.ThrowEnumException(
                                                                        writer, 
                                                                        targetPlatform);
                                                                }
                                                                else
                                                                {
                                                                    ProtoWriter.WriteInt32(12, writer);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                ProtoWriter.WriteInt32(11, writer);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            ProtoWriter.WriteInt32(10, writer);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ProtoWriter.WriteInt32(9, writer);
                                                    }
                                                }
                                                else
                                                {
                                                    ProtoWriter.WriteInt32(8, writer);
                                                }
                                            }
                                            else
                                            {
                                                ProtoWriter.WriteInt32(7, writer);
                                            }
                                        }
                                        else
                                        {
                                            ProtoWriter.WriteInt32(6, writer);
                                        }
                                    }
                                    else
                                    {
                                        ProtoWriter.WriteInt32(5, writer);
                                    }
                                }
                                else
                                {
                                    ProtoWriter.WriteInt32(4, writer);
                                }
                            }
                            else
                            {
                                ProtoWriter.WriteInt32(3, writer);
                            }
                        }
                        else
                        {
                            ProtoWriter.WriteInt32(2, writer);
                        }
                    }
                    else
                    {
                        ProtoWriter.WriteInt32(1, writer);
                    }
                }
                else
                {
                    ProtoWriter.WriteInt32(0, writer);
                }
            }

            byte[] expr_143 = platformData.Data;
            if (expr_143 != null)
            {
                ProtoWriter.WriteFieldHeader(2, WireType.String, writer);
                ProtoWriter.WriteBytes(expr_143, writer);
            }
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="targetPlatform">
        /// The target platform.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        private static void Write(TargetPlatform targetPlatform, ProtoWriter writer)
        {
            ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
            if (targetPlatform != TargetPlatform.Windows)
            {
                if (targetPlatform != TargetPlatform.Xbox360)
                {
                    if (targetPlatform != TargetPlatform.WindowsPhone)
                    {
                        if (targetPlatform != TargetPlatform.iOS)
                        {
                            if (targetPlatform != TargetPlatform.Android)
                            {
                                if (targetPlatform != TargetPlatform.Linux)
                                {
                                    if (targetPlatform != TargetPlatform.MacOSX)
                                    {
                                        if (targetPlatform != TargetPlatform.WindowsStoreApp)
                                        {
                                            if (targetPlatform != TargetPlatform.NativeClient)
                                            {
                                                if (targetPlatform != TargetPlatform.Ouya)
                                                {
                                                    if (targetPlatform != TargetPlatform.PlayStationMobile)
                                                    {
                                                        if (targetPlatform != TargetPlatform.WindowsPhone8)
                                                        {
                                                            if (targetPlatform != TargetPlatform.RaspberryPi)
                                                            {
                                                                ProtoWriter.ThrowEnumException(writer, targetPlatform);
                                                            }
                                                            else
                                                            {
                                                                ProtoWriter.WriteInt32(12, writer);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            ProtoWriter.WriteInt32(11, writer);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ProtoWriter.WriteInt32(10, writer);
                                                    }
                                                }
                                                else
                                                {
                                                    ProtoWriter.WriteInt32(9, writer);
                                                }
                                            }
                                            else
                                            {
                                                ProtoWriter.WriteInt32(8, writer);
                                            }
                                        }
                                        else
                                        {
                                            ProtoWriter.WriteInt32(7, writer);
                                        }
                                    }
                                    else
                                    {
                                        ProtoWriter.WriteInt32(6, writer);
                                    }
                                }
                                else
                                {
                                    ProtoWriter.WriteInt32(5, writer);
                                }
                            }
                            else
                            {
                                ProtoWriter.WriteInt32(4, writer);
                            }
                        }
                        else
                        {
                            ProtoWriter.WriteInt32(3, writer);
                        }
                    }
                    else
                    {
                        ProtoWriter.WriteInt32(2, writer);
                    }
                }
                else
                {
                    ProtoWriter.WriteInt32(1, writer);
                }
            }
            else
            {
                ProtoWriter.WriteInt32(0, writer);
            }
        }

        /// <summary>
        /// The _2.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="protoReader">
        /// The proto reader.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object _2(object obj, ProtoReader protoReader)
        {
            if (obj != null)
            {
                return Read((TargetPlatform)obj, protoReader);
            }

            return CompiledAssetSerializer.Read(TargetPlatform.Windows, protoReader);
        }
    }
}