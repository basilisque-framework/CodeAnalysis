/*
   Copyright 2023-2024 Alexander Stärk

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using System.Text;

namespace Basilisque.CodeAnalysis.Syntax
{
    /// <summary>
    /// Creates a new <see cref="CompilationInfo"/>
    /// </summary>
    /// <param name="compilationName" example="CodeAnalysis_GeneratedSource">The name of the current compilation or rather the 'source output' (e.g. used as the hintName for the <see cref="SourceProductionContext"/>)</param>
    /// <param name="targetNamespace" example="Basilisque.CodeAnalysis">The containing namespace of all child syntax nodes (when the targetNamespace is null, empty or whitespace, all children are in the global namespace)</param>
    /// <returns>A new instance of <see cref="CompilationInfo"/></returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="compilationName"/> is null, empty or whitespace</exception>
    public delegate CompilationInfo CompilationInfoFactory(string compilationName, string? targetNamespace);

    /// <summary>
    /// Represents a single source output for source generation
    /// (e.g. to be used as source input for a <see cref="SourceProductionContext"/>)
    /// </summary>
    public class CompilationInfo : CompilationInfo<CompilationInfo>
    {

        /// <summary>
        /// Creates a new <see cref="CompilationInfo"/> for the specified language
        /// </summary>
        /// <param name="compilationName" example="CodeAnalysis_GeneratedSource">The name of the current compilation or rather the 'source output' (e.g. used as the hintName for the <see cref="SourceProductionContext"/>)</param>
        /// <param name="targetNamespace" example="Basilisque.CodeAnalysis">The containing namespace of all child syntax nodes (when the <see cref="CompilationInfo{TCompilationInfo}.TargetNamespace"/> is null, empty or whitespace, all children are in the global namespace)</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="compilationName"/> is null, empty or whitespace</exception>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public CompilationInfo(
            string compilationName,
            string? targetNamespace
            )
            : base(compilationName, targetNamespace, null, null, System.Reflection.Assembly.GetCallingAssembly().GetName())
        { }

        /// <summary>
        /// Creates a new <see cref="CompilationInfo"/> for the specified language
        /// </summary>
        /// <param name="compilationName" example="CodeAnalysis_GeneratedSource">The name of the current compilation or rather the 'source output' (e.g. used as the hintName for the <see cref="SourceProductionContext"/>)</param>
        /// <param name="targetNamespace" example="Basilisque.CodeAnalysis">The containing namespace of all child syntax nodes (when the <see cref="CompilationInfo{TCompilationInfo}.TargetNamespace"/> is null, empty or whitespace, all children are in the global namespace)</param>
        /// <param name="generatedCodeToolName">The name of the tool that generated the source. (used for the <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/>)</param>
        /// <param name="generatedCodeToolVersion">The version of the tool that generated the source. (used for the <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/>)</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="compilationName"/> is null, empty or whitespace</exception>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public CompilationInfo(
            string compilationName,
            string? targetNamespace,
            string generatedCodeToolName,
            string generatedCodeToolVersion
            )
            : base(compilationName, targetNamespace, generatedCodeToolName, generatedCodeToolVersion, System.Reflection.Assembly.GetCallingAssembly().GetName())
        { }

        internal CompilationInfo(
            string compilationName,
            string? targetNamespace,
            string? generatedCodeToolName,
            string? generatedCodeToolVersion,
            System.Reflection.AssemblyName constructingAssemblyName
            )
            : base(compilationName,
                  targetNamespace,
                  generatedCodeToolName,
                  generatedCodeToolVersion,
                  constructingAssemblyName)
        { }
    }

    /// <summary>
    /// Represents a single source output for source generation
    /// (e.g. to be used as source input for a <see cref="SourceProductionContext"/>)
    /// </summary>
    public class CompilationInfo<TCompilationInfo> : SyntaxNode
        where TCompilationInfo : CompilationInfo<TCompilationInfo>
    {
        private System.Reflection.AssemblyName _constructingAssemblyName;
        private string _compilationName;
        private string? _targetNamespace;
        private bool _writeFileScopedNamespace = false;
        private string? _generatedCodeToolName;
        private string? _generatedCodeToolVersion;
        private List<ClassInfo>? _classes;
        private List<string>? _usings = null;

        /// <summary>
        /// The name of the current compilation or rather the 'source output'
        /// (e.g. used as the hintName for the <see cref="SourceProductionContext"/>)
        /// </summary>
        public string CompilationName
        {
            get
            {
                return _compilationName;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(CompilationName));

                _compilationName = value;
            }
        }

        /// <summary>
        /// The containing namespace of all child syntax nodes
        /// (when the <see cref="TargetNamespace"/> is null, empty or whitespace, all children are in the global namespace)
        /// </summary>
        public string? TargetNamespace
        {
            get
            {
                return _targetNamespace;
            }
            set
            {
                _targetNamespace = value;
            }
        }

        /// <summary>
        /// Determines if the namespace will be written as file scoped namespace or not
        /// </summary>
        public bool WriteFileScopedNamespace
        {
            get
            {
                return _writeFileScopedNamespace;
            }
            set
            {
                _writeFileScopedNamespace = value;
            }
        }

        /// <summary>
        /// Defines if the generated source contains attributes to mark it as generated code
        /// </summary>
        public bool AddGeneratedCodeAttributes { get; set; } = true;

        /// <summary>
        /// The name of the tool that generated the source.
        /// (used for the <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/>)
        /// </summary>
        public string? GeneratedCodeToolName
        {
            get
            {
                return _generatedCodeToolName;
            }
            set
            {
                _generatedCodeToolName = value;
            }
        }

        /// <summary>
        /// The version of the tool that generated the source.
        /// (used for the <see cref="System.CodeDom.Compiler.GeneratedCodeAttribute"/>)
        /// </summary>
        public string? GeneratedCodeToolVersion
        {
            get
            {
                return _generatedCodeToolVersion;
            }
            set
            {
                _generatedCodeToolVersion = value;
            }
        }

        /// <summary>
        /// Defines if a nullable context is explicitly enabled for the generated code.
        /// (When set to true, the generated code is enclosed in #nullable enable/restore)
        /// </summary>
        public bool EnableNullableContext { get; set; } = true;

        /// <summary>
        /// A list of namespaces that will be used by the current compilation
        /// </summary>
        public List<string> Usings
        {
            get
            {
                if (_usings == null)
                    _usings = new List<string>();

                return _usings;
            }
        }

        /// <summary>
        /// Determines if the current compilation contains any usings or not
        /// </summary>
        public bool HasUsings { get { return _usings?.Count > 0; } }

        /// <summary>
        /// A list of classes that are contained in the given <see cref="TargetNamespace"/>
        /// </summary>
        public List<ClassInfo> Classes
        {
            get
            {
                if (_classes == null)
                    _classes = new List<ClassInfo>();

                return _classes;
            }
        }

        internal CompilationInfo(
            string compilationName,
            string? targetNamespace,
            string? generatedCodeToolName,
            string? generatedCodeToolVersion,
            System.Reflection.AssemblyName constructingAssemblyName
            )
        {
            if (string.IsNullOrWhiteSpace(compilationName))
                throw new ArgumentNullException(nameof(compilationName));

            _compilationName = compilationName;
            _targetNamespace = targetNamespace;
            _generatedCodeToolName = generatedCodeToolName;
            _generatedCodeToolVersion = generatedCodeToolVersion;
            _constructingAssemblyName = constructingAssemblyName;
        }

        /// <summary>
        /// Creates a new <see cref="ClassInfo"/> with <see cref="ClassInfo.AccessModifier"/> set to <see cref="AccessModifier.Public"/> and adds it to the list of <see cref="Classes"/>
        /// </summary>
        /// <param name="className" example="MyClass">The name of the class</param>
        /// <param name="classConfiguration">A callback that is executed after creating the <see cref="ClassInfo"/> and that can be used to configure the created object</param>
        /// <returns>Returns the current <see cref="CompilationInfo"/> to enable use of fluent syntax</returns>
        public TCompilationInfo AddNewClassInfo(string className, Action<ClassInfo> classConfiguration)
        {
            return AddNewClassInfo(className, AccessModifier.Public, classConfiguration);
        }

        /// <summary>
        /// Creates a new <see cref="ClassInfo"/> and adds it to the list of <see cref="Classes"/>
        /// </summary>
        /// <param name="className" example="MyClass">The name of the class</param>
        /// <param name="accessModifier" example="AccessModifier.Public">The access modifier that specifies the accessibility of the class</param>
        /// <param name="classConfiguration">A callback that is executed after creating the <see cref="ClassInfo"/> and that can be used to configure the created object</param>
        /// <returns>Returns the current <see cref="CompilationInfo"/> to enable use of fluent syntax</returns>
        public TCompilationInfo AddNewClassInfo(string className, AccessModifier accessModifier, Action<ClassInfo> classConfiguration)
        {
            var classInfo = new ClassInfo(className, accessModifier, GeneratedCodeToolName, GeneratedCodeToolVersion, _constructingAssemblyName);

            classInfo.AddGeneratedCodeAttributes = this.AddGeneratedCodeAttributes;

            classConfiguration(classInfo);

            Classes.Add(classInfo);

            return (TCompilationInfo)this;
        }

        /// <summary>
        /// Converts the current <see cref="CompilationInfo"/> to its string representation and adds it to the given <see cref="SourceProductionContext"/>
        /// </summary>
        /// <param name="context">The <see cref="SourceProductionContext"/> where the <see cref="SourceProductionContext.AddSource(string, string)"/> method is called on</param>
        /// <param name="language">The <see cref="Language"/> that is used to generate the source code</param>
        /// <returns>Returns the current <see cref="CompilationInfo"/> to enable use of fluent syntax</returns>
        public TCompilationInfo AddToSourceProductionContext(SourceProductionContext context, Language language)
        {
            var compilationName = GetHintName(language);

            context.AddSource(compilationName, ToString(language));

            return (TCompilationInfo)this;
        }

        /// <summary>
        /// Returns the hint name that is used by <see cref="AddToSourceProductionContext"/> to add the <see cref="CompilationInfo"/> to a <see cref="SourceProductionContext"/>
        /// </summary>
        /// <param name="language">The <see cref="Language"/> that is used to generate the source code</param>
        /// <returns>The hint name as string</returns>
        public string GetHintName(Language language)
        {
            return GetHintName(_compilationName, language);
        }

        /// <summary>
        /// Returns the hint name that can be used to add source text to a <see cref="SourceProductionContext"/>.
        /// Supplements the provided <paramref name="compilationName"/> with a matching file ending.
        /// </summary>
        /// <param name="compilationName">The name of the compilation that is supplemented with a matching file ending</param>
        /// <param name="language">The <see cref="Language"/> that is used to generate the source code</param>
        /// <returns>The hint name as string</returns>
        public static string GetHintName(string compilationName, Language language)
        {
            string fileExt;
            switch (language)
            {
                case Language.VisualBasic:
                    fileExt = ".vb";
                    break;
                case Language.CSharp:
                default:
                    fileExt = ".cs";
                    break;
            }

            if (string.IsNullOrWhiteSpace(compilationName))
                compilationName = Guid.NewGuid().ToString("n");

            if (!compilationName.EndsWith(fileExt))
            {
                if (compilationName.EndsWith(".g"))
                    compilationName = compilationName + fileExt;
                else
                    compilationName = compilationName + ".g" + fileExt;
            }

            return compilationName;
        }

        /// <summary>
        /// Appends the current <see cref="CompilationInfo"/> and its children as C# code to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> that the <see cref="CompilationInfo"/> is added to</param>
        /// <param name="indentLvl">The count of indentation levels the <see cref="CompilationInfo"/> should be indented by</param>
        /// <param name="childIndentLvl">The count of indentation levels the direct children of this <see cref="CompilationInfo"/> should be indented by</param>
        /// <param name="indentCharCnt">The count of indentation characters for the current <see cref="CompilationInfo"/> (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the current level)</param>
        /// <param name="childIndentCharCnt">The count of indentation characters for the direct childre of this <see cref="CompilationInfo"/> (how many times should the <see cref="SyntaxNode.IndentationCharacter"/> be repeated for the direct child level)</param>
        protected override void ToCSharpString(StringBuilder sb, int indentLvl, int childIndentLvl, int indentCharCnt, int childIndentCharCnt)
        {
            var hasNamespace = !string.IsNullOrWhiteSpace(_targetNamespace);
            var hasClasses = _classes?.Any() == true;

            var hasAnythingToGenerate = hasNamespace || hasClasses;

            if (!hasAnythingToGenerate)
                return;

            if (AddGeneratedCodeAttributes)
            {
                var generatedCodeToolName = GeneratedCodeToolName ?? _constructingAssemblyName.Name;
                var generatedCodeToolVersion = GeneratedCodeToolVersion ?? _constructingAssemblyName.Version.ToString();

                sb.AppendLine("//------------------------------------------------------------------------");
                sb.AppendLine("// <auto-generated>");
                sb.AppendLine("//   This code was generated by a tool.");
                sb.Append("//   ");
                sb.Append(generatedCodeToolName);
                sb.Append(", ");
                sb.AppendLine(generatedCodeToolVersion);
                sb.AppendLine("//   ");
                sb.AppendLine("//   Changes to this file may cause incorrect behavior and will be lost if");
                sb.AppendLine("//   the code is regenerated.");
                sb.AppendLine("// </auto-generated>");
                sb.AppendLine("//------------------------------------------------------------------------");
                sb.AppendLine();
            }

            if (EnableNullableContext)
            {
                sb.AppendLine("#nullable enable");
                sb.AppendLine();
            }

            if (HasUsings)
            {
                var addedUsings = false;

                foreach (var ns in Usings)
                {
                    if (string.IsNullOrWhiteSpace(ns))
                        continue;

                    if (!ns.StartsWith("using "))
                        sb.Append("using ");

                    sb.Append(ns);

                    if (!ns.EndsWith(";"))
                        sb.Append(";");

                    sb.AppendLine();

                    addedUsings = true;
                }

                if (addedUsings)
                    sb.AppendLine();
            }

            if (hasNamespace)
            {
                if (_writeFileScopedNamespace)
                {
                    sb.Append("namespace ");
                    sb.Append(_targetNamespace);
                    sb.Append(";");
                }
                else
                {
                    sb.Append("namespace ");
                    sb.AppendLine(_targetNamespace);
                    sb.AppendLine("{");
                }
            }

            bool addEmptyLineForNextClass = _writeFileScopedNamespace;
            if (_classes != null)
            {
                var indentCls = hasNamespace && !_writeFileScopedNamespace ? childIndentLvl : indentLvl;

                foreach (var classInfo in _classes)
                {
                    if (addEmptyLineForNextClass)
                    {
                        sb.AppendLine();
                        AppendIntentationLine(sb, indentCharCnt);
                    }
                    else
                        addEmptyLineForNextClass = true;

                    classInfo.ToString(sb, indentCls, Language.CSharp);
                }
            }

            if (hasNamespace && !_writeFileScopedNamespace)
            {
                if (addEmptyLineForNextClass)
                    sb.AppendLine();
                sb.Append("}");
            }

            if (EnableNullableContext)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append("#nullable restore");
            }
        }
    }
}
