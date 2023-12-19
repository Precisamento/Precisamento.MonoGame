using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yarn;

namespace Precisamento.MonoGame.YarnSpinner
{
    public class CommandHandler
    {
        private class CommandInfo
        {
            public MethodInfo Method { get; }
            public object? Caller { get; }
            public Converter?[] Converters { get; }
            public int ParameterCount { get; }
            public int RequiredParameters { get; }
            public int OptionalParameters => ParameterCount - RequiredParameters;
            public object[]? Arguments { get; }

            public CommandInfo(MethodInfo method, object? caller, Converter?[] converters)
            {
                Method = method;
                Caller = caller;
                Converters = converters;
                var parameters = method.GetParameters();
                ParameterCount = parameters.Length;
                var optional = parameters.Count(p => p.IsOptional);
                RequiredParameters = ParameterCount - optional;
                if(ParameterCount != 0)
                {
                    Arguments = new object[ParameterCount];
                }
            }
        }

        private delegate object Converter (string argument, DialogueRunner runner);

        private Dictionary<string, CommandInfo> _commands = new Dictionary<string, CommandInfo>();

        public void RegisterAllCommandsInAssembly(Assembly assembly, bool includeInstanceMethods)
        {
            foreach(var type in assembly.GetTypes())
            {
                RegisterAllCommandsOnType(type, includeInstanceMethods);
            }
        }

        public void RegisterAllCommandsOnType(Type type, bool includeInstanceMethods)
            => RegisterAllCommandsOnType(type, null, includeInstanceMethods);

        public void RegisterAllCommandsOnType(object caller)
            => RegisterAllCommandsOnType(caller.GetType(), caller, true);

        private void RegisterAllCommandsOnType(Type type, object? instance, bool includeInstanceMethods)
        {
            var methods = type.GetMethods().Where(m => m.DeclaringType == type);
            var hasCheckedForEmptyCtor = false;

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    if (attribute is YarnCommandAttribute commandAttribute)
                    {
                        if (!method.IsStatic && (instance != null || includeInstanceMethods))
                        {
                            if (instance is null)
                            {
                                if (instance is null && hasCheckedForEmptyCtor)
                                    throw new InvalidOperationException("Cannot register instance commands without a target or an empty constructor");

                                hasCheckedForEmptyCtor = true;
                                var ctor = type.GetConstructor(Type.EmptyTypes);
                                if (ctor is null)
                                    throw new InvalidOperationException("Cannot register instance commands without a target or an empty constructor");

                                instance = ctor.Invoke(null);
                            }

                            RegisterCommand(method, instance, commandAttribute.Name);
                        }
                        else
                        {
                            RegisterCommand(method, null, commandAttribute.Name);
                        }
                    }
                }
            }
        }

        public void RegisterCommand(Delegate method) => RegisterCommand(method.Method, method.Target, null);

        public void RegisterCommand(MethodInfo method) => RegisterCommand(method, null, null);

        public void RegisterCommand(MethodInfo method, object? caller) => RegisterCommand(method, caller, null);

        public void RegisterCommand(Delegate method, string? commandName) => RegisterCommand(method.Method, method.Target, commandName);

        public void RegisterCommand(MethodInfo method, string? commandName) => RegisterCommand(method, null, commandName);

        public void RegisterCommand(MethodInfo method, object? caller, string? commandName)
        {
            if (!method.IsStatic && caller is null)
            {
                var ctor = method.DeclaringType?.GetConstructor(Type.EmptyTypes);
                if(ctor is null)
                    throw new ArgumentException("Must supply a caller to register a non-static command");

                caller = ctor.Invoke(Array.Empty<object>());
            }

            var converters = CreateConvertors(method);

            if(commandName is null)
            {
                var commandAttr = method.GetCustomAttribute<YarnCommandAttribute>();
                commandName = commandAttr is null ? method.Name : commandAttr.Name;
            }

            var info = new CommandInfo(method, caller, converters);

            _commands[commandName] = info;
        }

        public bool IsCommandRegistered(string commandName)
        {
            return _commands.ContainsKey(commandName);
        }

        public bool UnregisterCommand(string commandName)
        {
            return _commands.Remove(commandName);
        }

        public bool TryRunCommand(string[] args, DialogueRunner runner, out CommandResult? result)
        {
            if (_commands.ContainsKey(args[0]))
            {
                result = RunCommand(args, runner);
                return true;
            }

            result = default;
            return false;
        }

        public CommandResult? RunCommand(string[] args, DialogueRunner runner)
        {
            var commandName = args[0];
            var command = _commands[commandName];
            var count = args.Length - 1;

            if (count < command.RequiredParameters || count > command.ParameterCount)
            {
                var methodParams = command.Method
                    .GetParameters()
                    .Select(p => p.IsOptional ? $"[{p.ParameterType.Name}]" : p.ParameterType.Name);

                var signature = string.Join(", ", methodParams);

                throw new ArgumentException(
                    $"{commandName} requires between {command.RequiredParameters} and {command.ParameterCount}" +
                    $" parameters ({signature}), but {count} {(count == 1 ? "was" : "were")} provided.");
            }

            object[]? finalArgs = command.Arguments;

            if(command.ParameterCount > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var argument = args[i + 1];
                    finalArgs[i] = command.Converters[i] is null ? argument : command.Converters[i](argument, runner);
                }

                for (var i = count; i < finalArgs.Length; i++)
                {
                    finalArgs[i] = Type.Missing;
                }
            }

            var result = command.Method.Invoke(command.Caller, finalArgs);
            if(result is CommandResult commandResult)
            {
                return commandResult;
            }
            else
            {
                return null;
            }
        }

        private Converter?[] CreateConvertors(MethodInfo method)
        {
            return method.GetParameters().Select((param, i) => CreateConverter(method, param, i)).ToArray();
        }

        private Converter? CreateConverter(MethodInfo method, ParameterInfo parameter, int index)
        {
            var targetType = parameter.ParameterType;

            if (targetType == typeof(string))
                return null;

            if(typeof(bool).IsAssignableFrom(targetType))
            {
                return (arg, _) =>
                {
                    if (arg.Equals(parameter.Name, StringComparison.InvariantCultureIgnoreCase))
                        return true;

                    if (bool.TryParse(arg, out var result))
                        return result;

                    throw new ArgumentException(
                        $"Can't convert the given parameter at position {index} (\"{arg}\") to parameter "
                        + $"{parameter.Name} of type {typeof(bool).FullName}");
                };
            }

            if(typeof(ILogger).IsAssignableFrom(targetType))
            {
                return (_, dr) => dr.Logger;
            }

            if(typeof(IVariableStorage).IsAssignableFrom(targetType))
            {
                return (_, dr) => dr.VariableStorage;
            }

            if(typeof(Dialogue).IsAssignableFrom(targetType))
            {
                return (_, dr) => dr.Dialogue;
            }

            if(typeof(YarnLocalization).IsAssignableFrom(targetType))
            {
                return (_, dr) => dr.Localization;
            }

            if(typeof(DialogueRunner).IsAssignableFrom(targetType))
            {
                return (_, dr) => dr;
            }

            return (arg, _) =>
            {
                try
                {
                    return Convert.ChangeType(arg, targetType, CultureInfo.InvariantCulture);
                }
                catch(Exception e)
                {
                    throw new ArgumentException(
                        $"Can't convert the given parameter at position {index} (\"{arg}\") to parameter "
                        + $"{parameter.Name} of type {targetType.FullName}");
                }
            };
        }
    }
}
