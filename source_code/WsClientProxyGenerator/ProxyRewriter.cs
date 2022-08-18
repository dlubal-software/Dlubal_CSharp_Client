using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dlubal.WS.Tools.WsClientProxyGenerator
{
    /// <summary>
    /// V teto tride se provadeji vsechny upravy proxy objektu SOAP WS rozhrani.
    /// </summary>
    public sealed class ProxyRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            return base.VisitEnumDeclaration(node);
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // Odfiltruje tridu obsahujici implementaci SOAP WS klienta.
            if (node.Identifier.Text == Program.ClientName)
            {
                return null;
            }

            node = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);

            // Odfiltruje pouze tridy, ktere potrebujeme upravit.
            List<string> fieldNames = null;
            if (!Program.TypeToFixDictionary.ContainsKey(node.Identifier.Text))
            {
                return node;
            }
            else
            {
                fieldNames = Program.TypeToFixDictionary[node.Identifier.Text];
            }

            if ((fieldNames == null) || (fieldNames.Count == 0))
            {
                return node;
            }

            foreach (string fieldName in fieldNames)
            {
                string newPropertyName = fieldName + "_string";

                // Testuje, jestli uz vlastnost s danym nazvem neexistuje.
                var resultProperties = from property in node.DescendantNodes().OfType<PropertyDeclarationSyntax>()
                                       where property.Identifier.Text == fieldName ||
                                             property.Identifier.Text == newPropertyName
                                       select property;

                if ((resultProperties == null) || (resultProperties.Count() == 0))
                {
                    // Vytvori novou vlastnost.
                    PropertyDeclarationSyntax newProperty = CreateProperty(fieldName, newPropertyName);

                    var resultField = from field in node.DescendantNodes().OfType<FieldDeclarationSyntax>()
                                      from variable in field.Declaration.Variables
                                      where variable.Identifier.Text == newPropertyName ||
                                            variable.Identifier.Text == fieldName
                                      select field;

                    // Zaradi vlastnost za datovy clen.
                    if (resultField?.Count() > 0)
                    {
                        node = node.InsertNodesAfter(
                            resultField.First(),
                            new List<SyntaxNode>
                            {
                                newProperty
                            });
                    }
                    else
                    {
                        // Zaradi vlastnost pred konstruktor.
                        var resultConstructor = from constructor in node.DescendantNodes().OfType<ConstructorDeclarationSyntax>()
                                                where constructor.Identifier.Text == node.Identifier.Text
                                                select constructor;

                        if (resultConstructor?.Count() > 0)
                        {
                            node = node.InsertNodesBefore(
                                resultConstructor.First(),
                                new List<SyntaxNode>
                                {
                                    newProperty
                                });
                        }
                    }
                }
                else
                {
                    // Otestuje datovy typ vlastnosti.
                    if (resultProperties.First().Type.IsKind(SyntaxKind.PredefinedType))
                    {
                        var stringToken = from token in resultProperties.First().Type.ChildTokens()
                                          where token.IsKind(SyntaxKind.StringKeyword)
                                          select token;

                        // Vlastnost je typu string.
                        if (stringToken?.Count() > 0)
                        {
                            // Vytvori novou vlastnost typu int[].
                            string newFieldName = fieldName + "Field";
                            PropertyDeclarationSyntax newProperty = CreatePropertyOfTypeIntArray(fieldName, newFieldName);

                            var resultField = from field in node.DescendantNodes().OfType<FieldDeclarationSyntax>()
                                              from variable in field.Declaration.Variables
                                              where variable.Identifier.Text == newFieldName ||
                                                    variable.Identifier.Text == fieldName
                                              select field;

                            // Zaradi novou vlastnost za puvodni vlastnost.
                            node = node.InsertNodesAfter(
                                resultProperties.First(),
                                new List<SyntaxNode>
                                {
                                    newProperty
                                });
                        }
                    }
                }
            }

            return node;
        }

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            node = (ConstructorDeclarationSyntax)base.VisitConstructorDeclaration(node);

            // Odfiltruje konstruktory trid, ktere je potreba upravit.
            List<string> parameterNames = null;
            if (!Program.TypeToFixDictionary.ContainsKey(node.Identifier.Text))
            {
                return node;
            }
            else
            {
                parameterNames = Program.TypeToFixDictionary[node.Identifier.Text];
            }

            if ((parameterNames == null) || (parameterNames.Count == 0))
            {
                return node;
            }

            foreach (string parameterName in parameterNames)
            {
                // Hleda argumenty konstruktoru, ktere je potreba upravit.
                var typeNodes = from parameter in node.ParameterList.Parameters
                                where (parameter.Identifier.Text == parameterName) &&
                                       parameter.Type.IsKind(SyntaxKind.PredefinedType)
                                select parameter.Type;

                if (typeNodes?.Count() > 0)
                {
                    // Overuje puvodni typ argumentu konstruktoru.
                    var stringToken = from token in typeNodes.First().ChildTokens()
                                      where token.IsKind(SyntaxKind.StringKeyword)
                                      select token;

                    // Zmeni typ argumentu.
                    if (stringToken?.Count() > 0)
                    {
                        ArrayTypeSyntax newArrayType = ArrayType(
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

        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            node = (FieldDeclarationSyntax)base.VisitFieldDeclaration(node);

            // Reseni problemu s typem array_of_int
            // Filtruje datove slozky podle nazvu tridy - chceme upravit jen data urcitych proxy objektu.
            List<string> fieldNames = null;
            ClassDeclarationSyntax parentClass = node.Parent as ClassDeclarationSyntax;

            if ((parentClass == null) || (!Program.TypeToFixDictionary.ContainsKey(parentClass.Identifier.Text)))
            {
                return node;
            }

            fieldNames = Program.TypeToFixDictionary[parentClass.Identifier.Text];

            // Testuje vyskyt datoveho clenu tridy podle jmena.
            var resultVariable = from variable in node.Declaration.Variables
                                 where fieldNames.Contains(variable.Identifier.Text)
                                 select variable;

            // Datovy clen s hledanym jmenem neexistuje, hleda se jeste datovy clen se stejnym jmenem a priponou "Field"
            if ((resultVariable == null) || (resultVariable.Count() == 0))
            {
                resultVariable = from variable in node.Declaration.Variables
                                 where variable.Identifier.Text.ToLower().EndsWith("field") &&
                                       fieldNames.Contains(variable.Identifier.Text.Substring(0, variable.Identifier.Text.Length - "Field".Length))
                                 select variable;

                // Zmeni typ datoveho clenu string -> int[].
                if ((resultVariable?.Count() > 0) && node.Declaration.Type.IsKind(SyntaxKind.PredefinedType))
                {
                    // Overuje puvodni typ datoveho clenu.
                    var stringToken = from token in node.Declaration.Type.ChildTokens()
                                      where token.IsKind(SyntaxKind.StringKeyword)
                                      select token;

                    // Zmeni typ datoveho clenu.
                    if (stringToken?.Count() > 0)
                    {
                        ArrayTypeSyntax newArrayType = ArrayType(
                            PredefinedType(
                                Token(SyntaxKind.IntKeyword)
                            )
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

                        node = node.ReplaceNode(node.Declaration.Type, newArrayType);
                    }
                }
            }
            else
            {
                // Datovy clen s hledanym jmenem jiz existuje.
                string fieldName = resultVariable.First().Identifier.Text;

                // Zmeni jmeno datove promenne - prida koncovku _string.
                VariableDeclaratorSyntax newVariable = VariableDeclarator(Identifier(fieldName + "_string"));

                node = node.ReplaceNode(resultVariable.First(), newVariable);

                // Jeste je potreba upravit XmlElementAttribute - pridat puvodni jmeno datove slozky pro spravnou serializaci.

                // Testuje jestli ma datovy clen prirazeny atribut XmlElementAttribute.
                var resultAttributes = from attributeList in node.AttributeLists.OfType<AttributeListSyntax>()
                                       from attributes in attributeList.Attributes.OfType<AttributeSyntax>()
                                       where attributes.Name.GetText().ToString().Contains("XmlElementAttribute")
                                       select attributes;

                // Atribut neni prirazen.
                if ((resultAttributes == null) || (resultAttributes.Count() == 0))
                {
                    return node;
                }

                // Prohleda argumenty, jestli uz pridavany argument neexistuje.
                var resultArguments = from argument in resultAttributes.First().ArgumentList.Arguments
                                      where argument.Expression.IsKind(SyntaxKind.StringLiteralExpression) &&
                                            argument.GetText().ToString() == fieldName
                                      select argument;

                if ((resultArguments == null) || (resultArguments.Count() == 0))
                {
                    // Vytvori novy seznam argumentu atributu XmlElementAttribute.
                    AttributeArgumentListSyntax newAtributeArgumentList = AttributeArgumentList(
                        SeparatedList<AttributeArgumentSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                AttributeArgument(
                                    LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal(fieldName)
                                    )
                                ),
                                Token(
                                    TriviaList(),
                                    SyntaxKind.CommaToken,
                                    TriviaList(Space)
                                )
                            }
                        )
                    );

                    // Nahradi seznam argumentu a atributu XmlElementAttribute.
                    newAtributeArgumentList = AttributeArgumentList(newAtributeArgumentList.Arguments.AddRange(resultAttributes.First().ArgumentList.Arguments));
                    AttributeSyntax newAttribute = resultAttributes.First().WithArgumentList(newAtributeArgumentList);

                    node = node.ReplaceNode(resultAttributes.First(), newAttribute);
                }
            }

            return node;
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            node = (PropertyDeclarationSyntax)base.VisitPropertyDeclaration(node);

            // Filtruje vlastnosti podle nazvu tridy - chceme upravit jen vlastnosti urcite tridy.
            List<string> propertyNames = null;
            ClassDeclarationSyntax parentClass = node.Parent as ClassDeclarationSyntax;

            if ((parentClass == null) || (!Program.TypeToFixDictionary.ContainsKey(parentClass.Identifier.Text)))
            {
                return node;
            }

            propertyNames = Program.TypeToFixDictionary[parentClass.Identifier.Text];

            if (!propertyNames.Contains(node.Identifier.Text) || !node.Type.IsKind(SyntaxKind.PredefinedType))
            {
                return node;
            }

            string oldPropertyName = node.Identifier.Text;
            string newPropertyName = oldPropertyName + "_string";

            // Overuje puvodni typ vlastnosti.
            var stringToken = from token in node.Type.ChildTokens()
                              where token.IsKind(SyntaxKind.StringKeyword)
                              select token;

            // Pokud je vlastnost typu string, tak se jeji pristupove bloky upravi, aby vracela zkonvertovanou hodnotu z typu int[] na string a
            // pri zapisu se naopak string prevedl na int[]. Zaroven se upravi atributy a nazev vlastnosti.
            if (stringToken?.Count() > 0)
            {
                node = UpdatePropertyOfTypeString(node, oldPropertyName, newPropertyName, oldPropertyName + "Field");
            }

            return node;
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            // Timto zpusobem odfiltrujeme vsechny definice rozhrani.
            return null;
        }

        private static PropertyDeclarationSyntax CreateProperty(string propertyName, string fieldName)
        {
            PropertyDeclarationSyntax newProperty = PropertyDeclaration(
                ArrayType(
                    PredefinedType(
                        Token(SyntaxKind.IntKeyword)
                    )
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
                ),
                Identifier(
                    TriviaList(),
                    propertyName,
                    TriviaList(CarriageReturn, LineFeed)
                )
            )
            .WithAttributeLists(
                SingletonList(
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
                                    IdentifierName("XmlIgnore")
                                )
                            )
                        )
                    )
                    .WithOpenBracketToken(
                        Token(
                            TriviaList(
                                new[]
                                {
                                    CarriageReturn,
                                    LineFeed,
                                    Whitespace("        ")
                                }
                            ),
                            SyntaxKind.OpenBracketToken,
                            TriviaList()
                        )
                    )
                    .WithCloseBracketToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.CloseBracketToken,
                            TriviaList(CarriageReturn, LineFeed)
                        )
                    )
                )
            )
            .WithModifiers(
                TokenList(
                    Token(
                        TriviaList(Whitespace("        ")),
                        SyntaxKind.PublicKeyword,
                        TriviaList(Space)
                    )
                )
            )
            .WithAccessorList(
                AccessorList(
                    List(
                        new AccessorDeclarationSyntax[]
                        {
                            AccessorDeclaration(
                                SyntaxKind.GetAccessorDeclaration,
                                Block(
                                    IfStatement(
                                        BinaryExpression(
                                            SyntaxKind.EqualsExpression,
                                            IdentifierName(
                                                Identifier(
                                                    TriviaList(),
                                                    fieldName,
                                                    TriviaList(Space)
                                                )
                                            ),
                                            LiteralExpression(SyntaxKind.NullLiteralExpression)
                                        )
                                        .WithOperatorToken(
                                            Token(
                                                TriviaList(),
                                                SyntaxKind.EqualsEqualsToken,
                                                TriviaList(Space)
                                            )
                                        ),
                                        ExpressionStatement(
                                            AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                IdentifierName(
                                                    Identifier(
                                                        TriviaList(Whitespace("                    ")),
                                                        fieldName,
                                                        TriviaList(Space)
                                                    )
                                                ),
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    PredefinedType(
                                                        Token(SyntaxKind.StringKeyword)
                                                    ),
                                                    IdentifierName("Empty")
                                                )
                                            )
                                            .WithOperatorToken(
                                                Token(
                                                    TriviaList(),
                                                    SyntaxKind.EqualsToken,
                                                    TriviaList(Space)
                                                )
                                            )
                                        )
                                        .WithSemicolonToken(
                                            Token(
                                                TriviaList(),
                                                SyntaxKind.SemicolonToken,
                                                TriviaList(CarriageReturn, LineFeed)
                                            )
                                        )
                                    )
                                    .WithIfKeyword(
                                        Token(
                                            TriviaList(Whitespace("                ")),
                                            SyntaxKind.IfKeyword,
                                            TriviaList(Space)
                                        )
                                    )
                                    .WithCloseParenToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.CloseParenToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    ),
                                    LocalDeclarationStatement(
                                        VariableDeclaration(
                                            QualifiedName(
                                                QualifiedName(
                                                    QualifiedName(
                                                        IdentifierName(
                                                            Identifier(
                                                                TriviaList(
                                                                    new[]
                                                                    {
                                                                        CarriageReturn,
                                                                        LineFeed,
                                                                        Whitespace("                ")
                                                                    }
                                                                ),
                                                                "System",
                                                                TriviaList()
                                                            )
                                                        ),
                                                        IdentifierName("Collections")
                                                    ),
                                                    IdentifierName("Generic")
                                                ),
                                                GenericName(
                                                    Identifier("List")
                                                )
                                                .WithTypeArgumentList(
                                                    TypeArgumentList(
                                                        SingletonSeparatedList<TypeSyntax>(
                                                            PredefinedType(
                                                                Token(SyntaxKind.IntKeyword)
                                                            )
                                                        )
                                                    )
                                                    .WithGreaterThanToken(
                                                        Token(
                                                            TriviaList(),
                                                            SyntaxKind.GreaterThanToken,
                                                            TriviaList(Space)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                        .WithVariables(
                                            SingletonSeparatedList(
                                                VariableDeclarator(
                                                    Identifier(
                                                        TriviaList(),
                                                        "items",
                                                        TriviaList(Space)
                                                    )
                                                )
                                                .WithInitializer(
                                                    EqualsValueClause(
                                                        ObjectCreationExpression(
                                                            QualifiedName(
                                                                QualifiedName(
                                                                    QualifiedName(
                                                                        IdentifierName("System"),
                                                                        IdentifierName("Collections")
                                                                    ),
                                                                    IdentifierName("Generic")
                                                                ),
                                                                GenericName(
                                                                    Identifier("List")
                                                                )
                                                                .WithTypeArgumentList(
                                                                    TypeArgumentList(
                                                                        SingletonSeparatedList<TypeSyntax>(
                                                                            PredefinedType(
                                                                                Token(SyntaxKind.IntKeyword)
                                                                            )
                                                                        )
                                                                    )
                                                                )
                                                            )
                                                        )
                                                        .WithNewKeyword(
                                                            Token(
                                                                TriviaList(),
                                                                SyntaxKind.NewKeyword,
                                                                TriviaList(Space)
                                                            )
                                                        )
                                                        .WithArgumentList(ArgumentList())
                                                    )
                                                    .WithEqualsToken(
                                                        Token(
                                                            TriviaList(),
                                                            SyntaxKind.EqualsToken,
                                                            TriviaList(Space)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                    .WithSemicolonToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.SemicolonToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    ),
                                    LocalDeclarationStatement(
                                        VariableDeclaration(
                                            ArrayType(
                                                PredefinedType(
                                                    Token(
                                                        TriviaList(
                                                            new[]
                                                            {
                                                                CarriageReturn,
                                                                LineFeed,
                                                                Whitespace("                ")
                                                            }
                                                        ),
                                                        SyntaxKind.StringKeyword,
                                                        TriviaList()
                                                    )
                                                )
                                            )
                                            .WithRankSpecifiers(
                                               SingletonList(
                                                    ArrayRankSpecifier(
                                                        SingletonSeparatedList<ExpressionSyntax>(
                                                            OmittedArraySizeExpression()
                                                        )
                                                    )
                                                    .WithCloseBracketToken(
                                                        Token(
                                                            TriviaList(),
                                                            SyntaxKind.CloseBracketToken,
                                                            TriviaList(Space)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                        .WithVariables(
                                            SingletonSeparatedList(
                                                VariableDeclarator(
                                                    Identifier(
                                                        TriviaList(),
                                                        "stringItems",
                                                        TriviaList(Space)
                                                    )
                                                )
                                                .WithInitializer(
                                                    EqualsValueClause(
                                                        InvocationExpression(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName(fieldName),
                                                                IdentifierName("Split")
                                                            )
                                                        )
                                                        .WithArgumentList(
                                                            ArgumentList(
                                                                SingletonSeparatedList(
                                                                    Argument(
                                                                        LiteralExpression(
                                                                            SyntaxKind.CharacterLiteralExpression,
                                                                            Literal(' ')
                                                                        )
                                                                    )
                                                                )
                                                            )
                                                        )
                                                    )
                                                    .WithEqualsToken(
                                                        Token(
                                                            TriviaList(),
                                                            SyntaxKind.EqualsToken,
                                                            TriviaList(Space)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                    .WithSemicolonToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.SemicolonToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    ),
                                   ForEachStatement(
                                        PredefinedType(
                                            Token(
                                                TriviaList(),
                                                SyntaxKind.StringKeyword,
                                                TriviaList(Space)
                                            )
                                        ),
                                        Identifier(
                                            TriviaList(),
                                            "stringItem",
                                            TriviaList(Space)
                                        ),
                                        IdentifierName("stringItems"),
                                        Block(
                                            LocalDeclarationStatement(
                                                VariableDeclaration(
                                                    PredefinedType(
                                                        Token(
                                                            TriviaList(Whitespace("                    ")),
                                                            SyntaxKind.IntKeyword,
                                                            TriviaList(Space)
                                                        )
                                                    )
                                                )
                                                .WithVariables(
                                                    SingletonSeparatedList(
                                                        VariableDeclarator(
                                                            Identifier("result")
                                                        )
                                                    )
                                                )
                                            )
                                            .WithSemicolonToken(
                                                Token(
                                                    TriviaList(),
                                                    SyntaxKind.SemicolonToken,
                                                    TriviaList(CarriageReturn, LineFeed)
                                                )
                                            ),
                                            IfStatement(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        PredefinedType(
                                                            Token(SyntaxKind.IntKeyword)
                                                        ),
                                                        IdentifierName("TryParse")
                                                    )
                                                )
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SeparatedList<ArgumentSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                Argument(
                                                                    IdentifierName("stringItem")
                                                                ),
                                                                Token(
                                                                    TriviaList(),
                                                                    SyntaxKind.CommaToken,
                                                                    TriviaList(Space)
                                                                ),
                                                                Argument(
                                                                    IdentifierName("result")
                                                                )
                                                                .WithRefOrOutKeyword(
                                                                    Token(
                                                                        TriviaList(),
                                                                        SyntaxKind.OutKeyword,
                                                                        TriviaList(Space)
                                                                    )
                                                                )
                                                            }
                                                        )
                                                    )
                                                ),
                                                Block(
                                                    SingletonList<StatementSyntax>(
                                                        ExpressionStatement(
                                                            InvocationExpression(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    IdentifierName(
                                                                        Identifier(
                                                                            TriviaList(Whitespace("                        ")),
                                                                            "items",
                                                                            TriviaList()
                                                                        )
                                                                    ),
                                                                    IdentifierName("Add")
                                                                )
                                                            )
                                                            .WithArgumentList(
                                                                ArgumentList(
                                                                    SingletonSeparatedList(
                                                                        Argument(
                                                                            IdentifierName("result")
                                                                        )
                                                                    )
                                                                )
                                                            )
                                                        )
                                                        .WithSemicolonToken(
                                                           Token(
                                                                TriviaList(),
                                                                SyntaxKind.SemicolonToken,
                                                                TriviaList(CarriageReturn, LineFeed)
                                                            )
                                                        )
                                                    )
                                                )
                                                .WithOpenBraceToken(
                                                    Token(
                                                        TriviaList(Whitespace("                    ")),
                                                        SyntaxKind.OpenBraceToken,
                                                        TriviaList(CarriageReturn, LineFeed)
                                                    )
                                                )
                                                .WithCloseBraceToken(
                                                    Token(
                                                        TriviaList(Whitespace("                    ")),
                                                        SyntaxKind.CloseBraceToken,
                                                        TriviaList(CarriageReturn, LineFeed)
                                                    )
                                                )
                                            )
                                            .WithIfKeyword(
                                                Token(
                                                    TriviaList(Whitespace("                    ")),
                                                    SyntaxKind.IfKeyword,
                                                    TriviaList(Space)
                                                )
                                            )
                                            .WithCloseParenToken(
                                                Token(
                                                    TriviaList(),
                                                    SyntaxKind.CloseParenToken,
                                                    TriviaList(CarriageReturn, LineFeed)
                                                )
                                            )
                                        )
                                        .WithOpenBraceToken(
                                            Token(
                                                TriviaList(Whitespace("                ")),
                                                SyntaxKind.OpenBraceToken,
                                                TriviaList(CarriageReturn, LineFeed)
                                            )
                                        )
                                        .WithCloseBraceToken(
                                            Token(
                                                TriviaList(Whitespace("                ")),
                                                SyntaxKind.CloseBraceToken,
                                                TriviaList(CarriageReturn, LineFeed)
                                            )
                                        )
                                    )
                                    .WithForEachKeyword(
                                        Token(
                                            TriviaList(Whitespace("                ")),
                                            SyntaxKind.ForEachKeyword,
                                            TriviaList(Space)
                                        )
                                    )
                                    .WithInKeyword(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.InKeyword,
                                            TriviaList(Space)
                                        )
                                    )
                                    .WithCloseParenToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.CloseParenToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    ),
                                    ReturnStatement(
                                        InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("items"),
                                                IdentifierName("ToArray")
                                            )
                                        )
                                    )
                                    .WithReturnKeyword(
                                        Token(
                                            TriviaList(
                                                new[]
                                                {
                                                    CarriageReturn,
                                                    LineFeed,
                                                    Whitespace("                ")
                                                }
                                            ),
                                            SyntaxKind.ReturnKeyword,
                                            TriviaList(Space)
                                        )
                                    )
                                    .WithSemicolonToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.SemicolonToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    )
                                )
                                .WithOpenBraceToken(
                                    Token(
                                        TriviaList(Whitespace("            ")),
                                        SyntaxKind.OpenBraceToken,
                                        TriviaList(CarriageReturn, LineFeed)
                                    )
                                )
                                .WithCloseBraceToken(
                                    Token(
                                        TriviaList(Whitespace("            ")),
                                        SyntaxKind.CloseBraceToken,
                                        TriviaList(CarriageReturn, LineFeed)
                                    )
                                )
                            )
                            .WithKeyword(
                                Token(
                                    TriviaList(Whitespace("            ")),
                                    SyntaxKind.GetKeyword,
                                    TriviaList(CarriageReturn, LineFeed)
                                )
                            ),
                            AccessorDeclaration(
                                SyntaxKind.SetAccessorDeclaration,
                                Block(
                                    SingletonList<StatementSyntax>(
                                        IfStatement(
                                            BinaryExpression(
                                                SyntaxKind.GreaterThanExpression,
                                                ConditionalAccessExpression(
                                                    IdentifierName("value"),
                                                    MemberBindingExpression(
                                                        IdentifierName(
                                                            Identifier(
                                                                TriviaList(),
                                                                "Length",
                                                                TriviaList(Space)
                                                            )
                                                        )
                                                    )
                                                ),
                                                LiteralExpression(
                                                    SyntaxKind.NumericLiteralExpression,
                                                    Literal(0)
                                                )
                                            )
                                            .WithOperatorToken(
                                                Token(
                                                    TriviaList(),
                                                    SyntaxKind.GreaterThanToken,
                                                    TriviaList(Space)
                                                )
                                            ),
                                            Block(
                                                SingletonList<StatementSyntax>(
                                                    ExpressionStatement(
                                                        AssignmentExpression(
                                                            SyntaxKind.SimpleAssignmentExpression,
                                                            IdentifierName(
                                                                Identifier(
                                                                    TriviaList(Whitespace("                    ")),
                                                                    fieldName,
                                                                    TriviaList(Space)
                                                                )
                                                            ),
                                                            InvocationExpression(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    PredefinedType(
                                                                        Token(SyntaxKind.StringKeyword)
                                                                    ),
                                                                    IdentifierName("Join")
                                                                )
                                                            )
                                                            .WithArgumentList(
                                                                ArgumentList(
                                                                    SeparatedList<ArgumentSyntax>(
                                                                        new SyntaxNodeOrToken[]
                                                                        {
                                                                            Argument(
                                                                                LiteralExpression(
                                                                                    SyntaxKind.StringLiteralExpression,
                                                                                    Literal(" ")
                                                                                )
                                                                            ),
                                                                            Token(
                                                                                TriviaList(),
                                                                                SyntaxKind.CommaToken,
                                                                                TriviaList(Space)
                                                                            ),
                                                                            Argument(
                                                                                IdentifierName("value")
                                                                            )
                                                                        }
                                                                    )
                                                                )
                                                            )
                                                        )
                                                        .WithOperatorToken(
                                                            Token(
                                                                TriviaList(),
                                                                SyntaxKind.EqualsToken,
                                                                TriviaList(Space)
                                                            )
                                                        )
                                                    )
                                                    .WithSemicolonToken(
                                                        Token(
                                                            TriviaList(),
                                                            SyntaxKind.SemicolonToken,
                                                            TriviaList(CarriageReturn, LineFeed)
                                                        )
                                                    )
                                                )
                                            )
                                            .WithOpenBraceToken(
                                                Token(
                                                    TriviaList(Whitespace("                ")),
                                                    SyntaxKind.OpenBraceToken,
                                                    TriviaList(CarriageReturn, LineFeed)
                                                )
                                            )
                                            .WithCloseBraceToken(
                                                Token(
                                                    TriviaList(Whitespace("                ")),
                                                    SyntaxKind.CloseBraceToken,
                                                    TriviaList(CarriageReturn, LineFeed)
                                                )
                                            )
                                        )
                                        .WithIfKeyword(
                                            Token(
                                                TriviaList(Whitespace("                ")),
                                                SyntaxKind.IfKeyword,
                                                TriviaList(Space)
                                            )
                                        )
                                        .WithCloseParenToken(
                                            Token(
                                                TriviaList(),
                                                SyntaxKind.CloseParenToken,
                                                TriviaList(CarriageReturn, LineFeed)
                                            )
                                        )
                                        .WithElse(
                                            ElseClause(
                                                Block(
                                                    SingletonList<StatementSyntax>(
                                                        ExpressionStatement(
                                                            AssignmentExpression(
                                                                SyntaxKind.SimpleAssignmentExpression,
                                                                IdentifierName(
                                                                    Identifier(
                                                                        TriviaList(Whitespace("                    ")),
                                                                        fieldName,
                                                                        TriviaList(Space)
                                                                    )
                                                                ),
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    PredefinedType(
                                                                        Token(SyntaxKind.StringKeyword)
                                                                    ),
                                                                    IdentifierName("Empty")
                                                                )
                                                            )
                                                            .WithOperatorToken(
                                                                Token(
                                                                    TriviaList(),
                                                                    SyntaxKind.EqualsToken,
                                                                    TriviaList(Space)
                                                                )
                                                            )
                                                        )
                                                        .WithSemicolonToken(
                                                            Token(
                                                                TriviaList(),
                                                                SyntaxKind.SemicolonToken,
                                                                TriviaList(CarriageReturn, LineFeed)
                                                            )
                                                        )
                                                    )
                                                )
                                                .WithOpenBraceToken(
                                                    Token(
                                                        TriviaList(Whitespace("                ")),
                                                        SyntaxKind.OpenBraceToken,
                                                        TriviaList(CarriageReturn, LineFeed)
                                                    )
                                                )
                                                .WithCloseBraceToken(
                                                    Token(
                                                        TriviaList(Whitespace("                ")),
                                                        SyntaxKind.CloseBraceToken,
                                                        TriviaList(CarriageReturn, LineFeed)
                                                    )
                                                )
                                            )
                                            .WithElseKeyword(
                                                Token(
                                                    TriviaList(Whitespace("                ")),
                                                    SyntaxKind.ElseKeyword,
                                                    TriviaList(CarriageReturn, LineFeed)
                                                )
                                            )
                                        )
                                    )
                                )
                                .WithOpenBraceToken(
                                    Token(
                                        TriviaList(Whitespace("            ")),
                                        SyntaxKind.OpenBraceToken,
                                        TriviaList(CarriageReturn, LineFeed)
                                    )
                                )
                                .WithCloseBraceToken(
                                    Token(
                                        TriviaList(Whitespace("            ")),
                                        SyntaxKind.CloseBraceToken,
                                        TriviaList(CarriageReturn, LineFeed)
                                    )
                                )
                            )
                            .WithKeyword(
                                Token(
                                    TriviaList(Whitespace("            ")),
                                    SyntaxKind.SetKeyword,
                                    TriviaList(CarriageReturn, LineFeed)
                                )
                            )
                        }
                    )
                )
                .WithOpenBraceToken(
                    Token(
                        TriviaList(Whitespace("        ")),
                        SyntaxKind.OpenBraceToken,
                        TriviaList(CarriageReturn, LineFeed)
                    )
                )
                .WithCloseBraceToken(
                    Token(
                        TriviaList(Whitespace("        ")),
                        SyntaxKind.CloseBraceToken,
                        TriviaList(CarriageReturn, LineFeed)
                    )
                )
            );

            return newProperty;
        }

        private static PropertyDeclarationSyntax CreatePropertyOfTypeIntArray(string propertyName, string fieldName)
        {
            PropertyDeclarationSyntax newProperty = PropertyDeclaration(
                ArrayType(
                    PredefinedType(
                        Token(SyntaxKind.IntKeyword)
                    )
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
                ),
                Identifier(
                    TriviaList(),
                    propertyName,
                    TriviaList(Space)
                )
            )
            .WithAttributeLists(
                SingletonList(
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
                                    IdentifierName("XmlIgnore")
                                )
                            )
                        )
                    )
                    .WithOpenBracketToken(
                        Token(
                            TriviaList(
                                new[]
                                {
                                    Whitespace("        "),
                                    CarriageReturn,
                                    LineFeed,
                                    Whitespace("        "),
                                    Trivia(
                                        DocumentationCommentTrivia(
                                            SyntaxKind.SingleLineDocumentationCommentTrivia,
                                            List(
                                                new XmlNodeSyntax[]
                                                {
                                                    XmlText().WithTextTokens(
                                                        TokenList(
                                                            XmlTextLiteral(
                                                                TriviaList(DocumentationCommentExterior("///")),
                                                                " ",
                                                                " ",
                                                                TriviaList()
                                                            )
                                                        )
                                                    ),
                                                    XmlEmptyElement(
                                                        XmlName(
                                                            Identifier("remarks")
                                                        )
                                                    ),
                                                    XmlText().WithTextTokens(
                                                        TokenList(
                                                            XmlTextNewLine(
                                                                TriviaList(),
                                                                Environment.NewLine,
                                                                Environment.NewLine,
                                                                TriviaList()
                                                            )
                                                        )
                                                    )
                                                }
                                            )
                                        )
                                    ),
                                    Whitespace("        ")
                                }
                            ),
                            SyntaxKind.OpenBracketToken,
                            TriviaList()
                        )
                    )
                    .WithCloseBracketToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.CloseBracketToken,
                            TriviaList(CarriageReturn, LineFeed)
                        )
                    )
                )
            )
            .WithModifiers(
                TokenList(
                    Token(
                        TriviaList(Whitespace("        ")),
                        SyntaxKind.PublicKeyword,
                        TriviaList(Space)
                    )
                )
            )
            .WithAccessorList(
                AccessorList(
                    List(
                        new AccessorDeclarationSyntax[]
                        {
                            AccessorDeclaration(
                                SyntaxKind.GetAccessorDeclaration,
                                Block(
                                    SingletonList<StatementSyntax>(
                                        ReturnStatement(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                ThisExpression(),
                                                IdentifierName(fieldName)
                                            )
                                        )
                                        .WithReturnKeyword(
                                            Token(
                                                TriviaList(Whitespace("                ")),
                                                SyntaxKind.ReturnKeyword,
                                                TriviaList(Space)
                                            )
                                        )
                                        .WithSemicolonToken(
                                            Token(
                                                TriviaList(),
                                                SyntaxKind.SemicolonToken,
                                                TriviaList(CarriageReturn, LineFeed)
                                            )
                                        )
                                    )
                                )
                                .WithOpenBraceToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.OpenBraceToken,
                                        TriviaList(CarriageReturn, LineFeed)
                                    )
                                )
                                .WithCloseBraceToken(
                                    Token(
                                        TriviaList(Whitespace("            ")),
                                        SyntaxKind.CloseBraceToken,
                                        TriviaList(CarriageReturn, LineFeed)
                                    )
                                )
                            )
                            .WithKeyword(
                                Token(
                                    TriviaList(Whitespace("            ")),
                                    SyntaxKind.GetKeyword,
                                    TriviaList(Space)
                                )
                            ),
                            AccessorDeclaration(
                                SyntaxKind.SetAccessorDeclaration,
                                Block(
                                    ExpressionStatement(
                                        AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                ThisExpression().WithToken(
                                                    Token(
                                                        TriviaList(Whitespace("                ")),
                                                        SyntaxKind.ThisKeyword,
                                                        TriviaList()
                                                    )
                                                ),
                                                IdentifierName(
                                                    Identifier(
                                                        TriviaList(),
                                                        fieldName,
                                                        TriviaList(Space)
                                                    )
                                                )
                                            ),
                                            IdentifierName("value")
                                        )
                                        .WithOperatorToken(
                                            Token(
                                                TriviaList(),
                                                SyntaxKind.EqualsToken,
                                                TriviaList(Space)
                                            )
                                        )
                                    )
                                    .WithSemicolonToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.SemicolonToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    ),
                                    ExpressionStatement(
                                        InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                ThisExpression().WithToken(
                                                    Token(
                                                        TriviaList(Whitespace("                ")),
                                                        SyntaxKind.ThisKeyword,
                                                        TriviaList()
                                                    )
                                                ),
                                                IdentifierName("RaisePropertyChanged")
                                            )
                                        )
                                        .WithArgumentList(
                                            ArgumentList(
                                                SingletonSeparatedList(
                                                    Argument(
                                                        LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            Literal(propertyName)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                    .WithSemicolonToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.SemicolonToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    )
                                )
                                .WithOpenBraceToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.OpenBraceToken,
                                        TriviaList(CarriageReturn, LineFeed)
                                    )
                                )
                                .WithCloseBraceToken(
                                    Token(
                                        TriviaList(Whitespace("            ")),
                                        SyntaxKind.CloseBraceToken,
                                        TriviaList(CarriageReturn, LineFeed)
                                    )
                                )
                            )
                            .WithKeyword(
                                Token(
                                    TriviaList(Whitespace("            ")),
                                    SyntaxKind.SetKeyword,
                                    TriviaList(Space)
                                )
                            )
                        }
                    )
                )
                .WithOpenBraceToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.OpenBraceToken,
                        TriviaList(CarriageReturn, LineFeed)
                    )
                )
                .WithCloseBraceToken(
                    Token(
                        TriviaList(Whitespace("        ")),
                        SyntaxKind.CloseBraceToken,
                        TriviaList(CarriageReturn, LineFeed)
                    )
                )
            );

            return newProperty;
        }

        private static PropertyDeclarationSyntax UpdatePropertyOfTypeString(PropertyDeclarationSyntax node, string oldPropertyName, string newPropertyName, string fieldName)
        {
            PropertyDeclarationSyntax updatedProperty = node;

            // Testuje jestli ma vlastnost prirazeny atribut XmlElementAttribute.
            var resultAttributes = from attributeList in node.AttributeLists.OfType<AttributeListSyntax>()
                                   from attributes in attributeList.Attributes.OfType<AttributeSyntax>()
                                   where attributes.Name.GetText().ToString().Contains("XmlElementAttribute")
                                   select attributes;

            // Atribut neni prirazen.
            if ((resultAttributes == null) || (resultAttributes.Count() == 0))
            {
                return updatedProperty;
            }

            // Prohleda argumenty, jestli uz pridavany argument neexistuje.
            var resultArguments = from argument in resultAttributes.First().ArgumentList.Arguments
                                  where argument.Expression.IsKind(SyntaxKind.StringLiteralExpression) &&
                                        argument.GetText().ToString() == oldPropertyName
                                  select argument;

            if ((resultArguments == null) || (resultArguments.Count() == 0))
            {
                // Vytvori novy seznam argumentu atributu XmlElementAttribute.
                AttributeArgumentListSyntax newAtributeArgumentList = AttributeArgumentList(
                    SeparatedList<AttributeArgumentSyntax>(
                        new SyntaxNodeOrToken[]
                        {
                                AttributeArgument(
                                    LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal(oldPropertyName)
                                    )
                                ),
                                Token(
                                    TriviaList(),
                                    SyntaxKind.CommaToken,
                                    TriviaList(Space)
                                )
                        }
                    )
                );

                // Nahradi seznam argumentu a atributu XmlElementAttribute.
                newAtributeArgumentList = AttributeArgumentList(newAtributeArgumentList.Arguments.AddRange(resultAttributes.First().ArgumentList.Arguments));
                AttributeSyntax newAttribute = resultAttributes.First().WithArgumentList(newAtributeArgumentList);
                updatedProperty = node.ReplaceNode(resultAttributes.First(), newAttribute);
            }

            // Zmeni jmeno puvodni vlastnosti, aby nekolidovalo s novou vlastnosti, ktera je typu int[].
            updatedProperty = updatedProperty.WithIdentifier(
                Identifier(
                    TriviaList(),
                    newPropertyName,
                    TriviaList(Space)
                )
            );

            // Zmeni obsah pristupovych modifikatoru vlastnosti.
            updatedProperty = updatedProperty.WithAccessorList(
                AccessorList(
                    List(
                        new AccessorDeclarationSyntax[]
                        {
                            AccessorDeclaration(
                                SyntaxKind.GetAccessorDeclaration,
                                Block(
                                    IfStatement(
                                        BinaryExpression(
                                            SyntaxKind.GreaterThanExpression,
                                            ConditionalAccessExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    ThisExpression(),
                                                    IdentifierName(fieldName)
                                                ),
                                                MemberBindingExpression(
                                                    IdentifierName(
                                                        Identifier(
                                                            TriviaList(),
                                                            "Length",
                                                            TriviaList(Space)
                                                        )
                                                    )
                                                )
                                            ),
                                            LiteralExpression(
                                                SyntaxKind.NumericLiteralExpression,
                                                Literal(0)
                                            )
                                        )
                                        .WithOperatorToken(
                                            Token(
                                                TriviaList(),
                                                SyntaxKind.GreaterThanToken,
                                                TriviaList(Space)
                                            )
                                        ),
                                        Block(
                                            SingletonList<StatementSyntax>(
                                                ReturnStatement(
                                                    InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            PredefinedType(
                                                                Token(SyntaxKind.StringKeyword)
                                                            ),
                                                            IdentifierName("Join")
                                                        )
                                                    )
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SeparatedList<ArgumentSyntax>(
                                                                new SyntaxNodeOrToken[]
                                                                {
                                                                    Argument(
                                                                        LiteralExpression(
                                                                            SyntaxKind.StringLiteralExpression,
                                                                            Literal(" ")
                                                                        )
                                                                    ),
                                                                    Token(
                                                                        TriviaList(),
                                                                        SyntaxKind.CommaToken,
                                                                        TriviaList(Space)
                                                                    ),
                                                                    Argument(
                                                                        MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            ThisExpression(),
                                                                            IdentifierName(fieldName)
                                                                        )
                                                                    )
                                                                }
                                                            )
                                                        )
                                                    )
                                                )
                                                .WithReturnKeyword(
                                                    Token(
                                                        TriviaList(Whitespace("                    ")),
                                                        SyntaxKind.ReturnKeyword,
                                                        TriviaList(Space)
                                                    )
                                                )
                                                .WithSemicolonToken(
                                                    Token(
                                                        TriviaList(),
                                                        SyntaxKind.SemicolonToken,
                                                        TriviaList(CarriageReturn, LineFeed)
                                                    )
                                                )
                                            )
                                        )
                                        .WithOpenBraceToken(
                                            Token(
                                                TriviaList(Whitespace("                ")),
                                                SyntaxKind.OpenBraceToken,
                                                TriviaList(CarriageReturn, LineFeed)
                                            )
                                        )
                                        .WithCloseBraceToken(
                                            Token(
                                                TriviaList(Whitespace("                ")),
                                                SyntaxKind.CloseBraceToken,
                                                TriviaList(CarriageReturn, LineFeed)
                                            )
                                        )
                                    )
                                    .WithIfKeyword(
                                        Token(
                                            TriviaList(Whitespace("                ")),
                                            SyntaxKind.IfKeyword,
                                            TriviaList(Space)
                                        )
                                    )
                                    .WithCloseParenToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.CloseParenToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    )
                                    .WithElse(
                                        ElseClause(
                                            Block(
                                                SingletonList<StatementSyntax>(
                                                    ReturnStatement(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            PredefinedType(
                                                                Token(SyntaxKind.StringKeyword)
                                                            ),
                                                            IdentifierName("Empty")
                                                        )
                                                    )
                                                    .WithReturnKeyword(
                                                        Token(
                                                            TriviaList(Whitespace("                    ")),
                                                            SyntaxKind.ReturnKeyword,
                                                            TriviaList(Space)
                                                        )
                                                    )
                                                    .WithSemicolonToken(
                                                        Token(
                                                            TriviaList(),
                                                            SyntaxKind.SemicolonToken,
                                                            TriviaList(CarriageReturn, LineFeed)
                                                        )
                                                    )
                                                )
                                            )
                                            .WithOpenBraceToken(
                                                Token(
                                                    TriviaList(Whitespace("                ")),
                                                    SyntaxKind.OpenBraceToken,
                                                    TriviaList(CarriageReturn, LineFeed)
                                                )
                                            )
                                            .WithCloseBraceToken(
                                                Token(
                                                    TriviaList(Whitespace("                ")),
                                                    SyntaxKind.CloseBraceToken,
                                                    TriviaList(CarriageReturn, LineFeed)
                                                )
                                            )
                                        )
                                        .WithElseKeyword(
                                            Token(
                                                TriviaList(Whitespace("                ")),
                                                SyntaxKind.ElseKeyword,
                                                TriviaList(CarriageReturn, LineFeed)
                                            )
                                        )
                                    )
                                )
                                .WithOpenBraceToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.OpenBraceToken,
                                        TriviaList(CarriageReturn, LineFeed)
                                    )
                                )
                                .WithCloseBraceToken(
                                    Token(
                                        TriviaList(Whitespace("            ")),
                                        SyntaxKind.CloseBraceToken,
                                        TriviaList(CarriageReturn, LineFeed)
                                    )
                                )
                            )
                            .WithKeyword(
                                Token(
                                    TriviaList(Whitespace("            ")),
                                    SyntaxKind.GetKeyword,
                                    TriviaList(Space)
                                )
                            ),
                            AccessorDeclaration(
                                SyntaxKind.SetAccessorDeclaration,
                                Block(
                                    IfStatement(
                                        BinaryExpression(
                                            SyntaxKind.EqualsExpression,
                                            IdentifierName(
                                                Identifier(
                                                    TriviaList(),
                                                    "value",
                                                    TriviaList(Space))
                                                ),
                                                LiteralExpression(SyntaxKind.NullLiteralExpression)
                                            )
                                            .WithOperatorToken(
                                                Token(
                                                    TriviaList(),
                                                    SyntaxKind.EqualsEqualsToken,
                                                    TriviaList(Space)
                                                )
                                            ),
                                            Block(
                                                SingletonList<StatementSyntax>(
                                                    ExpressionStatement(
                                                        AssignmentExpression(
                                                            SyntaxKind.SimpleAssignmentExpression,
                                                            IdentifierName(
                                                                Identifier(
                                                                    TriviaList(Whitespace("                    ")),
                                                                    "value",
                                                                    TriviaList(Space)
                                                                )
                                                            ),
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                PredefinedType(
                                                                    Token(SyntaxKind.StringKeyword)
                                                                ),
                                                                IdentifierName("Empty")
                                                            )
                                                        )
                                                        .WithOperatorToken(
                                                            Token(
                                                                TriviaList(),
                                                                SyntaxKind.EqualsToken,
                                                                TriviaList(Space)
                                                            )
                                                        )
                                                    )
                                                    .WithSemicolonToken(
                                                        Token(
                                                            TriviaList(),
                                                            SyntaxKind.SemicolonToken,
                                                            TriviaList(CarriageReturn, LineFeed)
                                                        )
                                                    )
                                                )
                                            )
                                            .WithOpenBraceToken(
                                                Token(
                                                    TriviaList(Whitespace("                ")),
                                                    SyntaxKind.OpenBraceToken,
                                                    TriviaList(CarriageReturn, LineFeed)
                                                )
                                            )
                                            .WithCloseBraceToken(
                                                Token(
                                                    TriviaList(Whitespace("                ")),
                                                SyntaxKind.CloseBraceToken,
                                                TriviaList(CarriageReturn, LineFeed)
                                            )
                                        )
                                    )
                                    .WithIfKeyword(
                                        Token(
                                            TriviaList(Whitespace("                ")),
                                            SyntaxKind.IfKeyword,
                                            TriviaList(Space)
                                        )
                                    )
                                    .WithCloseParenToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.CloseParenToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    ),
                                    LocalDeclarationStatement(
                                        VariableDeclaration(
                                            QualifiedName(
                                                QualifiedName(
                                                    QualifiedName(
                                                        IdentifierName(
                                                            Identifier(
                                                                TriviaList(
                                                                    new[]
                                                                    {
                                                                        Whitespace("				"),
                                                                        CarriageReturn,
                                                                        LineFeed,
                                                                        Whitespace("                ")
                                                                    }
                                                                ),
                                                                "System",
                                                                TriviaList()
                                                            )
                                                        ),
                                                        IdentifierName("Collections")
                                                    ),
                                                    IdentifierName("Generic")
                                                ),
                                                GenericName(
                                                    Identifier("List")
                                                )
                                                .WithTypeArgumentList(
                                                    TypeArgumentList(
                                                        SingletonSeparatedList<TypeSyntax>(
                                                            PredefinedType(Token(SyntaxKind.IntKeyword))
                                                        )
                                                    )
                                                    .WithGreaterThanToken(
                                                        Token(
                                                            TriviaList(),
                                                            SyntaxKind.GreaterThanToken,
                                                            TriviaList(Space)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                        .WithVariables(
                                            SingletonSeparatedList(
                                                VariableDeclarator(
                                                    Identifier(
                                                        TriviaList(),
                                                        "items",
                                                        TriviaList(Space)
                                                    )
                                                )
                                                .WithInitializer(
                                                    EqualsValueClause(
                                                        ObjectCreationExpression(
                                                            QualifiedName(
                                                                QualifiedName(
                                                                    QualifiedName(
                                                                        IdentifierName("System"),
                                                                        IdentifierName("Collections")
                                                                    ),
                                                                    IdentifierName("Generic")
                                                                ),
                                                                GenericName(
                                                                    Identifier("List")
                                                                )
                                                                .WithTypeArgumentList(
                                                                    TypeArgumentList(
                                                                        SingletonSeparatedList<TypeSyntax>(
                                                                            PredefinedType(
                                                                                Token(SyntaxKind.IntKeyword)
                                                                            )
                                                                        )
                                                                    )
                                                                )
                                                            )
                                                        )
                                                        .WithNewKeyword(
                                                            Token(
                                                                TriviaList(),
                                                                SyntaxKind.NewKeyword,
                                                                TriviaList(Space)
                                                            )
                                                        )
                                                        .WithArgumentList(ArgumentList())
                                                    )
                                                    .WithEqualsToken(
                                                        Token(
                                                            TriviaList(),
                                                            SyntaxKind.EqualsToken,
                                                            TriviaList(Space)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                    .WithSemicolonToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.SemicolonToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    ),
                                    LocalDeclarationStatement(
                                        VariableDeclaration(
                                            ArrayType(
                                                PredefinedType(
                                                    Token(
                                                        TriviaList(
                                                            new[]
                                                            {
                                                                CarriageReturn,
                                                                LineFeed,
                                                                Whitespace("                ")
                                                            }
                                                        ),
                                                        SyntaxKind.StringKeyword,
                                                        TriviaList()
                                                    )
                                                )
                                            )
                                            .WithRankSpecifiers(
                                                SingletonList(
                                                    ArrayRankSpecifier(
                                                        SingletonSeparatedList<ExpressionSyntax>(
                                                            OmittedArraySizeExpression()
                                                        )
                                                    )
                                                    .WithCloseBracketToken(
                                                        Token(
                                                            TriviaList(),
                                                            SyntaxKind.CloseBracketToken,
                                                            TriviaList(Space)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                        .WithVariables(
                                            SingletonSeparatedList(
                                                VariableDeclarator(
                                                    Identifier(
                                                        TriviaList(),
                                                        "stringItems",
                                                        TriviaList(Space)
                                                    )
                                                )
                                                .WithInitializer(
                                                    EqualsValueClause(
                                                        InvocationExpression(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName("value"),
                                                                IdentifierName("Split")
                                                            )
                                                        )
                                                        .WithArgumentList(
                                                            ArgumentList(
                                                                SingletonSeparatedList(
                                                                    Argument(
                                                                        LiteralExpression(
                                                                            SyntaxKind.CharacterLiteralExpression,
                                                                            Literal(' ')
                                                                        )
                                                                    )
                                                                )
                                                            )
                                                        )
                                                    )
                                                    .WithEqualsToken(
                                                        Token(
                                                            TriviaList(),
                                                            SyntaxKind.EqualsToken,
                                                            TriviaList(Space)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                    .WithSemicolonToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.SemicolonToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    ),
                                    ForEachStatement(
                                        PredefinedType(
                                            Token(
                                                TriviaList(),
                                                SyntaxKind.StringKeyword,
                                                TriviaList(Space)
                                            )
                                        ),
                                        Identifier(
                                            TriviaList(),
                                            "stringItem",
                                            TriviaList(Space)
                                        ),
                                        IdentifierName("stringItems"),
                                        Block(
                                            LocalDeclarationStatement(
                                                VariableDeclaration(
                                                    PredefinedType(
                                                        Token(
                                                            TriviaList(Whitespace("                    ")),
                                                            SyntaxKind.IntKeyword,
                                                            TriviaList(Space)
                                                        )
                                                    )
                                                )
                                                .WithVariables(
                                                    SingletonSeparatedList(
                                                        VariableDeclarator(
                                                            Identifier("result")
                                                        )
                                                    )
                                                )
                                            )
                                            .WithSemicolonToken(
                                                Token(
                                                    TriviaList(),
                                                    SyntaxKind.SemicolonToken,
                                                    TriviaList(CarriageReturn, LineFeed)
                                                )
                                            ),
                                            IfStatement(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        PredefinedType(
                                                            Token(SyntaxKind.IntKeyword)
                                                        ),
                                                        IdentifierName("TryParse")
                                                    )
                                                )
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SeparatedList<ArgumentSyntax>(
                                                            new SyntaxNodeOrToken[]
                                                            {
                                                                Argument(
                                                                    IdentifierName("stringItem")
                                                                ),
                                                                Token(
                                                                    TriviaList(),
                                                                    SyntaxKind.CommaToken,
                                                                    TriviaList(Space)
                                                                ),
                                                                Argument(
                                                                    IdentifierName("result")
                                                                )
                                                                .WithRefOrOutKeyword(
                                                                    Token(
                                                                        TriviaList(),
                                                                        SyntaxKind.OutKeyword,
                                                                        TriviaList(Space)
                                                                    )
                                                                )
                                                            }
                                                        )
                                                    )
                                                ),
                                                Block(
                                                    SingletonList<StatementSyntax>(
                                                        ExpressionStatement(
                                                            InvocationExpression(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    IdentifierName(
                                                                        Identifier(
                                                                            TriviaList(Whitespace("                        ")),
                                                                            "items",
                                                                            TriviaList()
                                                                        )
                                                                    ),
                                                                    IdentifierName("Add")
                                                                )
                                                            )
                                                            .WithArgumentList(
                                                                ArgumentList(
                                                                    SingletonSeparatedList(
                                                                        Argument(
                                                                            IdentifierName("result")
                                                                        )
                                                                    )
                                                                )
                                                            )
                                                        )
                                                        .WithSemicolonToken(
                                                            Token(
                                                                TriviaList(),
                                                                SyntaxKind.SemicolonToken,
                                                                TriviaList(CarriageReturn, LineFeed)
                                                            )
                                                        )
                                                    )
                                                )
                                                .WithOpenBraceToken(
                                                    Token(
                                                        TriviaList(Whitespace("                    ")),
                                                        SyntaxKind.OpenBraceToken,
                                                        TriviaList(CarriageReturn, LineFeed)
                                                    )
                                                )
                                                .WithCloseBraceToken(
                                                    Token(
                                                        TriviaList(Whitespace("                    ")),
                                                        SyntaxKind.CloseBraceToken,
                                                        TriviaList(CarriageReturn, LineFeed)
                                                    )
                                                )
                                            )
                                            .WithIfKeyword(
                                                Token(
                                                    TriviaList(Whitespace("                    ")),
                                                    SyntaxKind.IfKeyword,
                                                    TriviaList(Space)
                                                )
                                            )
                                            .WithCloseParenToken(
                                                Token(
                                                    TriviaList(),
                                                    SyntaxKind.CloseParenToken,
                                                    TriviaList(CarriageReturn, LineFeed)
                                                )
                                            )
                                        )
                                        .WithOpenBraceToken(
                                            Token(
                                                TriviaList(Whitespace("                ")),
                                                SyntaxKind.OpenBraceToken,
                                                TriviaList(CarriageReturn, LineFeed)
                                            )
                                        )
                                        .WithCloseBraceToken(
                                            Token(
                                                TriviaList(Whitespace("                ")),
                                                SyntaxKind.CloseBraceToken,
                                                TriviaList(CarriageReturn, LineFeed)
                                            )
                                        )
                                    )
                                    .WithForEachKeyword(
                                        Token(
                                            TriviaList(Whitespace("                ")),
                                            SyntaxKind.ForEachKeyword,
                                            TriviaList(Space)
                                        )
                                    )
                                    .WithInKeyword(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.InKeyword,
                                            TriviaList(Space)
                                        )
                                    )
                                    .WithCloseParenToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.CloseParenToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    ),
                                    ExpressionStatement(
                                        AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                ThisExpression().WithToken(
                                                    Token(
                                                        TriviaList(
                                                            new[]
                                                            {
                                                                Whitespace("                "),
                                                                CarriageReturn,
                                                                LineFeed,
                                                                Whitespace("                ")
                                                            }
                                                        ),
                                                        SyntaxKind.ThisKeyword,
                                                        TriviaList()
                                                    )
                                                ),
                                                IdentifierName(
                                                    Identifier(
                                                        TriviaList(),
                                                        fieldName,
                                                        TriviaList(Space)
                                                    )
                                                )
                                            ),
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("items"),
                                                    IdentifierName("ToArray")
                                                )
                                            )
                                        )
                                        .WithOperatorToken(
                                            Token(
                                                TriviaList(),
                                                SyntaxKind.EqualsToken,
                                                TriviaList(Space)
                                            )
                                        )
                                    )
                                    .WithSemicolonToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.SemicolonToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    ),
                                    ExpressionStatement(
                                        InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                ThisExpression().WithToken(
                                                    Token(
                                                        TriviaList(Whitespace("                ")),
                                                        SyntaxKind.ThisKeyword,
                                                        TriviaList()
                                                    )
                                                ),
                                                IdentifierName("RaisePropertyChanged")
                                            )
                                        )
                                        .WithArgumentList(
                                            ArgumentList(
                                                SingletonSeparatedList(
                                                    Argument(
                                                        LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            Literal(oldPropertyName)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                    .WithSemicolonToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.SemicolonToken,
                                            TriviaList(CarriageReturn, LineFeed)
                                        )
                                    )
                                )
                                .WithOpenBraceToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.OpenBraceToken,
                                        TriviaList(CarriageReturn, LineFeed)
                                    )
                                )
                                .WithCloseBraceToken(
                                    Token(
                                        TriviaList(Whitespace("            ")),
                                        SyntaxKind.CloseBraceToken,
                                        TriviaList(CarriageReturn, LineFeed)
                                    )
                                )
                            )
                            .WithKeyword(
                                Token(
                                    TriviaList(Whitespace("            ")),
                                    SyntaxKind.SetKeyword,
                                    TriviaList(Space)
                                )
                            )
                        }
                    )
                )
                .WithOpenBraceToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.OpenBraceToken,
                        TriviaList(CarriageReturn, LineFeed)
                    )
                )
                .WithCloseBraceToken(
                    Token(
                        TriviaList(Whitespace("        ")),
                        SyntaxKind.CloseBraceToken,
                        TriviaList(CarriageReturn, LineFeed)
                    )
                )
            );

            return updatedProperty;
        }
    }
}