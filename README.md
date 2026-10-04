<!--
   Copyright 2023-2026 Alexander Stärk

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
-->
# Basilisque - Code Analysis

## Overview
This project provides helpers for writing .Net code analyzers and source generators.

[![NuGet Basilisque.CodeAnalysis](https://img.shields.io/badge/NuGet_Basilisque.CodeAnalysis-latest-%23004880.svg?logo=nuget)](https://www.nuget.org/packages/Basilisque.CodeAnalysis)
[![License](https://img.shields.io/badge/License-Apache%20License%202.0-%23D22128.svg?logo=apache&logoColor=%23D22128)](LICENSE.txt)  
[![NuGet Basilisque.CodeAnalysis](https://img.shields.io/badge/NuGet_Basilisque.CodeAnalysis.TestSupport-latest-%23004880.svg?logo=nuget)](https://www.nuget.org/packages/Basilisque.CodeAnalysis.TestSupport)
[![SonarCloud](https://img.shields.io/badge/SonarCloud-main-%23F3702A.svg?logo=sonarcloud&logoColor=%23F3702A)](https://sonarcloud.io/project/overview?id=basilisque-framework_CodeAnalysis)  

## Description
This project contains helpers and structured classes, that support you in generating source code that is syntactically correct and well formatted.
It also contains helpers for unit testing source generators.

For example this code...
```csharp
private string generateMyDemoMethod()
{
    var classInfo = new ClassInfo("MyDemoClass", AccessModifier.Public);
    classInfo.GenericTypes.Add("T1", (new List<string>() { "class", "new()" }, "Description of T1"));
    classInfo.GenericTypes.Add("T2", null);
    classInfo.Methods.Add(new MethodInfo(AccessModifier.Public, "bool", "MyDemoMethod")
    {
        Parameters =
        {
            new ParameterInfo(ParameterKind.Ordinary, "int", "myMaxParam")
        },
        Body =
        {
            @"
// this is the method body. You can write any code in here...
var r = System.Random.Shared.Next(myMaxParam);
if (r > 5)
    return true;
else
    return false;
"
        }
    });

    return classInfo.ToString();
}
```
...will result in this output:
```csharp
/// <summary>
/// MyDemoClass
/// </summary>
/// <typeparam name="T1">Description of T1</typeparam>
/// <typeparam name="T2"></typeparam>
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("My.Demo.SourceGenerator", "1.0.0.0")]
[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
public class MyDemoClass<T1, T2>
    where T1 : class, new()
{
    public bool MyDemoMethod(int myMaxParam)
    {
        // this is the method body. You can write any code in here...
        var r = System.Random.Shared.Next(myMaxParam);
        if (r > 5)
            return true;
        else
            return false;
    }
}

```

There is support for __namespaces__ with usings, __classes__, __properties__, __methods__ and __fields__ with matching support for __XML code documentation__, __generics__ and extension methods.  

In addition there are some helpers for registering the generated source in an incremental source generator. They will automatically provide you with __information about__ the used __language__, the __language version__ and the __nullable context__.

You don't have to care about the order in that you specify the code parts. The generated source code will always be __well structured__ and __correctly indented__.

For details see the __[wiki](https://github.com/basilisque-framework/CodeAnalysis/wiki)__.

### Installation for Code Analyzers and Source Generators
Install the NuGet package [Basilisque.CodeAnalysis](https://www.nuget.org/packages/Basilisque.CodeAnalysis).  
Installing the package will automaticall configure your project to be packed as code analyzer/source generator. It will also pack its own assemblies beside your analyzer/generator as dependency.

So you're ready to [get started](https://github.com/basilisque-framework/CodeAnalysis/wiki/Getting-Started).

### Analyzer package validation
The analyzer assembly is collected during framework-specific packing, after
`GetTargetPath` has finalized its name and output directory. This supports version
suffixes supplied by CommonBuild, SDK artifacts output, and `dotnet pack --no-build`.
The empty `lib/<framework>/_._` placeholder preserves the dependency framework
without exposing the analyzer as a runtime library.

Run the packaging regression checks with:

```powershell
pwsh -File tests/Packaging/Test-AnalyzerPacking.ps1
```

The checks import the actual CodeAnalysis build files and exercise CommonBuild
versioning with both SDK output layouts, with and without an assembly-name suffix,
and with packing disabled. They also compare the packaged DLL with the compiler
output. Existing CommonBuild integration tests independently cover its own analyzer
packing target.

If another support package already bundles `Basilisque.CodeAnalysis` alongside the
analyzer, set `BAS_CA_Dependency_PackCS` to `false` in the analyzer project to avoid
packing the same dependency twice. `BAS_CA_Assembly_PackCS` independently controls
packing the analyzer itself.

### Installation for Unit Tests
This project provides helpers for unit tests.  
To make use of the install the package:  
[Basilisque.CodeAnalysis.TestSupport](https://www.nuget.org/packages/Basilisque.CodeAnalysis.TestSupport)  

Now you can write [your first Unit Test](https://github.com/basilisque-framework/CodeAnalysis/wiki/Getting-Started#unit-tests).

## License
The Basilisque framework (including this repository) is licensed under the [Apache License, Version 2.0](LICENSE.txt).