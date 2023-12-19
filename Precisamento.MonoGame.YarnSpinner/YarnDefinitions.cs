using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Yarn;
using Yarn.Compiler;

namespace Precisamento.MonoGame.YarnSpinner
{
    /// <summary>
    /// Maps to a the definitions file declared in a Yarn Project.
    /// Meant to be JSON deserialized.
    /// </summary>
    public class YarnDefinitions
    {
        /// <summary>
        /// List of commands to make available in Yarn scripts
        /// </summary>
        public List<Command> Commands = new();

        /// <summary>
        /// List of functions to make available in Yarn scripts
        /// </summary>
        public List<Function> Functions = new();

        public IEnumerable<Declaration> GetDeclarations()
        {
            var decls = new List<Declaration>();
            foreach(var command in Commands)
            {
                var type = new FunctionTypeBuilder();
                foreach(var param in command.Parameters)
                {
                    type.WithParameter(ParseType(param.Type));
                }

                var decl = new DeclarationBuilder()
                    .WithName(command.YarnName)
                    .WithType(type.FunctionType)
                    .WithDescription(command.Documentation)
                    .Declaration;

                decls.Add(decl);
            }

            foreach (var function in Functions)
            {
                var type = new FunctionTypeBuilder();
                foreach (var param in function.Parameters)
                {
                    type.WithParameter(ParseType(param.Type));
                }

                type.WithReturnType(ParseType(function.ReturnType));

                var decl = new DeclarationBuilder()
                    .WithName(function.YarnName)
                    .WithType(type.FunctionType)
                    .WithDescription(function.Documentation)
                    .Declaration;

                decls.Add(decl);
            }

            return decls;
        }

        public class Command
        {
            /// <summary>
            /// Name of this method in Yarn Spinner scripts
            /// </summary>
            public string YarnName { get; set; }

            /// <summary>
            /// Name of this method in code
            /// </summary>
            public string? DefinitionName { get; set; }

            /// <summary>
            /// Name of the file this method is defined in.
            /// <para>
            /// Primarily used when 'Deep Command Lookup' is disabled to make sure the source file is still found (doesn't need to be the full path, even 'foo.cs' is helpful).
            /// </para>
            /// </summary>
            public string? FileName { get; set; }

            /// <summary>
            /// Language id of the method definition.
            /// <para>
            /// Must be 'csharp' to override/merge with methods found in C# files.
            /// </para>
            /// </summary>
            public string? Language { get; set; }

            /// <summary>
            /// Description that shows up in suggestions and hover tooltips.
            /// </summary>
            public string? Documentation { get; set; }

            /// <summary>
            /// Method signature of the method definition. Good way to show parameters, especially if they have default values or are params[].
            /// </summary>
            public string? Signature { get; set; }

            /// <summary>
            /// Method parameters.
            /// <para>
            /// Note that if you are overriding information for a method found via parsing code, setting this in json will completely override that parameter information.
            /// </para>
            /// </summary>
            public List<YarnParameter> Parameters { get; set; } = new();
        }

        public class Function : Command
        {
            /// <summary>
            /// The return type of the function.
            /// </summary>
            public string ReturnType { get; set; }
        }

        public class YarnParameter
        {
            /// <summary>
            /// The name of the parameter in the function signature.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// The type of the parameter.
            /// </summary>
            public string? Type { get; set; }

            /// <summary>
            /// Parameter Documentation, used in method signature hinting.
            /// </summary>
            public string? Documentation { get; set; }

            /// <summary>
            /// Default value if it exists. 
            /// Also will make this parameter optional for parameter count validation.
            /// </summary>
            public string? DefaultValue { get; set; }

            /// <summary>
            /// Corresponds to the params keyword in C#. Makes this parameter optional, and further parameters will use the information from this parameter.
            /// <para>
            /// Undefined behavior if true for any parameter except for the last.
            /// </para>
            /// </summary>
            public bool IsParamsArray { get; set; }
        }

        public static IType ParseType(string? type)
        {
            return type switch
            {
                "number" => BuiltinTypes.Number,
                "string" => BuiltinTypes.String,
                "bool" => BuiltinTypes.Boolean,
                _ => BuiltinTypes.Any,
            };
        }
    }
}
