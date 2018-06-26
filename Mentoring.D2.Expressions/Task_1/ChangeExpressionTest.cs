using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Task_1
{
    [TestClass]
    public class ChangeExpressionTest
    {
        public class CusomTransform : ExpressionVisitor
        {
            private readonly Dictionary<string, int> _replacements = new Dictionary<string, int>();

            public CusomTransform(Dictionary<string, int> replacements)
            {
                _replacements = replacements;
            }

            public CusomTransform() { }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                ParameterExpression param = null;
                ConstantExpression constant = null;

                if (node.NodeType == ExpressionType.Add || node.NodeType == ExpressionType.Subtract)
                {

                    if (node.Left.NodeType == ExpressionType.Parameter)
                    {
                        if (node.Left is ParameterExpression p)
                        {
                            Expression newP = this.VisitParameter(p);
                            if (newP is ParameterExpression)
                            {
                                param = (ParameterExpression)node.Left;
                            }
                        }
                    }
                    else if (node.Left.NodeType == ExpressionType.Constant && node.NodeType == ExpressionType.Add)
                    {
                        constant = (ConstantExpression)node.Left;
                    }

                    if (node.Right.NodeType == ExpressionType.Parameter && node.NodeType == ExpressionType.Add)
                    {
                        if (node.Right is ParameterExpression p)
                        {
                            Expression newP = this.VisitParameter(p);
                            if (newP is ParameterExpression)
                            {
                                param = (ParameterExpression)node.Right;
                            }
                        }
                    }
                    else if (node.Right.NodeType == ExpressionType.Constant)
                    {
                        constant = (ConstantExpression)node.Right;
                    }

                    if (param != null && constant != null && constant.Type == typeof(int) && (int)constant.Value == 1)
                    {
                        return node.NodeType == ExpressionType.Add ? Expression.Increment(param) : Expression.Decrement(param);
                    }
                }

                return base.VisitBinary(node);
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
                => Expression.Lambda(Visit(node.Body), node.Parameters);

            protected override Expression VisitParameter(ParameterExpression node)
                => _replacements.TryGetValue(node.Name, out int constant) ? 
                    Expression.Constant(constant) : base.VisitParameter(node);
        }

        [TestMethod]
        public void TransformTest()
        {
            Expression<Func<int, int, int>> sourseExp = (a, b) => a + (a + 1) * (a + 5) * (a - 1) * (1 - a) + 1 + (b - 1) + (1 + b) / (1 - b);
            var  dict = new Dictionary<string, int>{{ "a", 5 }};
            var resultExp = new CusomTransform(dict).VisitAndConvert(sourseExp, "");


            Console.WriteLine(sourseExp + " " + sourseExp.Compile().Invoke(5, 3));
            Console.WriteLine(resultExp + " " + resultExp.Compile().Invoke(5, 3));

        }

    }
}


