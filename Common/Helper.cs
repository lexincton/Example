using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Common
{
    public static class Helper
    {
        public static bool TryConvertStringToExpression<T>(string stringExpr, out Expression<Func<T, bool>>? expr)
        {
            expr = null;
            if (string.IsNullOrEmpty(stringExpr))
            {
                return true;
            }

            var parameter = CreateParameter(typeof(T));
            var expression = DynamicExpressionParser
                .ParseLambda(new[] { parameter }, null, stringExpr);

            var convertedBody = Expression.Convert(expression.Body, typeof(bool));
            expr = Expression.Lambda<Func<T, bool>>(convertedBody, expression.Parameters);
            return true;

            ParameterExpression CreateParameter(Type typeParameter)
            {
                return Expression.Parameter(typeParameter, "x");
            }
        }

        public static bool TryConvertExpressionToString<T>(Expression<Func<T, bool>> expr, out string sExpr)
        {
            var visitor = new EvalVisitor();
            var exprLoc = visitor.Visit(expr);

            var visitorToString = new ExpressionToStringVisitor();
            sExpr = visitorToString.Convert(exprLoc);
            return true;
        }

        public static string GetJoinedError(this AggregateException aggregateException)
        {
            return string
                .Join(", ", aggregateException.InnerExceptions
                    .Select(x => x.Message));
        }
    }

    internal class ExpressionToStringVisitor : ExpressionVisitor
    {
        private string _result;

        public string Convert(Expression expression)
        {
            _result = string.Empty;
            Visit(expression);
            return _result;
        }        

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return Visit(node.Body);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            _result += node.Name;
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _result += $"{node.Member.Name}";
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _result += node.Value.ToString();
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Visit(node.Arguments[0]);
            _result += $".{node.Method.Name}(";
            Visit(node.Arguments[1]);
            _result += ")";
            return node;
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            var start = '{';
            _result += $"new {node.Type.GetElementType()}[] {start} ";
            for (int i = 0; i < node.Expressions.Count; i++)
            {
                Visit(node.Expressions[i]);
                if (i < node.Expressions.Count - 1)
                    _result += ", ";
            }
            _result += " }";
            return node;
        }
    }
}
