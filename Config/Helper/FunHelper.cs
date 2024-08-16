using Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Config
{
    public class FunHelper
    {
        private bool result = true;

        public bool Result { get => result; set => result = value; }
        public string Error { get; set; }
        public FunHelper CheckValid(Func<bool> func, bool condition = true)
        {
            if (!this.Result || !condition)
                return this;
            var result = func.Invoke();
            this.Result = result;
            return this;
        }
        public FunHelper CheckValid(Func<UnitResult> func, bool condition = true)
        {
            if (!this.Result || !condition)
                return this;
            var result = func.Invoke();
            this.Error = result.message;
            this.Result = result.result;
            return this;
        }
        public FunHelper CheckValid(UnitResult result, bool condition = true)
        {
            if (!this.Result || !condition)
                return this;
            this.Error = result.message;
            this.Result = result.result;
            return this;
        }
        public async Task<FunHelper> CheckValid(Func<Task<UnitResult>> func, bool condition = true)
        {
            if (!this.Result || !condition)
                return this;
            var result = await func.Invoke();
            this.Error = result.message;
            this.Result = result.result;
            return this;
        }

        public void ExecuteFun(Action func)
        {
            try
            {
                if (!this.Result)
                    return;
                func.Invoke();
            }
            catch { }
        }
        public async Task ExecuteFun(Func<Task> func)
        {
            if (!this.Result)
                return;
            await func.Invoke();
        }
        public FunHelper ThenCon(bool condition, Action fun)
        {
            return ExecuteWithCon(condition, fun);
        }
        public FunHelper ExecuteWithCon(bool condition, Action fun)
        {
            if (condition)
                fun.Invoke();
            return this;
        }
        public async Task<FunHelper> ExecuteWithConAsync(bool condition, Func<Task> fun)
        {
            if (condition)
                await fun.Invoke();
            return this;
        }
        public async Task<FunHelper> ThenConAsync(bool condition, Func<Task> fun)
        {
            return await ExecuteWithConAsync(condition, fun);
        }
        public FunHelper ContinueWithTask(bool condition, Func<Task> fun, List<Func<Task>> tasks)
        {
            if (condition)
                tasks.Add(fun);
            return this;
        }
        public async Task<T> ExecuteByExpression<T>(bool expression, Func<Task<T>> Fun)
        {
            if (expression)
                return await Fun.Invoke();
            return default;
        }
        public T ExecuteByExpression<T>(bool expression, Func<T> Fun)
        {
            if (expression)
                return Fun.Invoke();
            return default;
        }
        public FunHelper ExecuteFunCheck(List<ExecuteExpression> checkExpressions)
        {
            checkExpressions.OrderBy(c => c.Sort).ToList().ForEach(item =>
            {
                if ((item.IsCheckTrue && item.Expression) || (!item.IsCheckTrue && !item.Expression))
                {
                    this.Result = false;
                    this.Error = item.Msg;
                    return;
                }
            });
            return this;
        }
        public UnitResult<T> ExecuteFunComplete<T>(List<ExecuteResultCon<T>> executeByCon)
        {
            if (!this.Result)
                return new UnitResult<T>(false, this.Error, default(T));
            T obj = default(T);
            executeByCon.OrderBy(c => c.Sort).ToList().ForEach(item =>
            {
                if (item.Condition)
                {
                    obj = item.Fun.Invoke();
                    return;
                }
            });
            return new UnitResult<T>(true, "", obj);
        }
        public async Task<UnitResult<List<T>>> ExecuteFunByConAsync<T>(List<ExecuteResultConAsync<T>> executeByCon)
        {
            if (!this.Result)
                return new UnitResult<List<T>>(false, this.Error, null);
            List<T> listData = new List<T>();
            foreach (var item in executeByCon.OrderBy(c => c.Sort).ToList())
            {
                if (item.Condition)
                {
                    var data = await item.Fun.Invoke();
                    if (data != null)
                        listData.Add(data);
                }
            }
            return new UnitResult<List<T>>(true, "", listData);
        }
    }
    public class ExecuteExpression
    {
        public int Sort { get; set; }
        public bool Expression { get; set; }
        public string Msg { get; set; }
        public bool IsCheckTrue { get; set; }
    }
    public class ExecuteResultCon<T>
    {
        public bool Condition { get; set; }
        public Func<T> Fun { get; set; }
        public int Sort { get; set; }
    }
    public class ExecuteResultConAsync<T>
    {
        private bool condition = true;

        public bool Condition { get => condition; set => condition = value; }
        public Func<Task<T>> Fun { get; set; }
        public int Sort { get; set; }
    }
}
