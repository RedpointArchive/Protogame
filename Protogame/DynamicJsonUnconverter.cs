//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Script.Serialization;

public sealed class DynamicJsonUnconverter : JavaScriptConverter
{
    public override object Deserialize(
        IDictionary<string, object> dictionary,
        Type type,
        JavaScriptSerializer serializer)
    {
        if (dictionary == null)
            throw new ArgumentNullException("dictionary");

        return type == typeof(object) ? new DynamicJsonConverter.DynamicJsonObject(dictionary) : null;
    }

    public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
    {
        var casted = obj as DynamicJsonConverter.DynamicJsonObject;
        return casted == null ? null : casted.OriginalDictionary();
    }


    public override IEnumerable<Type> SupportedTypes
    {
        get
        {
            return new ReadOnlyCollection<Type>(
                new List<Type>(new[] { typeof(DynamicJsonConverter.DynamicJsonObject) }));
        }
    }
}

