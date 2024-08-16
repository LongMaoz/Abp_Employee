namespace Domain.DataFilter;

public interface ICustomSoftDelete
{
    public DateTime? DeleteTime { get; set; }
}