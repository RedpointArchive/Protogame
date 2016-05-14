using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.Xna.Framework;
using Protogame;
using Protogame.ATFLevelEditor;

namespace ProtogamePostBuild
{
    public class ATFLevelEditorBaking
    {
        public void BakeOutSchemaFile(string intermediateAssembly, string[] referencedAssemblies)
        {
            if (!referencedAssemblies.Any(x => x.EndsWith("Protogame.dll")))
            {
                // This assembly doesn't reference Protogame, so skip it.
                return;
            }

            // NOTE: This must be the last operation in the post-build system, because we can't modify the
            // intermediate assembly after this.

            intermediateAssembly = Path.Combine(Environment.CurrentDirectory, intermediateAssembly);

            Console.WriteLine("Searching for editable entities to export to ATF level schema...");

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var f = referencedAssemblies.FirstOrDefault(x => x.EndsWith(args.Name + ".dll"));
                if (f != null)
                {
                    return Assembly.LoadFile(f);
                }

                return null;
            };

            var asm = Assembly.LoadFile(intermediateAssembly);

            var xsd = new XmlDocument();
            xsd.InsertBefore(xsd.CreateXmlDeclaration("1.0", "utf-8", null), xsd.DocumentElement);
            var schema = CreateElement(xsd, "schema");
            schema.SetAttribute("elementFormDefault", "qualified");
            schema.SetAttribute("targetNamespace", "gap");
            schema.SetAttribute("xmlns", "gap");
            schema.SetAttribute("xmlns:xs", "http://www.w3.org/2001/XMLSchema");
            xsd.AppendChild(schema);

            try
            {
                foreach (var type in asm.GetTypes())
                {
                    try
                    {
                        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

                        foreach (var constructor in constructors)
                        {
                            var constructorParameters = constructor.GetParameters();
                            var isEditorQueryConstructor = constructorParameters
                                .Any(
                                    x =>
                                        x.ParameterType.IsGenericType &&
                                        x.ParameterType == typeof(IEditorQuery<>).MakeGenericType(type));

                            if (isEditorQueryConstructor)
                            {
                                Console.WriteLine("Found editable entity to export: " + type.FullName);

                                // Call the constructor with the editor query instance provided, but everything else
                                // null.  We expect entity code to handle this.  We don't use full DI here.
                                var bakingEditorQueryType = typeof(BakingEditorQuery<>).MakeGenericType(type);
                                var bakingEditorQuery =
                                    (BakingEditorQuery)Activator.CreateInstance(bakingEditorQueryType);

                                var arguments = new object[constructorParameters.Length];
                                for (var i = 0; i < constructorParameters.Length; i++)
                                {
                                    var x = constructorParameters[i];
                                    if (x.ParameterType.IsGenericType &&
                                        x.ParameterType == typeof(IEditorQuery<>).MakeGenericType(type))
                                    {
                                        arguments[i] = bakingEditorQuery;
                                    }
                                    else
                                    {
                                        if (x.ParameterType.IsValueType)
                                        {
                                            arguments[i] = Activator.CreateInstance(x.ParameterType);
                                        }
                                        else
                                        {
                                            arguments[i] = null;
                                        }
                                    }
                                }

                                constructor.Invoke(arguments);

                                foreach (var kv in bakingEditorQuery.IDToName)
                                {
                                    Console.WriteLine("  Found custom property '" + kv.Key + "' with name '" + kv.Value +
                                                      "' and type '" + bakingEditorQuery.IDToType[kv.Key].FullName +
                                                      "'...");
                                }

                                var complexType = CreateElement(xsd, "complexType");
                                schema.AppendChild(complexType);
                                complexType.SetAttribute("name", type.Name);
                                var annotation = CreateElement(xsd, "annotation");
                                complexType.AppendChild(annotation);

                                var appInfo = CreateElement(xsd, "appinfo");
                                annotation.AppendChild(appInfo);
                                var nativeType = xsd.CreateElement("" ,"LeGe.NativeType", "gap");
                                nativeType.SetAttribute("nativeName", "CubeGob");
                                appInfo.AppendChild(nativeType);
                                appInfo.AppendChild(CreateNativeAttribute(xsd, "color", "Color", "int", "set"));
                                appInfo.AppendChild(CreateNativeAttribute(xsd, "emissive", "Emissive", "int", "set"));
                                appInfo.AppendChild(CreateNativeAttribute(xsd, "specular", "Specular", "int", "set"));
                                appInfo.AppendChild(CreateNativeAttribute(xsd, "specularPower", "SpecularPower", "float", "set"));
                                appInfo.AppendChild(CreateNativeAttribute(xsd, "diffuse", "Diffuse", "wchar_t*", "set"));
                                appInfo.AppendChild(CreateNativeAttribute(xsd, "normal", "Normal", "wchar_t*", "set"));
                                appInfo.AppendChild(CreateNativeAttribute(xsd, "textureTransform", "TextureTransform", "Matrix", "set"));
                                foreach (var kv in bakingEditorQuery.IDToName)
                                {
                                    // TODO Allow Protogame properties to affect C++ properties in editor.
                                }

                                var sceaEditors = xsd.CreateElement("", "scea.dom.editors", "gap");
                                sceaEditors.SetAttribute("name", type.Name);
                                sceaEditors.SetAttribute("category", "Entities");
                                sceaEditors.SetAttribute("menuText", "Entities/" + type.Name);
                                sceaEditors.SetAttribute("description", type.FullName);
                                appInfo.AppendChild(sceaEditors);
                                appInfo.AppendChild(CreateEditorAttribute(xsd, "Shape", "color", "Color", "Object Color", "Sce.Atf.Controls.PropertyEditing.ColorPickerEditor,Atf.Gui.WinForms:true", "Sce.Atf.Controls.PropertyEditing.IntColorConverter"));
                                appInfo.AppendChild(CreateEditorAttribute(xsd, "Shape", "emissive", "Emissive", "Object emissive Color", "Sce.Atf.Controls.PropertyEditing.ColorPickerEditor,Atf.Gui.WinForms:true", "Sce.Atf.Controls.PropertyEditing.IntColorConverter"));
                                appInfo.AppendChild(CreateEditorAttribute(xsd, "Shape", "specular", "Specular", "Object specular Color", "Sce.Atf.Controls.PropertyEditing.ColorPickerEditor,Atf.Gui.WinForms:true", "Sce.Atf.Controls.PropertyEditing.IntColorConverter"));
                                appInfo.AppendChild(CreateEditorAttribute(xsd, "Shape", "specularPower", "Specular Power", "specular power", null, null));
                                appInfo.AppendChild(CreateEditorAttribute(xsd, "Shape", "diffuse", "Diffuse", "diffuse texture", "Sce.Atf.Controls.PropertyEditing.FileUriEditor,Atf.Gui.WinForms:Texture (*.png, *.dds, *.bmp, *.tga, *.tif)|*.png;*.dds;*.bmp;*.tga;*.tif", null));
                                appInfo.AppendChild(CreateEditorAttribute(xsd, "Shape", "normal", "Normal", "normal texture", "Sce.Atf.Controls.PropertyEditing.FileUriEditor,Atf.Gui.WinForms:Texture (*.png, *.dds, *.bmp, *.tga, *.tif)|*.png;*.dds;*.bmp;*.tga;*.tif", null));
                                appInfo.AppendChild(CreateEditorAttribute(xsd, "Shape", "textureTransform", "Texture Transform", "Transform for texture coordinates", "Sce.Atf.Controls.PropertyEditing.NumericMatrixEditor,Atf.Gui.WinForms", null));
                                foreach (var kv in bakingEditorQuery.IDToName)
                                {
                                    // TODO: Make shape properties and lighting properties like available methods on the
                                    // entity query system like MapStandardLightingModel, etc.
                                    if (
                                        !new[]
                                        {
                                            "color", "emissive", "specular", "specularPower", "diffuse", "normal",
                                            "textureTransform"
                                        }.Contains(kv.Key))
                                    {
                                        appInfo.AppendChild(CreateEditorAttribute(xsd, "Entity", kv.Key, kv.Value,
                                            "Entity property", GetEditorForTypeID(bakingEditorQuery.IDToType[kv.Key]),
                                            GetConverterForTypeID(bakingEditorQuery.IDToType[kv.Key])));
                                    }
                                }

                                var complexContent = CreateElement(xsd, "complexContent");
                                complexType.AppendChild(complexContent);
                                var extension = CreateElement(xsd, "extension");
                                complexContent.AppendChild(extension);
                                extension.SetAttribute("base", "gameObjectType");
                                extension.AppendChild(CreateDescriptorAttribute(xsd, "color", "xs:int", "-1"));
                                extension.AppendChild(CreateDescriptorAttribute(xsd, "emissive", "xs:int", "0"));
                                extension.AppendChild(CreateDescriptorAttribute(xsd, "specular", "xs:int", "0"));
                                extension.AppendChild(CreateDescriptorAttribute(xsd, "specularPower", "xs:float", "1"));
                                extension.AppendChild(CreateDescriptorAttribute(xsd, "diffuse", "xs:anyURI", null));
                                extension.AppendChild(CreateDescriptorAttribute(xsd, "normal", "xs:anyURI", null));
                                extension.AppendChild(CreateDescriptorAttribute(xsd, "textureTransform", "matrixType", "1 0 0 0 0 1 0 0 0 0 1 0 0 0 0 1"));

                                foreach (var kv in bakingEditorQuery.IDToName)
                                {
                                    if (
                                        !new[]
                                        {
                                            "color", "emissive", "specular", "specularPower", "diffuse", "normal",
                                            "textureTransform"
                                        }.Contains(kv.Key))
                                    {
                                        extension.AppendChild(CreateDescriptorAttribute(xsd, kv.Key,
                                            ConvertToTypeID(bakingEditorQuery.IDToType[kv.Key]), null));
                                    }
                                }

                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

            var fi = new FileInfo(intermediateAssembly);
            var nwe = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);

            xsd.Save(Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(intermediateAssembly), "bundle." + nwe + ".xsd"));
        }

        private string GetEditorForTypeID(Type type)
        {
            if (type == typeof (Color))
            {
                return "Sce.Atf.Controls.PropertyEditing.ColorPickerEditor,Atf.Gui.WinForms:true";
            }

            return null;
        }

        private string GetConverterForTypeID(Type type)
        {
            if (type == typeof(Color))
            {
                return "Sce.Atf.Controls.PropertyEditing.IntColorConverter";
            }

            return null;
        }

        private string ConvertToTypeID(Type type)
        {
            if (type == typeof(int))
            {
                return "xs:int";
            }
            if (type == typeof (float))
            {
                return "xs:float";
            }
            if (type == typeof(string))
            {
                return "xs:string";
            }
            if (type == typeof(bool))
            {
                return "xs:boolean";
            }
            if (type == typeof(float[]))
            {
                return "floatListType";
            }
            if (type == typeof(Vector2))
            {
                return "vector2Type";
            }
            if (type == typeof(Vector3))
            {
                return "vector3Type";
            }
            if (type == typeof(Color))
            {
                return "xs:int";
            }
            if (type == typeof(Vector4))
            {
                return "vector4Type";
            }
            if (type == typeof(Matrix))
            {
                return "matrixType";
            }
            throw new Exception("Can't yet support type " + type);
        }

        private XmlNode CreateDescriptorAttribute(XmlDocument xsd, string name, string type, string @default)
        {
            var attribute = CreateElement(xsd, "attribute");
            attribute.SetAttribute("name", name);
            attribute.SetAttribute("type", type);
            if (@default != null)
            {
                attribute.SetAttribute("default", @default);
            }
            return attribute;
        }

        private XmlNode CreateNativeAttribute(XmlDocument xsd, string name, string nativeName, string nativeType, string access)
        {
            var attribute = xsd.CreateElement("", "LeGe.NativeProperty", "gap");
            attribute.SetAttribute("name", name);
            attribute.SetAttribute("nativeName", nativeName);
            attribute.SetAttribute("nativeType", nativeType);
            attribute.SetAttribute("access", access);
            return attribute;
        }

        private XmlNode CreateEditorAttribute(
            XmlDocument xsd, 
            string category, 
            string name,
            string displayName,
            string description,
            string editor,
            string converter)
        {
            var attribute = xsd.CreateElement("", "scea.dom.editors.attribute", "gap");
            attribute.SetAttribute("category", category);
            attribute.SetAttribute("name", name);
            attribute.SetAttribute("displayName", displayName);
            attribute.SetAttribute("description", description);
            if (editor != null)
            {
                attribute.SetAttribute("nativeType", editor);
            }
            if (converter != null)
            {
                attribute.SetAttribute("access", converter);
            }
            return attribute;
        }

        private static XmlElement CreateElement(XmlDocument xsd, string name)
        {
            return xsd.CreateElement("xs", name, "http://www.w3.org/2001/XMLSchema");
        }
    }
}