using System;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
                { "Jitter", "Jitter.xml", false },
            };

            var outputPath = Path.Combine(currentLocation, "Protogame.combined.xml");

            var forceRef = typeof(Protogame.DefaultCollision).Assembly;

            var output = new XmlDocument();
            var typesElem = output.CreateElement("Types");
            output.AppendChild(typesElem);

            foreach (var entry in toProcess)
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == entry.AssemblyFile);
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
            return docIdentifier.Substring(2).Replace("`", "*");
        }

        static string ConvertXmlElementToReST(XmlElement toConvert)
        {
            var text = string.Empty;
            foreach (XmlNode node in toConvert.ChildNodes)
            {
                if (node is XmlText)
                {
                    text += node.InnerText;
                }
                else if (node is XmlElement)
                {
                    var element = (XmlElement)node;

                    switch (element.LocalName)
                    {
                        case "see":
                            var cref = element.GetAttribute("cref");
                            if (cref.Trim().Length > 0)
                            {
                                text += ":ref:`" + 
                                    GetAnchor(cref).Trim() + "`";
                            }
                            break;
                        default:
                            text += ConvertXmlElementToReST(element);
                            break;
                    }
                }
            }
            return text;
        }

        static void PortElementFromDocs(XmlElement source, string sourceName, XmlElement target, string targetName, Func<XmlElement, bool> sourceFilter = null)
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

                targetElem.InnerText = ConvertXmlElementToReST((XmlElement)sourceElem);

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
            typeElem.SetAttribute("Name", type.Name);
            typeElem.SetAttribute("Namespace", type.Namespace);
            typeElem.SetAttribute("FullName", type.FullName);
            typeElem.SetAttribute("Module", NormalizeName(moduleName));
            typeElem.SetAttribute("Anchor", GetAnchor(typeXmlName));
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
                PortElementFromDocs(docs, "summary", typeElem, "Summary");
            }

            foreach (var nestedType in type.GetNestedTypes(BindingFlagsAll))
            {
                ProcessType(output, typeElem, lookup, nestedType, supportsModules);
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
            propertyElem.SetAttribute("Anchor", GetAnchor(propertyXmlName));
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
            propertyElem.SetAttribute("TypeAnchor", GetTypeXmlName(property.PropertyType));

            if (docs != null)
            {
                PortElementFromDocs(docs, "summary", propertyElem, "Summary");
                PortElementFromDocs(docs, "value", propertyElem, "Value");
            }
        }

        static void ProcessMethod(XmlDocument output, XmlElement container, Dictionary<string, XmlElement> lookup, Type type, MethodInfo method)
        {
            var methodXmlName = "M:";

            methodXmlName += GetTypeXmlName(type) + ".";
            methodXmlName += method.Name;
            methodXmlName += "(";
            methodXmlName += method.GetParameters().Select(x => x.ParameterType.FullName).DefaultIfEmpty("").Aggregate((a, b) => a + "," + b);
            methodXmlName += ")";

            XmlElement docs = null;
            if (lookup.ContainsKey(methodXmlName))
            {
                docs = lookup[methodXmlName];
            }

            Console.WriteLine("## Processing documentation for " + methodXmlName);

            var methodElem = output.CreateElement("Method");
            container.AppendChild(methodElem);
            methodElem.SetAttribute("Name", method.Name);
            methodElem.SetAttribute("Anchor", GetAnchor(methodXmlName));
            methodElem.SetAttribute("IsPublic", method.IsPublic ? "True" : "False");
            methodElem.SetAttribute("IsProtected", method.IsFamily ? "True" : "False");
            methodElem.SetAttribute("IsPrivate", method.IsPrivate ? "True" : "False");
            methodElem.SetAttribute("IsAbstract", method.IsAbstract ? "True" : "False");
            methodElem.SetAttribute("ReturnTypeName", method.ReturnType.Name);
            methodElem.SetAttribute("ReturnTypeNamespace", method.ReturnType.Namespace);
            methodElem.SetAttribute("ReturnTypeFullName", method.ReturnType.FullName);
            methodElem.SetAttribute("ReturnTypeAnchor", GetTypeXmlName(method.ReturnType));

            if (docs != null)
            {
                PortElementFromDocs(docs, "summary", methodElem, "Summary");
                PortElementFromDocs(docs, "returns", methodElem, "Returns");
            }

            foreach (var parameter in method.GetParameters())
            {
                var parameterElem = output.CreateElement("Parameter");
                methodElem.AppendChild(parameterElem);
                parameterElem.SetAttribute("Name", parameter.Name);
                parameterElem.SetAttribute("TypeName", parameter.ParameterType.Name);
                parameterElem.SetAttribute("TypeNamespace", parameter.ParameterType.Namespace);
                parameterElem.SetAttribute("TypeFullName", parameter.ParameterType.FullName);
                parameterElem.SetAttribute("TypeAnchor", GetTypeXmlName(parameter.ParameterType));
                parameterElem.SetAttribute("IsRef", parameter.IsRetval ? "True" : "False");
                parameterElem.SetAttribute("IsOut", parameter.IsOut ? "True" : "False");

                if (docs != null)
                {
                    PortElementFromDocs(docs, "param", parameterElem, null, x => x.GetAttribute("name") == parameter.Name);
                }
            }
        }
    }
}
