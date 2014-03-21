namespace LogicControl
{
    using System;
    using System.Linq.Expressions;

    public class AssignLogicStatement : LogicStatement
    {
        public LogicAssignmentTarget AssignmentTarget { get; set; }

        public LogicExpression Expression { get; set; }

        public AssignLogicStatement(LogicAssignmentTarget assignmentTarget, string op, LogicExpression expression)
        {
            this.AssignmentTarget = assignmentTarget;

            switch (op)
            {
                case "=":
                    this.Expression = expression;
                    break;
                case "+=":
                    this.Expression = new AdditionLogicExpression(
                        this.AssignmentTarget.GetReadExpression(),
                        "+",
                        expression);
                    break;
                case "-=":
                    this.Expression = new AdditionLogicExpression(
                        this.AssignmentTarget.GetReadExpression(),
                        "-",
                        expression);
                    break;
                case "*=":
                    this.Expression = new MultiplyLogicExpression(
                        this.AssignmentTarget.GetReadExpression(),
                        "*",
                        expression);
                    break;
                case "/=":
                    this.Expression = new MultiplyLogicExpression(
                        this.AssignmentTarget.GetReadExpression(),
                        "/",
                        expression);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public override void Execute(LogicExecutionState state)
        {
            this.AssignmentTarget.Assign(state, this.Expression.Result(state));
        }

        public override Expression Compile(ParameterExpression stateParameterExpression, LabelTarget returnTarget)
        {
            return this.AssignmentTarget.Compile(stateParameterExpression, returnTarget, this.Expression);
        }
    }
}