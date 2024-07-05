using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace BlittableCheckerGenerator {
    public static class ITypeSymbolExtension {
        public static bool IsBlittableType(this ITypeSymbol type, out string reason) {
            var visited = new HashSet<string>();
            return IsBlittableType(type, out reason, visited);
        }

        private static bool IsBlittableType(this ITypeSymbol type, out string reason, HashSet<string> visited) {
            reason = string.Empty;
            
            if(!visited.Add(type.ToDisplayString()))
                return true;

            switch (type.SpecialType) {
                case SpecialType.System_Boolean:
                case SpecialType.System_Byte:
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_Char:
                    return true;
            }
            
            if(type.TypeKind == TypeKind.Enum)
                return true;

            if (type.TypeKind == TypeKind.Struct) {
                foreach (var member in type.GetMembers().OfType<IFieldSymbol>()) {
                    if (member.IsFixedSizeBuffer) {
                        continue;
                    }

                    if (member.Type is IPointerTypeSymbol) {
                        reason = $"Field '{member.Name}' is a pointer";
                        return false;
                    }

                    if (member.Type.IsBlittableType(out reason, visited))
                        continue;
                    
                    reason = $"Field '{member.Name}' is not blittable. - {reason}";
                    return false;
                }

                return true;
            }

            reason = $"Type '{type.Name}' is not blittable";
            return false;
        }
    }
}