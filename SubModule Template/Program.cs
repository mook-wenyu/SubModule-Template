using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubModule_Template
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Start();
            Console.WriteLine("请按任意键关闭此控制台。\n");
            Console.ReadKey();
        }

        static void Start()
        {
            string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string mBConfigs = string.Format("{0}\\Mount and Blade II Bannerlord\\Configs", myDocuments);
            string SubModuleTemplateConfig = string.Format("{0}\\SubModuleTemplateConfig.txt", mBConfigs);
            string smtcContent = "";

            if (File.Exists(SubModuleTemplateConfig))
            {
                smtcContent = ReadFile(SubModuleTemplateConfig);
                
                smtcContent = Regex.Replace(smtcContent, @"# *.*\r*\n*", "");
                smtcContent = smtcContent.Trim();
                Console.WriteLine("《骑马与砍杀2：霸主》根目录：" + smtcContent + "\n");
                CreateModule(smtcContent);
            }
            else
            {
                Console.WriteLine("没有找到《骑马与砍杀2：霸主》根目录。\n");
                SelectTheHomeDirectory();
            }
        }

        /// <summary>
        /// 选择主目录
        /// </summary>
        static void SelectTheHomeDirectory()
        {
            Console.WriteLine("请按【任意】键选择《骑马与砍杀2：霸主》的主目录。\n");
            Console.ReadKey();
            Console.WriteLine("\n");
            string mbDirectory = "";

            mbDirectory = SelectCommonOpenFileDialog();
            if (!string.IsNullOrWhiteSpace(mbDirectory))
            {
                string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string mBConfigs = string.Format("{0}\\Mount and Blade II Bannerlord\\Configs", myDocuments);
                if (false == Directory.Exists(mBConfigs))
                {
                    //创建文件夹
                    Directory.CreateDirectory(mBConfigs);
                }

                string note = "# 非官方配置文件\n";
                string conetxt = note + mbDirectory;

                WriteFile(string.Format("{0}\\SubModuleTemplateConfig.txt", mBConfigs), conetxt);
                
                Console.Write("\n配置文件路径：" + string.Format("{0}\\SubModuleTemplateConfig.txt\n\n", mBConfigs));

                CreateModule(mbDirectory);
            }
            else
            {
                Start();
            }

            
        }

        /// <summary>
        /// 创建子模组
        /// </summary>
        /// <param name="mbDirectory"></param>
        static void CreateModule(string mbDirectory)
        {
            string moduleTemplateDirectory = Environment.CurrentDirectory + "\\" + "ModuleTemplate";
            string mbModulesDirectory = string.Format("{0}\\Modules", mbDirectory);
            string moduleName = "";
            string moduleNameNoSpaces = "";
            string moduleDirectory = "";
            string subModuleXMLContent = "";
            
            string vsProjectPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string vsProjectDirectory = "";
            string vsSLNContent = "";

            Console.Write("请输入模组的名称(\"New Module\")：");
            moduleName = Console.ReadLine();
            Console.Write("\n");

            if (string.IsNullOrWhiteSpace(moduleName))
            {
                moduleName = "New Module";
            }
            moduleNameNoSpaces = moduleName.Replace(" ", "");

            //设置模组文件夹路径
            moduleDirectory = mbModulesDirectory + "\\" + moduleNameNoSpaces;

            if (!Directory.Exists(moduleDirectory))
            {
                //创建模组文件夹
                Directory.CreateDirectory(moduleDirectory);
                Directory.CreateDirectory(moduleDirectory + "\\" + "bin");
                Directory.CreateDirectory(moduleDirectory + "\\" + "bin" + "\\" + "Win64_Shipping_Client");

                //打开SubModule.xml模板
                subModuleXMLContent = ReadFile(string.Format("{0}\\{1}\\{2}", moduleTemplateDirectory, "ModuleTemplate", "SubModule.xml"));

                // 替换模板字符串
                subModuleXMLContent = Regex.Replace(subModuleXMLContent, "Module Template", moduleName);
                subModuleXMLContent = Regex.Replace(subModuleXMLContent, "ModuleTemplate", moduleNameNoSpaces);

                //创建SubModule.xml文件
                WriteFile(moduleDirectory + "\\" + "SubModule.xml", subModuleXMLContent);
                
                //创建VS项目
                vsProjectDirectory = vsProjectPath + "\\" + moduleNameNoSpaces;
                if (!Directory.Exists(vsProjectPath + "\\" + moduleNameNoSpaces))
                {
                    
                    Directory.CreateDirectory(vsProjectDirectory);
                    Directory.CreateDirectory(string.Format("{0}\\{1}", vsProjectDirectory, moduleNameNoSpaces));
                    Directory.CreateDirectory(string.Format("{0}\\{1}\\Properties", vsProjectDirectory, moduleNameNoSpaces));
                    Directory.CreateDirectory(string.Format("{0}\\{1}\\obj", vsProjectDirectory, moduleNameNoSpaces));
                    Directory.CreateDirectory(string.Format("{0}\\{1}\\obj\\Debug", vsProjectDirectory, moduleNameNoSpaces));
                    Directory.CreateDirectory(string.Format("{0}\\{1}\\obj\\Release", vsProjectDirectory, moduleNameNoSpaces));

                    //打开vs模板
                    vsSLNContent = ReadFile(string.Format("{0}\\{1}", moduleTemplateDirectory, "ModuleTemplate.sln"));
                    string vsAssemblyInfoContent = ReadFile(string.Format("{0}\\{1}\\{2}\\{3}", moduleTemplateDirectory, "ModuleTemplate", "Properties", "AssemblyInfo.cs"));
                    string vsCSPROJContent = ReadFile(string.Format("{0}\\{1}\\{2}", moduleTemplateDirectory, "ModuleTemplate", "ModuleTemplate.csproj"));
                    string vsSubModuleContent = ReadFile(string.Format("{0}\\{1}\\{2}", moduleTemplateDirectory, "ModuleTemplate", "SubModule.cs"));
                    string vsXMLContent = ReadFile(string.Format("{0}\\{1}\\{2}", moduleTemplateDirectory, "ModuleTemplate", "SubModule.xml"));

                    // 替换vs模板字符串
                    vsSLNContent = Regex.Replace(vsSLNContent, "ModuleTemplate", moduleNameNoSpaces);
                    vsAssemblyInfoContent = Regex.Replace(vsAssemblyInfoContent, "ModuleTemplate", moduleNameNoSpaces);
                    vsCSPROJContent = Regex.Replace(vsCSPROJContent, "ModuleTemplate", moduleNameNoSpaces);
                    vsCSPROJContent = Regex.Replace(vsCSPROJContent, @"E:\\Program Files %28x86%29\\Steam\\steamapps\\common\\Mount &amp; Blade II Bannerlord", mbDirectory.Replace("(", "%28").Replace(")", "%29").Replace("&", "&amp;"));
                    vsCSPROJContent = Regex.Replace(vsCSPROJContent, @"E:\\Program Files \(x86\)\\Steam\\steamapps\\common\\Mount &amp; Blade II Bannerlord", mbDirectory.Replace("&", "&amp;"));
                    vsSubModuleContent = Regex.Replace(vsSubModuleContent, "ModuleTemplate", moduleNameNoSpaces);
                    vsXMLContent = Regex.Replace(vsXMLContent, "Module Template", moduleName);
                    vsXMLContent = Regex.Replace(vsXMLContent, "ModuleTemplate", moduleNameNoSpaces);

                    //创建vs文件
                    WriteFile(string.Format("{0}\\{1}.sln", vsProjectDirectory, moduleNameNoSpaces), vsSLNContent);
                    WriteFile(string.Format("{0}\\{1}\\{2}\\{3}", vsProjectDirectory, moduleNameNoSpaces, "Properties", "AssemblyInfo.cs"), vsAssemblyInfoContent);
                    WriteFile(string.Format("{0}\\{1}\\{2}.csproj", vsProjectDirectory, moduleNameNoSpaces, moduleNameNoSpaces), vsCSPROJContent);
                    WriteFile(string.Format("{0}\\{1}\\{2}", vsProjectDirectory, moduleNameNoSpaces, "SubModule.cs"), vsSubModuleContent);
                    WriteFile(string.Format("{0}\\{1}\\{2}", vsProjectDirectory, moduleNameNoSpaces, "SubModule.xml"), vsXMLContent);

                    //
                    System.Diagnostics.Process.Start(vsProjectDirectory);
                    Console.WriteLine(string.Format("模组项目已创建：{0}\n", moduleDirectory));
                    Console.WriteLine(string.Format("VS项目已创建：{0}\n", vsProjectDirectory));
                }
                else
                {
                    Console.WriteLine("该文件夹已存在，请更改模组名。\n");
                    DeleteFileAndDirectory(moduleDirectory);
                    CreateModule(mbDirectory);
                }
            }
            else
            {
                Console.WriteLine("该文件夹已存在，请更改模组名。\n");
                CreateModule(mbDirectory);
            }

        }

        static string ReadFile(string path)
        {
            string content = "";
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, new UTF8Encoding(false));
            content = sr.ReadToEnd();
            sr.Close();
            fs.Close();

            return content;
        }

        static void WriteFile(string path, string content)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, new UTF8Encoding(false));
            sw.Write(content);
            sw.Close();
            fs.Close();
        }

        /// <summary>
        /// 选择文件夹对话框
        /// </summary>
        /// <returns></returns>
        static string SelectCommonOpenFileDialog()
        {
            CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            cofd.IsFolderPicker = true;
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return cofd.FileName;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 遍历文件夹和文件夹并删除
        /// </summary>
        /// <param name="path"></param>
        static void DeleteFileAndDirectory(string path)
        {
            DirectoryInfo theFolder = new DirectoryInfo(@path);
            //遍历文件
            foreach (FileInfo NextFile in theFolder.GetFiles())
            {
                File.Delete(NextFile.FullName);
            }
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in theFolder.GetDirectories())
            {
                DeleteFileAndDirectory(NextFolder.FullName);
                //Directory.Delete(NextFolder.FullName);
            }
            Directory.Delete(theFolder.FullName);
        }
    }
}
