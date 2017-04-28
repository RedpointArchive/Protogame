﻿using System;
using System.Text;

namespace Protogame
{
    public static class SerializedAssetExtensions
    {
        public static void SetLoader<T>(this ISerializedAsset asset)
        {
            asset.SetString("_Loader", typeof(T).AssemblyQualifiedName);
        }

        public static Type GetLoader(this ISerializedAsset asset)
        {
            return Type.GetType(asset.GetString("_Loader"));
        }

        public static void SetPlatform(this ISerializedAsset asset, TargetPlatform platform)
        {
            asset.SetInt32("_Platform", (int)platform);
        }

        public static TargetPlatform GetPlatform(this ISerializedAsset asset)
        {
            return (TargetPlatform)asset.GetInt32("_Platform");
        }

        public static void SetString(this ISerializedAsset asset, string property, string value)
        {
            asset.SetByteArray(property, Encoding.UTF8.GetBytes(value));
        }

        public static string GetString(this ISerializedAsset asset, string property)
        {
            return Encoding.UTF8.GetString(asset.GetByteArray(property));
        }

        public static void SetInt32(this ISerializedAsset asset, string property, Int32 value)
        {
            asset.SetByteArray(property, BitConverter.GetBytes(value));
        }

        public static Int32 GetInt32(this ISerializedAsset asset, string property)
        {
            return BitConverter.ToInt32(asset.GetByteArray(property), 0);
        }

        public static void SetFloat(this ISerializedAsset asset, string property, float value)
        {
            asset.SetByteArray(property, BitConverter.GetBytes(value));
        }

        public static float GetFloat(this ISerializedAsset asset, string property)
        {
            return BitConverter.ToSingle(asset.GetByteArray(property), 0);
        }

        public static void SetBoolean(this ISerializedAsset asset, string property, bool value)
        {
            asset.SetByteArray(property, BitConverter.GetBytes(value));
        }

        public static bool GetBoolean(this ISerializedAsset asset, string property)
        {
            return BitConverter.ToBoolean(asset.GetByteArray(property), 0);
        }
    }
}
