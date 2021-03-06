using LabCMS.EquipmentDomain.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using LabCMS.EquipmentDomain.Server.Repositories;
//using Microsoft.ClearScript;
//using Microsoft.ClearScript.JavaScript;
//using Microsoft.ClearScript.V8;

namespace LabCMS.EquipmentDomain.Server.Services
{
    public class DynamicQueryService
    {
        private readonly UsageRecordsRepository  _usageRecordsRepository;
        public DynamicQueryService(UsageRecordsRepository usageRecordsRepository)
        { 
            _usageRecordsRepository=usageRecordsRepository;
            //_jsEngine.AddHostType("Console", typeof(Console));
            //_jsEngine.AddHostType(typeof(Enumerable));
            //_jsEngine.AddHostType(typeof(UsageRecord));
            //_jsEngine.AddHostObject("lib", new HostTypeCollection("mscorlib", "System.Core"
            //    ,"System.Collections","System.Runtime"));
            
        }

        private readonly CSharpCompilationOptions _compilationOptions = new(
            outputKind:OutputKind.DynamicallyLinkedLibrary,
            optimizationLevel:OptimizationLevel.Release);

        private Assembly Compile(string assemblyName,string code,Assembly[] referenceAssemblies)
        {
            IEnumerable<PortableExecutableReference> _references = referenceAssemblies
                .Select(assembly=>MetadataReference.CreateFromFile(assembly.Location));
            CSharpCompilation cSharpCompilation = CSharpCompilation.Create(assemblyName)
                .WithReferences(_references).WithOptions(_compilationOptions)
			    .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));
            using MemoryStream memoryStream = new MemoryStream();
		    cSharpCompilation.Emit(memoryStream);
		    memoryStream.Seek(0L, SeekOrigin.Begin);
            return Assembly.Load(memoryStream.ToArray());
        }

        public dynamic DynamicQuery(string codePiece)
        {
            string assemblyId =Guid.NewGuid().ToString().Replace('-','_');
            string code =
$@"
using LabCMS.EquipmentDomain.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabCMS.EquipmentDomain.Temp_{assemblyId}
{{
    public class DynamicQuereInstance
    {{
        public dynamic DynamicQuery(IEnumerable<UsageRecord> usageRecords)
        {{
            {codePiece}
        }}
    }}
}}
";
        Assembly tempAssembly = Compile($"AssemblyTemp{assemblyId}",code,
            AssemblyLoadContext.Default.Assemblies.Where(assembly=>!assembly.IsDynamic).ToArray());
        Type instanceType = tempAssembly.GetType($"LabCMS.EquipmentDomain.Temp_{assemblyId}.DynamicQuereInstance")!;
        dynamic instance = Activator.CreateInstance(instanceType)!;
        dynamic result = instance.DynamicQuery(_usageRecordsRepository.UsageRecords.AsNoTracking());
        return result;

        }
        //private readonly V8ScriptEngine _jsEngine = new();
        //public dynamic DynamicQueryByV8(string codePiece)
        //{
        //    IEnumerable<UsageRecord> usageRecords = _usageRecordsRepository.UsageRecords.AsNoTracking().ToList();
        //    _jsEngine.AddHostObject(nameof(usageRecords), usageRecords);
        //    string wrappedCode = @$"
        //        function queryFunc(){{
        //            {codePiece}
        //        }}
        //        queryFunc();
        //    ";
        //    return _jsEngine.Evaluate(wrappedCode);
        //}
    }
}