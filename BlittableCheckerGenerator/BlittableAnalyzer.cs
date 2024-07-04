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
                select Diagnostic.Create(
                    new(
                        "BLT001",
                        "Non-blittable struct",
                        $"Struct '{structSymbol.Name}' is not blittable: {reason}",
                        "Blittable",
                        DiagnosticSeverity.Error,
                        isEnabledByDefault: true),
                    structDeclaration.Identifier.GetLocation()
                );
            
            foreach (var diagnostic in diagnostics) {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}