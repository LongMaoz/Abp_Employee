using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    public static class ExpressionHelper
    {
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1,bool condition, Expression<Func<T, bool>> expr2)
        {
            if(!condition)
                return expr1;
          return  expr1.And(expr2);
        }
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, bool condition, Func<Expression<Func<T, bool>>> expr2)
        {
            if (!condition)
                return expr1;
            return expr1.And(expr2.Invoke());
        }
    }
}
