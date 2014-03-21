namespace LogicControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.Xna.Framework;

    public class FunctionCallLogicExpression : TruthfulLogicExpression
    {
        public string Name { get; set; }

        public List<LogicExpression> Arguments { get; set; }

        public FunctionCallLogicExpression(string name, List<LogicExpression> arguments)
        {
            this.Name = name;
            this.Arguments = arguments;
        }

        public override object Result(LogicExecutionState state)
        {
            return DoCall(this.Name, this.Arguments.Select(x => x.Result(state)), state);
        }

        public override Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget)
        {
            Expression<Func<string, IEnumerable<object>, LogicExecutionState, object>> callInvoke =
                (name, arguments, state) => DoCall(name, arguments, state);

            var addMethod = typeof(List<object>).GetMethod("Add");

            var elementInits = this.Arguments.Select(x => Expression.ElementInit(addMethod, Expression.Convert(x.Compile(stateParameterExpression, returnTarget), typeof(object)))).ToList();

            var newList = Expression.New(typeof(List<object>));

            var listInit = elementInits.Count == 0 ? (Expression)newList : Expression.ListInit(newList, elementInits);

            return
                Expression.Invoke(
                    callInvoke,
                    Expression.Constant(this.Name),
                    listInit,
                    stateParameterExpression);
        }

        public static object DoCall(string name, IEnumerable<object> arguments, LogicExecutionState state)
        {
            var structType = state.Structures.FirstOrDefault(x => x.Name == name);

            if (structType != null)
            {
                return new LogicStructureInstance(structType);
            }

            var values = arguments.ToList();

            switch (name)
            {
                case "float":
                    return Convert.ToSingle(values[0]);
                case "string":
                    return Convert.ToString(values[0]);
                case "float2":
                    return new Vector2(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]));
                case "float3":
                    if (values[0] is Vector2)
                    {
                        return new Vector3((Vector2)values[0], Convert.ToSingle(values[1]));
                    }

                    return new Vector3(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]));
                case "float4":
                    if (values[0] is Vector3)
                    {
                        return new Vector4((Vector3)values[0], Convert.ToSingle(values[1]));
                    }

                    return new Vector4(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]), Convert.ToSingle(values[3]));
                case "matrix":
                    return new Matrix();
            }

            switch (name)
            {
                case "max":
                    return LogicBuiltins.Max(values);
                case "min":
                    return LogicBuiltins.Min(values);
                case "sin":
                    return LogicBuiltins.Sin(values);
                case "cos":
                    return LogicBuiltins.Cos(values);
                case "tan":
                    return LogicBuiltins.Tan(values);
                case "atan":
                    return LogicBuiltins.Atan(values);
                case "atan2":
                    return LogicBuiltins.Atan2(values);
                case "lerp":
                    return LogicBuiltins.Lerp(values);
                case "abs":
                    return LogicBuiltins.Abs(values);
                case "normalize":
                    return LogicBuiltins.Normalize(values);
                case "ceil":
                    return LogicBuiltins.Ceiling(values);
                case "floor":
                    return LogicBuiltins.Floor(values);
                case "round":
                    return LogicBuiltins.Round(values);
                case "distance":
                    return LogicBuiltins.Distance(values);
                case "distancesqr":
                    return LogicBuiltins.DistanceSquared(values);
                case "matident":
                    return Matrix.Identity;
                case "mattrans":
                    return Matrix.CreateTranslation((Vector3)values[0]);
                case "matrotx":
                    return Matrix.CreateRotationX(Convert.ToSingle(values[0]));
                case "matroty":
                    return Matrix.CreateRotationY(Convert.ToSingle(values[0]));
                case "matrotz":
                    return Matrix.CreateRotationZ(Convert.ToSingle(values[0]));
                case "matscale":
                    return Matrix.CreateScale(Convert.ToSingle(values[0]));
                case "pi":
                    return MathHelper.Pi;
                default:
                    if (state.AppFunctions.ContainsKey(name))
                    {
                        return state.AppFunctions[name](values.ToArray());
                    }
                    break;
            }

            var userFunction = state.Functions.FirstOrDefault(x => x.Name == name);

            if (userFunction != null)
            {
                throw new NotImplementedException();
            }

            throw new InvalidOperationException("Function missing: " + name);
        }
    }
}