using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dlubal.WS.Tools.WsClientProxyGenerator
{
    /// <summary>
    /// V teto tride se provadeji vsechny upravy SOAP WS klienta.
    /// </summary>
    public sealed class ClientRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // Hleda tridu obsahujici implementaci SOAP WS klienta.
            if (node.Identifier.Text != Program.ClientName)
            {
                return null;
            }

            return base.VisitClassDeclaration(node);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            node = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node);

            if (Program.TypeToFixDictionary.Count == 0)
            {
                return node;
            }

            // Hleda metody, ktere je potreba upravit.
            if (Program.TypeToFixDictionary.ContainsKey(node.Identifier.Text))
            {
                List<string> parameterNames = Program.TypeToFixDictionary[node.Identifier.Text];
                node = FixMethodParameters(node, parameterNames);
            }
            else if (Program.MethodToFixList.Contains(node.Identifier.Text))
            {
                node = FixMethodReturnType(node);
            }

            return node;
        }

        private static MethodDeclarationSyntax FixMethodReturnType(MethodDeclarationSyntax node)
        {
            if (node.ReturnType.IsKind(SyntaxKind.PredefinedType))
            {
                var tokens = node.ReturnType.DescendantTokens();

                if ((tokens?.Count() > 0) && tokens.First().IsKind(SyntaxKind.StringKeyword))
                {
                    var newArrayType = ArrayType(
                        PredefinedType(Token(SyntaxKind.IntKeyword))
                    )
                    .WithRankSpecifiers(
                        SingletonList(
                            ArrayRankSpecifier(
                                SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression())
                            )
                            .WithCloseBracketToken(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.CloseBracketToken,
                                    TriviaList(Space)
                                )
                            )
                        )
                    );

                    node = node.ReplaceNode(node.ReturnType, newArrayType);
                }
            }

            return node;
        }

        private static MethodDeclarationSyntax FixMethodParameters(MethodDeclarationSyntax node, List<string> paramaterNames)
        {
            if ((paramaterNames == null) || (paramaterNames.Count == 0))
            {
                return node;
            }

            foreach (string parameterName in paramaterNames)
            {
                // Hleda argumenty metody, ktere je potreba upravit.
                var typeNodes = from parameter in node.ParameterList.Parameters
                                where (parameter.Identifier.Text == parameterName) &&
                                       parameter.Type.IsKind(SyntaxKind.PredefinedType)
                                select parameter.Type;

                if (typeNodes?.Count() > 0)
                {
                    // Overuje puvodni typ argumentu metody.
                    var stringToken = from token in typeNodes.First().ChildTokens()
                                      where token.IsKind(SyntaxKind.StringKeyword)
                                      select token;

                    if (stringToken?.Count() > 0)
                    {
                        var newArrayType = ArrayType(
                            PredefinedType(Token(SyntaxKind.IntKeyword))
                        )
                        .WithRankSpecifiers(
                            SingletonList(
                                ArrayRankSpecifier(
                                    SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression())
                                )
                                .WithCloseBracketToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.CloseBracketToken,
                                        TriviaList(Space)
                                    )
                                )
                            )
                        );

                        node = node.ReplaceNode(typeNodes.First(), newArrayType);
                    }
                }
            }

            return node;
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            // Timto zpusobem odfiltrujeme vsechny definice rozhrani.
            return null;
        }

        public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            // Timto zpusobem odfiltrujeme vsechny definice vyctovych typu.
            return null;
        }
    }
}