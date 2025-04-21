namespace DevHabit.Api.DTOs.Commons;

public interface ICollectionResponse<T>
{
    List<T> Items { get; init; }
}
