using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Common
{
    public class EvalVisitor : ExpressionVisitor
    {
        #region Constructors 

        public EvalVisitor()
        {
        }

        #endregion Constructors 

        #region Methods   

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var value = node.Value;
            if (typeof(Expression).IsAssignableFrom(node.Type))
            {
                return Visit(node.Value as Expression);
            }
            else
            {
                value = Expression.Lambda(node).Compile().DynamicInvoke();
            }   
            
            if (value is IEnumerable values)
            {
                var type = node.Type.IsArray?  node.Type.GetElementType() : node.Type.GetGenericArguments()[0];

                var intValues = new List<Expression>();
                foreach (var item in values)
                {
                    intValues.Add(Expression.Constant(item, type));
                }
                return Expression.NewArrayInit(type, intValues.ToArray());
            }
          
            return Expression.Constant(value, node.Type);
        }

        protected override Expression VisitMember(MemberExpression memberExpr)
        {
            var temp = memberExpr;
            var q = new Stack<Expression>();

            while (temp != null)
            {
                if (temp.Expression?.NodeType == ExpressionType.Parameter)
                    return memberExpr;

                q.Push(temp);

                if (temp.Expression is MemberExpression memberExpression)
                {
                    temp = memberExpression;
                }
                else
                {
                    q.Push(temp.Expression);
                    temp = null;
                }
            }

            Expression result = memberExpr;

            while (q.Count > 0)
            {
                var node = q.Pop();

                if (node is MemberExpression memberExpression)
                {
                    if (TryReplaceExpression(memberExpression, out var newExpr))
                        return newExpr;

                    if (memberExpression.Expression is UnaryExpression unary)
                    {
                        var convertMember = Expression
                            .MakeMemberAccess(unary.Operand, memberExpression.Member);
                        return convertMember;
                    }

                    var value = Expression.Lambda(memberExpression)
                        .Compile().DynamicInvoke();

                    result = Expression.Constant(value, memberExpression.Type);
                }
                else
                {
                    result = Visit(node);
                }
            }

            return result;

            bool TryReplaceExpression(MemberExpression memberExpression, out Expression exprReplace)
            {
                exprReplace = null;
                var memberInnerExpr = memberExpr.Expression;
                if (memberInnerExpr == null || (memberInnerExpr.NodeType != ExpressionType.Constant &&
                    !typeof(Expression).IsAssignableFrom(memberExpression.Type)))
                    return false;

                object target = ((ConstantExpression)memberInnerExpr).Value;
                object valueLoc = null;

                switch (memberExpression.Member.MemberType)
                {
                    case MemberTypes.Property:
                        valueLoc = ((PropertyInfo)memberExpression.Member).GetValue(target, null);
                        break;
                    case MemberTypes.Field:
                        valueLoc = ((FieldInfo)memberExpression.Member).GetValue(target);
                        break;
                    default:
                        valueLoc = target = null;
                        break;
                }
                if (target != null)
                {
                    exprReplace = Expression.Constant(valueLoc, memberExpression.Type);
                    exprReplace = Visit(exprReplace);
                }

                return exprReplace != null;
            }
        }

        #endregion Methods

        #region Override ExpressionVisitor methods

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var operand = Visit(node.Operand);
            return Expression.MakeUnary(node.NodeType, operand, node.Type);

        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            Expression left = Visit(b.Left);
            Expression right = Visit(Visit(b.Right)); // EvaluateIfNeed
            Expression conversion = Visit(b.Conversion);

            if (left != b.Left || right != b.Right || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                else
                {
                    return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
                }
            }

            return b;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!node.Method.DeclaringType.FullName.StartsWith("System."))
            {
                var val = Expression.Lambda(node).Compile().DynamicInvoke();
                var typeVal = val.GetType();

                return Expression.Constant(val, val.GetType());
            }

            var obj = Visit(node.Object);
            var args = node.Arguments
                .Select(x => Visit(x))
                .ToArray();

            return Expression.Call(obj, node.Method, args);
        }

        #endregion Methods 
    }

}
