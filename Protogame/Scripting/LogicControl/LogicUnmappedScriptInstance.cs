namespace LogicControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;

    public class LogicUnmappedScriptInstance
    {
        private readonly List<LogicStructure> m_Structures;

        private readonly List<LogicFunction> m_Functions;

        private readonly Dictionary<string, Func<object[], object>> m_ApplicationFunctions;

        private readonly Dictionary<string, Func<LogicExecutionState, object>> m_CompiledFunctions;

        public LogicUnmappedScriptInstance(List<LogicStructure> logicStructures, List<LogicFunction> logicFunctions)
        {
            this.m_Structures = logicStructures;
            this.m_Functions = logicFunctions;
            this.m_ApplicationFunctions = new Dictionary<string, Func<object[], object>>();
            this.m_CompiledFunctions = new Dictionary<string, Func<LogicExecutionState, object>>();
        }

        public Dictionary<string, object> Execute(string name, Dictionary<string, object> semanticInputs)
        {
            var executionState = new LogicExecutionState();
            executionState.Structures = this.m_Structures;
            executionState.Functions = this.m_Functions;
            executionState.AppFunctions = this.m_ApplicationFunctions;

            var function = this.m_Functions.First(x => x.Name == name);

            semanticInputs = this.NormalizeSemanticInputs(semanticInputs);

            foreach (var parameter in function.Parameters)
            {
                if (parameter.Semantic != null)
                {
                    // Map the semantic input to this parameter.
                    executionState.Variables[parameter.Name] = semanticInputs[parameter.Semantic];
                }
                else
                {
                    // Assume structure
                    // TODO: Validation
                    var structType = this.m_Structures.First(x => x.Name == parameter.Type);

                    var structObj = new LogicStructureInstance(structType);

                    foreach (var field in structType.Fields)
                    {
                        structObj.Fields[field] = semanticInputs[field.Semantic];
                    }

                    executionState.Variables[parameter.Name] = structObj;
                }
            }

            Func<LogicExecutionState, object> compiledFunc;
            if (!this.m_CompiledFunctions.TryGetValue(name, out compiledFunc))
            {
                compiledFunc = LogicScriptCompiler.Compile(function);
                this.m_CompiledFunctions[name] = compiledFunc;
            }

            var result = compiledFunc(executionState);

            if (function.ReturnSemantic != null)
            {
                return new Dictionary<string, object>
                {
                    { function.ReturnSemantic, result }
                };
            }

            var instance = result as LogicStructureInstance;
            if (instance != null)
            {
                var results = new Dictionary<string, object>();
                var structResult = instance;
                foreach (var kv in structResult.Fields)
                {
                    // We only set output fields where the code has explicitly set a value.
                    if (structResult.FieldsSet[kv.Key])
                    {
                        results[kv.Key.Semantic] = kv.Value;
                    }
                }

                return results;
            }

            throw new InvalidOperationException("Missing return semantic for function " + function.Name);
        }

        private Dictionary<string, object> NormalizeSemanticInputs(Dictionary<string, object> semanticInputs)
        {
            foreach (var key in semanticInputs.Keys.ToArray())
            {
                var value = semanticInputs[key];

                if (value == null)
                {
                    value = string.Empty;
                }
                else if (value is byte)
                {
                    value = Convert.ToSingle((byte)value);
                }
                else if (value is char)
                {
                    value = Convert.ToSingle((char)value);
                }
                else if (value is short)
                {
                    value = Convert.ToSingle((short)value);
                }
                else if (value is ushort)
                {
                    value = Convert.ToSingle((ushort)value);
                }
                else if (value is int)
                {
                    value = Convert.ToSingle((int)value);
                }
                else if (value is uint)
                {
                    value = Convert.ToSingle((uint)value);
                }
                else if (value is long)
                {
                    value = Convert.ToSingle((long)value);
                }
                else if (value is ulong)
                {
                    value = Convert.ToSingle((ulong)value);
                }
                else if (value is double)
                {
                    value = Convert.ToSingle((double)value);
                }
                else if (value is float || value is string || value is Vector2 || value is Vector3 
                    || value is Vector4 || value is LogicStructureInstance || value is bool)
                {
                    // Value is fine.
                }
                else
                {
                    throw new InvalidOperationException("Invalid type " + value.GetType());
                }

                semanticInputs[key] = value;
            }

            return semanticInputs;
        }

        public void RegisterApplicationFunction(string functionName, Func<object[], object> callback)
        {
            this.m_ApplicationFunctions[functionName] = callback;
        }
    }
}