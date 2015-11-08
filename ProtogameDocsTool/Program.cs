using System;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace ProtogameDocsTool
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            var currentLocation = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;

            var kv = new KeyValuePair<string, string>();

            var toProcess = new DocumentationList
            {
                { "Protogame", "Protogame.xml", true },
                { "MonoGame.Framework", "MonoGame.Framework.xml", false },
                { "MonoGame.Framework.Content.Pipeline", "MonoGame.Framework.Content.Pipeline.xml", false },
                { "Jitter", "Jitter.xml", false },
                { "Protoinject", "Protoinject.xml", false },
                { "Protoinject.Extensions.Factory", "Protoinject.Extensions.Factory.xml", false },
                { "Protoinject.Extensions.Interception", "Protoinject.Extensions.Interception.xml", false },
                { "NDesk.Options", "NDesk.Options.xml", false },
                { "Newtonsoft.Json", "Newtonsoft.Json.xml", false },
            };

            var outputPath = Path.Combine(currentLocation, "Protogame.combined.xml");

            var forceRef = typeof(Protogame.DefaultCollision).Assembly;

            var output = new XmlDocument();
            var typesElem = output.CreateElement("Types");
            output.AppendChild(typesElem);

            foreach (var entry in toProcess)
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == entry.AssemblyFile);
                if (assembly == null)
                {
                    assembly = Assembly.Load(entry.AssemblyFile);
                }

                var documentationPath = Path.Combine(currentLocation, entry.DocumentationFile);
                var documentation = new XmlDocument();
                documentation.Load(documentationPath);

                CombineDocumentationWithAssembly(
                    assembly,
                    documentation,
                    typesElem,
                    entry.SupportsModules);
            }

            output.Save(outputPath);
        }

        const BindingFlags BindingFlagsAll = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        static void CombineDocumentationWithAssembly (Assembly assembly, XmlDocument documentation, XmlElement typesElem, bool supportsModules)
        {
            var lookup = new Dictionary<string, XmlElement>();
            foreach (var node in documentation.SelectNodes("/doc/members/member").OfType<XmlElement>())
            {
                lookup.Add(node.GetAttribute("name"), node);
            }

            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                if (type.IsNested)
                {
                    continue;
                }

                ProcessType(typesElem.OwnerDocument, typesElem, lookup, type, supportsModules);
            }
        }

        static string NormalizeName(string name)
        {
            return name.Trim().ToLower().Replace(" ", "_");
        }

        static string ProcessTypeName(Type type)
        {
            if (type.DeclaringType != null)
            {
                return ProcessTypeName(type.DeclaringType) + "." + type.Name;
            }

            return type.Name;
        }

        static string GetTypeXmlName(Type type)
        {
            return type.Namespace + "." + ProcessTypeName(type);
        }

        static string GetAnchor(string docIdentifier)
        {
            return docIdentifier;
        }

        static string IndentText(string currentText, string text, int indent)
        {
            var indentStr = Regex.Match(currentText, "^[ \n]*").Value.TrimStart('\n');
            for (var i = 0; i < indent; i++) indentStr += " ";
            return text.Split('\n').Select(x => indentStr + x).Aggregate((a, b) => a + "\n" + b);
        }

        static string ConvertXmlElementToReST(object context, XmlElement toConvert, int indent = 0)
        {
            var text = string.Empty;
            foreach (XmlNode node in toConvert.ChildNodes)
            {
                if (node is XmlText)
                {
                    text += IndentText("", node.InnerText, indent);
                }
                else if (node is XmlCharacterData)
                {
                    text += IndentText("", node.InnerText, indent);
                }
                else if (node is XmlElement)
                {
                    var element = (XmlElement)node;

                    switch (element.LocalName)
                    {
                        case "see":
                            var cref = element.GetAttribute("cref");
                            var refTarget = string.Empty;
                            if (cref.Trim().Length > 0)
                            {
                                refTarget = GetAnchor(cref).Trim();
                            }
                            else
                            {
                                refTarget = element.InnerText.Trim();
                            }

                            if (context is ParameterInfo)
                            {
                                context = ((ParameterInfo)context).Member;
                            }

                            if (refTarget.StartsWith("**"))
                            {
                                var methodInfo = context as MethodInfo;
                                if (methodInfo != null && methodInfo.IsGenericMethod)
                                {
                                    refTarget = methodInfo.GetGenericArguments()[int.Parse(refTarget.Substring(2))].Name;
                                }
                            }
                            else if (refTarget.StartsWith("*"))
                            {
                                var methodInfo = context as MethodInfo;
                                if (methodInfo != null && methodInfo.DeclaringType.IsGenericType)
                                {
                                    refTarget = methodInfo.DeclaringType.GetGenericArguments()[int.Parse(refTarget.Substring(1))].Name;
                                }

                                var typeInfo = context as Type;
                                if (typeInfo != null && typeInfo.IsGenericType)
                                {
                                    refTarget = typeInfo.GetGenericArguments()[int.Parse(refTarget.Substring(1))].Name;
                                }
                            }

                            text += ":ref:`" + refTarget + "`";
                            break;
                        case "para":
                            text += ConvertXmlElementToReST(context, element, indent);
                            break;
                        case "code":
                            text += IndentText(text, "\n.. code-block:: csharp", indent) + "\n";
                            text += ConvertXmlElementToReST(context, element, indent + 4);
                            break;
                        case "i":
                            text += "";
                            break;
                        default:
                            text += ConvertXmlElementToReST(context, element, indent);
                            break;
                    }
                }
            }
            return text;
        }

        static void PortElementFromDocs(object context, XmlElement source, string sourceName, XmlElement target, string targetName, Func<XmlElement, bool> sourceFilter = null)
        {
            XmlNode sourceElem;
            if (sourceFilter == null)
            {
                sourceElem = source.SelectSingleNode("./" + sourceName);
            }
            else
            {
                sourceElem = source.SelectNodes("./" + sourceName).OfType<XmlElement>().FirstOrDefault(sourceFilter);
            }

            if (sourceElem != null)
            {
                var targetElem = targetName == null ? target : target.OwnerDocument.CreateElement(targetName);

                targetElem.InnerText = ConvertXmlElementToReST(context, (XmlElement)sourceElem);

                if (targetName != null)
                {
                    target.AppendChild(targetElem);
                }
            }
        }

        static void ProcessType(XmlDocument output, XmlElement container, Dictionary<string, XmlElement> lookup, Type type, bool supportsModules)
        {
            var typeXmlName = "T:" + GetTypeXmlName(type);

            if (type.Name.StartsWith(@"<"))
            {
                return;
            }

            var internalText = "False";
            string interfaceRef = null;
            string moduleName = type.Assembly.GetName().Name;
            XmlElement docs = null;

            if (lookup.ContainsKey(typeXmlName))
            {
                docs = lookup[typeXmlName];

                var moduleElem = docs.SelectSingleNode("./module");
                var @internalElem = docs.SelectSingleNode("./internal");
                var interfaceRefElem = docs.SelectSingleNode("./interface_ref");

                if (moduleElem == null && supportsModules)
                {
                    Console.WriteLine(".. Skipping   documentation for " + typeXmlName);
                    return;
                }

                if (supportsModules)
                {
                    moduleName = moduleElem.InnerText;
                }

                if (@internalElem != null)
                {
                    if (@internalElem.InnerText.Trim() == "True")
                    {
                        internalText = "True";
                    }
                }

                if (interfaceRefElem != null)
                {
                    interfaceRef = interfaceRefElem.InnerText.Trim();
                }
            }
            else if (supportsModules)
            {
                Console.WriteLine(".. Skipping   documentation for " + typeXmlName);
                return;
            }

            Console.WriteLine("## Processing documentation for " + typeXmlName);

            var typeElem = output.CreateElement("Type");
            container.AppendChild(typeElem);
            typeElem.SetAttribute("Name", GetGenericName(type));
            typeElem.SetAttribute("Namespace", type.Namespace);
            typeElem.SetAttribute("FullName", type.FullName);
            typeElem.SetAttribute("Module", NormalizeName(moduleName));
            typeElem.SetAttribute("Anchor", GetAnchor(type));
            typeElem.SetAttribute("IsProtogameInternal", internalText);

            if (interfaceRef != null)
            {
                typeElem.SetAttribute("InterfaceRef", interfaceRef);
            }

            typeElem.SetAttribute("IsPublic", type.IsPublic ? "True" : "False");
            typeElem.SetAttribute("IsAbstract", type.IsAbstract ? "True" : "False");

            if (type.IsEnum)
            {
                typeElem.SetAttribute("Type", "Enum");
            }
            else if (type.IsValueType)
            {
                typeElem.SetAttribute("Type", "Struct");
            }
            else if (type.IsClass)
            {
                typeElem.SetAttribute("Type", "Class");
            }
            else if (type.IsInterface)
            {
                typeElem.SetAttribute("Type", "Interface");
            }
            else
            {
                typeElem.SetAttribute("Type", "Unknown");
            }

            if (docs != null)
            {
                PortElementFromDocs(type, docs, "summary", typeElem, "Summary");
            }

            if (type.BaseType != null)
            {
                var baseField = output.CreateElement("Inherits");
                typeElem.AppendChild(baseField);
                baseField.SetAttribute("TypeName", type.BaseType.Name);
                baseField.SetAttribute("TypeNamespace", type.BaseType.Namespace);
                baseField.SetAttribute("TypeFullName", type.BaseType.FullName);
                baseField.SetAttribute("TypeAnchor", GetAnchor(type.BaseType));
            }

            foreach (var @interface in type.GetInterfaces())
            {
                var implementsField = output.CreateElement("Implements");
                typeElem.AppendChild(implementsField);
                implementsField.SetAttribute("TypeName", @interface.Name);
                implementsField.SetAttribute("TypeNamespace", @interface.Namespace);
                implementsField.SetAttribute("TypeFullName", @interface.FullName);
                implementsField.SetAttribute("TypeAnchor", GetAnchor(@interface));
            }

            foreach (var typeParameter in type.GetGenericArguments())
            {
                var typeParameterElem = output.CreateElement("TypeParameter");
                typeElem.AppendChild(typeParameterElem);
                typeParameterElem.SetAttribute("TypeName", typeParameter.Name);
                typeParameterElem.SetAttribute("TypeNamespace", typeParameter.Namespace);
                typeParameterElem.SetAttribute("TypeFullName", typeParameter.FullName);
                typeParameterElem.SetAttribute("TypeAnchor", GetAnchor(typeParameter));

                if (docs != null)
                {
                    PortElementFromDocs(typeParameter, docs, "typeparam", typeParameterElem, null, x => x.GetAttribute("name") == typeParameter.Name);
                }
            }

            foreach (var nestedType in type.GetNestedTypes(BindingFlagsAll))
            {
                ProcessType(output, typeElem, lookup, nestedType, supportsModules);
            }

            foreach (var field in type.GetFields(BindingFlagsAll))
            {
                ProcessField(output, typeElem, lookup, type, field);
            }

            List<string> methodNamesToIgnore = new List<string>();
            foreach (var property in type.GetProperties(BindingFlagsAll))
            {
                ProcessProperty(output, typeElem, lookup, type, property, methodNamesToIgnore);
            }

            foreach (var method in type.GetMethods(BindingFlagsAll))
            {
                if (!methodNamesToIgnore.Contains(method.Name))
                {
                    ProcessMethod(output, typeElem, lookup, type, method);
                }
            }
        }

        static void ProcessField(XmlDocument output, XmlElement container, Dictionary<string, XmlElement> lookup, Type type, FieldInfo field)
        {
            if ((field.Attributes & FieldAttributes.SpecialName) != 0)
            {
                return;
            }

            var fieldXmlName = "F:";

            fieldXmlName += GetTypeXmlName(type) + ".";
            fieldXmlName += field.Name;

            XmlElement docs = null;
            if (lookup.ContainsKey(fieldXmlName))
            {
                docs = lookup[fieldXmlName];
            }

            Console.WriteLine("## Processing documentation for " + fieldXmlName);

            var fieldElem = output.CreateElement("Field");
            container.AppendChild(fieldElem);
            fieldElem.SetAttribute("Name", field.Name);
            fieldElem.SetAttribute("Anchor", GetAnchor(field));
            fieldElem.SetAttribute("IsPublic", field.IsPublic ? "True" : "False");
            fieldElem.SetAttribute("IsProtected", field.IsFamily ? "True" : "False");
            fieldElem.SetAttribute("IsPrivate", field.IsPrivate ? "True" : "False");
            fieldElem.SetAttribute("TypeName", field.FieldType.Name);
            fieldElem.SetAttribute("TypeNamespace", field.FieldType.Namespace);
            fieldElem.SetAttribute("TypeFullName", field.FieldType.FullName);
            fieldElem.SetAttribute("TypeAnchor", GetAnchor(field.FieldType));
            try
            {
                var constVal = field.GetRawConstantValue();
                if (constVal != null)
                {
                    if (constVal is string)
                    {
                        fieldElem.SetAttribute("ConstValue", "\"" + field.GetRawConstantValue().ToString().Replace("\\","\\\\").Replace("\"", "\\\"") + "\"");
                    }
                    else
                    {
                        fieldElem.SetAttribute("ConstValue", field.GetRawConstantValue().ToString());
                    }
                }
                else
                {
                    fieldElem.SetAttribute("ConstValue", "null");
                }
            }
            catch (InvalidOperationException ex)
            {
            }

            if (docs != null)
            {
                PortElementFromDocs(field, docs, "summary", fieldElem, "Summary");
                PortElementFromDocs(field, docs, "value", fieldElem, "Value");
            }
        }

        static void ProcessProperty(XmlDocument output, XmlElement container, Dictionary<string, XmlElement> lookup, Type type, PropertyInfo property, List<string> methodNamesToIgnore)
        {
            var propertyXmlName = "P:";

            propertyXmlName += GetTypeXmlName(type) + ".";
            propertyXmlName += property.Name;

            XmlElement docs = null;
            if (lookup.ContainsKey(propertyXmlName))
            {
                docs = lookup[propertyXmlName];
            }

            Console.WriteLine("## Processing documentation for " + propertyXmlName);

            var propertyElem = output.CreateElement("Property");
            container.AppendChild(propertyElem);
            propertyElem.SetAttribute("Name", property.Name);
            propertyElem.SetAttribute("Anchor", GetAnchor(property));
            if (property.GetGetMethod() != null)
            {
                propertyElem.SetAttribute("HasGet", "True");
                propertyElem.SetAttribute("IsGetPublic", property.GetGetMethod().IsPublic ? "True" : "False");
                propertyElem.SetAttribute("IsGetProtected", property.GetGetMethod().IsFamily ? "True" : "False");
                propertyElem.SetAttribute("IsGetPrivate", property.GetGetMethod().IsPrivate ? "True" : "False");
                methodNamesToIgnore.Add(property.GetGetMethod().Name);
            }
            else
            {
                propertyElem.SetAttribute("HasGet", "False");
            }
            if (property.GetSetMethod() != null)
            {
                propertyElem.SetAttribute("HasSet", "True");
                propertyElem.SetAttribute("IsSetPublic", property.GetSetMethod().IsPublic ? "True" : "False");
                propertyElem.SetAttribute("IsSetProtected", property.GetSetMethod().IsFamily ? "True" : "False");
                propertyElem.SetAttribute("IsSetPrivate", property.GetSetMethod().IsPrivate ? "True" : "False");
                methodNamesToIgnore.Add(property.GetSetMethod().Name);
            }
            else
            {
                propertyElem.SetAttribute("HasSet", "False");
            }
            propertyElem.SetAttribute("TypeName", property.PropertyType.Name);
            propertyElem.SetAttribute("TypeNamespace", property.PropertyType.Namespace);
            propertyElem.SetAttribute("TypeFullName", property.PropertyType.FullName);
            propertyElem.SetAttribute("TypeAnchor", GetAnchor(property.PropertyType));

            if (docs != null)
            {
                PortElementFromDocs(property, docs, "summary", propertyElem, "Summary");
                PortElementFromDocs(property, docs, "value", propertyElem, "Value");
            }
        }

        static string GetGenericName(Type type)
        {
            var name = type.Name;

            if (name.IndexOf("`") > 0)
            {
                name = name.Substring(0, name.IndexOf("`"));
            }

            if (type.IsGenericType)
            {
                name += "<";
                name += type.GetGenericArguments().Select(x => x.Name).Aggregate((a, b) => a + ", " + b);
                name += ">";
            }

            return name;
        }

        static string GetGenericName(MethodInfo method)
        {
            var name = method.Name;

            if (name.IndexOf("`") > 0)
            {
                name = name.Substring(0, name.IndexOf("`"));
            }

            if (method.IsGenericMethod)
            {
                name += "<";
                name += method.GetGenericArguments().Select(x => x.Name).Aggregate((a, b) => a + ", " + b);
                name += ">";
            }

            return name;
        }

        static string GetAnchor(Type type)
        {
            return 
                type.Namespace + "." + 
                GetGenericName(type);
        }

        static string GetAnchor(MethodInfo method)
        {
            string methodParams = string.Empty;
            var genericMethodParams = method.GetParameters().Select(x => GetGenericName(x.ParameterType)).DefaultIfEmpty("").Aggregate((a, b) => a + "," + b);
            if (genericMethodParams != string.Empty)
            {
                methodParams += "(";
                methodParams += genericMethodParams;
                methodParams += ")";
            }

            return 
                method.DeclaringType.Namespace + "." + 
                GetGenericName(method.DeclaringType) + "." + 
                GetGenericName(method) + methodParams;
        }

        static string GetAnchor(FieldInfo field)
        {
            return 
                field.DeclaringType.Namespace + "." + 
                GetGenericName(field.DeclaringType) + "." + 
                field.Name;
        }

        static string GetAnchor(PropertyInfo property)
        {
            return 
                property.DeclaringType.Namespace + "." + 
                GetGenericName(property.DeclaringType) + "." + 
                property.Name;
        }

        static void ProcessMethod(XmlDocument output, XmlElement container, Dictionary<string, XmlElement> lookup, Type type, MethodInfo method)
        {
            var methodXmlName = "M:";

            methodXmlName += GetTypeXmlName(type) + ".";
            methodXmlName += method.Name;

            if (method.IsGenericMethod)
            {
                methodXmlName += "``";
                methodXmlName += method.GetGenericArguments().Length.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            var xmlParams = method.GetParameters().Select(x => x.ParameterType.FullName).DefaultIfEmpty("").Aggregate((a, b) => a + "," + b);
            if (xmlParams != string.Empty)
            {
                methodXmlName += "(";
                methodXmlName += xmlParams;
                methodXmlName += ")";
            }

            XmlElement docs = null;
            if (lookup.ContainsKey(methodXmlName))
            {
                docs = lookup[methodXmlName];
            }

            Console.WriteLine("## Processing documentation for " + methodXmlName);

            var methodElem = output.CreateElement("Method");
            container.AppendChild(methodElem);
            methodElem.SetAttribute("Name", GetGenericName(method));
            methodElem.SetAttribute("Anchor", GetAnchor(method));
            methodElem.SetAttribute("IsPublic", method.IsPublic ? "True" : "False");
            methodElem.SetAttribute("IsProtected", method.IsFamily ? "True" : "False");
            methodElem.SetAttribute("IsPrivate", method.IsPrivate ? "True" : "False");
            methodElem.SetAttribute("IsAbstract", method.IsAbstract ? "True" : "False");
            methodElem.SetAttribute("ReturnTypeName", method.ReturnType.Name);
            methodElem.SetAttribute("ReturnTypeNamespace", method.ReturnType.Namespace);
            methodElem.SetAttribute("ReturnTypeFullName", method.ReturnType.FullName);
            methodElem.SetAttribute("ReturnTypeAnchor", GetAnchor(method.ReturnType));

            if (docs != null)
            {
                PortElementFromDocs(method, docs, "summary", methodElem, "Summary");
                PortElementFromDocs(method, docs, "returns", methodElem, "Returns");
            }

            foreach (var typeParameter in method.GetGenericArguments())
            {
                var typeParameterElem = output.CreateElement("TypeParameter");
                methodElem.AppendChild(typeParameterElem);
                typeParameterElem.SetAttribute("TypeName", typeParameter.Name);
                typeParameterElem.SetAttribute("TypeNamespace", typeParameter.Namespace);
                typeParameterElem.SetAttribute("TypeFullName", typeParameter.FullName);
                typeParameterElem.SetAttribute("TypeAnchor", GetAnchor(typeParameter));

                if (docs != null)
                {
                    PortElementFromDocs(typeParameter, docs, "typeparam", typeParameterElem, null, x => x.GetAttribute("name") == typeParameter.Name);
                }
            }

            foreach (var parameter in method.GetParameters())
            {
                var parameterElem = output.CreateElement("Parameter");
                methodElem.AppendChild(parameterElem);
                parameterElem.SetAttribute("Name", parameter.Name);

                var isRetval = false;//parameter.IsRetval;
                var isOut = false;//parameter.IsOut;
                var paramType = parameter.ParameterType;
                if (paramType.IsByRef)
                {
                    isRetval = true;
                    paramType = paramType.GetElementType();
                }
                else if (paramType.IsPointer)
                {
                    isOut = true;
                    paramType = paramType.GetElementType();
                }

                parameterElem.SetAttribute("TypeName", paramType.Name);
                parameterElem.SetAttribute("TypeNamespace", paramType.Namespace);
                parameterElem.SetAttribute("TypeFullName", paramType.FullName);
                parameterElem.SetAttribute("TypeAnchor", GetAnchor(paramType));
                parameterElem.SetAttribute("IsRef", isRetval ? "True" : "False");
                parameterElem.SetAttribute("IsOut", isOut ? "True" : "False");

                if (docs != null)
                {
                    PortElementFromDocs(parameter, docs, "param", parameterElem, null, x => x.GetAttribute("name") == parameter.Name);
                }
            }
        }
    }
}
