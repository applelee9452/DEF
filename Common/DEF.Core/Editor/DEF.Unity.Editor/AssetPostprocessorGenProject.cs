//using UnityEditor;
//using System.Xml;
//using System.IO;
//using System.Collections.Generic;

//public class AssetPostprocessorGenProject : AssetPostprocessor
//{
//    public static string OnGeneratedCSProject(string path, string content)
//    {
//        return GenCsProj(path, content);
//    }

//    public static string GenCsProj(string path, string content)
//    {
//        if (path.EndsWith("DEF.Common.csproj"))
//        {
//            content = content.Replace("<Compile Include=\"Assets\\DEF.Common\\Dummy.cs\" />", string.Empty);
//            content = content.Replace("<None Include=\"Assets\\DEF.Common\\DEF.Common.asmdef\" />", string.Empty);
//        }

//        if (path.EndsWith("DEF.Common.csproj"))
//        {
//            return GenerateCustomProject(path, content, @"..\..\DEF\Common\DEF.Common");
//        }

//        if (path.EndsWith("DEF.Client.csproj"))
//        {
//            content = content.Replace("<Compile Include=\"Assets\\DEF.Client\\Dummy.cs\" />", string.Empty);
//            content = content.Replace("<None Include=\"Assets\\DEF.Client\\DEF.Client.asmdef\" />", string.Empty);
//        }

//        if (path.EndsWith("DEF.Client.csproj"))
//        {
//            return GenerateCustomProject(path, content, @"..\..\DEF\Client\DEF.Client");
//        }

//        return content;
//    }

//    public static string GenerateCustomProject(string path, string content, string codesPath)
//    {
//        XmlDocument doc = new();
//        doc.LoadXml(content);

//        var newDoc = doc.Clone() as XmlDocument;

//        var rootNode = newDoc.GetElementsByTagName("Project")[0];

//        {
//            var itemGroup = newDoc.CreateElement("ItemGroup", newDoc.DocumentElement.NamespaceURI);

//            //{
//            //    DirectoryInfo di1 = new(path);
//            //    di1 = di1.Parent;
//            //    string s = Path.Combine(di1.FullName, codesPath);
//            //    s = Path.GetFullPath(s);

//            //    Dictionary<string, string> map_file = new(1024);
//            //    List<string> excludes = new()
//            //        {
//            //            s + @"\obj",
//            //            s + @"\bin"
//            //        };

//            //    GetFiles(s.Length, s, map_file, excludes);

//            //    foreach (var i in map_file)
//            //    {
//            //        var compile = newDoc.CreateElement("Compile", newDoc.DocumentElement.NamespaceURI);
//            //        compile.SetAttribute("Include", i.Value);//codesPath + @"\**\*.cs");
//            //        compile.SetAttribute("Link", i.Key);
//            //        itemGroup.AppendChild(compile);
//            //    }
//            //}

//            {
//                var compile = newDoc.CreateElement("Compile", newDoc.DocumentElement.NamespaceURI);
//                compile.SetAttribute("Include", codesPath + @"\**\*.cs");
//                compile.SetAttribute("Link", @"%(RecursiveDir)%(FileName)%(Extension)");
//                itemGroup.AppendChild(compile);
//            }

//            {
//                var compile = newDoc.CreateElement("Compile", newDoc.DocumentElement.NamespaceURI);
//                compile.SetAttribute("Remove", codesPath + @"\bin\**\*.*");
//                itemGroup.AppendChild(compile);
//            }

//            {
//                var compile = newDoc.CreateElement("Compile", newDoc.DocumentElement.NamespaceURI);
//                compile.SetAttribute("Remove", codesPath + @"\obj\**\*.*");
//                itemGroup.AppendChild(compile);
//            }

//            rootNode.AppendChild(itemGroup);
//        }

//        {
//            var itemGroup = newDoc.CreateElement("ItemGroup", newDoc.DocumentElement.NamespaceURI);

//            var projectReference = newDoc.CreateElement("ProjectReference", newDoc.DocumentElement.NamespaceURI);
//            projectReference.SetAttribute("Include", @"..\..\DEF\Common\DEF.CodeGenerator\DEF.CodeGenerator.csproj");
//            projectReference.SetAttribute("OutputItemType", @"Analyzer");
//            projectReference.SetAttribute("ReferenceOutputAssembly", @"false");

//            //var analyzer = newDoc.CreateElement("Analyzer", newDoc.DocumentElement.NamespaceURI);
//            //analyzer.SetAttribute("Include", @"Assets/Plugins/DEF.CodeGenerator.dll");

//            //var project = newDoc.CreateElement("Project", newDoc.DocumentElement.NamespaceURI);
//            //project.InnerText = @"{9A19103F-16F7-4668-BE54-9A1E7A4F7556}";
//            //projectReference.AppendChild(project);

//            //var name = newDoc.CreateElement("Name", newDoc.DocumentElement.NamespaceURI);
//            //name.InnerText = "DEF.CodeGenerator";
//            //projectReference.AppendChild(name);

//            itemGroup.AppendChild(projectReference);

//            rootNode.AppendChild(itemGroup);
//        }

//        using StringWriter sw = new();
//        using XmlTextWriter tx = new(sw);
//        tx.Formatting = Formatting.Indented;
//        newDoc.WriteTo(tx);
//        tx.Flush();
//        return sw.GetStringBuilder().ToString();
//    }

//    public static string GenerateCustomProject(string path, string content, List<string> list_codepath)
//    {
//        XmlDocument doc = new();
//        doc.LoadXml(content);

//        var newDoc = doc.Clone() as XmlDocument;

//        var rootNode = newDoc.GetElementsByTagName("Project")[0];

//        foreach (var i in list_codepath)
//        {
//            var itemGroup = newDoc.CreateElement("ItemGroup", newDoc.DocumentElement.NamespaceURI);

//            {
//                var compile = newDoc.CreateElement("Compile", newDoc.DocumentElement.NamespaceURI);
//                compile.SetAttribute("Include", i + @"\**\*.cs");
//                compile.SetAttribute("Link", @"%(RecursiveDir)%(FileName)%(Extension)");
//                itemGroup.AppendChild(compile);
//            }

//            {
//                var compile = newDoc.CreateElement("Compile", newDoc.DocumentElement.NamespaceURI);
//                compile.SetAttribute("Remove", i + @"\**\bin\**\*.*");
//                itemGroup.AppendChild(compile);
//            }

//            {
//                var compile = newDoc.CreateElement("Compile", newDoc.DocumentElement.NamespaceURI);
//                compile.SetAttribute("Remove", i + @"\**\obj\**\*.*");
//                itemGroup.AppendChild(compile);
//            }

//            rootNode.AppendChild(itemGroup);
//        }

//        {
//            var itemGroup = newDoc.CreateElement("ItemGroup", newDoc.DocumentElement.NamespaceURI);

//            var projectReference = newDoc.CreateElement("ProjectReference", newDoc.DocumentElement.NamespaceURI);
//            projectReference.SetAttribute("Include", @"..\..\DEF\Common\DEF.CodeGenerator\DEF.CodeGenerator.csproj");
//            projectReference.SetAttribute("OutputItemType", @"Analyzer");
//            projectReference.SetAttribute("ReferenceOutputAssembly", @"false");

//            itemGroup.AppendChild(projectReference);

//            rootNode.AppendChild(itemGroup);
//        }

//        using StringWriter sw = new();
//        using XmlTextWriter tx = new(sw);
//        tx.Formatting = Formatting.Indented;
//        newDoc.WriteTo(tx);
//        tx.Flush();
//        return sw.GetStringBuilder().ToString();
//    }

//    static void GetFiles(int trim_count, string dir, Dictionary<string, string> map_file, List<string> excludes)
//    {
//        DirectoryInfo di = new(dir);

//        var arr_file = di.GetFiles("*.cs");
//        foreach (var i in arr_file)
//        {
//            string s = i.FullName.Substring(trim_count + 1, i.FullName.Length - trim_count - 1);
//            map_file[s] = i.FullName;
//        }

//        var arr_dir = di.GetDirectories();
//        foreach (var i in arr_dir)
//        {
//            if (excludes.Contains(i.FullName)) continue;

//            GetFiles(trim_count, i.FullName, map_file, excludes);
//        }
//    }
//}
