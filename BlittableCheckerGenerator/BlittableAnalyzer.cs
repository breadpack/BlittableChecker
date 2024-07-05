using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlittableCheckerGenerator {
    public class SyntaxReceiver : ISyntaxReceiver {
        public List<StructDeclarationSyntax> CandidateStructs { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (syntaxNode is StructDeclarationSyntax structDeclaration && structDeclaration.AttributeLists.Count > 0) {
                CandidateStructs.Add(structDeclaration);
            }
        }
    }

    [Generator]
    public class BlittableAnalyzer : ISourceGenerator {
        public void Initialize(GeneratorInitializationContext context) {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context) {
            if (context.SyntaxReceiver is not SyntaxReceiver receiver)
                return;

            var compilation              = context.Compilation;
            var blittableAttributeSymbol = compilation.GetTypeByMetadataName("BlittableChecker.BlittableAttribute");

            var diagnostics =
                from structDeclaration in receiver.CandidateStructs
                let model = compilation.GetSemanticModel(structDeclaration.SyntaxTree)
                let structSymbol = model.GetDeclaredSymbol(structDeclaration) as INamedTypeSymbol
                where structSymbol.GetAttributes()
                                  .Any(ad => ad.AttributeClass.Equals(blittableAttributeSymbol, SymbolEqualityComparer.Default))
                let reason = structSymbol.IsBlittableType(out var _reason) ? string.Empty : _reason
                where !string.IsNullOrEmpty(reason)
                select new {
                    structSymbol,
                    reason,
                };

            // generate diagnostics and source code that has compile error for each struct because of unity engine can not handle diagnostics
            foreach (var info in diagnostics) {
                var structSymbol = info.structSymbol;
                var reason       = info.reason;

                var diagnosticDescriptor = new DiagnosticDescriptor(
                    id: "BLITTABLE001",
                    title: "BlittableChecker",
                    messageFormat: $"Struct '{structSymbol.Name}' is not blittable. - {reason}",
                    category: "BlittableChecker",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description: reason
                );

                var diagnostic = Diagnostic.Create(diagnosticDescriptor, structSymbol.Locations[0]);
                context.ReportDiagnostic(diagnostic);

//                 var structName = structSymbol.Name;
//                 var location   = structSymbol.Locations[0];
//                 var filePath   = location.GetLineSpan().Path.Replace("\\", "\\\\");
//                 var lineNumber = location.GetLineSpan().StartLinePosition.Line + 1;
//                 var source = $@"
// namespace Generated
// {{
//     public static class {structName}BlittableCheck
//     {{
//         public static void EnsureBlittable()
//         {{
//             // This will cause a compile-time error
//             int error = ""{structName} in {filePath} at line {lineNumber} contains non-blittable fields"";
//         }}
//     }}
// }}
// ";
//                 context.AddSource($"{structName}_BlittableCheck.cs", source);
            }
        }
    }
}