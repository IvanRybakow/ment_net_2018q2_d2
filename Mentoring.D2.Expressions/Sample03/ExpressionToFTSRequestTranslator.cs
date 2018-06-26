using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sample03
{
	public class ExpressionToFTSRequestTranslator : ExpressionVisitor
	{
		StringBuilder resultString;

		public string Translate(Expression exp)
		{
			resultString = new StringBuilder();
			Visit(exp);

			return resultString.ToString();
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method.DeclaringType == typeof(Queryable)
				&& node.Method.Name == "Where")
			{
				var predicate = node.Arguments[1];
				Visit(predicate);

				return node;
			}
			else if (node.Method.DeclaringType == typeof(string))
			{
				if (node.Object is MemberExpression me && node.Arguments[0] is ConstantExpression cst)
				{
					if (!(cst.Value is string val)) return node;
					switch (node.Method.Name)
					{
						case "StartsWith":
							resultString
								.Append(me.Member.Name)
								.Append(":(")
								.Append(val)
								.Append("*)");
							break;
						case "EndsWith":
							resultString
								.Append(me.Member.Name)
								.Append(":(*")
								.Append(val)
								.Append(")");
							break;
						case "Contains":
							resultString
								.Append(me.Member.Name)
								.Append(":(*")
								.Append(val)
								.Append("*)");
							break;
						default:
							throw new NotSupportedException(
								$"This method call is not supported: {node.Method.Name}");
					}
				}

				return node;
			}
			return base.VisitMethodCall(node);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			switch (node.NodeType)
			{
				case ExpressionType.Equal:
					var memberExpresiion = node.Left is MemberExpression ? node.Left : node.Right as MemberExpression;
					var constantExpresiion = node.Left is ConstantExpression ? node.Left : node.Right as ConstantExpression;
					if (memberExpresiion == null || constantExpresiion == null)
						throw new NotSupportedException(string.Format("One of operands should be property or field and other should be constant"));

					Visit(memberExpresiion);
					resultString.Append("(");
					Visit(constantExpresiion);
					resultString.Append(")");
					break;
				case ExpressionType.AndAlso:
					Visit(node.Left);
					resultString.Append(" AND ");
					Visit(node.Right);
					break;
			};
			return node;
		}
		protected override Expression VisitMember(MemberExpression node)
		{
			resultString.Append(node.Member.Name).Append(":");

			return base.VisitMember(node);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			resultString.Append(node.Value);

			return node;
		}
	}
}
