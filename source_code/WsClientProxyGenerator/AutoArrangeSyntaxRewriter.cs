using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dlubal.WS.Tools.WsClientProxyGenerator
{
    /// <summary>
    /// Tato trida provadi prerovnani zdrojoveho kodu podle nazvu entit.
    /// </summary>
    public sealed class AutoArrangeSyntaxRewriter : CSharpSyntaxRewriter
    {
        private readonly List<SyntaxNode> nodes;
        private int count;

        /// <summary>
        /// Metoda pro porovnani deklaraci metod - pouziva se pri trideni obsahu tridy podle nazvu.
        /// </summary>
        /// <param name="method1">Reference na prvni instanci MethodDeclarationSyntax, ktera ma byt porovnana.</param>
        /// <param name="method2">Reference na druhou instanci MethodDeclarationSyntax, ktera ma byt porovnana.</param>
        /// <returns>Vysledek porovnani (-1 = mensi, 0 = rovno, 1 = vetsi).</returns>
        public static int CompareMethodDeclarations(MethodDeclarationSyntax method1, MethodDeclarationSyntax method2)
        {
            int result = 0;

            // Metody maji stejny nazev -> porovnaji se navratove hodnoty.
            if (method1.Identifier.ValueText == method2.Identifier.ValueText)
            {
                // Detekce preddefinovanych typu a poli v navratove hodnote metody
                // -> tyto metody budou zarazeny az za temi s uzivatelsky definovanym typem.
                if (method1.ReturnType.IsKind(SyntaxKind.PredefinedType) || method1.ReturnType.IsKind(SyntaxKind.ArrayType))
                {
                    // Detekce void jako navratove hodnoty metody -> metoda s void bude jako posledni.
                    if (method1.ReturnType.GetFirstToken().IsKind(SyntaxKind.VoidKeyword))
                    {
                        result = 1;
                    }
                    else
                    {
                        // Obe metody maji preddefinovany typ v navratove hodnote.
                        if (method2.ReturnType.IsKind(SyntaxKind.PredefinedType) || method2.ReturnType.IsKind(SyntaxKind.ArrayType))
                        {
                            // Metoda s void bude jako posledni.
                            if (method2.ReturnType.GetFirstToken().IsKind(SyntaxKind.VoidKeyword))
                            {
                                result = -1;
                            }
                            else
                            {
                                // Obe metody maji preddefinovany typ nebo pole (ne void) v navratove hodnote
                                // -> vyhodnoti se poradi podle typu navratovych hodnot.
                                result = method1.ReturnType.ToString().CompareTo(method2.ReturnType.ToString());
                            }
                        }
                        else
                        {
                            // Metoda s preddefinovanym typem nebo polem v navratove hodnote bude zarazena az za metody s uzivatelsky definovanym typem.
                            result = 1;
                        }
                    }
                }
                else if (method2.ReturnType.IsKind(SyntaxKind.PredefinedType) || method2.ReturnType.IsKind(SyntaxKind.ArrayType))
                {
                    // Metoda s uzivatelsky definovanym typem v navratove hodnote ma prednost pred metodami s preddefinovanym typem nebo polem.
                    result = -1;
                }
                else
                {
                    // Obe metody maji uzivatelsky definovany typ v navratove hodnote
                    // -> vyhodnoti se poradi podle typu navratovych hodnot.
                    result = method1.ReturnType.ToString().CompareTo(method2.ReturnType.ToString());
                }
            }
            else
            {
                // Metody maji rozdilne nazvy -> vyhodnoti se jen poradi podle nazvu.
                result = method1.Identifier.ValueText.CompareTo(method2.Identifier.ValueText);
            }

            return result;
        }

        public AutoArrangeSyntaxRewriter(AutoArrangeSyntaxWalker syntaxWalker)
        {
            // Zmapovane entity se nejprve setridi.
            syntaxWalker.Types.Sort((a, b) => a.Target.Identifier.ValueText.CompareTo(b.Target.Identifier.ValueText));
            syntaxWalker.Enums.Sort((a, b) => a.Identifier.ValueText.CompareTo(b.Identifier.ValueText));
            syntaxWalker.Events.Sort((a, b) => a.Identifier.ValueText.CompareTo(b.Identifier.ValueText));
            syntaxWalker.Constructors.Sort((a, b) => a.Identifier.ValueText.CompareTo(b.Identifier.ValueText));
            syntaxWalker.Methods.Sort((a, b) => CompareMethodDeclarations(a, b));

            // Ze setridenych entit se posklada novy serazeny seznam.
            nodes = new List<SyntaxNode>();
            nodes.AddRange(syntaxWalker.Enums);
            nodes.AddRange(syntaxWalker.Events);
            nodes.AddRange(syntaxWalker.Fields);
            nodes.AddRange(syntaxWalker.Properties);
            nodes.AddRange(syntaxWalker.Constructors);
            nodes.AddRange(syntaxWalker.Methods);

            nodes.AddRange(
                from typeRewriter in syntaxWalker.Types
                select new AutoArrangeSyntaxRewriter(typeRewriter).VisitTypeDeclaration(typeRewriter.Target) as TypeDeclarationSyntax);
        }

        private SyntaxNode Replace(SyntaxNode node)
        {
            SyntaxNode result = null;

            if (count < nodes.Count)
            {
                result = nodes[count];
                count++;
            }
            else
            {
                throw new NotSupportedException();
            }

            return result;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            return Replace(node);
        }

        public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
        {
            return Replace(node);
        }

        public TypeDeclarationSyntax VisitTypeDeclaration(TypeDeclarationSyntax node)
        {
            if (node is ClassDeclarationSyntax)
            {
                return base.VisitClassDeclaration(node as ClassDeclarationSyntax) as ClassDeclarationSyntax;
            }
            else
            {
                return base.VisitStructDeclaration(node as StructDeclarationSyntax) as StructDeclarationSyntax;
            }
        }

        public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            return Replace(node);
        }

        public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node)
        {
            return Replace(node);
        }

        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            return Replace(node);
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            return Replace(node);
        }

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            return Replace(node);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            return Replace(node);
        }
    }
}
