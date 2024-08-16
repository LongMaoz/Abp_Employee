namespace Domain.Dto;

public class EntitiesResultDto<T>
{
    public EntitiesResultDto()
    {

    }
    public EntitiesResultDto(long total, List<T> data)
    {
        Total = total;
        Data = data;
    }

    public long Total { get; set; } 

    public List<T> Data { get; set; }
}