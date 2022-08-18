using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dlubal.WS.Tools.WsClientProxyGenerator
{
    /// <summary>
    /// V teto tride se provadeji vsechny upravy rozhrani SOAP WS.
    /// </summary>
    public sealed class InterfaceRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            node = (InterfaceDeclarationSyntax)base.VisitInterfaceDeclaration(node);

            // Hleda specificke rozhrani SOAP WS.
            if (node.Identifier.Text != Program.ServerInterfaceName)
            {
                return node;
            }

            // Testuje jestli ma interface prirazeny atribut XmlSerializerAssembly.
            var resultAttributeList = from attributeList in node.AttributeLists.OfType<AttributeListSyntax>()
                                      from attributes in attributeList.Attributes.OfType<AttributeSyntax>()
                                      where attributes.Name.GetText().ToString().Contains("XmlSerializerAssembly")
                                      select attributeList;

            // Atribut neni prirazen, je potreba ho vytvorit a pridat.
            if ((resultAttributeList == null) || (resultAttributeList.Count() == 0))
            {
                node = node.AddAttributeLists(
                    AttributeList(
                        SingletonSeparatedList(
                            Attribute(
                                QualifiedName(
                                    QualifiedName(
                                        QualifiedName(
                                            IdentifierName("System"),
                                            IdentifierName("Xml")
                                        ),
                                        IdentifierName("Serialization")
                                    ),
                                    IdentifierName("XmlSerializerAssembly")
                                )
                            )
                        )
                    )
                    .WithCloseBracketToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.CloseBracketToken,
                            TriviaList(CarriageReturn, LineFeed)
                        )
                    )
                    .WithOpenBracketToken(
                        Token(
                            TriviaList(Whitespace("    ")),
                            SyntaxKind.OpenBracketToken,
                            TriviaList()
                        )
                    )
                );
            }

            return node;
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            node = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node);

            if (node.HasLeadingTrivia)
            {
                // Odstrani z deklaraci metod prebytecny komentar zacinajici CODEGEN.
                SyntaxTriviaList leadingTrivia = node.GetLeadingTrivia();

                for (int i = 0; i < leadingTrivia.Count; i++)
                {
                    if ((leadingTrivia[i].Kind() == SyntaxKind.SingleLineCommentTrivia) && leadingTrivia[i].ToString().Contains("CODEGEN"))
                    {
                        SyntaxTriviaList newLeadingTrivia = leadingTrivia;

                        if (((i + 1) < newLeadingTrivia.Count) && (newLeadingTrivia[i + 1].Kind() == SyntaxKind.EndOfLineTrivia))
                        {
                            newLeadingTrivia = newLeadingTrivia.Remove(newLeadingTrivia[i + 1]);
                        }

                        newLeadingTrivia = newLeadingTrivia.Remove(newLeadingTrivia[i]);

                        if (((i - 1) >= 0) && (newLeadingTrivia[i - 1].Kind() == SyntaxKind.WhitespaceTrivia))
                        {
                            newLeadingTrivia = newLeadingTrivia.Remove(newLeadingTrivia[i - 1]);
                        }

                        node = node.WithLeadingTrivia(newLeadingTrivia);
                        break;
                    }
                }
            }

            return node;
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // Timto zpusobem odfiltrujeme vsechny definice trid.
            return null;
        }

        public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            // Timto zpusobem odfiltrujeme vsechny definice vyctovych typu.
            return null;
        }
    }
}