using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Uow;

namespace Volo.Abp.Service;

public class UnitAppManage
{
    readonly IUnitOfWorkManager _unitOfWorkManager;
    readonly ILogger<UnitAppManage> _logger;
    public UnitAppManage(IUnitOfWorkManager unitOfWorkManager, ILogger<UnitAppManage> logger)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _logger = logger;
    }
    public async Task<UnitTranResult<T>> TranUnitOfWork<T>(Func<Task<T>> func, string? keyId = null)
    {
        using var unit = _unitOfWorkManager.Begin(isTransactional: true);
        try
        {
            var obj = await func();
            await unit.CompleteAsync();
            return new UnitTranResult<T> { Result = true, Value = obj };
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (unit != null)
                await unit.RollbackAsync();
            _logger.LogError($"UnitAppManage DbUpdateConcurrencyException {keyId},{func.Method.Name},{ex.Message},{ex.InnerException?.Message},{ex.Source},{ex.StackTrace}");
            throw;
        }
        catch (Exception ex)
        {
            if (unit != null)
                await unit.RollbackAsync();
            _logger.LogError($"UnitAppManage Exception {keyId},{func.Method.Name},{ex.Message},{ex.InnerException?.Message},{ex.Source},{ex.StackTrace}");
            throw;
        }
    }
    public async Task<UnitTranResult<T, K>> TranUnitOfWork<T, K>(Func<Task<UnitParams<T, K>>> func, string? keyId = null)
    {
        using var unit = _unitOfWorkManager.Begin(isTransactional: true);
        try
        {
            var obj = await func();
            await unit.CompleteAsync();
            return new UnitTranResult<T, K> { Result = true, Value = obj.Value, Extra = obj.Extra };
        }
        catch (Exception ex)
        {
            if (unit != null)
                await unit.RollbackAsync();
            _logger.LogError($"UnitAppManage {keyId},{func.Method.Name},{ex.Message},{ex.InnerException?.Message},{ex.Source},{ex.StackTrace}");
            return new UnitTranResult<T, K> { Result = false, Message = $"{ex.Message}, {ex.InnerException?.Message}" };
        }
    }
}
public class UnitTranResult<T>
{
    public bool Result { get; set; }
    public string? Message { get; set; }
    public T? Value { get; set; }
}
public class UnitTranResult<T, K>
{
    public bool Result { get; set; }
    public string? Message { get; set; }
    public T? Value { get; set; }
    public K? Extra { get; set; }
}
public class UnitParams<T, K>
{
    public T? Value { get; set; }
    public K? Extra { get; set; }
    public UnitParams(T? value, K? extra)
    {
        Value = value;
        Extra = extra;
    }
}