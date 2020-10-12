﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspileTest.Nodes;
using TranspileTest.Parser;

namespace TranspileTest
{
    class Program
    {
        static NodeEngine nodeEngine = new NodeEngine();

        static ScriptParser ScriptParser = new ScriptParser();

        private static ScriptNode ParseScript(string input)
        {
            using (new ScoppedTimer("ParseScript"))
            {
                var scriptProgram = new ScriptProgram();
                var script1 = ScriptParser.ParseScript(scriptProgram, input);
                return script1;
            }
        }

        private static ScriptNode ParseBinaryScript(byte[] input)
        {
            using (new ScoppedTimer("ReadBinaryScript"))
            {
                var script1 = Serializer.Read(input);
                return script1;
            }
        }

        static void Main(string[] args)
        {
        /*
            var nullscript = ParseScript("@GossipScript @p{}");
            var nullscriptBinary = ParseBinaryScript(new byte[16]);
            */
            var scriptCompiler = new ScriptCompiler();

            /*
            var scripts = new List<string>
            {
                "Scripts/Test001.gs",
                "Scripts/Test002.gs",
                "Scripts/Test003.gs"
            };

            var program = scriptCompiler.CompileScript("Scripts/Test004.gs", scripts);

            nodeEngine.RunScriptContinous(program.MainScript);
            
            ScriptNode script4 = LoadScript("Test004.gs");
            nodeEngine.RunScriptContinous(script4);
            */

            /*
            var program = scriptCompiler.CompileScript("Scripts/Test005a.gs", new List<string>() { "Scripts/Test005b.gs" });

            nodeEngine.LoadVariableTable(program.GlobalVariables);

            foreach(var s in program.Scripts.Values)
            {
                nodeEngine.RunScriptContinous(s);
            }
    */

            var program = scriptCompiler.CompileScript("Scripts/Test007.gs", new List<string>());

            ScriptNode script4 = LoadScript("Test007.gs");

            nodeEngine.LoadVariableTable(program.GlobalVariables);

            foreach (var s in program.Scripts.Values)
            {
                nodeEngine.RunScriptContinous(s);
            }

            Console.WriteLine();
        }

        private static ScriptNode LoadScript(string filename)
        {
            var script4 = ParseScript(File.ReadAllText("Scripts/" + filename));
            TestScripts.AssignRandomGuids(script4);
            var serial4 = Serializer.Write(script4);
            script4 = ParseBinaryScript(serial4);

            File.WriteAllBytes(filename + ".b.script", serial4);
            Console.WriteLine("Byte Size:" + serial4.Length);
            return script4;
        }

        private static void PrintNodeTree(Node node, int level)
        {
            if (node.NodeType == NodeType.Command)
            {
                Console.WriteLine(node.NodeType + ":" + node.CommandType);
            }
            else
            {
                Console.WriteLine(node.NodeType);
            }

            foreach(var c in node.Children)
            {
                for (int i = 0; i < level; i++)
                {
                    Console.Write(" ");
                }
                
                PrintNodeTree(c, ++level);
            }
        }
    }
}
