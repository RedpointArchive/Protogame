using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
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

                                var skip = false;
                                var nativeTypeName = string.Empty;
                                var xsdBaseTypeName = string.Empty;
                                switch (bakingEditorQuery.DeclaredAs)
                                {
                                    case DeclaredAs.Entity:
                                        Console.WriteLine("Found editable entity: " + type.FullName);
                                        nativeTypeName = type.FullName.Replace(".", "_").Replace("?","") + "_NativeEntity";
                                        xsdBaseTypeName = "gameObjectType";
                                        break;
                                    case DeclaredAs.Component:
                                        Console.WriteLine("Found editable component: " + type.FullName);
                                        nativeTypeName = type.FullName.Replace(".", "_").Replace("?", "") + "_NativeComponent";
                                        if (bakingEditorQuery.HasTransform)
                                        {
                                            xsdBaseTypeName = "transformComponentType";
                                        }
                                        else
                                        {
                                            xsdBaseTypeName = "gameObjectComponentType";
                                        }
                                        break;
                                    default:
                                        Console.WriteLine("Skipping unknown object (use one of the Declare* methods): " + type.FullName);
                                        skip = true;
                                        break;
                                }

                                if (skip)
                                {
                                    break;
                                }

                                var complexType = CreateElement(xsd, "complexType");
                                schema.AppendChild(complexType);
                                complexType.SetAttribute("name", type.Name);
                                var annotation = CreateElement(xsd, "annotation");
                                complexType.AppendChild(annotation);

                                var appInfo = CreateElement(xsd, "appinfo");
                                annotation.AppendChild(appInfo);

                                var protogameInfo = xsd.CreateElement("", "Protogame.Info", "gap");
                                appInfo.AppendChild(protogameInfo);
                                protogameInfo.SetAttribute("NeedsNativeTypeGenerated", nativeTypeName);
                                protogameInfo.SetAttribute("DeclaredAs", bakingEditorQuery.DeclaredAs.ToString());
                                protogameInfo.SetAttribute("RenderMode", bakingEditorQuery.RenderMode.ToString());
                                protogameInfo.SetAttribute("PrimitiveShape", bakingEditorQuery.PrimitiveShape.ToString());
                                protogameInfo.SetAttribute("IconAbsolutePath", bakingEditorQuery.IconAbsolutePath ?? string.Empty);
                                protogameInfo.SetAttribute("CanContainComponents", bakingEditorQuery.CanContainComponents.ToString(CultureInfo.InvariantCulture));
                                protogameInfo.SetAttribute("CanContainEntities", bakingEditorQuery.CanContainEntities.ToString(CultureInfo.InvariantCulture));
                                protogameInfo.SetAttribute("HasTransform", bakingEditorQuery.HasTransform.ToString(CultureInfo.InvariantCulture));
                                protogameInfo.SetAttribute("QualifiedName", type.AssemblyQualifiedName);

                                // If an icon path is specified, we must read the icon data and base64-encode it.
                                if (bakingEditorQuery.IconAbsolutePath != null)
                                {
                                    using (
                                        var reader = new FileStream(bakingEditorQuery.IconAbsolutePath, FileMode.Open,
                                            FileAccess.Read))
                                    {
                                        var bytes = new byte[reader.Length];
                                        reader.Read(bytes, 0, bytes.Length);
                                        var iconData = Convert.ToBase64String(bytes);
                                        protogameInfo.SetAttribute("IconData", iconData);
                                    }
                                }

                                var nativeType = xsd.CreateElement("" ,"LeGe.NativeType", "gap");
                                nativeType.SetAttribute("nativeName", nativeTypeName);
                                appInfo.AppendChild(nativeType);
                                foreach (var prop in bakingEditorQuery.Properties.Values)
                                {
                                    appInfo.AppendChild(CreateNativeAttribute(
                                        xsd,
                                        prop.ID,
                                        prop.CamelCaseName,
                                        prop.NativeType,
                                        prop.NativeAccess));
                                }

                                if (bakingEditorQuery.CanContainComponents)
                                {
                                    var nativeElement = xsd.CreateElement("", "LeGe.NativeElement", "gap");
                                    nativeElement.SetAttribute("name", "component");
                                    nativeElement.SetAttribute("nativeName", "Component");
                                    nativeElement.SetAttribute("nativeType", "GameObjectComponent");
                                    appInfo.AppendChild(nativeElement);
                                }

                                if (bakingEditorQuery.CanContainEntities)
                                {
                                    var nativeElement = xsd.CreateElement("", "LeGe.NativeElement", "gap");
                                    nativeElement.SetAttribute("name", "gameObject");
                                    nativeElement.SetAttribute("nativeName", "Child");
                                    nativeElement.SetAttribute("nativeType", "GameObject");
                                    appInfo.AppendChild(nativeElement);
                                }

                                // <LeGe.NativeElement  name="component" nativeName="Component" nativeType="GameObjectComponent" />

                                var sceaEditors = xsd.CreateElement("", "scea.dom.editors", "gap");
                                sceaEditors.SetAttribute("name", type.Name);
                                sceaEditors.SetAttribute("category", "Entities");
                                sceaEditors.SetAttribute("menuText", "Entities/" + type.Name);
                                sceaEditors.SetAttribute("description", type.FullName);
                                appInfo.AppendChild(sceaEditors);
                                foreach (var prop in bakingEditorQuery.Properties.Values)
                                {
                                    appInfo.AppendChild(CreateEditorAttribute(
                                        xsd,
                                        "Entity",
                                        prop.ID,
                                        prop.DisplayName,
                                        prop.EditorDescription,
                                        prop.EditorType,
                                        prop.EditorConverterType));
                                }
                                
                                var complexContent = CreateElement(xsd, "complexContent");
                                complexType.AppendChild(complexContent);
                                var extension = CreateElement(xsd, "extension");
                                complexContent.AppendChild(extension);
                                extension.SetAttribute("base", xsdBaseTypeName);

                                if (bakingEditorQuery.CanContainComponents)
                                {
                                    var sequence = CreateElement(xsd, "sequence");
                                    extension.AppendChild(sequence);

                                    var element = CreateElement(xsd, "element");
                                    element.SetAttribute("name", "component");
                                    element.SetAttribute("type", "gameObjectComponentType");
                                    element.SetAttribute("minOccurs", "0");
                                    element.SetAttribute("maxOccurs", "unbounded");
                                    sequence.AppendChild(element);
                                }

                                if (bakingEditorQuery.CanContainEntities)
                                {
                                    var sequence = CreateElement(xsd, "sequence");
                                    extension.AppendChild(sequence);

                                    var element = CreateElement(xsd, "element");
                                    element.SetAttribute("name", "component");
                                    element.SetAttribute("type", "gameObjectType");
                                    element.SetAttribute("minOccurs", "0");
                                    element.SetAttribute("maxOccurs", "unbounded");
                                    sequence.AppendChild(element);
                                }

                                foreach (var prop in bakingEditorQuery.Properties.Values)
                                {
                                    extension.AppendChild(CreateDescriptorAttribute(
                                        xsd,
                                        prop.ID,
                                        prop.XSDType,
                                        prop.XSDDefaultValue));
                                }

                                break;
                            }
                        }
                    }
                    catch (FileNotFoundException ex)
                    {
                        Console.Error.WriteLine("WARNING: Can't scan type " + type.FullName + " because the assembly " + ex.FileName + " was not available.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("ERROR: Encountered a fatal exception during processing: " + ex);
                Environment.ExitCode = 1;
            }

            var fi = new FileInfo(intermediateAssembly);
            var nwe = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);

            xsd.Save(Path.Combine(Environment.CurrentDirectory, Path.GetDirectoryName(intermediateAssembly), "bundle." + nwe + ".xsd"));
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
                attribute.SetAttribute("editor", editor);
            }
            if (converter != null)
            {
                attribute.SetAttribute("converter", converter);
            }
            return attribute;
        }

        private static XmlElement CreateElement(XmlDocument xsd, string name)
        {
            return xsd.CreateElement("xs", name, "http://www.w3.org/2001/XMLSchema");
        }
    }
}