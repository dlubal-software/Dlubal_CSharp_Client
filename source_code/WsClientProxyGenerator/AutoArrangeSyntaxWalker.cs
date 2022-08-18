using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Dlubal.WS.Tools.WsClientProxyGenerator
{
    /// <summary>
    /// Tento objekt slouzi pouze ke zmapovani struktury zdrojoveho kodu a roztrideni jednotlivych entit.
    /// </summary>
    public sealed class AutoArrangeSyntaxWalker : CSharpSyntaxWalker
    {
        public TypeDeclarationSyntax Target { get; private set; }

        public List<AutoArrangeSyntaxWalker> Types { get; private set; }

        public List<EnumDeclarationSyntax> Enums { get; private set; }

        public List<EventDeclarationSyntax> Events { get; private set; }

        public List<FieldDeclarationSyntax> Fields { get; private set; }

        public List<PropertyDeclarationSyntax> Properties { get; private set; }

        public List<ConstructorDeclarationSyntax> Constructors { get; private set; }

        public List<MethodDeclarationSyntax> Methods { get; private set; }

        public AutoArrangeSyntaxWalker() : base()
        {
            // Inicializace listu pro jednotlive typy entit
            Types = new List<AutoArrangeSyntaxWalker>();
            Enums = new List<EnumDeclarationSyntax>();
            Events = new List<EventDeclarationSyntax>();
            Fields = new List<FieldDeclarationSyntax>();
            Properties = new List<PropertyDeclarationSyntax>();
            Constructors = new List<ConstructorDeclarationSyntax>();
            Methods = new List<MethodDeclarationSyntax>();
        }

        #region Mapovaci a tridici metody
        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            AutoArrangeSyntaxWalker syntaxWalker = new AutoArrangeSyntaxWalker();
            syntaxWalker.VisitTypeDeclaration(node);
            Types.Add(syntaxWalker);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            AutoArrangeSyntaxWalker syntaxWalker = new AutoArrangeSyntaxWalker();
            syntaxWalker.VisitTypeDeclaration(node);
            Types.Add(syntaxWalker);
        }

        public void VisitTypeDeclaration(TypeDeclarationSyntax node)
        {
            Target = node;

            if (node is ClassDeclarationSyntax)
            {
                base.VisitClassDeclaration(node as ClassDeclarationSyntax);
            }
            else
            {
                base.VisitStructDeclaration(node as StructDeclarationSyntax);
            }
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            Enums.Add(node);
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            Events.Add(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            Fields.Add(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            Properties.Add(node);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            Constructors.Add(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            Methods.Add(node);
        }
        #endregion // Mapovaci a tridici metody
    }
}
